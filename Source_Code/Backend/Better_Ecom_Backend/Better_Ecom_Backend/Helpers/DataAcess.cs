using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Better_Ecom_Backend
{
    public class DataAcess
    {
        public string ConnectionString { get; set; }

        public DataAcess(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public List<T> selectData<T,P>(string sql, P parameters)
        {
            using( IDbConnection connection = new MySqlConnection(ConnectionString) )
            {
                List<T> rows = connection.Query<T>(sql, parameters).ToList();

                return rows;
            }

        }

        public void nonSelect<P>(string sql, P parameters)
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Execute(sql, parameters);
            }

        }


        public bool update<T>(T obj) where T:class
        {
            using (IDbConnection connection = new MySqlConnection(ConnectionString))
            {
                return connection.Update(obj);
            }
        }
    }
}
