using BankAPI.Model;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Service
{
    public class BankService
    {

        private SQLHandler sqlHandler;

        public BankService()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            var configuration = builder.Build();

            sqlHandler = new SQLHandler(configuration.GetConnectionString("DefaultConnectionString"));
        }

        private static string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public List<CustomerAccount> GetAllCustomerAccounts()
        {
            List<CustomerAccount> customers = new List<CustomerAccount>();
            DataTable customersDataTable = sqlHandler.ExecuteReader(SqlQueries.GetAllCustomers);

            foreach (DataRow dr in customersDataTable.Rows)
            {
                CustomerAccount customer = new CustomerAccount();
                customer.AccountId = dr["AccountId"].ToString();
                customer.Balance = (Single)dr["Balance"];
                customer.BankId = dr["BankId"].ToString();
                customer.Name = dr["Name"].ToString();
                customer.Password = dr["Password"].ToString();
                customer.IsActive = (SByte)dr["IsActive"];
                customers.Add(customer);

            }

            return customers;
        }

        public CustomerAccount GetCustomerAccount(string id)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", id));
            DataTable customerDataTable = sqlHandler.ExecuteReader(SqlQueries.GetCustomerAccount, parameterList);

            foreach (DataRow dr in customerDataTable.Rows)
            {
                CustomerAccount customer = new CustomerAccount();
                customer.AccountId = dr["AccountId"].ToString();
                customer.Balance = (Single)dr["Balance"];
                customer.BankId = dr["BankId"].ToString();
                customer.Name = dr["Name"].ToString();
                customer.Password = dr["Password"].ToString();
                customer.IsActive = (SByte)dr["IsActive"];
                return customer;
            }
            return new CustomerAccount();

        }

        public void UpdateCustomerBalance(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@newBalance", customer.Balance));
            parameterList.Add(new MySqlParameter("@AccountId", customer.AccountId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBalance, parameterList);
        }

        public void CreateCustomer(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            string accountId = $"{customer.Name.Substring(0, 3)}{GetDateTimeNow(true)}";
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@Balance", customer.Balance));
            parameterList.Add(new MySqlParameter("@Name", customer.Name));
            parameterList.Add(new MySqlParameter("@Password", customer.Password));
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoCustomersTable, parameterList);
        }


        public bool AuthenticateCustomer(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", customer.AccountId));
            parameterList.Add(new MySqlParameter("@Password", customer.Password));
            var dbOutput = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateCustomer, sqlParameters: parameterList);
            return dbOutput != null && dbOutput.ToString() == customer.AccountId;
        }

        public StaffAccount GetStaffAccount(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            DataTable staffDataTable = sqlHandler.ExecuteReader(SqlQueries.GetStaffAccount, parameterList);

            foreach (DataRow dr in staffDataTable.Rows)
            {
                StaffAccount staff = new StaffAccount();
                staff.AccountId = dr["AccountId"].ToString();
                staff.BankId = dr["BankId"].ToString();
                staff.Name = dr["Name"].ToString();
                staff.Password = dr["Password"].ToString();
                return staff;
            }

            return new StaffAccount();
        }

        public StaffAccount CreateStaffAccount(StaffAccount account)
        {
            string accountId = $"{account.Name.Substring(0, 3)}{GetDateTimeNow(true)}";
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@BankId", account.BankId));
            parameterList.Add(new MySqlParameter("@Name", account.Name));
            parameterList.Add(new MySqlParameter("@Password", account.Password));

            StaffAccount staff = new StaffAccount();
            staff.AccountId = accountId;
            staff.BankId = account.BankId;
            staff.Name = account.Name;
            staff.Password = account.Password;

            sqlHandler.ExecuiteNonQuery(SqlQueries.InsertIntoStaffsTable, sqlParameters: parameterList);


            return staff;
        }

        public bool AuthenticateStaff(StaffAccount staff)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@AccountId", staff.AccountId));
                parameterList.Add(new MySqlParameter("@Password", staff.Password));
                var dbOutput = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateStaff, sqlParameters: parameterList);
                return dbOutput != null && dbOutput.ToString() == staff.AccountId;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteCustomer(CustomerAccount customer)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@AccountId", customer.AccountId));
                return sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteCustomerAccount, sqlParameters: parameterList) == 1;
            }
            catch
            {
                return false;
            }
        }


        public DataTable GetTransactions(CustomerAccount customer)
        {
            try
            {
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@ReceiverId", customer.AccountId));
                parameterList.Add(new MySqlParameter("@SenderId", customer.AccountId));
                DataTable dataTable = sqlHandler.ExecuteReader(SqlQueries.GetTransactions, parameterList);
                return dataTable;
            }
            catch
            {
                return new DataTable();
            }
        }


        public string GetCharges(CustomerAccount customer, string id)
        {
            switch (id.ToLower())
            {
                case "srtgs":
                    return GetBanksRTGSCharges(customer);

                case "simps":
                    return GetBanksIMPSCharges(customer);

                case "ortgs":
                    return GetBankoRTGSCharges(customer);

                case "oimps":
                    return GetBankoIMPSCharges(customer);

                case "profits":
                    return GetBankProfits(customer);

                default:
                    return "Not Found !";
            }
        }

        private string GetBankProfits(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));
            var bankProfits = sqlHandler.ExecuiteScaler(SqlQueries.GetBankProfits, sqlParameters: parameterList);
            return bankProfits.ToString();
        }


        private string GetBanksRTGSCharges(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));
            var sRTGSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBanksRTGSCharges, sqlParameters: parameterList);
            return sRTGSValue.ToString();
        }
        private string GetBanksIMPSCharges(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));
            var sIMPSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBanksIMPSCharges, sqlParameters: parameterList);
            return sIMPSValue.ToString();
        }
        private string GetBankoRTGSCharges(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));
            var oRTGSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBankoRTGSCharges, sqlParameters: parameterList);
            return oRTGSValue.ToString();
        }
        private string GetBankoIMPSCharges(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@BankId", customer.BankId));
            var oIMPSValue = sqlHandler.ExecuiteScaler(SqlQueries.GetBankoIMPSCharges, sqlParameters: parameterList);
            return oIMPSValue.ToString();
        }


        public void UpdateDetails(string type, string value, string id)
        {
            switch (type.ToLower())
            {
                case "srtgs":
                    UpdatesRTGS(float.Parse(value),id);
                    break;
                case "simps":
                    UpdatesIMPS(float.Parse(value), id);
                    break;
                case "ortgs":
                    UpdateoRTGS(float.Parse(value), id);
                    break;
                case "oimps":
                    UpdateoIMPS(float.Parse(value), id);
                    break;
                case "name":
                    UpdateCustomerName(id, value);
                    break;
                case "password":
                    UpdateCustomerPassword(id, value);
                    break;
                case "revertTransaction":
                    RevertTransaction(id);
                    break;
            }
        }

        private int UpdateCustomerPassword(string accountId, string newPassword)
        {

            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newPassword));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatePassword, sqlParameters: parameterList);
        }

        private int UpdateCustomerName(string accountId, string newName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newName));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateName, sqlParameters: parameterList);
        }

        private int UpdatesRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesRTGS, sqlParameters: parameterList);
        }

        private int UpdatesIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sIMPSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesIMPS, sqlParameters: parameterList);
        }

        private int UpdateoRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoRTGS, sqlParameters: parameterList);
        }

        private int UpdateoIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oIMPSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoIMPS, sqlParameters: parameterList);
        }



        public bool RevertTransaction(string transactionId)
        {
            try
            {
                float transactionAmount = GetTransactionAmount(transactionId);
                string receiversId = GetTransactionReceiverId(transactionId);
                float receiversBalance = GetBalance(receiversId);
                switch (GetTransactionType(transactionId))
                {
                    case (int)Transaction.TransactionType.Deposit:
                        if (receiversBalance - transactionAmount >= 0)
                        {
                            float newBalance = receiversBalance - transactionAmount;
                            UpdateBalance(receiversId, newBalance);
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case (int)Transaction.TransactionType.Withdraw:
                        UpdateBalance(receiversId, receiversBalance + transactionAmount);
                        break;
                    case (int)Transaction.TransactionType.Transfer:
                        string sendersId = GetTransactionSenderId(transactionId);
                        float newBal = receiversBalance - transactionAmount;
                        UpdateBalance(receiversId, newBal);
                        float sendersBalance = GetBalance(sendersId);
                        UpdateBalance(sendersId, sendersBalance + transactionAmount);
                        break;
                }
                List<MySqlParameter> parameterList = new List<MySqlParameter>();
                parameterList.Add(new MySqlParameter("@TransactionId", transactionId));
                sqlHandler.ExecuiteNonQuery(SqlQueries.DeleteTransaction, sqlParameters: parameterList);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private float GetTransactionAmount(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));
            var transactionAmount = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionAmount, sqlParameters: parameterList);
            return float.Parse(transactionAmount.ToString());
        }

        private int GetTransactionType(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));
            var transactionType = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionType, sqlParameters: parameterList);
            return int.Parse(transactionType.ToString());
        }

        public string GetTransactionSenderId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));
            var senderId = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionSenderId, sqlParameters: parameterList);
            return senderId.ToString();
        }

        private string GetTransactionReceiverId(string transactionId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@TransactionId", transactionId));
            var receiverId = sqlHandler.ExecuiteScaler(SqlQueries.GetTransactionReceiverId, sqlParameters: parameterList);
            return receiverId.ToString();
        }

        private float GetBalance(string accountId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            return float.Parse(sqlHandler.ExecuiteScaler(SqlQueries.GetBalance, sqlParameters: parameterList).ToString());
        }

        private bool UpdateBalance(string accountId, float newBalance)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@newBalance", newBalance));
            parameterList.Add(new MySqlParameter("@AccountId", accountId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBalance, sqlParameters: parameterList);
            return true;
        }
    }

}
