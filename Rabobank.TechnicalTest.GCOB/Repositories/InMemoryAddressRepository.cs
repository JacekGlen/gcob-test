using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Entities;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryAddressRepository : IAddressRepository
    {
        private ConcurrentDictionary<int, Address> Addresses { get; } = new();
        private readonly ILogger _logger;

        public InMemoryAddressRepository(ILogger logger)
        {
            _logger = logger;
        }

        public Task<int> GenerateIdentityAsync()
        {
            _logger.LogDebug("Generating Address identity");
            return Task.Run(() =>
            {
                if (Addresses.Count == 0) return 1;

                var x = Addresses.Values.Max(c => c.Id);
                return ++x;
            });
        }

        public Task InsertAsync(Address address)
        {
            if (Addresses.ContainsKey(address.Id))
            {
                throw new Exception(
                    $"Cannot insert address with identity '{address.Id}' " +
                    "as it already exists in the collection");
            }

            Addresses.TryAdd(address.Id, address);
            _logger.LogDebug($"New address inserted [ID:{address.Id}]. " +
                          $"There are now {Addresses.Count} legal entities in the store.");
            return Task.FromResult(address);
        }

        public Task<Address> GetAsync(int identity)
        {
            _logger.LogDebug($"FindMany Addresses with identity {identity}");

            if (!Addresses.ContainsKey(identity)) throw new Exception(identity.ToString());
            _logger.LogDebug($"Found Address with identity {identity}");
            return Task.FromResult(Addresses[identity]);
        }

        public Task UpdateAsync(Address address)
        {
            if (!Addresses.ContainsKey(address.Id))
            {
                throw new Exception(
                    $"Cannot update address with identity '{address.Id}' " +
                    "as it doesn't exist");
            }

            Addresses[address.Id] = address;
            _logger.LogDebug($"New address updated [ID:{address.Id}].");

            return Task.FromResult(address);
        }
    }
}
