using JWTApi.Application.DTOs.FinancialOperationses;
using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.AccountSides;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.Dashboards;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Dtos.FinancialOperationses;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Cheques;
using JWTApi.Domain.Interfaces.Financiales;
using JWTApi.Domain.Interfaces.FinancialOperationses;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Repositories.Cheques;
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
        private readonly IChequesRepository _ChequesRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FinancialOperationsService(IFinancialOperationsRepository financialRepository, 
            IUnitOfWork unitOfWork,
            IChequesRepository chequesRepository)
        {
            _unitOfWork = unitOfWork;
            _financialRepository = financialRepository;
            _ChequesRepository= chequesRepository;
        }

        public async Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int? projectId,int? bankId, string search, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            return await _financialRepository.GetFinancialOperations(projectId, bankId, search, pageNumber , pageSize, cancellationToken);
        }
        public async Task Insert(FinancialOperationsCrudDtos financialOperationsCrudDtos, string userId, CancellationToken cancellationToken)
        {
           // await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                if (financialOperationsCrudDtos.PaymentStatus==1)
                {
                    financialOperationsCrudDtos.OperationCompleted = 1;
                }
                

                FinancialOperations financialOperations = new FinancialOperations();
                financialOperations.create(financialOperationsCrudDtos.PaymentOrderNumber, financialOperationsCrudDtos.AccountSideName, financialOperationsCrudDtos.DescriptionRows
                    , financialOperationsCrudDtos.DateOfIssue, financialOperationsCrudDtos.DateOfIssue_Persian, financialOperationsCrudDtos.PaymentStatus,
                    financialOperationsCrudDtos.Amount, financialOperationsCrudDtos.DueDate, financialOperationsCrudDtos.DueDate_Persian,
                    financialOperationsCrudDtos.OperationCompleted, financialOperationsCrudDtos.ProjectId, financialOperationsCrudDtos.FinancialId,
                    financialOperationsCrudDtos.BankId, DateTime.Now, userId, financialOperationsCrudDtos.CompanyId,
                    financialOperationsCrudDtos.AccountSideId,financialOperationsCrudDtos.AmountCash,financialOperationsCrudDtos.AmountCheque);
                await _financialRepository.Insert(financialOperations, cancellationToken);
         
                if (financialOperationsCrudDtos.Cheques?.Any() == true)
                {
                    var serialNumbers = financialOperationsCrudDtos.Cheques.Select(c => c.SerialNumber).ToList();
                    await _ChequesRepository.CheckDuplicateSerialNumbers(serialNumbers, cancellationToken);
                    var cheques = financialOperationsCrudDtos.Cheques.Select(cheque => new Cheque
                    {
                        SerialNumber = cheque.SerialNumber,
                        ChequeDate = cheque.ChequeDate,
                        ChequeDate_Persion = cheque.ChequeDate_Persion,
                        Amount = cheque.Amount,
                        PaymentChequeStatus = (PaymentChequeStatusEnum)cheque.PaymentChequeStatus,
                        BankName = cheque.BankName,
                        Desc = cheque.Desc,
                        //FinancialOperationsId = financialOperations.Id
                        FinancialOperations = financialOperations // تنظیم رابطه به جای Id
                    }).ToList();
                    financialOperations.Cheques = cheques; // اضافه کردن به ناوبری پراپرتی

                    await _ChequesRepository.Create(cheques, cancellationToken);
                }

                // بررسی دوبلیکیت و ذخیره نهایی
          
                await _unitOfWork.SaveChanges(cancellationToken);
  

              //  await _unitOfWork.CommitTransactionAsync(cancellationToken);

           

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
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
                financialOperationsCrudDtos.BankId, DateTime.Now, userId, financialOperationsCrudDtos.CompanyId,
                financialOperationsCrudDtos.AccountSideId, financialOperationsCrudDtos.AmountCash, financialOperationsCrudDtos.AmountCheque);
            await _financialRepository.Update(financialOperations, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
        public async Task Delete(int id, string userId, CancellationToken cancellationToken)
        {
            FinancialOperations financialOperations = new FinancialOperations();
            financialOperations = await _financialRepository.getByIdFinan(id, cancellationToken);
            financialOperations.Delete(userId);
            await _financialRepository.Delete(financialOperations, cancellationToken);
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
        public async Task<List<AccountSideDtos>> GetAccountSideCombo(string userId,
            int companyId, CancellationToken cancellationToken)
        {
            return await _financialRepository.GetAccountSideCombo(userId, companyId, cancellationToken);
        }


        public async Task<DashboardsDtos> GetDashboardsDtosAsync(int id, string userId, CancellationToken cancellationToken)
        {
            return await _financialRepository.GetDashboardsDtosAsync(id, userId, cancellationToken);
        }
    }
}
