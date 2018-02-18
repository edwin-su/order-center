using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Utilities
{
    public class Parameter
    {
        public Parameter() { }

        public Parameter(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; set; }

        public object Value { get; set; }
    }

    public class TransactionParameter
    {
        public string SQL { get; set; }

        public List<Parameter> Parameters { get; set; }
    }

    public static class DBHelper
    {
        private static string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static bool ExecuteNonQuery(string sql, params Parameter[] parameters)
        {
            return Execute<bool>(sql, parameters, sqlCommand =>
            {
                return sqlCommand.ExecuteNonQuery() > 0;
            });
        }

        public static T ExecuteDataReader<T>(string sql, Parameter[] parameters, Func<MySqlDataReader, T> func)
        {
            return Execute<T>(sql, parameters, sqlCommand =>
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    return func(reader);
                }
            });
        }

        public static T ExecuteScalar<T>(string sql, params Parameter[] parameters)
        {
            return Execute<T>(sql, parameters, sqlCommand =>
            {
                return (T)sqlCommand.ExecuteScalar();
            });
        }

        private static T Execute<T>(string sql, Parameter[] parameters, Func<MySqlCommand, T> func)
        {
            try
            {
                using (var conn = new MySqlConnection(CONNECTION_STRING))
                {
                    using (var comm = new MySqlCommand(sql, conn))
                    {
                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var item in parameters)
                            {
                                if (item.Value == null)
                                {
                                    var tempParameter = comm.Parameters.AddWithValue(item.Key, DBNull.Value);
                                    tempParameter.IsNullable = true;
                                }
                                else
                                {
                                    comm.Parameters.AddWithValue(item.Key, item.Value);
                                }
                            }
                        }
                        conn.Open();
                        return func(comm);
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: log
                throw;
            }
        }

        private static void Execute(Action<MySqlConnection, MySqlTransaction> action)
        {
            Execute(conn =>
            {
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        if (action != null)
                        {
                            action(conn, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        //TODO: log
                        throw;
                    }
                }
            });
        }

        private static void Execute(Action<MySqlConnection> action)
        {
            using (var conn = new MySqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                try
                {
                    if (action != null)
                    {
                        action(conn);
                    }
                }
                catch (Exception e)
                {
                    //TODO: log
                    throw;
                }
            }
        }

        private static DataTable ExecuteAdapter(string sql, params Parameter[] parameters)
        {
            return Execute<DataTable>(sql, parameters, comm =>
            {
                using (var adapter = new MySqlDataAdapter(comm))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            });
        }

        public static void ExecuteTransaction(List<TransactionParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                Execute((conn, tran) =>
                {
                    var comm = new MySqlCommand();
                    comm.Connection = conn;
                    comm.Transaction = tran;
                    parameters.ForEach(p =>
                    {
                        comm.CommandText = p.SQL;
                        if (p.Parameters != null)
                        {
                            p.Parameters.ForEach(item =>
                            {
                                if (item.Value == null)
                                {
                                    var tempParameter = comm.Parameters.AddWithValue(item.Key, DBNull.Value);
                                    tempParameter.IsNullable = true;
                                }
                                else
                                {
                                    comm.Parameters.AddWithValue(item.Key, item.Value);
                                }
                            });
                            comm.ExecuteNonQuery();
                            comm.Parameters.Clear();
                        }
                    });
                });
            }
        }

        /// <summary>
        /// example : 
        ///void invoke()
        ///{
        ///    DBHelper.ExecuteBulkCopy("userinfo", table =>
        ///    {
        ///        for (int i = 0; i < 1000000; i++)
        ///        {
        ///            var row = table.NewRow();
        ///            row["name"] = "leo";
        ///            row["age"] = 25;
        ///            row["email"] = "leo@leo.com";
        ///            row["test"]=null
        ///            table.Rows.Add(row);
        ///        }
        ///    });
        ///}
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="action"></param>
        public static void ExecuteBulkCopy(string tableName, Action<DataTable> action)
        {
            //var table = ExecuteAdapter("select top 0 * from " + tableName + " with (nolock)", null);

            //if (action != null)
            //{
            //    action(table);
            //}

            //Execute((conn, transaction) =>
            //{
            //    using (var bulkCopy = new MySqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
            //    {
            //        bulkCopy.DestinationTableName = tableName;
            //        bulkCopy.BatchSize = 800;
            //        bulkCopy.WriteToServer(table);
            //    }
            //});
        }
    }
}