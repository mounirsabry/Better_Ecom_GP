using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataLibrary
{
    public class DataAccess : IDataAccess
    {
        public List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
        {

            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    var rows = connection.Query<T>(sql, parameters);
                    return rows.ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }

        public int SaveData<T>(string sql, T parameters, string connectionString)
        {
            int state = 0;
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    state = connection.Execute(sql, parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    state = -1;
                }
            }
            return state;
        }

        public List<int> SaveDataTransaction<T>(List<string> sqlList, List<T> parameters, string connectionString)
        {
            List<int> states = new List<int>();
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    for (int i = 0; i < sqlList.Count; i++)
                    {
                        states.Add(connection.Execute(sqlList[i], parameters[i]));
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                    transaction.Rollback();
                    states.Add(-1);
                }
                finally
                {
                    connection.Close();
                }
            }
            return states;
        }
    }
}
