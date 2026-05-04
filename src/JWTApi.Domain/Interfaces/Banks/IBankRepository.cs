using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Banks
{
    public interface IBankRepository
    {
        Task<PagedResult<BankDtos>> GetAllBank(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
        Task Insert(Bank bank,CancellationToken cancellationToken);
        Task Update(Bank bank,CancellationToken cancellationToken);
        Task Delete(int id,CancellationToken cancellationToken);
        Task<Bank> GetById(int id, CancellationToken cancellationToken);

    }
}
