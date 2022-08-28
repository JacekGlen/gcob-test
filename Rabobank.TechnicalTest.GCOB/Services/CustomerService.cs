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

            return await GetCustomer(customer.Id);
        }

        public async Task<Result<CustomerDto>> GetCustomer(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(customerId);
                return Result<CustomerDto>.Success(customer);
            }
            catch (Exception ex)
            {
                return Result<CustomerDto>.Failure(ex);
            }
            
        }
    }
}
