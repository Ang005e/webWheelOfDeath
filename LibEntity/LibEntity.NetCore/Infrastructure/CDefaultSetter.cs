using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibEntity.NetCore.Annotations;

namespace LibEntity.NetCore.Infrastructure
{
    public class CDefaultSetter
    {
        public static void Apply(object target)
        {
            var props = CPropertyGetter.GetPropertyInfo(target);

            // At compile-time, we can't know the type of each [DataProp] property. So we can't
            // just use FetchDefaultPropertyValue<T>() directly.
            // As such, we "Bake in" the generic method for each type/corresponding property, by
            // determining the type of each [DataProp] property, and "baking" it, using MethodInfo.

            // 1. Imagine that RecipeTemplate<T>() is a "recipe template" with a slot *T*:
            //      '*VALUE* grams of *T* goes into the bowl'.
            //          --> We need to know what *T* is before compile time.
            // 2. We need to fill RecipeTemplate<*T*>()'s slots before calling it.
            // 3. We use `typeof(RecipeClass).GetMethod(nameof(RecipeClass.RecipeTemplate))` to "find"
            //      the recipe template.
            // 4. We complete the recipe; `MethodInfo uncookedRecipe = recipeTemplate.MakeGenericMethod(*T*)`
            // 5. We cook the recipe; `object? baked = uncookedRecipe.Invoke(null, null)`

            // FetchDefaultPropertyValue<T>() is RecipeTemplate<T>(). *T* is the type of each property.

            // First, get the generic method. 
            MethodInfo openMethod = typeof(CDataDefaults)
                .GetMethod(nameof(CDataDefaults.FetchDefaultTypeValue))!;

            foreach (var prop in props)
            {
                if (prop.Name == "Id")
                {
                    // Skip the Id property and properties without the DataProp attribute.
                    continue;
                }
                // For each property, "bake" the generic method for the type of the property.
                MethodInfo generic = openMethod.MakeGenericMethod(prop.PropertyType);

                // Invoke the method, which returns the default value for the property type.
                object? defaultValue = generic.Invoke(null, null);

                // Set the property value to the default value.
                prop.SetValue(target, defaultValue);
            }

        }
    }
}
