using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rabobank.TechnicalTest.GCOB.Repositories
{
    public interface ICountryRepository
    {
        Task<Country> GetAsync(int identity);

        Task<IEnumerable<Country>> GetAllAsync();
    }
}
