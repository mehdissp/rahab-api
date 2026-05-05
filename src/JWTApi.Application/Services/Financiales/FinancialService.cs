using JWTApi.Application.DTOs.Financiales;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Financiales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Financiales
{
    public class FinancialService
    {
        private readonly IFinancialRepository _financialRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FinancialService(IFinancialRepository financialRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _financialRepository =financialRepository;
        }
        public async Task<PagedResult<FinancialDtos>> GetFinancialDtos(
         int pageNumber = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default)
        {
            return await _financialRepository.GetFinancialDtos(pageNumber,pageSize,cancellationToken);
        }
        public async Task Insert(FinancialCrudDtos financialDtos,CancellationToken cancellationToken)
        {
            Financial financial = new Financial();
            financial.create(financialDtos.Title, financialDtos.Financial_transactions, financialDtos.ParentId);

            await _financialRepository.Insert(financial, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

                
        }
        public async Task Update(FinancialCrudDtos financialDtos, CancellationToken cancellationToken)
        {
            Financial financial = new Financial();
            financial.update(financialDtos.Id,financialDtos.Title, financialDtos.Financial_transactions, financialDtos.ParentId);

            await _financialRepository.Update(financial, cancellationToken);
           
            await _unitOfWork.SaveChanges(cancellationToken);

        }
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            await _financialRepository.Delete(id, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

        }
    }
}
