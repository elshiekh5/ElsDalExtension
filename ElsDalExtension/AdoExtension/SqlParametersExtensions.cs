using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElsDalExtension
{
    internal static class SqlParametersExtensions
    {
        #region --------------ConvertToParameters--------------
        //---------------------------------------------------------------------
        //ConvertToParameters
        //---------------------------------------------------------------------
        /// <summary>
        /// convert any object properties to a collection of  sql parameters as CustomDbParameterList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        internal static CustomDbParameterList ConvertToParameters<T>(this T instance)
        {
            string name = null;
            object value;
            CustomDbParameterList plist = new CustomDbParameterList();
            Type t = instance.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                name = prop.Name;
                value = prop.GetValue(instance);
                plist.Add(name, value);
            }
            return plist;
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------ConvertToParameters--------------
        //---------------------------------------------------------------------
        //ConvertToParameters
        //---------------------------------------------------------------------
        /// <summary>
        /// convert specific properties to a collection of  sql parameters as CustomDbParameterList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        internal static CustomDbParameterList ConvertToParameters<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            CustomDbParameterList parameterlist = new CustomDbParameterList();
            List<PropertyInfo> propList = ExpressionHelper.GetProperiesFromExpression(expressions);
            foreach (PropertyInfo prop in propList)
            {
                parameterlist.Add(prop, prop.GetValue(instance));
            }
            return parameterlist;
        }
        //---------------------------------------------------------------------

        #endregion

        #region --------------ConvertToParametersExcept--------------
        //---------------------------------------------------------------------
        //ConvertToParametersExcept
        //---------------------------------------------------------------------
        /// <summary>
        /// convert any object properties to a collection of  sql parameters as CustomDbParameterList except some properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        //---------------------------------------------------------------------
        internal static CustomDbParameterList ConvertToParametersExcept<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            CustomDbParameterList parameterlist = new CustomDbParameterList();
            List<PropertyInfo> execludedpropList = ExpressionHelper.GetProperiesFromExpression(expressions);
            List<string> execludedpropListNames = execludedpropList.Select(e => e.Name).ToList();
            Type t = instance.GetType();
            PropertyInfo[] propList = t.GetProperties();
            foreach (var prop in propList)
            {
                if (expressions == null || !execludedpropListNames.Contains(prop.Name))
                {
                    parameterlist.Add(prop, prop.GetValue(instance));
                }
            }
            return parameterlist;
        }
        //---------------------------------------------------------------------
        #endregion
    }
}
