using BankAPI.BankServices;
using BankAPI.Service;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAccountsController : ControllerBase
    {
        private IBankServices _bankService;
        public CustomerAccountsController(IBankServices bankServices)
        {
            _bankService =  bankServices;
        }

        
        // GET: api/<CustomerAccounts>
        [HttpGet]
        public IEnumerable<CustomerAccount> Get()
        {
            return _bankService.GetAllCustomerAccounts();
        }

        // GET api/<CustomerAccounts>/5
        [HttpGet("{id}")]
        public CustomerAccount Get(string id)
        {
            return _bankService.GetCustomerAccount(id);
        }


        // POST api/<CustomerAccounts>
        [HttpPost]
        public StatusCodeResult Post([FromBody] CustomerAccount customer)
        {
            _bankService.UpdateCustomerBalance(customer);
            return Ok();
        }

        //POST api/CustomerAccounts/Authenticate
        [HttpPost("{Authenticate}")]
        public StatusCodeResult AuthenticateCustomer([FromBody] CustomerAccount customer)
        {
            if (_bankService.AuthenticateCustomer(customer))
            {
                return Ok();
            }
            return NotFound();
        }

        // PUT api/<CustomerAccounts>
        [HttpPut]
        public StatusCodeResult Put([FromBody] CustomerAccount customer)
        {
            _bankService.CreateCustomer(customer);
            return Ok();
        }

        // DELETE api/<CustomerAccounts>
        [HttpDelete]
        public StatusCodeResult Delete([FromBody] CustomerAccount customer)
        {
            _bankService.DeleteCustomer(customer);
            return Ok();
        }
    }
}
