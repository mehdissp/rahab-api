using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Cheques;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Cheques;
using JWTApi.Domain.Interfaces.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Cheques
{
    public class ChequesService
    {
        private IChequesRepository _chequesRepository;
        private IUnitOfWork _unitOfWork;
        public ChequesService(IChequesRepository chequesRepository, IUnitOfWork unitOfWork)
        {
            _chequesRepository = chequesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ChequesDtos>> ChequesDtosAsync(
              int? id,
              string userId,
              int pageNumber = 1,
              int pageSize = 10,
             
              string? searchTerm = null, CancellationToken cancellationToken=default
            )
        {
            return await _chequesRepository.ChequesDtosAsync(id, userId,pageNumber,pageSize,searchTerm,cancellationToken);
        }
    }
}
