using JWTApi.Application.DTOs.Cheques;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Cheques;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Cheques;
using JWTApi.Domain.Interfaces.Companies;
using JWTApi.Domain.Shared;
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

        public async Task UpdateResult(ChequesViewModel chequesViewModel ,string userId,CancellationToken cancellationToken)
        {
            if (chequesViewModel == null)
                throw new ArgumentNullException(nameof(chequesViewModel));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be null or empty", nameof(userId));

            if (!Guid.TryParse(userId, out Guid parsedUserId))
                throw new ArgumentException("Invalid UserId format", nameof(userId));

            Cheque cheque = await _chequesRepository.GetChequeForEdit(chequesViewModel.Id, cancellationToken);
            cheque.UpdateResult(chequesViewModel.Id, (PaymentChequeStatusEnum)chequesViewModel.TypeResult, chequesViewModel.ChequeDateAccepts
                , chequesViewModel.ChequeDatePersianAccepts, parsedUserId, chequesViewModel.Desc);
            await _chequesRepository.Update(cheque, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
                
        }

    }
}
