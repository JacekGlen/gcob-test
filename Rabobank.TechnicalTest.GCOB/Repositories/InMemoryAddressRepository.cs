using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Entities;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryAddressRepository : IAddressRepository
    {
        private ConcurrentDictionary<int, Address> Addresses { get; } = new ConcurrentDictionary<int, Address>();

        public Task<int> GenerateIdentityAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task InsertAsync(Address address)
        {
            throw new System.NotImplementedException();
        }

        Task<Customer> IAddressRepository.GetAsync(int identity)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(Address address)
        {
            throw new System.NotImplementedException();
        }

        public Task<Address> GetAsync(int identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
