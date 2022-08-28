using System;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<CustomerDto>> AddCustomer(CustomerDto customer)
        {
            try
            {
                await _customerRepository.InsertAsync(customer);
            }
            catch (Exception ex)
            {
                return Result<CustomerDto>.Failure(ex);
            }

            var customerFromDb = await _customerRepository.GetAsync(customer.Id);
            return Result<CustomerDto>.Success(customerFromDb);
        }
    }
}
