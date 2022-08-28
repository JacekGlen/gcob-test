using System;
using System.Linq;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Entities;
using Rabobank.TechnicalTest.GCOB.Enums;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICountryRepository _countryRepository;

        public CustomerService(ICustomerRepository customerRepository,
            IAddressRepository addressRepository,
            ICountryRepository countryRepository)
        {
            _customerRepository = customerRepository;
            _addressRepository = addressRepository;
            _countryRepository = countryRepository;
        }

        public async Task<Result<CustomerDto>> AddCustomer(CustomerCreateRequest customerCreateRequest)
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                var country = countries.FirstOrDefault(x =>
                    x.Name.Equals(customerCreateRequest.Country, StringComparison.InvariantCultureIgnoreCase));
                
                if (country == null)
                {
                    return Result<CustomerDto>.Failure($"Country '{customerCreateRequest.Country}' does not exist!",
                        ErrorType.ValidationError);
                }

                var addressId = await _addressRepository.GenerateIdentityAsync();

                var address = new Address()
                {
                    City = customerCreateRequest.City,
                    CountryId = country.Id,
                    Id = addressId,
                    Postcode = customerCreateRequest.Postcode,
                    Street = customerCreateRequest.Street
                };

                await _addressRepository.InsertAsync(address);
                var customerId = await _customerRepository.GenerateIdentityAsync();

                var customer = new Customer()
                {
                    AddressId = address.Id,
                    FirstName = customerCreateRequest.Firstname,
                    LastName = customerCreateRequest.Lastname,
                    Id = customerId
                };
                await _customerRepository.InsertAsync(customer);
                return await GetCustomer(customerId);
            }
            catch (Exception ex)
            {
                return Result<CustomerDto>.Failure(ex);
            }
        }

        public async Task<Result<CustomerDto>> GetCustomer(int customerId)
        {
            try
            {
                var customer = await _customerRepository.GetAsync(customerId);
                var address = await _addressRepository.GetAsync(customer.AddressId);
                var country = await _countryRepository.GetAsync(address.CountryId);
                var customerDto = new CustomerDto()
                {
                    City = address.City,
                    Country = country.Name,
                    FullName = $"{customer.FirstName} {customer.LastName}",
                    Id = customer.Id,
                    Postcode = address.Postcode,
                    Street = address.Street
                };

                return Result<CustomerDto>.Success(customerDto);
            }
            catch (Exception ex)
            {
                return Result<CustomerDto>.Failure(ex);
            }
            
        }
    }
}
