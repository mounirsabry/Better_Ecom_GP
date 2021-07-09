using System.Collections.Generic;

namespace DataLibrary
{
    public interface IDataAccess
    {
        List<T> LoadData<T, U>(string sql, U parameters, string connectionString);
        int SaveData<T>(string sql, T parameters, string connectionString);
    }
}