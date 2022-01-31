using BankAPI.BankServices;
using BankAPI.Model;
using BankAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffAccountsController : ControllerBase
    {
        private IBankServices _bankService;
        public StaffAccountsController(IBankServices bankServices)
        {
            _bankService = bankServices;
        }

        // GET api/<StaffAccountsController>/5
        [HttpGet("{id}")]
        public StaffAccount Get(string id)
        {
            return _bankService.GetStaffAccount(id);
        }

        // POST api/<StaffAccountsController>
        [HttpPost]
        public StatusCodeResult Post([FromBody] StaffAccount staff)
        {
            if (_bankService.AuthenticateStaff(staff))
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        // PUT api/<StaffAccountsController>
        [HttpPut]
        public StatusCodeResult Put([FromBody] StaffAccount staff)
        {
            _bankService.CreateStaffAccount(staff);
            return Ok();
        }

    }
}
