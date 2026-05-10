using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Financiales
{
    public interface IFinancialRepository
    {
        Task Insert(Financial financial,CancellationToken cancellationToken);
        Task Update(Financial financial,CancellationToken cancellationToken);
        Task Update(List<Financial> financial, CancellationToken cancellationToken);
        Task Delete(int id,CancellationToken cancellationToken);
        Task<Financial> GetById(int id, CancellationToken cancellationToken);
        Task<List<FinancialDtos>> GetComboFinancial(int? id, CancellationToken cancellationToken);
        Task<PagedResult<FinancialDtos>> GetFinancialDtos(
         int pageNumber = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default);
        Task<List<Financial>> GetByParentId(int id, CancellationToken cancellationToken);
    }
}
