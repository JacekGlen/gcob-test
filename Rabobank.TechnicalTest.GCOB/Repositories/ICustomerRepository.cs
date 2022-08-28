using Rabobank.TechnicalTest.GCOB.Dtos;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Entities;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface ICustomerRepository
    {
        Task<int> GenerateIdentityAsync();
        Task InsertAsync(Customer customer);
        Task<Customer> GetAsync(int identity);
        Task UpdateAsync(Customer customer);
    }
}
