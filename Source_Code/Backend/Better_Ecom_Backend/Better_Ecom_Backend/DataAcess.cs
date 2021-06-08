using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend
{
    public static class DataAcess
    {
        
        public static List<T> LoadData<T,P>(string sql, P parameters, string connectionString )
        {
            using( IDbConnection connection = new MySqlConnection(connectionString) )
            {
                List<T> rows = connection.Query<T>(sql, parameters).ToList();

                return rows;
            }

        }

        public static void saveData<P>(string sql, P parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Execute(sql, parameters);
            }

        }
    }
}
