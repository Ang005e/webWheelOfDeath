using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LibEntity.NetCore.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace LibEntity.NetCore.Infrastructure
{
    
    public enum EnumValidationRule
    {
        Greater,
        Less,
        Equal,
        NotEqual,
        NotDefault
    }

    /// <summary>
    /// Password-specific validation rules
    /// </summary>
    public enum EnumPasswordRule
    {
        MinLength,
        MaxLength,
        RequiresUppercase,
        RequiresLowercase,
        RequiresDigit,
        RequiresSpecialChar
    }

    public partial class CValidatorComponents
    {
        /// <summary>
        /// User to translate between various engines and their results, despite different methods of use.
        /// </summary>
        public interface IValidationResult
        {
            public bool Valid { get; }
            public string Message { get; }
            EnumValidationRule Rule { get; }
            EnumValidationFailure FailureType { get; } // ToDo: Functionality should be moved into CValidationFailureBuilder. Create IValidationFailure.
        }

        /// <summary>
        /// The core interface that all validation engines must implement.
        /// </summary>
        interface IValidationEngine
        {
            bool Validate();
            string BuildMessage();
        }

        /// <summary>
        /// Validates two properties of an entity object using a specified validation rule.
        /// Hides the complextity of the validation logic and provides a simple interface for validation.
        /// </summary>
        /// <typeparam name="TProp"></typeparam>
        /// <typeparam name="TEntity"></typeparam>
        public struct Validation<TProp, TEntity> : IValidationResult 
            where TProp : IComparable
        {

            #region Properties
            private readonly IValidationEngine _engine;
            private TEntity _entity;

            public string Message { get; }

            public bool Valid { get; }

            public EnumValidationRule Rule { get; }

            public EnumValidationFailure FailureType { get; } = EnumValidationFailure.OutOfRange;

            #endregion



            #region Constructors
            /// <summary>
            /// Builds a comparator that compares two objects of type <typeparamref name="TProp"/> using a specified validation rule 
            /// and encapsulates the validation result. 
            /// Retrieves the corresponding property name of each comparison object where they are declared properties of class 
            /// <typeparamref name="TEntity"/> (<paramref name="entity"/>).
            /// Acts as a syntactic wrapper for <see cref="ValidationComparatorEngine"/>.
            /// </summary>
            /// <typeparam name="TProp">The property type; conforms to <see cref="IComparable{TProp}"/> and <see cref="struct"/></typeparam>
            /// <param name="entity">The object instance which the properties belong to</param>
            /// <param name="rule">The validation rule</param>
            /// <param name="left">The left comparison <see cref="TProp"/> instance</param>
            /// <param name="right">The right comparison <see cref="TProp"/> instance</param>
            /// <param name="leftName">
            /// The name of the left <see cref="TProp"/> instance. 
            /// If passing a property of type <paramref name="entity"/>, it will be the property name. 
            /// Should remain unset by the caller, unless passing a raw value instead of a property.
            /// </param>
            /// <param name="rightName">
            /// The name of the right <see cref="TProp"/> instance. 
            /// If passing a property of type <paramref name="entity"/>, it will be the property name. 
            /// Should remain unset by the caller, unless passing a raw value instead of a property.
            /// </param>
            /// <returns>A <see cref="ComparatorValidationEngine{TProp}"/> encapsulating the validation result</returns>
            public Validation(
                TEntity entity,
                EnumValidationRule rule,
                TProp left,
                TProp right,
                /*[CallerArgumentExpression("left")]*/ string leftName = "",
                /*[CallerArgumentExpression("right")]*/ string rightName = "")
            {
                Rule = rule;
                _entity = entity;

                switch (rule)
                {
                    case EnumValidationRule.Greater:
                    case EnumValidationRule.Less:
                    case EnumValidationRule.Equal:
                    case EnumValidationRule.NotEqual:
                        ComparatorValidationEngine<TProp> comparatorEngine = new(rule, left, right, leftName, rightName);
                        _engine = comparatorEngine;
                        break;

                    case EnumValidationRule.NotDefault:
                        string customSkeleton = " must be set";
                        ComparatorValidationEngine<TProp> defaultEngine = new(rule, left, right, leftName, "", customSkeleton);
                        _engine = defaultEngine;
                        break;

                    //case EnumValidationRule.StrongPassword:
                        // For strong password validation, we need to use a different engine.
                        // This is a placeholder for future implementation.
                        //throw new NotImplementedException("Strong password validation is not implemented yet.");

                    default:
                        throw new ArgumentOutOfRangeException(nameof(rule), "Invalid validation rule");
                }

                Valid = _engine.Validate();
                Message = !Valid ? _engine.BuildMessage() : "";

            }
            #endregion



            #region Expression experiments

            // old version of BuildEngine, using Expressions (quite clunky).
            ///// <summary>
            ///// Compares two properties of an entity object using a specified validation rule.
            ///// </summary>
            ///// <typeparam name="TProp">The property type; conforms to <see cref="IComparable{TProp}"/> and <see cref="struct"/></typeparam>
            ///// <param name="entity">The object instance which these properties belong to.</param>
            ///// <param name="rule">The validation rule</param>
            ///// <param name="prop1Sel">Expression tree for property 1</param>
            ///// <param name="prop2Sel">Expression tree for property 2</param>
            ///// <returns>A <see cref="bool"/> marking pass or fail of the validation rule</returns>
            //public ValidationComparatorEngine<TProp> BuildEngine(
            //    TEntity entity, 
            //    EnumValidationRule rule, 
            //    Expression<Func<TEntity, TProp>> prop1Sel, 
            //    Expression<Func<TEntity, TProp>> prop2Sel)
            //{
            //    // Use Expressions to get the property names without passing them through explicitly.
            //    string prop1Name = ((MemberExpression)prop1Sel.Body).Member.Name;
            //    string prop2Name = ((MemberExpression)prop2Sel.Body).Member.Name;
            //    Func<TEntity, TProp> getter1 = prop1Sel.Compile();
            //    Func<TEntity, TProp> getter2 = prop2Sel.Compile();
            //    var prop1Val = getter1(entity);
            //    var prop2Val = getter2(entity);

            //    // After we've unwrapped the metadata layer (property names), we work on down,
            //    // using it to build the engine...
            //    ValidationComparatorEngine<TProp> engine = new(rule, prop1Val, prop2Val, prop1Name, prop2Name);

            //    // ...and return the engine, to be used elsewhere.
            //    return engine;
            //}

            #endregion
        }


        #region PASSWORD COMPONENT



        #endregion


        #region COMPARATOR COMPONENTS
        /// <summary>
        /// Builds the base chassis for validation logic.
        /// </summary>
        /// <typeparam name="T">The type of the properties to validate.</typeparam>
        public readonly struct ComparatorValidationEngine<T> : IValidationEngine where T : IComparable
        {
            public readonly EnumValidationRule Rule { get; }

            private T _prop1Value { get; }
            private T _prop2Value { get; }
            private string _prop1Name { get; }
            private string _prop2Name { get; }

            private string _messageSkeleton { get; }

            private readonly Func<T, T, bool> _validate { get; }
            public readonly bool Validate() => _validate(_prop1Value, _prop2Value);

            internal ComparatorValidationEngine(EnumValidationRule rule, T value1, T value2, string prop1Name, string prop2Name, string? customMessageSkeleton = null)
            {
                // 1. Rule/Validation function
                Rule = rule;

                // Select the validation function (based on the rule enum).
                _validate = Comparator<T>.FromRule(Rule).Test;

                _prop1Value = value1;
                _prop2Value = value2;

                // 2. Message builder
                _prop1Name = prop1Name;
                _prop2Name = prop2Name;

                // Select the appropriate corresponding skeleton message text.
                if ( ! customMessageSkeleton.IsNullOrEmpty())
                { 
                    _messageSkeleton = customMessageSkeleton!;
                    return;
                }
                switch (Rule)
                {
                    case EnumValidationRule.Greater:
                        _messageSkeleton = $" must be greater than ";
                        break;
                    case EnumValidationRule.Less:
                        _messageSkeleton = $" must be less than ";
                        break;
                    case EnumValidationRule.Equal:
                        _messageSkeleton = $" must be equal to ";
                        break;
                    case EnumValidationRule.NotEqual:
                        _messageSkeleton = $" must not be equal to ";
                        break;
                    default:
                        throw new ArgumentException("Invalid validation rule");
                }
            }

            // Place all components in their respective correct locations.
            public string BuildMessage()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_prop1Name);
                sb.Append(_messageSkeleton);
                sb.Append(_prop2Name);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Immutable “holder” that knows how to test two values.
        /// Acts as the base component for ValidationEngine.
        /// This is the most base functionality that can be extracted from <see cref="ComparatorValidationEngine{T}"/>.
        /// </summary>
        internal readonly struct Comparator<T> where T : IComparable
        {
            public Func<T, T, bool> Test { get; }

            private Comparator(Func<T, T, bool> test) => Test = test;

            public static Comparator<T> FromRule(EnumValidationRule rule) =>
                new(rule switch
                {
                    EnumValidationRule.Greater => (a, b) => a.CompareTo(b) > 0,
                    EnumValidationRule.Less => (a, b) => a.CompareTo(b) < 0,
                    EnumValidationRule.Equal => (a, b) => a.CompareTo(b) == 0,
                    EnumValidationRule.NotEqual => (a, b) => a.CompareTo(b) != 0,
                    EnumValidationRule.NotDefault => (a, b) => a.CompareTo(b) != 0,
            _ => throw new ArgumentOutOfRangeException(nameof(rule))
                });
        }
        #endregion


        //#region Password Validation Engine


        ///// <summary>
        ///// Validates passwords against configurable rules
        ///// </summary>
        //public readonly struct PasswordValidationEngine : IValidationEngine
        //{
        //    public readonly EnumPasswordRule Rule { get; }
        //    private readonly string _password;
        //    internal readonly object _parameter { get; }
        //    private readonly string _messageSkeleton;
        //    private readonly Func<string, object, bool> _validate;

        //    public readonly bool Validate() => _validate(_password, _parameter);

        //    internal PasswordValidationEngine(EnumPasswordRule rule, string password, object parameter = null, string customMessageSkeleton = null)
        //    {
        //        Rule = rule;
        //        _password = password;
        //        _parameter = parameter;

        //        // Select validation function
        //        _validate = PasswordValidator.FromRule(Rule).Test;

        //        // Build message skeleton
        //        _messageSkeleton = customMessageSkeleton ?? BuildDefaultMessage(rule, parameter);
        //    }

        //    private static string BuildDefaultMessage(EnumPasswordRule rule, object parameter)
        //    {
        //        return rule switch
        //        {
        //            EnumPasswordRule.MinLength => $"Password must be at least {parameter} characters long",
        //            EnumPasswordRule.MaxLength => $"Password must not exceed {parameter} characters",
        //            EnumPasswordRule.RequiresUppercase => "Password must contain at least one uppercase letter",
        //            EnumPasswordRule.RequiresLowercase => "Password must contain at least one lowercase letter",
        //            EnumPasswordRule.RequiresDigit => "Password must contain at least one digit",
        //            EnumPasswordRule.RequiresSpecialChar => "Password must contain at least one special character",
        //            _ => "Invalid password"
        //        };
        //    }

        //    public string BuildMessage() => _messageSkeleton;
        //}

        ///// <summary>
        ///// Password validation logic holder
        ///// </summary>
        //internal readonly partial struct PasswordValidator
        //{
        //    public Func<string, object, bool> Test { get; }

        //    private PasswordValidator(Func<string, object, bool> test) => Test = test;

        //    public static PasswordValidator FromRule(EnumPasswordRule rule) =>
        //        new(rule switch
        //        {
        //            EnumPasswordRule.MinLength => (pwd, param) => pwd.Length >= Convert.ToInt32(param),
        //            EnumPasswordRule.MaxLength => (pwd, param) => pwd.Length <= Convert.ToInt32(param),
        //            EnumPasswordRule.RequiresUppercase => (pwd, _) => pwd.Any(char.IsUpper),
        //            EnumPasswordRule.RequiresLowercase => (pwd, _) => pwd.Any(char.IsLower),
        //            EnumPasswordRule.RequiresDigit => (pwd, _) => pwd.Any(char.IsDigit),
        //            EnumPasswordRule.RequiresSpecialChar => (pwd, _) => MyRegex().IsMatch(pwd),
        //            _ => throw new ArgumentOutOfRangeException(nameof(rule))
        //        });

        //    private static bool HasRepeatingChars(string pwd, int maxRepeats)
        //    {
        //        return pwd.GroupBy(c => c).Any(g => g.Count() > maxRepeats);
        //    }

        //    [GeneratedRegex(@"[!@#$%^&*(),.?"":{}|<>]")]
        //    private static partial Regex MyRegex();
        //}

        //#endregion

        //#region Password Validation Component

        ///// <summary>
        ///// Complete password validation component with multiple rules
        ///// </summary>
        //public class PasswordValidationComponent
        //{
        //    private readonly List<PasswordValidationEngine> _validators = new();

        //    public PasswordValidationComponent AddRule(EnumPasswordRule rule, object parameter)
        //    {
        //        _validators.Add(new PasswordValidationEngine(rule, "", parameter));
        //        return this;
        //    }

        //    public ValidationResult Validate(string password)
        //    {
        //        var errors = new List<string>();

        //        foreach (var validator in _validators)
        //        {
        //            var engine = new PasswordValidationEngine(validator.Rule, password, validator._parameter);
        //            if (!engine.Validate())
        //            {
        //                errors.Add(engine.BuildMessage());
        //            }
        //        }

        //        return new ValidationResult
        //        {
        //            IsValid = errors.Count == 0,
        //            Errors = errors
        //        };
        //    }

        //    public static PasswordValidationComponent Default()
        //    {
        //        return new PasswordValidationComponent()
        //            .AddRule(EnumPasswordRule.MinLength, 12)
        //            .AddRule(EnumPasswordRule.MaxLength, 255)
        //            .AddRule(EnumPasswordRule.RequiresUppercase)
        //            .AddRule(EnumPasswordRule.RequiresLowercase)
        //            .AddRule(EnumPasswordRule.RequiresDigit)
        //            .AddRule(EnumPasswordRule.RequiresSpecialChar);
        //    }
        //}

        //public class ValidationResult
        //{
        //    public bool IsValid { get; set; }
        //    public List<string> Errors { get; set; } = new();
        //}

        //#endregion


    }
}
