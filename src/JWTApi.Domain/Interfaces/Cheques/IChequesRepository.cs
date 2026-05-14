using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Cheques;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Cheques
{
    public interface IChequesRepository
    {
        Task Create(List<Cheque> cheque, CancellationToken cancellationToken);
        Task Update(Cheque cheque, CancellationToken cancellationToken);
        Task Delete(Cheque cheque, CancellationToken cancellationToken);
        Task CheckDuplicateSerialNumbers(List<string> serialNumbers, CancellationToken cancellationToken);
        Task<PagedResult<ChequesDtos>> ChequesDtosAsync(
              int? id,
              string userId,
              int pageNumber = 1,
              int pageSize = 10,
              string? searchTerm = null,
              CancellationToken cancellationToken = default);
        Task<Cheque> GetChequeForEdit(int id, CancellationToken cancellationToken);

    }
}
