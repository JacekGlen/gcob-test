using System.Collections.Generic;
using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Entities;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface ICountryRepository
    {
        Task<Country> GetAsync(int identity);

        Task<IEnumerable<Country>> GetAllAsync();
    }
}
