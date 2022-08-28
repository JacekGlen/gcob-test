using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rabobank.TechnicalTest.GCOB.Dtos;
using Rabobank.TechnicalTest.GCOB.Entities;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Tests.Repositories
{
    [TestClass]
    public class CustomerRepositoryTest
    {
        private IFixture _fixture;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<ILogger>>();
        }


        [TestMethod]
        public async Task GetAsync_GivenHaveACustomer_WhenIGetTheCustomerFromDB_ThenTheCustomerIsRetrieved()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();

            var expectedCustomer = _fixture.Create<Customer>();
            await sut.InsertAsync(expectedCustomer);

            // Act
            var result = await sut.GetAsync(expectedCustomer.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedCustomer);
        }

        [TestMethod]
        public async Task GetAsync_GivenNoCustomerExistsInDB_WhenIGetTheCustomerFromDB_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var id = _fixture.Create<int>();

            // Act
            Func<Task> act = async () => await sut.GetAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.Equals(id.ToString(), StringComparison.InvariantCulture));
        }

        [TestMethod]
        public async Task InsertAsync_GivenTheCustomerDoesNotExistInDB_WhenIInsertTheCustomerIntoDB_TheCustomerIsInsertedIntoDB()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var customerToBeInserted = _fixture.Create<Customer>();

            // Act
            await sut.InsertAsync(customerToBeInserted);

            // Assert
            var customerFromDb = await sut.GetAsync(customerToBeInserted.Id);
            customerFromDb.Should().BeEquivalentTo(customerToBeInserted);
        }

        [TestMethod]
        public async Task InsertAsync_GivenTheCustomerExistInDB_WhenIInsertTheCustomerIntoDB_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var customerToBeInserted = _fixture.Create<Customer>();
            await sut.InsertAsync(customerToBeInserted);

            // Act
            Func<Task> act = async () => await sut.InsertAsync(customerToBeInserted);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.StartsWith($"Cannot insert customer with identity '{customerToBeInserted.Id}' ", StringComparison.InvariantCulture));
        }

        [TestMethod]
        public async Task GenerateIdentityAsync_GivenNoCustomerExistsInDB_WhenIGetGenerateIdentity_ItGenerates1()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            
            // Act
            var result = await sut.GenerateIdentityAsync();

            // Assert
            result.Should().Be(1);
        }

        [TestMethod]
        public async Task GenerateIdentityAsync_GivenSeveralRecordsInDB_WhenIGetGenerateIdentity_ItGeneratesOneGraterIdThenTheMaxId()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var customers = new List<Customer>();
            _fixture.AddManyTo(customers, 3);
            foreach (var customer in customers)
            {
                await sut.InsertAsync(customer);
            }

            var maxId = customers.Max(x => x.Id);

            // Act
            var result = await sut.GenerateIdentityAsync();

            // Assert
            result.Should().Be(maxId + 1);
        }

        [TestMethod]
        public async Task UpdateAsync_GivenHaveACustomer_WhenIUpdateTheCustomer_TheCustomerIsUpdatedInDB()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var customerToBeUpdated = _fixture.Create<Customer>();
            await sut.InsertAsync(customerToBeUpdated);
            const string newCustomerName = "New Name";
            customerToBeUpdated.FirstName = newCustomerName;

            // Act
            await sut.UpdateAsync(customerToBeUpdated);

            // Assert
            var customerFromDb = await sut.GetAsync(customerToBeUpdated.Id);
            customerFromDb.Should().NotBeNull();
            customerFromDb.FirstName.Should().Be(newCustomerName);
        }

        [TestMethod]
        public async Task UpdateAsync_GivenNoCustomerExistsInDBr_WhenIUpdateTheCustomer_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryCustomerRepository>();
            var customerToBeUpdated = _fixture.Create<Customer>();

            // Act
            Func<Task> act = async () => await sut.UpdateAsync(customerToBeUpdated);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.StartsWith($"Cannot update customer with identity '{customerToBeUpdated.Id}' ", StringComparison.InvariantCulture));
        }
    }
}
