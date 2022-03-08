using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Repository.Conexion
{
    public class BaseRepository
    {
        string _connectionString;

        public BaseRepository(IConfiguration _configuration)
        {
            //configuration = _configuration;
            var host = _configuration["DATABASE_ORACLE_HOST"];
            var port = _configuration["DATABASE_ORACLE_PORT"];
            var password = _configuration["DATABASE_ORACLE_PASSWORD"];
            var userid = _configuration["DATABASE_ORACLE_USER"];
            var databaseServiceName = _configuration["DATABASE_ORACLE_SERVICE_NAME"];

            _connectionString = $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={databaseServiceName})));User Id={userid};Password={password};pooling=false";
            //_connectionString = configuration.GetSection("ConnectionStrings").GetSection("EmployeeConnection").Value;
        }

        public List<T> Query<T>(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.Query<T>(query, parameters, commandType: CommandType.StoredProcedure).ToList()
                    : connection.Query<T>(query, parameters).ToList();

                connection.Close();

                return collection;
            }
        }

        public string Query(string query, object parameters = null, bool isProc = true)//Ejecuta stored o sentencias que no devuelven un resultado
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                if (isProc)
                {
                    connection.Query(query, parameters, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    connection.Query(query, parameters);
                }

                connection.Close();

                return "OK";
            }
        }

        public List<T> Function<T>(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.Query<T>(query, parameters, commandType: CommandType.Text).ToList()
                    : connection.Query<T>(query, parameters).ToList();

                connection.Close();

                return collection;
            }
        }

        public T FirstOrDefault<T>(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var entity = isProc
                    ? connection.QueryFirstOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryFirstOrDefault<T>(query, parameters);

                connection.Close();

                return entity;
            }
        }

        public void FirstObjectSecondList<TFirst, TSecond>(string query, ref TFirst param1, ref List<TSecond> param2, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().First();
                param2 = collection.Read<TSecond>().ToList();

                connection.Close();
            }
        }

        public void QueryMultiple<TFirst, TSecond>(string query, ref List<TFirst> param1, ref List<TSecond> param2, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().ToList();
                param2 = collection.Read<TSecond>().ToList();
                connection.Close();
            }
        }

        public void QueryMultiple<TFirst, TSecond, TThird>(string query, ref List<TFirst> param1, ref List<TSecond> param2, ref List<TThird> param3, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().ToList();
                param2 = collection.Read<TSecond>().ToList();
                param3 = collection.Read<TThird>().ToList();
                connection.Close();
            }
        }

        public void QueryMultiple<TFirst, TSecond, TThird, TFourth>(string query, ref List<TFirst> param1, ref List<TSecond> param2, ref List<TThird> param3, ref List<TFourth> param4, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().ToList();
                param2 = collection.Read<TSecond>().ToList();
                param3 = collection.Read<TThird>().ToList();
                param4 = collection.Read<TFourth>().ToList();
                connection.Close();
            }
        }

        public void QueryMultiple<TFirst, TSecond, TThird, TFourth, TFive>(string query, ref List<TFirst> param1, ref List<TSecond> param2, ref List<TThird> param3, ref List<TFourth> param4, ref List<TFive> param5, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().ToList();
                param2 = collection.Read<TSecond>().ToList();
                param3 = collection.Read<TThird>().ToList();
                param4 = collection.Read<TFourth>().ToList();
                param5 = collection.Read<TFive>().ToList();
                connection.Close();
            }
        }

        public void QueryMultiple<TFirst, TSecond, TThird, TFourth, TFive, TSix>(string query, ref List<TFirst> param1, ref List<TSecond> param2, ref List<TThird> param3, ref List<TFourth> param4, ref List<TFive> param5, ref List<TSix> param6, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var collection = isProc
                    ? connection.QueryMultiple(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.QueryMultiple(query, parameters);

                param1 = collection.Read<TFirst>().ToList();
                param2 = collection.Read<TSecond>().ToList();
                param3 = collection.Read<TThird>().ToList();
                param4 = collection.Read<TFourth>().ToList();
                param5 = collection.Read<TFive>().ToList();
                param6 = collection.Read<TSix>().ToList();
                connection.Close();
            }
        }

        public int Execute(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var affectedRows = isProc
                    ? connection.Execute(query, parameters, commandType: CommandType.StoredProcedure)
                    : connection.Execute(query, parameters);

                connection.Close();

                return affectedRows;
            }
        }

        public DataTable ExecuteDT(string query, List<OracleParameter> parameters, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = connection;

                cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;
                cmd.InitialLONGFetchSize = 1000;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (OracleParameter parm in parameters)
                {
                    cmd.Parameters.Add(parm);
                }
                da.SelectCommand = cmd;
                DataTable dt = new DataTable();
                da.Fill(dt);

                connection.Close();

                return dt;

            }
        }

        public DataTable ExecuteDTS(string query, List<OracleParameter> parameters, int IdCur, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                OracleDataAdapter da = new OracleDataAdapter();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = connection;

                cmd.CommandType = isProc ? CommandType.StoredProcedure : CommandType.Text;
                cmd.InitialLONGFetchSize = 1000;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (OracleParameter parm in parameters)
                {
                    cmd.Parameters.Add(parm);
                }
                da.SelectCommand = cmd;
                da.TableMappings.Add("table", "curCab");
                da.TableMappings.Add("table2", "curFicha");
                da.TableMappings.Add("table3", "curFhtri");
                DataSet dt = new DataSet();
                da.Fill(dt);

                dt.Tables[0].TableName = "Tabla1";
                dt.Tables[1].TableName = "Tabla2";
                dt.Tables[2].TableName = "Tabla3";

                connection.Close();

                return dt.Tables[IdCur];
            }
        }

        public string CadenaConexion()
        {
            return _connectionString;
        }

        public void ExecuteVoid(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();

                var affectedRows = isProc
                     ? connection.Execute(query, parameters, commandType: CommandType.StoredProcedure)
                     : connection.Execute(query, parameters);

                connection.Close();
            }
        }

        public int QueryRowAffected(string query, object parameters = null, bool isProc = true)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                connection.Open();
                int affectedRows = 0;
                if (isProc)
                {
                    affectedRows = connection.Query<int>(query, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                }
                else
                {
                    affectedRows = connection.Query<int>(query, parameters).SingleOrDefault();
                }

                connection.Close();

                return affectedRows;
            }
        }

    }
}

