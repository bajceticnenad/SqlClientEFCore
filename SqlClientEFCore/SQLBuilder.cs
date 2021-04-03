using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace SqlClientEFCore
{
    public static class SQLBuilder
    {
        public static DbContextOptions GetOptions(string connStr)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connStr).Options;
        }

        public static DbCommand LoadStoredProcedure(this DbContext dbContext, string storedProcedure)
        {
            var cmd = dbContext.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = storedProcedure;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return cmd;
        }
        public static DbCommand LoadSqlCommand(this DbContext dbContext, string sqlCommand)
        {
            var cmd = dbContext.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sqlCommand;
            cmd.CommandType = System.Data.CommandType.Text;
            return cmd;
        }
        public static DbCommand AddParam(this DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                throw new InvalidOperationException("Stored Procedure name is empty! Call LoadStoredProcedure before use this method!");
            }
            var param = cmd.CreateParameter();
            param.Direction = System.Data.ParameterDirection.Input;
            param.ParameterName = paramName;
            param.Value = paramValue;
            cmd.Parameters.Add(param);
            return cmd;
        }
        //public static DbCommand AddOutputParam(this DbCommand cmd, string paramName, object paramValue)
        //{
        //    if (string.IsNullOrEmpty(cmd.CommandText))
        //    {
        //        throw new InvalidOperationException("Stored Procedure name is empty! Call LoadStoredProcedure before use this method!");
        //    }
        //    var param = cmd.CreateParameter();
        //    param.Direction = System.Data.ParameterDirection.Output;
        //    param.ParameterName = paramName;
        //    param.Value = paramValue;
        //    cmd.Parameters.Add(param);
        //    return cmd;
        //}
        private static List<T> MapToList<T>(this DbDataReader dr)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());


            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props)
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                }
            }
            return objList;
        }

        #region SyncMethods
        public static List<T> ExecuteSqlQuery<T>(this DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.MapToList<T>();
                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
        public static object ExecuteSqlScalar(this DbCommand command)
        {
            object result;
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    result = command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }

                return result;
            }
        }
        public static int ExecuteSqlNonQuery(this DbCommand command)
        {
            int result = 0;
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    result = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }

                return result;
            }
        }
        #endregion SyncMethods

        #region AsyncMethods
        public static async Task<List<T>> ExecuteSqlQueryAsync<T>(this DbCommand command)
        {
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return reader.MapToList<T>();
                    }
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }
            }
        }
        public static async Task<object> ExecuteSqlScalarAsync(this DbCommand command)
        {
            object result;
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    result = await command.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }

                return result;
            }
        }
        public static async Task<int> ExecuteSqlNonQueryAsync(this DbCommand command)
        {
            int result = 0;
            using (command)
            {
                if (command.Connection.State == System.Data.ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    result = await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                finally
                {
                    command.Connection.Close();
                }

                return result;
            }
        }
        #endregion AsyncMethods


    }
}
