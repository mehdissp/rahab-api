using JWTApi.Application.DTOs.FinancialOperationses;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Dtos.FinancialOperationses;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Financiales;
using JWTApi.Domain.Interfaces.FinancialOperationses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.FinancialOperationses
{
    public class FinancialOperationsService
    {
        private readonly IFinancialOperationsRepository _financialRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FinancialOperationsService(IFinancialOperationsRepository financialRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _financialRepository = financialRepository;
        }

        public async Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _financialRepository.GetFinancialOperations(pageNumber, pageSize, cancellationToken);
        }
        public async Task Insert(FinancialOperationsCrudDtos financialOperationsCrudDtos, string userId, CancellationToken cancellationToken)
        {
            try
            {
                FinancialOperations financialOperations = new FinancialOperations();
                financialOperations.create(financialOperationsCrudDtos.PaymentOrderNumber, financialOperationsCrudDtos.AccountSideName, financialOperationsCrudDtos.DescriptionRows
                    , financialOperationsCrudDtos.DateOfIssue, financialOperationsCrudDtos.DateOfIssue_Persian, financialOperationsCrudDtos.PaymentStatus,
                    financialOperationsCrudDtos.Amount, financialOperationsCrudDtos.DueDate, financialOperationsCrudDtos.DueDate_Persian,
                    financialOperationsCrudDtos.OperationCompleted, financialOperationsCrudDtos.ProjectId, financialOperationsCrudDtos.FinancialId,
                    financialOperationsCrudDtos.BankId, DateTime.Now, userId, financialOperationsCrudDtos.CompanyId);
                await _financialRepository.Insert(financialOperations, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
   
        }
        public async Task Update(FinancialOperationsCrudDtos financialOperationsCrudDtos, string userId, CancellationToken cancellationToken)
        {
            FinancialOperations financialOperations = new FinancialOperations();
            financialOperations = await _financialRepository.getByIdFinan(financialOperationsCrudDtos.Id, cancellationToken);
            financialOperations.update(financialOperationsCrudDtos.Id, financialOperationsCrudDtos.PaymentOrderNumber, financialOperationsCrudDtos.AccountSideName, financialOperationsCrudDtos.DescriptionRows
                , financialOperationsCrudDtos.DateOfIssue, financialOperationsCrudDtos.DateOfIssue_Persian, financialOperationsCrudDtos.PaymentStatus,
                financialOperationsCrudDtos.Amount, financialOperationsCrudDtos.DueDate, financialOperationsCrudDtos.DueDate_Persian,
                financialOperationsCrudDtos.OperationCompleted, financialOperationsCrudDtos.ProjectId, financialOperationsCrudDtos.FinancialId,
                financialOperationsCrudDtos.BankId, DateTime.Now, userId, financialOperationsCrudDtos.CompanyId);
            await _financialRepository.Update(financialOperations, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task Delete(int id, string userId, CancellationToken cancellationToken)
        {
            FinancialOperations financialOperations = new FinancialOperations();
            financialOperations = await _financialRepository.getByIdFinan(id, cancellationToken);
            financialOperations.Delete(userId);
            await _financialRepository.Update(financialOperations, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task<List<BankDtos>> GetComboBank(string userId, int comId, CancellationToken cancellationToken)
        {
            return await _financialRepository.GetComboBank(userId, comId, cancellationToken);

        }
        public async Task<List<Project>> GetComboProject(string userId, int comId, CancellationToken cancellationToken)
        {
            return await _financialRepository.GetComboProject(userId, comId, cancellationToken);
        }
        public async Task<List<CompanyDtos>> GetComboCompany(string userId, CancellationToken cancellationToken)
        {
            return await _financialRepository.GetComboCompany(userId, cancellationToken);
        }

        public async Task<List<FinancialComboDto>> GetComboFinancialsAsync(
         string userId,
         int? parentId,
         int? financialTransactionType,
         CancellationToken cancellationToken)
        {
            return await _financialRepository.GetComboFinancialsAsync(userId, parentId, financialTransactionType, cancellationToken);
        }
    }
}
