using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ElsDalExtension
{
    /// <summary>
    /// this class hold some extensions that uses to get property names using linq expression
    /// </summary>
    internal static class ExpressionHelper
    {
        private static readonly string expressionCannotBeNullMessage = "The expression cannot be null.";
        private static readonly string invalidExpressionMessage = "Invalid expression.";

        /// <summary>
        /// Get properiesfrom expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions"></param>
        /// <returns></returns>
        internal static List<PropertyInfo> GetProperiesFromExpression<T>(params Expression<Func<T, object>>[] expressions)
        {
            PropertyInfo prop;
            List<PropertyInfo> plist = new List<PropertyInfo>();
            foreach (var cExpression in expressions)
            {
                if (cExpression.Body is MemberExpression)
                {
                    prop = (PropertyInfo)((MemberExpression)cExpression.Body).Member;
                }
                else
                {
                    var op = ((UnaryExpression)cExpression.Body).Operand;
                    prop = (PropertyInfo)((MemberExpression)op).Member;
                }
                plist.Add(prop);

            }
            return plist;
        }

        /// <summary>
        /// Get property name from expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string GetPropertyNameFromExpression<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return GetPropertyNameFromExpression(expression.Body);
        }
        /// <summary>
        /// Get properties name from expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        internal static List<string> GetPropertiesNamesFromExpression<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetPropertyNameFromExpression(cExpression.Body));
            }

            return memberNames;
        }
        /// <summary>
        /// Get properties name from expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string GetPropertyNameFromExpression<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetPropertyNameFromExpression(expression.Body);
        }
        /// <summary>
        /// Get properties name from expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string GetPropertyNameFromExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }

            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                return memberExpression.Member.Name;
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetPropertyNameFromExpression(unaryExpression);
            }

            throw new ArgumentException(invalidExpressionMessage);
        }
        /// <summary>
        /// Get properties name from expression
        /// </summary>
        /// <param name="unaryExpression"></param>
        /// <returns></returns>
        private static string GetPropertyNameFromExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression)unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression)unaryExpression.Operand).Member.Name;
        }
    }
}
