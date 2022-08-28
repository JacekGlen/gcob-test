using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
using FluentAssertions;
using Moq;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Entities;
using Rabobank.TechnicalTest.GCOB.Enums;
using Rabobank.TechnicalTest.GCOB.Repositories;
using Rabobank.TechnicalTest.GCOB.Services;

namespace Rabobank.TechnicalTest.GCOB.Tests.Services
{
    [TestClass]
    public class CustomerServiceTest
    {
        private IFixture _fixture;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<ILogger>>();
        }


        [TestMethod]
        public async Task GetCustomer_GivenTheCustomerExistsInDB_WhenICallGetCustomer_ThenTheCustomerIsReturned()
        {
            // Arrange
            var country = _fixture.Create<Country>();
            var address = _fixture.Build<Address>().With(a => a.CountryId, country.Id).Create();
            var customer = _fixture.Build<Customer>().With(c => c.AddressId, address.Id).Create();
            _fixture.Freeze<Mock<ICustomerRepository>>().Setup(x => x.GetAsync(customer.Id))
                .Returns(Task.FromResult(customer));
            _fixture.Freeze<Mock<IAddressRepository>>().Setup(x => x.GetAsync(address.Id))
                .Returns(Task.FromResult(address));
            _fixture.Freeze<Mock<ICountryRepository>>().Setup(x => x.GetAsync(country.Id))
                .Returns(Task.FromResult(country));

            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.GetCustomer(customer.Id);

            // Assert
            serviceResult.Succeeded.Should().BeTrue();
            var customerDto = serviceResult.Value;
            customerDto.Should().NotBeNull();
            customerDto.City.Should().Be(address.City);
            customerDto.Country.Should().Be(country.Name);
            customerDto.FullName.Should().Be($"{customer.FirstName} {customer.LastName}");
            customerDto.Id.Should().Be(customer.Id);
            customerDto.Postcode.Should().Be(address.Postcode);
            customerDto.Street.Should().Be(address.Street);
        }

        [TestMethod]
        public async Task GetCustomer_GivenTheCustomerDoesNotExist_WhenICallGetCustomer_ThenAnErrorMessageIsReturned()
        {
            // Arrange
            var customerId = _fixture.Create<int>();
            var errorMessage = customerId.ToString();
            _fixture.Freeze<Mock<ICustomerRepository>>().Setup(x => x.GetAsync(customerId))
                .Throws(new Exception(errorMessage));
            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.GetCustomer(customerId);

            // Assert
            serviceResult.Succeeded.Should().BeFalse();
            serviceResult.Value.Should().BeNull();
            serviceResult.ErrorType.Should().Be(ErrorType.Exception);
            serviceResult.ErrorMessage.Should().Be(errorMessage);
        }



        [TestMethod]
        public async Task AddCustomer_GivenTheCustomerDoesNotExist_WhenICallAddCustomer_ThenTheCustomerIsSavedToDb_AndTheCustomerIsReturned()
        {
            // Arrange
            var countries = new List<Country>();
            _fixture.AddManyTo(countries, 3);
            var country = countries.First();
            
            var customerCreateRequest =
                _fixture.Build<CustomerCreateRequest>().With(c => c.Country, country.Name).Create();
            var address = new Address()
            {
                City = customerCreateRequest.City,
                CountryId = country.Id,
                Id = _fixture.Create<int>(),
                Postcode = customerCreateRequest.Postcode,
                Street = customerCreateRequest.Street
            };

            var customer = new Customer()
            {
                Id = _fixture.Create<int>(),
                AddressId = address.Id,
                FirstName = customerCreateRequest.Firstname,
                LastName = customerCreateRequest.Lastname
            };
            
            var mockCustomerRepo = _fixture.Freeze<Mock<ICustomerRepository>>();
            mockCustomerRepo.Setup(x => x.InsertAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            mockCustomerRepo.Setup(x => x.GetAsync(customer.Id)).Returns(Task.FromResult(customer));
            mockCustomerRepo.Setup(x => x.GenerateIdentityAsync()).Returns(Task.FromResult(customer.Id));

            var mockAddressRepo = _fixture.Freeze<Mock<IAddressRepository>>();
            mockAddressRepo.Setup(x => x.InsertAsync(It.IsAny<Address>())).Returns(Task.CompletedTask);
            mockAddressRepo.Setup(x => x.GetAsync(address.Id)).Returns(Task.FromResult(address));
            mockAddressRepo.Setup(x => x.GenerateIdentityAsync()).Returns(Task.FromResult(address.Id));

            var mockCountryRepo = _fixture.Freeze<Mock<ICountryRepository>>();
            mockCountryRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(countries.AsEnumerable()));
            mockCountryRepo.Setup(x => x.GetAsync(country.Id)).Returns(Task.FromResult(country));
                                                                       
            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.AddCustomer(customerCreateRequest);

            // Assert
            // Assert
            serviceResult.Succeeded.Should().BeTrue();
            var customerDto = serviceResult.Value;
            customerDto.Should().NotBeNull();
            customerDto.City.Should().Be(address.City);
            customerDto.Country.Should().Be(country.Name);
            customerDto.FullName.Should().Be($"{customer.FirstName} {customer.LastName}");
            customerDto.Id.Should().Be(customer.Id);
            customerDto.Postcode.Should().Be(address.Postcode);
            customerDto.Street.Should().Be(address.Street);
        }

        [TestMethod]
        public async Task AddCustomer_GivenTheCountryDoesNotExist_WhenICallAddCustomer_ThenServiceReturnsErrorMessage()
        {
            // Arrange
            var countries = new List<Country>();
            _fixture.AddManyTo(countries, 3);
            var country = countries.First();

            var customerCreateRequest =
                _fixture.Build<CustomerCreateRequest>().With(c => c.Country, "Dummy Country").Create();
          
            _fixture.Freeze<Mock<ICustomerRepository>>();
            _fixture.Freeze<Mock<IAddressRepository>>();
            

            var mockCountryRepo = _fixture.Freeze<Mock<ICountryRepository>>();
            mockCountryRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(countries.AsEnumerable()));
            mockCountryRepo.Setup(x => x.GetAsync(country.Id)).Returns(Task.FromResult(country));

            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.AddCustomer(customerCreateRequest);

            // Assert
            serviceResult.Succeeded.Should().BeFalse();
            serviceResult.Value.Should().BeNull();
            serviceResult.ErrorType.Should().Be(ErrorType.ValidationError);
            serviceResult.ErrorMessage.Should().Be($"Country '{customerCreateRequest.Country}' does not exist!");
        }
    }
}
