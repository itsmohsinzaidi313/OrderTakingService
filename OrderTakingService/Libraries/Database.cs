using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace OrderTakingService.Lib
{
    public static class Database
    {

        private static bool Testing = true;
        private static string ConnString
        {
            get
            {
                if (Testing)
                {
                    return ConfigurationManager.ConnectionStrings["ConnectionStringTest"].ConnectionString;
                }
                else
                {
                    return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
            }
        }

        public static string SecKey => ConfigurationManager.AppSettings.GetValues("SecKey")[0];

        private static SqlConnection GetSqlConnection()
        {
            return new SqlConnection
            {
                ConnectionString = ConnString,
            };
        }

        private static SqlCommand GetSqlCommand()
        {
            return new SqlCommand
            {
                Connection = GetSqlConnection()
            };
        }

        public static DataTable GetData(string Query)
        {
            DataTable data = new DataTable();
            SqlCommand sqlCmd = GetSqlCommand();
            sqlCmd.CommandText = Query;
            sqlCmd.Connection.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
            dataAdapter.Fill(data);
            sqlCmd.Connection.Close();
            return data;
        }

        public static int GetIntegerData(string Query)
        {
            SqlCommand sqlCmd = GetSqlCommand();
            sqlCmd.CommandText = Query;
            sqlCmd.Connection.Open();
            SqlDataReader dataReader = sqlCmd.ExecuteReader();
            int value = 0;
            while (dataReader.Read())
            {
                value = dataReader.GetInt32(0);
            }
            sqlCmd.Connection.Close();
            return value;
        }

        public static string GetStringData(string Query)
        {
            SqlCommand sqlCmd = GetSqlCommand();
            sqlCmd.CommandText = Query;
            sqlCmd.Connection.Open();
            SqlDataReader dataReader = sqlCmd.ExecuteReader();
            string value = string.Empty;
            while (dataReader.Read())
            {
                value = dataReader.GetString(0);
            }
            sqlCmd.Connection.Close();

            return value;
        }

        public static bool GetBoolData(string Query)
        {
            SqlCommand sqlCmd = GetSqlCommand();
            sqlCmd.CommandText = Query;
            sqlCmd.Connection.Open();
            SqlDataReader dataReader = sqlCmd.ExecuteReader();
            bool value = false;
            while (dataReader.Read())
            {
                value = dataReader.GetBoolean(0);
            }
            sqlCmd.Connection.Close();

            return value;
        }

        public static double GetDoubleData(string Query)
        {
            SqlCommand sqlCmd = GetSqlCommand();
            sqlCmd.CommandText = Query;
            sqlCmd.Connection.Open();
            SqlDataReader dataReader = sqlCmd.ExecuteReader();
            double value = 0.0;
            while (dataReader.Read())
            {
                value = dataReader.GetDouble(0);
            }
            sqlCmd.Connection.Close();
            return value;
        }

        public static DataTable ExecProc(string ProcName, string[] parameters)
         {
            try
            {
                DataTable data = new DataTable();
                SqlCommand sqlCmd = GetSqlCommand();
                sqlCmd.Connection.Open();
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = ProcName;
                DataTable procParameters = GetData("select PARAMETER_NAME, DATA_TYPE from information_schema.parameters where specific_name='" + ProcName + "'") ?? new DataTable();

                for (int i = 0; i < procParameters.Rows.Count; i++)
                {
                    sqlCmd.Parameters.AddWithValue(procParameters.Rows[i]["PARAMETER_NAME"].ToString(), parameters[i]);
                }
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCmd);
                dataAdapter.Fill(data);
                sqlCmd.Connection.Close();
                return data;
            }
            catch(Exception e)
            {
                return GetData($"{ProcName} @xml = '{parameters[0]}'");
            }
        }
    }
}