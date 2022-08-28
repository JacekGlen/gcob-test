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
using Rabobank.TechnicalTest.GCOB.Entities;
using Rabobank.TechnicalTest.GCOB.Repositories;

namespace Rabobank.TechnicalTest.GCOB.Tests.Repositories
{
    [TestClass]
    public class AddressRepositoryTest
    {
        private IFixture _fixture;

        [TestInitialize]
        public void Initialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _fixture.Freeze<Mock<ILogger>>();
        }


        [TestMethod]
        public async Task GetAsync_GivenHaveAnAddress_WhenIGetTheAddressFromDB_ThenTheAddressIsRetrieved()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();

            var expectedAddress = _fixture.Create<Address>();
            await sut.InsertAsync(expectedAddress);

            // Act
            var result = await sut.GetAsync(expectedAddress.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedAddress);
        }

        [TestMethod]
        public async Task GetAsync_GivenNoAddressExistsInDB_WhenIGetTheAddressFromDB_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var id = _fixture.Create<int>();

            // Act
            Func<Task> act = async () => await sut.GetAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.Equals(id.ToString(), StringComparison.InvariantCulture));
        }

        [TestMethod]
        public async Task InsertAsync_GivenTheAddressDoesNotExistInDB_WhenIInsertTheAddressIntoDB_TheAddressIsInsertedIntoDB()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var addressToBeInserted = _fixture.Create<Address>();

            // Act
            await sut.InsertAsync(addressToBeInserted);

            // Assert
            var addressFromDb = await sut.GetAsync(addressToBeInserted.Id);
            addressFromDb.Should().BeEquivalentTo(addressToBeInserted);
        }

        [TestMethod]
        public async Task InsertAsync_GivenTheAddressExistInDB_WhenIInsertTheAddressIntoDB_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var addressToBeInserted = _fixture.Create<Address>();
            await sut.InsertAsync(addressToBeInserted);

            // Act
            Func<Task> act = async () => await sut.InsertAsync(addressToBeInserted);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.StartsWith($"Cannot insert address with identity '{addressToBeInserted.Id}' ", StringComparison.InvariantCulture));
        }

        [TestMethod]
        public async Task GenerateIdentityAsync_GivenNoAddressExistsInDB_WhenIGetGenerateIdentity_ItGenerates1()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            
            // Act
            var result = await sut.GenerateIdentityAsync();

            // Assert
            result.Should().Be(1);
        }

        [TestMethod]
        public async Task GenerateIdentityAsync_GivenSeveralRecordsInDB_WhenIGetGenerateIdentity_ItGeneratesOneGraterIdThenTheMaxId()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var addresses = new List<Address>();
            _fixture.AddManyTo(addresses, 3);
            foreach (var address in addresses)
            {
                await sut.InsertAsync(address);
            }

            var maxId = addresses.Max(x => x.Id);

            // Act
            var result = await sut.GenerateIdentityAsync();

            // Assert
            result.Should().Be(maxId + 1);
        }

        [TestMethod]
        public async Task UpdateAsync_GivenHaveAnAddress_WhenIUpdateTheAddress_TheAddressIsUpdatedInDB()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var addressToBeUpdated = _fixture.Create<Address>();
            await sut.InsertAsync(addressToBeUpdated);
            const string newCity = "Casablanca";
            addressToBeUpdated.City = newCity;

            // Act
            await sut.UpdateAsync(addressToBeUpdated);

            // Assert
            var addressFromDb = await sut.GetAsync(addressToBeUpdated.Id);
            addressFromDb.Should().NotBeNull();
            addressFromDb.City.Should().Be(newCity);
        }

        [TestMethod]
        public async Task UpdateAsync_GivenNoAddressExistsInDBr_WhenIUpdateTheAddress_ThrowsException()
        {
            // Arrange
            var sut = _fixture.Create<InMemoryAddressRepository>();
            var addressToBeUpdated = _fixture.Create<Address>();

            // Act
            Func<Task> act = async () => await sut.UpdateAsync(addressToBeUpdated);

            // Assert
            await act.Should().ThrowAsync<Exception>().Where(e => e.Message.StartsWith($"Cannot update address with identity '{addressToBeUpdated.Id}' ", StringComparison.InvariantCulture));
        }
    }
}
