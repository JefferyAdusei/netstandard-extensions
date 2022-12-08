namespace Spyder.Extensions.Expression
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class Extensions
    {
        #region Get Property Value

        /// <summary>
        /// Compiles an expression and gets the functions return value.
        /// </summary>
        /// <typeparam name="T">The type of return value.</typeparam>
        /// <param name="lambda">The expression to compile.</param>
        /// <returns>Expression</returns>
        public static T GetPropertyValue<T>(this Expression<Func<T>> lambda) => lambda.Compile().Invoke();

        /// <summary>
        /// Compiles an expression and gets the functions return value.
        /// </summary>
        /// <typeparam name="TIn">The data type of the input to the expression.</typeparam>
        /// <typeparam name="TRt">The type of return value.</typeparam>
        /// <param name="lambda">The expression to compile.</param>
        /// <param name="input">The input to the expression.</param>
        /// <returns>The compiled expression</returns>
        public static TRt GetPropertyValue<TIn, TRt>(this Expression<Func<TIn, TRt>> lambda, TIn input) => lambda.Compile().Invoke(input);

        #endregion

        #region Set Property Value

        /// <summary>
        /// Sets the underlying properties value to the given value
        /// from an expression that contains the property.
        /// </summary>
        /// <typeparam name="T">The type of value to set.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetPropertyValue<T>(this Expression<Func<T>> lambda, T value)
        {
            // Converts a lambda () => some.Property, to some.Property
            if (!(lambda.Body is MemberExpression expression))
            {
                return;
            }

            // Get the property information so we can set it.
            PropertyInfo propertyInfo = (PropertyInfo)expression.Member;

            object target = Expression.Lambda(expression.Expression).Compile().DynamicInvoke();
            
            // Set the property value.
            propertyInfo.SetValue(target, value);
        }

        /// <summary>
        /// Sets the underlying properties value to the given value
        /// from an expression that contains the property.
        /// </summary>
        /// <typeparam name="TIm">The type of value to set.</typeparam>
        /// <typeparam name="TIn">Data type of the input value.</typeparam>
        /// <param name="lambda">The lambda expression.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="input">The input to the expression</param>
        public static void SetPropertyValue<TIn, TIm>(this Expression<Func<TIn, TIm>> lambda, TIm value, TIn input)
        {
            // Converts a lambda () => some.Property, to some.Property
            if (!(lambda.Body is MemberExpression expression))
            {
                return;
            }

            // Get the property information so we can set it.
            PropertyInfo propertyInfo = (PropertyInfo)expression.Member;

            // Set the property value.
            propertyInfo.SetValue(input, value);
        }

        #endregion
    }
}
