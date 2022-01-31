using BankAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankAPI.BankServices
{
    public interface IBankServices
    {
        public List<CustomerAccount> GetAllCustomerAccounts();
        public CustomerAccount GetCustomerAccount(string id);
        public void UpdateCustomerBalance(CustomerAccount customer);
        public void CreateCustomer(CustomerAccount customer);
        public bool AuthenticateCustomer(CustomerAccount customer);
        public StaffAccount GetStaffAccount(string accountId);
        public StaffAccount CreateStaffAccount(StaffAccount account);
        public bool AuthenticateStaff(StaffAccount staff);
        public bool DeleteCustomer(CustomerAccount customer);
        public DataTable GetTransactions(CustomerAccount customer);
        public string GetCharges(CustomerAccount customer, string id);
        public bool UpdateDetails(string type, string value, string id);
    }
}
