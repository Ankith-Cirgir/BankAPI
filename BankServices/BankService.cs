using BankAPI.BankServices;
using BankAPI.Model;
using BankServices;
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
    public class BankService : IBankServices
    {
        private SQLHandler sqlHandler;

        public BankService()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            var configuration = builder.Build();

            sqlHandler = new SQLHandler(configuration.GetConnectionString("DefaultConnectionString"));
        }

        /// <summary>
        /// Returns Date and time in two different formats
        /// </summary>
        /// <param name="forId">(Bool) wheter the datetime is required for ID purpose or for storage purpose</param>
        /// <returns></returns>
        private static string GetDateTimeNow(bool forId)
        {
            return forId ? DateTime.Now.ToString("ddMMyyyyHHmmss") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// returns a list of customer objects in database
        /// </summary>
        /// <returns>A list of customer objects in database</returns>
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

        /// <summary>
        /// Returns a specific Customer object
        /// </summary>
        /// <param name="id">(String)AccountId of the customer whose details you want to fetch</param>
        /// <returns>A specific Customer object</returns>
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

        /// <summary>
        /// Used to update Cusotmer Balance
        /// </summary>
        /// <param name="customer">Customer object which has Customer AccountId and Updated Balance</param>
        public void UpdateCustomerBalance(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@newBalance", customer.Balance));
            parameterList.Add(new MySqlParameter("@AccountId", customer.AccountId));

            sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateBalance, parameterList);
        }

        /// <summary>
        /// Create a Customer Account
        /// </summary>
        /// <param name="customer"> a Customer Object which contains the details of the customer to be created </param>
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

        /// <summary>
        /// Authenticate Customer
        /// </summary>
        /// <param name="customer">Customer object with AccountId and Password</param>
        /// <returns>(Bool) If the Password matching the AccountId in the Local DB</returns>
        public bool AuthenticateCustomer(CustomerAccount customer)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", customer.AccountId));
            parameterList.Add(new MySqlParameter("@Password", customer.Password));
            var dbOutput = sqlHandler.ExecuiteScaler(SqlQueries.AuthenticateCustomer, sqlParameters: parameterList);
            return dbOutput != null && dbOutput.ToString() == customer.AccountId;
        }

        /// <summary>
        /// Get Staff Account Details
        /// </summary>
        /// <param name="accountId">AccountId of the Staff</param>
        /// <returns>StaffAccount object which has all the details if the staff</returns>
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

        /// <summary>
        /// Create a Staff Account
        /// </summary>
        /// <param name="account">StaffAccount with all the details which needs to be created</param>
        /// <returns>Staff Account object which is just created</returns>
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

        /// <summary>
        /// Authenticates Staff Credentials
        /// </summary>
        /// <param name="staff">StaffAccount Object with Details of AccountId and Password</param>
        /// <returns>(Bool) Whether the Credentials match the Password stored in Local DB</returns>
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

        /// <summary>
        /// Delete a Customer in Local DB
        /// </summary>
        /// <param name="customer"> Custome Account Object which has the details like AccountId and Passowrd</param>
        /// <returns>(Bool) If the Account is found and deleted from Local DB</returns>
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

        /// <summary>
        /// Get the Transactions made by a specific customer
        /// </summary>
        /// <param name="customer">CustomerAccount with Details like AccountId</param>
        /// <returns>DataTable object which holds the transaction details of the customer</returns>
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

        /// <summary>
        /// Get charges
        /// </summary>
        /// <param name="customer">Customer object which contains information of BankId</param>
        /// <param name="id">Enum value which determines the type of operation to be performed</param>
        /// <returns>value if BankId is found and "Not Found" if BankId is not in database</returns>
        public string GetCharges(CustomerAccount customer, string id)
        {
            switch ((GetCharge)Enum.Parse(typeof(GetCharge),id))
            {
                case GetCharge.SRTGS:
                    return GetBanksRTGSCharges(customer);

                case GetCharge.SIMPS:
                    return GetBanksIMPSCharges(customer);

                case GetCharge.ORTGS:
                    return GetBankoRTGSCharges(customer);

                case GetCharge.OIMPS:
                    return GetBankoIMPSCharges(customer);

                case GetCharge.PROFITS:
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

        /// <summary>
        /// Updated Details of a Bank or a Transaction
        /// </summary>
        /// <param name="type">what exactly needs to be changed</param>
        /// <param name="value">New value</param>
        /// <param name="id">BankId which need to be changed</param>
        /// <returns></returns>
        public bool UpdateDetails(string type, string value, string id)
        {
            switch (( UpdateCharges)Enum.Parse(typeof( UpdateCharges),type))
            {
                case  UpdateCharges.SRTGS:
                    return UpdatesRTGS(float.Parse(value),id);
                case  UpdateCharges.SIMPS:
                    return UpdatesIMPS(float.Parse(value), id);
                case  UpdateCharges.ORTGS:
                    return UpdateoRTGS(float.Parse(value), id);
                case  UpdateCharges.OIMPS:
                    return UpdateoIMPS(float.Parse(value), id);
                case  UpdateCharges.NAME:
                    return UpdateCustomerName(id, value);
                case  UpdateCharges.PASSWORD:
                    return UpdateCustomerPassword(id, value);
                case  UpdateCharges.REVERT_TRANSACTION:
                    return RevertTransaction(id);
                default:
                    return false;
            }
        }

        private bool UpdateCustomerPassword(string accountId, string newPassword)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newPassword));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatePassword, sqlParameters: parameterList)==1? true : false;
        }

        private bool UpdateCustomerName(string accountId, string newName)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@AccountId", accountId));
            parameterList.Add(new MySqlParameter("@newName", newName));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateName, sqlParameters: parameterList) == 1 ? true : false;
        }

        private bool UpdatesRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesRTGS, sqlParameters: parameterList) == 1 ? true : false;
        }

        private bool UpdatesIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@sIMPSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdatesIMPS, sqlParameters: parameterList) == 1 ? true : false;
        }

        private bool UpdateoRTGS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oRTGSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoRTGS, sqlParameters: parameterList) == 1 ? true : false;
        }

        private bool UpdateoIMPS(float currencyValue, string bankId)
        {
            List<MySqlParameter> parameterList = new List<MySqlParameter>();
            parameterList.Add(new MySqlParameter("@oIMPSCharge", currencyValue));
            parameterList.Add(new MySqlParameter("@BankId", bankId));
            return sqlHandler.ExecuiteNonQuery(SqlQueries.UpdateoIMPS, sqlParameters: parameterList) == 1 ? true : false;
        }


        private bool RevertTransaction(string transactionId)
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

        private string GetTransactionSenderId(string transactionId)
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
