using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
using FluentAssertions;
using Moq;
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
            var customerToBeRetrieved = _fixture.Create<CustomerDto>();
            _fixture.Freeze<Mock<ICustomerRepository>>().Setup(x => x.GetAsync(customerToBeRetrieved.Id))
                .Returns(Task.FromResult(customerToBeRetrieved));
            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.GetCustomer(customerToBeRetrieved.Id);

            // Assert
            serviceResult.Succeeded.Should().BeTrue();
            serviceResult.Value.Should().BeEquivalentTo(customerToBeRetrieved);
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
            var customerToBeAdded = _fixture.Create<CustomerDto>(); 
            var mockRepo = _fixture.Freeze<Mock<ICustomerRepository>>();
            mockRepo.Setup(x => x.InsertAsync(customerToBeAdded)).Returns(Task.CompletedTask);
            mockRepo.Setup(x => x.GetAsync(customerToBeAdded.Id)).Returns(Task.FromResult(customerToBeAdded));
            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.AddCustomer(customerToBeAdded);

            // Assert
            serviceResult.Succeeded.Should().BeTrue();
            serviceResult.Value.Should().BeEquivalentTo(customerToBeAdded);
        }

        [TestMethod]
        public async Task AddCustomer_GivenTheCustomerAlreadyExists_WhenICallAddCustomer_ThenServiceReturnsErrorMessage()
        {
            // Arrange
            var customerToBeAdded = _fixture.Create<CustomerDto>();
            var errorMessage = $"Cannot insert customer with identity '{customerToBeAdded.Id}' as it already exists in the collection";
            var mockRepo = _fixture.Freeze<Mock<ICustomerRepository>>();
            mockRepo.Setup(x => x.InsertAsync(customerToBeAdded)).Throws(new Exception(errorMessage));
            mockRepo.Setup(x => x.GetAsync(customerToBeAdded.Id)).Returns(Task.FromResult(customerToBeAdded));
            var sut = _fixture.Create<CustomerService>();

            // Act
            var serviceResult = await sut.AddCustomer(customerToBeAdded);

            // Assert
            serviceResult.Succeeded.Should().BeFalse();
            serviceResult.Value.Should().BeNull();
            serviceResult.ErrorType.Should().Be(ErrorType.Exception);
            serviceResult.ErrorMessage.Should().Be(errorMessage);
        }
    }
}