using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Entities;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface IAddressRepository
    {
        Task<int> GenerateIdentityAsync();
        Task InsertAsync(Address address);
        Task<Customer> GetAsync(int identity);
        Task UpdateAsync(Address address);
    }
}
