using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace BankAPI
{
    class SQLHandler
    {
        private readonly string ConnectionString;

        public SQLHandler(string connectionString)
        {
            ConnectionString = connectionString;
            Init();
        }

        private void Init()
        {
            if (int.Parse(ExecuiteScaler(SqlQueries.CheckTabelsExist).ToString()) == 0)
            {
                CreateTables();
            }
        }

        public void CreateTables()
        {
            ExecuiteNonQuery(SqlQueries.CreateBanksTable);
            ExecuiteNonQuery(SqlQueries.CreateCustomerAccountsTable);
            ExecuiteNonQuery(SqlQueries.CreateStaffAccountsTable);
            ExecuiteNonQuery(SqlQueries.CreateTransactionsTable);
            ExecuiteNonQuery(SqlQueries.CreateCurrencyTable);
        }

        public DataTable ExecuteReader(string query, [Optional] List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                using (MySqlDataAdapter adr = new MySqlDataAdapter())
                {
                    try
                    {
                        adr.SelectCommand = new MySqlCommand(query, conn);
                        if (sqlParameters != null)
                        {
                            adr.SelectCommand.Parameters.AddRange(sqlParameters.ToArray());
                        }
                        DataTable dt = new DataTable();
                        adr.Fill(dt);
                        return dt;
                    }
                    catch (Exception e)
                    {
                        return new DataTable();
                    }

                }
            }
        }

        public int ExecuiteNonQuery(string query, [Optional] List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (sqlParameters != null)
                    {
                        cmd.Parameters.AddRange(sqlParameters.ToArray());
                    }
                    cmd.Connection.Open();

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public object ExecuiteScaler(string query, [Optional] List<MySqlParameter> sqlParameters)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (sqlParameters != null)
                    {
                        cmd.Parameters.AddRange(sqlParameters.ToArray());
                    }
                    cmd.Connection.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}