using BankAPI.BankServices;
using BankAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffOperationsController : ControllerBase
    {
        private IBankServices _bankService;
        public StaffOperationsController(IBankServices bankServices)
        {
            _bankService = bankServices;
        }

        // GET: api/<StaffOperations>
        [HttpGet]
        public DataTable Get(CustomerAccount customer)
        {
            return _bankService.GetTransactions(customer);
        }

        // GET api/<StaffOperations>/5
        [HttpGet("{id}")]
        public string Get(string id, CustomerAccount customer)
        {
            return _bankService.GetCharges(customer, id);
        }

        // POST api/<StaffOperations>
        [HttpPost("{type}")][RequireHttps]
        public void Post(string type, [FromForm] string value, [FromForm] string id)
        {
            _bankService.UpdateDetails(type, value, id);
        }

        // PUT api/<StaffOperations>/5
        [HttpPut]
        public void Put([FromBody] CustomerAccount customer)
        {
            _bankService.CreateCustomer(customer);
        }

        // DELETE api/<StaffOperations>/5
        [HttpDelete]
        public StatusCodeResult Delete(CustomerAccount customer)
        {
            if (_bankService.DeleteCustomer(customer))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
