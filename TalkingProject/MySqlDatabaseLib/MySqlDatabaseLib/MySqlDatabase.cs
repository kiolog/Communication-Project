using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MySqlDatabaseLib
{
    public class MySqlDatabase
    {
        private MySqlConnection m_MySqlConnection;
        private object m_oDataBaseLock = new object();
        public MySqlDatabase()
        {

        }
        public void Connect(string _strHost, string _strDBUser, string _strDBPassword, string _strDBName)
        {
            string strConnectInfo = "server=" + _strHost + ";uid=" + _strDBUser + ";pwd=" + _strDBPassword + ";database=" + _strDBName;
            m_MySqlConnection = new MySqlConnection(strConnectInfo);
            try
            {
                m_MySqlConnection.Open();
                Console.WriteLine("ConnectToDataBase");
            }
            catch (MySql.Data.MySqlClient.MySqlException _MySqlException)
            {
                Console.WriteLine("Error : " + _MySqlException.Number + " " + _MySqlException.Message);
            }
        }
        public List<List<string>> GetQueryData(string _strQuery, List<string> _ListParameterName, List<object> _ListParameterValue)
        {
            List<List<string>> ListReturnString = new List<List<string>>();
            MySqlCommand NowSqlCommand = new MySqlCommand(_strQuery, m_MySqlConnection);
            if (_ListParameterName != null)
            {
                int iListCount = _ListParameterName.Count;
                for (int i = 0; i < iListCount; ++i)
                {
                    NowSqlCommand.Parameters.AddWithValue(_ListParameterName[i], _ListParameterValue[i]);
                }
            }
            try
            {
                MySqlDataReader NowReader = NowSqlCommand.ExecuteReader();
                int iFieldCount = NowReader.FieldCount;
                if (NowReader.HasRows)
                {
                    while (NowReader.Read())
                    {
                        List<string> ListNewLine = new List<string>();
                        for (int i = 0; i < iFieldCount; ++i)
                        {
                            ListNewLine.Add(NowReader.GetString(i));
                            //Console.WriteLine(SqlReader.GetString(i));
                        }
                        ListReturnString.Add(ListNewLine);
                    }
                }
                NowReader.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException _MySqlException)
            {
                Console.WriteLine("Error : " + _MySqlException.Number + " " + _MySqlException.Message);
            }
            return ListReturnString;

        }
        public void ExecuteQuery(string _strQuery, List<string> _ListParameterName, List<object> _ListParameterValue)
        {
            MySqlCommand NowSqlCommand = new MySqlCommand(_strQuery, m_MySqlConnection);
            int iListCount = _ListParameterName.Count;
            for (int i = 0; i < iListCount; ++i)
            {
                NowSqlCommand.Parameters.AddWithValue(_ListParameterName[i], _ListParameterValue[i]);
            }
            try
            {
                NowSqlCommand.ExecuteNonQuery();

            }
            catch (MySql.Data.MySqlClient.MySqlException _MySqlException)
            {
                Console.WriteLine("Error : " + _MySqlException.Number + " " + _MySqlException.Message);
            }
        }

    }
}
