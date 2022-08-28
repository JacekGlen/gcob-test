using System;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Entities;
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

        public async Task<Result<Customer>> AddCustomer(Customer customer)
        {
            try
            {
                await _customerRepository.InsertAsync(customer);
            }
            catch (Exception ex)
            {
                return Result<Customer>.Failure(ex);
            }

            return await GetCustomer(customer.Id);
        }

        public async Task<Result<Customer>> GetCustomer(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(customerId);
                return Result<Customer>.Success(customer);
            }
            catch (Exception ex)
            {
                return Result<Customer>.Failure(ex);
            }
            
        }
    }
}
