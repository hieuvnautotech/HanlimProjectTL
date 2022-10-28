using Dapper;
using System.Data;
using System.Data.SqlClient;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.DbAccess
{
    public interface ISqlDataAccess
    {

        #region Stored Procedure
        Task<IEnumerable<T>> LoadDataUsingStoredProcedure<T>(string storedProcedure, DynamicParameters? parameters = null);
        Task<string> SaveDataUsingStoredProcedure<T>(string storedProcedure, DynamicParameters parameters);
        #endregion

        #region Raw Query
        Task<IEnumerable<T>> LoadDataUsingRawQuery<T>(string rawQuery, DynamicParameters? parameters = null); 
        #endregion
    }

    [SingletonRegistration]
    public class SqlDataAccess : ISqlDataAccess
    {
        //private readonly IConfiguration _configuration;
        private static readonly string connectionString = ConnectionString.CONNECTIONSTRING;

        //public SqlDataAccess(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        #region Stored Procedure
        //Used for getting gatas (select query) from database
        public async Task<IEnumerable<T>> LoadDataUsingStoredProcedure<T>(string storedProcedure, DynamicParameters? parameters = null)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);

            return await dbConnection.QueryAsync<T>(storedProcedure, parameters, transaction: null, commandTimeout: 20, commandType: CommandType.StoredProcedure);
        }

        //Used for saving datas (insert/update/delete) into database
        public async Task<string> SaveDataUsingStoredProcedure<T>(string storedProcedure, DynamicParameters parameters)
        {
            string result = string.Empty;
            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();
                using IDbTransaction tran = dbConnection.BeginTransaction();
                try
                {
                    await dbConnection.ExecuteAsync(storedProcedure, parameters, transaction: tran, commandTimeout: 20, commandType: CommandType.StoredProcedure);
                    tran.Commit();
                    result = parameters.Get<string?>("@output") ?? string.Empty;
                }
                catch (Exception)
                {
                    tran.Rollback();
                }
            }

            return result;
        }
        #endregion

        #region Raw Query
        //Used for getting gatas (select query) from database
        public async Task<IEnumerable<T>> LoadDataUsingRawQuery<T>(string rawQuery, DynamicParameters? parameters = null)
        {
            using IDbConnection dbConnection = new SqlConnection(connectionString);

            return await dbConnection.QueryAsync<T>(rawQuery, parameters, transaction: null, commandTimeout: 20, commandType: CommandType.Text);
        }
        #endregion
    }
}
