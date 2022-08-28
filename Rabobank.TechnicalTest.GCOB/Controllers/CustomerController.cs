using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Enums;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _customerService.GetCustomer(id);
            
            if(result.Succeeded)
                return Ok(result.Value);
            
            if(result.ErrorType == ErrorType.ValidationError)
                return BadRequest(result.ErrorMessage);
            
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        public Task<IActionResult> Post(CustomerCreateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
