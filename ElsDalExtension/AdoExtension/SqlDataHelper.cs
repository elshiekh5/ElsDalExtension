using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;


namespace ElsDalExtension
{
  
    public static class SqlDataHelper
    {
        #region GetColumnsName
        public static StringDictionary GetColumnsSchema(IDataReader reader)
        {
            StringDictionary columnsNames = new StringDictionary();
            DataTable dt = reader.GetSchemaTable();
            //---------------------------------
            foreach (DataColumn c in dt.Columns)
            {
                columnsNames.Add(c.ColumnName, null);
            }
            //---------------------------------
            return columnsNames;
        }
        #endregion

        #region --------------GetSqlConnection--------------
        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        }
        //------------------------------------------
        #endregion

        #region --------------GetEntity--------------
        //---------------------------------------------------------------------
        //GetEntity
        //---------------------------------------------------------------------
        /// <summary>
        /// conver datareader object to an entity object
        /// </summary>
        /// <param name="reader">data reader </param>
        /// <param name="t">type of object we need to convert to</param>
        /// <returns></returns>
        private static T GetEntity<T>(IDataReader reader)
        {
            Type t = typeof(T);
            return (T)GetEntity(reader, t);
        }
        //---------------------------------------------------------------------
        private static object GetEntity(IDataReader reader, Type t)
        {

            object obj = Activator.CreateInstance(t);
            //object obj = new t();
            StringDictionary columnsNames = new StringDictionary();
            DataTable dt = reader.GetSchemaTable();
            Type nullableType;
            object value;
            object safeValue;
            //---------------------------------
            string columnname;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnname = reader.GetName(i);
                if (!columnsNames.ContainsKey(columnname))
                {
                    columnsNames.Add(columnname, null);
                    PropertyInfo myPropInfo;
                    myPropInfo = t.GetProperty(columnname);
                    value = reader[columnname];

                    if (value != DBNull.Value && myPropInfo != null)
                    {
                        //myPropInfo.SetValue(obj, Convert.ChangeType(value, myPropInfo.PropertyType), null);

                        if (myPropInfo.PropertyType.BaseType == typeof(System.Enum))
                        {
                            //int intVal = Convert.ToInt32(attr.Value);
                            myPropInfo.SetValue(obj, Enum.Parse(myPropInfo.PropertyType, value.ToString()), null);
                            //Enum.Parse(typeof(myPropInfo.), "FirstName");   
                        }
                        /*
                        else if (value.GetType() == typeof(Byte[]))
                        {
                            byte[] buf = (byte[])value;
                            myPropInfo.SetValue(obj, Convert.ChangeType(OurSerializer.Deserialize(buf), myPropInfo.PropertyType), null);
                        }
                        */
                        else if (Nullable.GetUnderlyingType(myPropInfo.PropertyType) != null)
                        {
                            nullableType = Nullable.GetUnderlyingType(myPropInfo.PropertyType) ?? myPropInfo.PropertyType;
                            safeValue = (value == null) ? null : Convert.ChangeType(value, nullableType);
                            myPropInfo.SetValue(obj, safeValue);

                        }
                        else
                        {
                            myPropInfo.SetValue(obj, Convert.ChangeType(value, myPropInfo.PropertyType), null);
                        }
                    }
                }
            }
            //---------------------------------
            return obj;
        }

        private static object GetDynamic(IDataReader reader)
        {
            return null;
        }
        #endregion

        #region --------------ExecuteStoredProcedure--------------

        public static int ExecuteStoredProcedure(string procedureName, CustomDbParameterList customParameters)
        {
            return ExecuteStoredProcedure(procedureName, customParameters.Parameters);
        }

        public static int ExecuteStoredProcedure(string procedureName, List<SqlParameter> parameters)
        {
            int resultCount = 0;
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------------------------------------------
                // Execute the command
                myConnection.Open();
                resultCount = myCommand.ExecuteNonQuery();
                myConnection.Close();
                //----------------------------------------------------------------
                return resultCount;
            }
        }
        #endregion


        #region --------------ExecuteScalarStoredProcedure--------------

        public static object ExecuteScalarStoredProcedure(string procedureName, CustomDbParameterList customParameters)
        {
            return ExecuteScalarStoredProcedure(procedureName, customParameters.Parameters);
        }

        public static object ExecuteScalarStoredProcedure(string procedureName, List<SqlParameter> parameters)
        {
            object result = 0;
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------------------------------------------
                // ExecuteScalar the command
                myConnection.Open();
                result = myCommand.ExecuteScalar();
                myConnection.Close();
                //----------------------------------------------------------------
                return result;
            }
        }
        #endregion



        #region --------------RetrieveEntitySingleOrDefault--------------
        public static T RetrieveEntitySingleOrDefault<T>(string procedureName, CustomDbParameterList customParameters)
        {
            return RetrieveEntitySingleOrDefault<T>(procedureName, customParameters.Parameters);
        }

        public static T RetrieveEntitySingleOrDefault<T>(string procedureName, List<SqlParameter> parameters)
        {
            Type t = typeof(T);
            List<T> list = RetrieveEntityList<T>(procedureName, parameters);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return (T)Activator.CreateInstance(t);
            }
        }
        #endregion

        #region --------------RetrieveMultiRecordSet--------------
        public static Dictionary<string, object> RetrieveMultiRecordSet(string procedureName, CustomDbParameterList customParameters, params Type[] types)
        {
            return RetrieveMultiRecordSet(procedureName, customParameters.Parameters, types);
        }

        public static Dictionary<string, object> RetrieveMultiRecordSet(string procedureName, List<SqlParameter> parameters, params Type[] types)
        {
            var recordSetDefinitions = GenerateRecordSetDefinition(types);
            return RetrieveMultiRecordSet(procedureName, parameters, recordSetDefinitions);

        }
        public static Dictionary<string, object> RetrieveMultiRecordSet(string procedureName, List<SqlParameter> parameters, Type[] types, List<string> names)
        {
            var rsDefinitionManager = new RecordSetDefinitionManager();
            var recordSetDefinitions = rsDefinitionManager.GenerateRecordSetDefinition(types, names);
            return RetrieveMultiRecordSet(procedureName, parameters, recordSetDefinitions);
        }
        #region oldCode of RetrieveMultiRecordSet
        /*
        public static Dictionary<string, object> RetrieveMultiRecordSet(string procedureName, List<SqlParameter> parameters, List<Type> types)
        {
            Dictionary<string, object> resultSet = new Dictionary<string, object>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------
                // Execute the command
                int index = 0;
                Type itemType = null;
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                foreach (var t in types)
                {

                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(t);

                    var itemsList = (IList) Activator.CreateInstance(constructedListType);

                    if (index++ > 0) { dr.NextResult(); }

                    while (dr.Read())
                    {
                        var item = GetEntity(dr,t);
                        if (item != null)
                        {
                            itemsList.Add(item);
                        }
                    }
                    resultSet.Add(t.Name, itemsList);


                }
                dr.Close();
                myConnection.Close();
                //----------------------------------------------------------------
                return resultSet;
            }
        }
        */
        #endregion

        public static Dictionary<string, object> RetrieveMultiRecordSet(string procedureName, List<SqlParameter> parameters, List<RecordSetDefinition> tepesDefinition)
        {
            Dictionary<string, object> resultSet = new Dictionary<string, object>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------
                // Execute the command
                int index = 0;
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                foreach (var t in tepesDefinition)
                {


                    if (index > 0) { dr.NextResult(); }
                    if (t.IsGenericType)
                    {
                        IList itemsList = GetListOfDataFromDataReader(dr, t);
                        resultSet.Add(t.Name, itemsList);
                    }
                    else
                    {
                        var item = GetSingleobjectFromDataReader(dr, t);
                        resultSet.Add(t.Name, item);
                    }
                    ++index;
                }
                dr.Close();
                myConnection.Close();
                //----------------------------------------------------------------
                return resultSet;
            }
        }

        #endregion
        #region ----------------------------
        /* public static int ExecuteStoredProcedure(string procedureName, CustomDbParameterList customParameters, SqlTransaction transaction = null)
         {
             return ExecuteStoredProcedure(procedureName, customParameters.Parameters);
         }*/

        public static void ExecuteForMultiItems<T>(string procedureName, List<SqlParameter> parameters, List<T> items)
        {

            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }
                //---------------------------------------------------------------------
                PropertyInfo myPropInfo;
                Type t = typeof(T);
                myConnection.Open();
                //excute for all items
                foreach (T item in items)
                {
                    //fill all paremetrs with values
                    foreach (var p in parameters)
                    {
                        myPropInfo = t.GetProperty(p.ParameterName);
                        p.Value = myPropInfo.GetValue(item);
                    }
                    // Execute the command
                    myCommand.ExecuteNonQuery();
                }
                //---------------------------------------------------------------------
                myConnection.Close();
                //----------------------------------------------------------------

            }
        }
        #endregion

        #region --------------RetrieveEntityList--------------
        public static List<T> RetrieveEntityList<T>(string procedureName, CustomDbParameterList customParameters)
        {
            return RetrieveEntityList<T>(procedureName, customParameters.Parameters);
        }

        public static List<T> RetrieveEntityList<T>(string procedureName, List<SqlParameter> parameters)
        {
            List<T> itemsList = new List<T>();
            using (SqlConnection myConnection = GetSqlConnection())
            {

                SqlCommand myCommand = new SqlCommand(procedureName, myConnection);
                myCommand.CommandType = CommandType.StoredProcedure;
                // Set the parameters
                foreach (SqlParameter p in parameters)
                {
                    myCommand.Parameters.Add(p);
                }

                // Execute the command
                SqlDataReader dr;
                myConnection.Open();
                dr = myCommand.ExecuteReader();
                while (dr.Read())
                {
                    var item = GetEntity<T>(dr);
                    if (item != null)
                    {
                        itemsList.Add(item);
                    }
                }
                dr.Close();
                myConnection.Close();
                //----------------------------------------------------------------
                return itemsList;
            }
        }
        #endregion


        private static List<RecordSetDefinition> GenerateRecordSetDefinition(Type[] types)
        {
            var rsDefinitionManager = new RecordSetDefinitionManager();
            return rsDefinitionManager.GenerateRecordSetDefinition(types, null);

        }

        private static IList GetListOfDataFromDataReader(IDataReader dr, RecordSetDefinition t)
        {

            var itemsList = (IList)Activator.CreateInstance(t.Type);
            while (dr.Read())
            {
                var item = GetEntity(dr, t.GenericObjectType);
                if (item != null)
                {
                    itemsList.Add(item);
                }
            }
            return itemsList;
        }

        private static object GetSingleobjectFromDataReader(IDataReader dr, RecordSetDefinition t)
        {
            object item = null;
            while (dr.Read())
            {
                item = GetEntity(dr, t.Type);
                break;
            }
            return item;

        }




    }
}