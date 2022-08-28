using System.Threading.Tasks;
using Rabobank.TechnicalTest.GCOB.Dtos;

namespace Rabobank.TechnicalTest.GCOB.Services
{
    public interface ICustomerService
    {
        Task<Result<CustomerDto>> AddCustomer(CustomerCreateRequest customerCreateRequest);

        Task<Result<CustomerDto>> GetCustomer(int customerId);
    }
}
