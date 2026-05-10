using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.AccountSides;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.Dashboards;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Dtos.FinancialOperationses;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.FinancialOperationses
{
    public interface IFinancialOperationsRepository
    {
        Task Insert(FinancialOperations financialOperations, CancellationToken cancellationToken);
        Task Delete(FinancialOperations financialOperations,CancellationToken cancellationToken);
        Task Update(FinancialOperations financialOperations, CancellationToken cancellationToken);
  

        Task<FinancialOperationsDtos> getById(int id, CancellationToken cancellationToken);
        Task<FinancialOperations> getByIdFinan(int id, CancellationToken cancellationToken);
        //Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int? projectId, int? bankId, string search, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<List<CompanyDtos>> GetComboCompany(string userId, CancellationToken cancellationToken);
        Task<List<Project>> GetComboProject(string userId, int comId, CancellationToken cancellationToken);
        Task<List<BankDtos>> GetComboBank(string userId, int comId, CancellationToken cancellationToken);

        Task<List<FinancialComboDto>> GetComboFinancialsAsync(
         string userId,
         int? parentId,
         int? financialTransactionType,
         CancellationToken cancellationToken);

        Task<List<AccountSideDtos>> GetAccountSideCombo(string userId,
            int companyId, CancellationToken cancellationToken);

        Task<DashboardsDtos> GetDashboardsDtosAsync(int id, string userId, CancellationToken cancellationToken);

    }
}
