using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public class InMemoryAddressRepository : IAddressRepository
    {
        private ConcurrentDictionary<int, AddressDto> Addresses { get; } = new ConcurrentDictionary<int, AddressDto>();

        public Task<int> GenerateIdentityAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task InsertAsync(AddressDto address)
        {
            throw new System.NotImplementedException();
        }

        Task<CustomerDto> IAddressRepository.GetAsync(int identity)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateAsync(AddressDto address)
        {
            throw new System.NotImplementedException();
        }

        public Task<AddressDto> GetAsync(int identity)
        {
            throw new System.NotImplementedException();
        }
    }
}
