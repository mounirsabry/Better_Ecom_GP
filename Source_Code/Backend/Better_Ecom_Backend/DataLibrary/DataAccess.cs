using Dapper;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataLibrary
{
    public class DataAccess : IDataAccess
    {
        public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = connection.Query<T>(sql, parameters);
                return rows.ToList();
            }
        }

        public void SaveData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Execute(sql, parameters);
            }
        }
    }
}
