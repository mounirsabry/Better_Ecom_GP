using System.Collections.Generic;

namespace DataLibrary
{
    public interface IDataAccess
    {
        List<T> LoadData<T, U>(string sql, U parameters, string connectionString);
        int SaveData<T>(string sql, T parameters, string connectionString);
        List<int> SaveDataTransaction<T>(List<string> sqlList, List<T> parameters, string connectionString);
    }
}