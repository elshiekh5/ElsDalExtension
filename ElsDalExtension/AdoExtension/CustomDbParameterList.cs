using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace ElsDalExtension
{
    /// <summary>
    /// custom parameter list is a collection class that holds sql parameters
    /// </summary>
    public class CustomDbParameterList
    {
        public List<SqlParameter> Parameters { get; set; }
        public List<Object> ParametersValues { get; set; }

        #region --------------Constructor--------------
        //---------------------------------------------
        //Constructor
        //---------------------------------------------
        public CustomDbParameterList()
        {
            Parameters = new List<SqlParameter>();
        }
        //---------------------------------------------
        #endregion

        #region --------------Add(PropertyInfo prop, object value)--------------
        //---------------------------------------------
        //Add
        //---------------------------------------------
        public SqlParameter Add(PropertyInfo prop, object value)
        {
            var p = new SqlParameter(prop.Name, value);
            if (value != null)
            {
                Type typeOfParameter = prop.GetType();
                if (typeOfParameter == typeof(string))
                {
                    p.Size = ((string)value).Length;
                }
            }
            return this.Add(p);
        }
        //---------------------------------------------
        #endregion

        #region --------------Add(string parameterName, object value)--------------
        //---------------------------------------------------------------------
        //Add(string parameterName, object value)
        //---------------------------------------------------------------------
        public SqlParameter Add(string parameterName, object value)
        {
            var p = new SqlParameter(parameterName, value);
            if (value != null)
            {
                Type typeOfParameter = value.GetType();
                if (typeOfParameter == typeof(string))
                {
                    p.Size = ((string)value).Length;
                }
            }
            return this.Add(p);
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------Add(string parameterName, SqlDbType sqlDbType)--------------
        //---------------------------------------------------------------------
        //Add
        //---------------------------------------------------------------------
        public SqlParameter Add(string parameterName, SqlDbType sqlDbType)
        {
            return this.Add(new SqlParameter(parameterName, sqlDbType));
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------Add(string parameterName, SqlDbType sqlDbType, int size)--------------
        //---------------------------------------------------------------------
        //Add
        //---------------------------------------------------------------------
        public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size)
        {
            return this.Add(new SqlParameter(parameterName, sqlDbType, size));
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------Add(string parameterName, SqlDbType sqlDbType, int size, string sourceColumn)--------------
        //---------------------------------------------------------------------
        //Add
        //---------------------------------------------------------------------
        public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size, string sourceColumn)
        {
            return this.Add(new SqlParameter(parameterName, sqlDbType, size, sourceColumn));
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------Add(SqlParameter parameter)--------------
        //---------------------------------------------------------------------
        //Add(SqlParameter parameter)
        //---------------------------------------------------------------------
        public SqlParameter Add(SqlParameter parameter)
        {
            Parameters.Add(parameter);
            return parameter;
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------AddOutputParameter(string parameterName, SqlDbType sqltype)--------------
        //---------------------------------------------------------------------
        //AddOutputParameter(string parameterName, SqlDbType sqltype)
        //---------------------------------------------------------------------
        public SqlParameter AddOutputParameter(string parameterName, SqlDbType sqltype)
        {
            var p = new SqlParameter();
            p.ParameterName = parameterName;
            p.SqlDbType = sqltype;
            p.Direction = ParameterDirection.Output;
            this.Add(p);

            return p;
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------AddOutputParameter_Integer(string parameterName)--------------
        //---------------------------------------------------------------------
        //AddOutputParameter_Integer(string parameterName)
        //---------------------------------------------------------------------
        public SqlParameter AddOutputParameter_Integer(string parameterName)
        {
            return AddOutputParameter(parameterName, SqlDbType.Int);
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------AddOutputParameter_Long(string parameterName)--------------
        //---------------------------------------------------------------------
        //AddOutputParameter_Long(string parameterName)
        //---------------------------------------------------------------------
        public SqlParameter AddOutputParameter_Long(string parameterName)
        {
            return AddOutputParameter(parameterName, SqlDbType.BigInt);
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------AddOutputParameter_Boolean(string parameterName)--------------
        //---------------------------------------------------------------------
        //AddOutputParameter_Boolean(string parameterName)
        //---------------------------------------------------------------------
        public SqlParameter AddOutputParameter_Boolean(string parameterName)
        {
            return AddOutputParameter(parameterName, SqlDbType.Bit);
        }
        //---------------------------------------------------------------------
        #endregion

        #region --------------GenerateParametersFromEntity(object obj, string parametersNames)--------------
        //---------------------------------------------------------------------
        //GenerateParametersFromEntity(object obj, string parametersNames)
        //---------------------------------------------------------------------
        public void GenerateParametersFromEntity(object obj, string parametersNames)
        {
            Type t = obj.GetType();
            string[] parametersNamesArray = parametersNames.Split(',');

            PropertyInfo myPropInfo;
            object parameterValue;
            foreach (var parameterName in parametersNamesArray)
            {
                myPropInfo = t.GetProperty(parameterName);
                parameterValue = myPropInfo.GetValue(obj);
                this.Add("parameterName", parameterValue);
            }
        }
        //---------------------------------------------------------------------
        #endregion

    }
}