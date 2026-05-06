using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Dtos.FinancialOperationses;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.FinancialOperationses;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JWTApi.Infrastructure.Repositories.FinancialOperationses
{
    public class FinancialOperationsRepository : IFinancialOperationsRepository
    {
        private readonly AppDbContext _context;
        public FinancialOperationsRepository(AppDbContext app)
        {
            _context = app;
        }
        public async Task Delete(FinancialOperations financialOperations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialOperationsDtos> getById(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<FinancialOperations> getByIdFinan(int id, CancellationToken cancellationToken)
        {
            return await _context.FinancialOperations.FindAsync(id, cancellationToken);
        }

        public async Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            // ایجاد کوئری پایه
            var query = _context.FinancialOperations
                .Include(s => s.Bank)
                .Include(s => s.Financial)
                .Include(s => s.Project)
                .Include(s => s.User)
                .Where(s => s.IsDeleted == false);

            // محاسبه تعداد کل رکوردها (قبل از صفحه‌بندی)
            var totalCount = await query.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی و دریافت داده‌ها
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new FinancialOperationsDtos
                {
                    Id = s.Id,
                    PaymentOrderNumber = s.PaymentOrderNumber,
                    AccountSideName = s.AccountSideName,
                    DescriptionRows = s.DescriptionRows,
                    DateOfIssue = s.DateOfIssue,
                    DateOfIssue_Persian = ConvertToPersianDate(s.DateOfIssue),
                    PaymentStatus = s.PaymentStatus,
                    Amount = s.Amount,
                    DueDate = s.DueDate,
                    DueDate_Persian = ConvertToPersianDate(s.DueDate),
                    OperationCompleted = s.OperationCompleted,
                    ProjectId = s.ProjectId,
                    ProjectName = s.Project != null ? s.Project.Name : null,
                    FinancialId = s.FinancialId,
                    FinancialName = s.Financial != null ? s.Financial.Title : "",
                    BankId = s.BankId,
                    BankName = s.Bank != null ? s.Bank.Name : null,
                    CreatedAt = s.CreatedAt,
                    UserId = s.UserId,
                    UserName = s.User != null ? s.User.Username : null
                })
                .ToListAsync(cancellationToken);

            // ایجاد نتیجه صفحه‌بندی شده
            var result = new PagedResult<FinancialOperationsDtos>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return result;
        }
        // متد کمکی برای تبدیل تاریخ به شمسی
        // ابتدا متد را static کنید
        private static string ConvertToPersianDate(DateTime date)
        {
            var persianCalendar = new PersianCalendar();
            return $"{persianCalendar.GetYear(date)}/{persianCalendar.GetMonth(date):00}/{persianCalendar.GetDayOfMonth(date):00}";
        }

        public async Task Insert(FinancialOperations financialOperations, CancellationToken cancellationToken)
        {
            await _context.FinancialOperations.AddAsync(financialOperations, cancellationToken);
        }

        public async Task Update(FinancialOperations financialOperations, CancellationToken cancellationToken)
        {
            _context.FinancialOperations.Update(financialOperations);
        }

        public async Task<List<CompanyDtos>> GetComboCompany(string userId,CancellationToken cancellationToken)
        {
            return await _context.Companies.Where(s => s.IsDeleted == false && s.IsHolding == false)
                .Select(s=>new CompanyDtos
                {   Id = s.Id, 
                    Name=s.Name,

                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Project>> GetComboProject(string userId,int comId, CancellationToken cancellationToken)
        {
            return await _context.Projects.Where(s => s.IsDeleted == false && s.CompanyId == comId)
                .Select(s => new Project
                {
                    Id = s.Id,
                    Name = s.Name,

                })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<BankDtos>> GetComboBank(string userId, int comId, CancellationToken cancellationToken)
        {
            return await _context.BankCompanies.Include(s=>s.Bank).Where(s => s.Bank.IsDeleted == false && s.CompanyId == comId)
                .Select(s => new BankDtos
                {
                    Id = s.Bank.Id,
                    Name = s.Bank.Name,

                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<FinancialComboDto>> GetComboFinancialsAsync(
            string userId,
            int? parentId,
            int? financialTransactionType,
            CancellationToken cancellationToken)
        {
            // ساخت کوئری پایه
            var query = _context.Financials
                .Where(f => !f.IsDeleted);

            // اعمال فیلتر بر اساس نوع تراکنش مالی (در صورت وجود)
            if (financialTransactionType.HasValue)
            {
                var transactionType = (Financial_transactionsEnum)financialTransactionType.Value;
                query = query.Where(f => f.Financial_transactions == transactionType);
            }

            // اعمال فیلتر بر اساس والد (در صورت وجود)
            if (parentId.HasValue)
            {
                query = query.Where(f => f.ParentId == parentId.Value);
            }
            else
            {
                // اگر parentId نداشت، فقط مواردی که ParentId null هستند را برگردان
                query = query.Where(f => f.ParentId == null);
            }

            // اجرا و بازگردانی نتیجه با بررسی وجود فرزند
            var result = await query
                .Select(f => new FinancialComboDto
                {
                    Id = f.Id,
                    Name = f.Title,
                    ParentId = f.ParentId,
                    HasChildren = _context.Financials.Any(child => child.ParentId == f.Id && !child.IsDeleted)
                })
                .OrderBy(f => f.Name)
                .ToListAsync(cancellationToken);

            return result;
        }

    }
}
