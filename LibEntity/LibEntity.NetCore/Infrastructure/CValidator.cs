using System.Reflection;
using System.Runtime.CompilerServices;
using LibEntity.NetCore.Exceptions;
using static LibEntity.NetCore.Infrastructure.CValidatorComponents;

namespace LibEntity.NetCore.Infrastructure
{

    public static class ValidatorExtensions
    {
        public static bool NotDefaultValue<T>(this CValidator<CEntity> validator, T? value) where T : struct
        {
            return value.HasValue;
        }
    }

    /// <summary>
    /// ToDo
    /// CHANGES INBOUND
    /// Currently doesn't work; sabotages CRUDS class usage due to bad setup. 
    /// Instead, I'll put nullable backing fields on each CRUDS class and getter/setter properties which return viable values.
    /// This class will validate the backing fields, and the getters/setters will act as safety nets in case of null values.
    /// The null values will serve as a warning flag that a field is default (unset).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CValidator<TEntity> 
        where TEntity : CEntity
    {

        private TEntity _entity { get; set; }
        private CValidatorFailureBuilder _failureBuilder { get; } = new();

        private List<IValidationResult> _validations = new();

        // Set when we've manually added a failure (bypassing _validations and the internal ComparatorValidationEngine)
        // and added a failure record directly (for example, if we've got a rule that's a bit of an outlier and checked for it manually)
        private bool _forceFail = false;


        public CValidator(TEntity entity)
        {
            _entity = entity;
        }



        #region COMPARATOR HELPERS
        public void Greater<TProp>(
                TProp leftProp, TProp rightProp, [CallerArgumentExpression("leftProp")] string leftName = "", [CallerArgumentExpression("rightProp")] string rightName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Greater, leftProp, rightProp, leftName, rightName));
        }
        public void GreaterConst<TProp>(
                TProp property, TProp smallerConst, [CallerArgumentExpression("property")] string propName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Greater, property, smallerConst, propName, smallerConst.ToString() ?? "NONAME"));
        }

        public void Less<TProp>(
            TProp leftProp, TProp rightProp, [CallerArgumentExpression("leftProp")] string leftName = "", [CallerArgumentExpression("rightProp")] string rightName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Less, leftProp, rightProp, leftName, rightName));
        }
        public void LessConst<TProp>(
                TProp property, TProp largerConst, [CallerArgumentExpression("property")] string propName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Less, property, largerConst, propName, largerConst.ToString() ?? "NONAME"));
        }
        public void Equal<TProp>(
            TProp leftProp, TProp rightProp, [CallerArgumentExpression("leftProp")] string leftName = "", [CallerArgumentExpression("rightProp")] string rightName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Equal, leftProp, rightProp, leftName, rightName));
        }
        public void EqualConst<TProp>(
        TProp property, TProp equalConst, [CallerArgumentExpression("property")] string propName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.Equal, property, equalConst, propName, equalConst.ToString() ?? "NONAME"));
        }

        public void NotEqual<TProp>(
            TProp leftProp, TProp rightProp, [CallerArgumentExpression("leftProp")] string leftName = "", [CallerArgumentExpression("rightProp")] string rightName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.NotEqual, leftProp, rightProp, leftName, rightName));
        }
        public void NotEqualConst<TProp>(
        TProp property, TProp notEqualConst, [CallerArgumentExpression("property")] string propName = "")
            where TProp : struct, IComparable
        {
            Add(new Validation<TProp, TEntity>(_entity, EnumValidationRule.NotEqual, property, notEqualConst, propName, notEqualConst.ToString() ?? "NONAME"));
        }
        #endregion



        #region DEFAULT VALIDATION HELPERS

        /// <summary>
        /// Assumes that all nullable properties are not explicitly set -- meaning, they are defaults.
        /// </summary>
        /// <param name="excludeProps"></param>
        public void NoDefaults(params string[]? excludeProps)
        {
            // Id is always excluded, as it is set by the framework.
            excludeProps = (excludeProps ?? Array.Empty<string>())
               .Append("Id")
               .ToArray();

            var props = CPropertyGetter.GetPropertyInfo(_entity);

            foreach (var p in props)
            {
                if (excludeProps != null && excludeProps.Contains(p.Name))
                    continue;

                if (p.GetType() == typeof(bool))
                {
                    continue;
                }

                // Check if it's nullable
                Type? underlyingType = Nullable.GetUnderlyingType(p.PropertyType);

                if (underlyingType != null)
                {
                    // For nullable types, null means "not set"
                    object? value = p.GetValue(_entity);
                    if (value == null)
                    {
                        ManualAddFailure(EnumValidationFailure.NotSet, $"{p.Name} must be set");
                    }
                    // If it has a value (even if it's false/0/default), that's fine - it was explicitly set
                }
                else
                {
                    // For non-nullable types, use your existing logic
                    // Only process types that implement IComparable
                    if (!typeof(IComparable).IsAssignableFrom(p.PropertyType))
                        continue;

                    MethodInfo fetchDefOpen =
                        typeof(CDataDefaults).GetMethod(nameof(CDataDefaults.FetchDefaultTypeValue))!;
                    MethodInfo addOpen =
                        GetType().GetMethod(nameof(AddDefaultValidation),
                                            BindingFlags.Instance | BindingFlags.NonPublic)!;

                    object defaultVal = fetchDefOpen.MakeGenericMethod(p.PropertyType)
                                                    .Invoke(null, null)!;
                    object currentVal = p.GetValue(_entity)!;

                    MethodInfo addClosed = addOpen.MakeGenericMethod(p.PropertyType);
                    addClosed.Invoke(this, new[] { currentVal, defaultVal, p.Name });
                }
            }
        }


        public void NoDefaultsExcept(params string[] include)
        {
            NoDefaults(CPropertyGetter
                       .GetPropertyInfo(_entity)
                       .Select(pr => pr.Name)
                       .Except(include)
                       .ToArray());
        }

        private void AddDefaultValidation<TProp>(
            TProp current, TProp defaultVal, string propName)
            where TProp : IComparable
        {
            // rightName = "". Custom skeleton " must be set" is used
            Add(new Validation<TProp, TEntity>(
                    _entity,
                    EnumValidationRule.NotDefault,
                    current,
                    defaultVal,
                    propName,
                    ""));
        }

        private void PasswordStrength<TProp>(
        TProp passwordProp, [CallerArgumentExpression("passwordProp")] string propName = "")
        {

            //passwordProp.

            //_failureBuilder.AddFailure(new CValidationFailure(ruleBroken, displayMessage));
        }

        #endregion


        public void ManualAddFailure(EnumValidationFailure ruleBroken, string displayMessage)
        {
            _failureBuilder.AddFailure(new CValidationFailure(ruleBroken, displayMessage));
        }


        private void Add<TProp>(Validation<TProp, TEntity> v)
            where TProp : IComparable
            => _validations.Add(v);

        //private void Add(List<IValidation> validations)
        //{
        //    foreach (var validation in validations) _validations.Add(validation);
        //}   

        public void Validate()
        {
            Run();

            if (_failureBuilder.ShouldThrow() || _forceFail)
            {
                throw _failureBuilder.ValidationException();
            }
        }

        private void Run() 
        { 
            foreach (IValidationResult validation in _validations)
            {
                if (!validation.Valid) _failureBuilder.AddFailure(validation.FailureType, validation.Message);
            }
        }

        #region ToDos
        // notes
        // 1. May want to pass things other than only pairs of properties. For example, a prop and a value (i.e. if testing `prop > 0`).
        // 2. ToDo: seperate property-property comparison from property-value comparison (the message would be different, i.e. {prop1Name} must be more than {prop2Name} as
        //      opposed to {prop1Name} must be greater than {value})
        //      DONE, switched from expressions to [CallerArgumentExpression], can pass name in manually
        #endregion

    }

}