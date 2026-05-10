using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.AccountSides;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.Dashboards;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Dtos.FinancialOperationses;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.FinancialOperationses;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace JWTApi.Infrastructure.Repositories.FinancialOperationses;

public class FinancialOperationsRepository : IFinancialOperationsRepository
{
    private readonly AppDbContext _context;
    public FinancialOperationsRepository(AppDbContext app)
    {
        _context = app;
    }
    public async Task Delete(FinancialOperations financialOperations, CancellationToken cancellationToken)
    {
        //var check = await getByIdFinan(financialOperations.Id,cancellationToken);
        //var checkChq = await _context.Cheques.Where(c => c.FinancialOperationsId == check.Id).CountAsync();
        //if (checkChq>0)
        //{
        //    throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
        //}
        ArgumentNullException.ThrowIfNull(financialOperations);

        // 2. دریافت موجودیت اصلی از دیتابیس
        var existingOperation = await getByIdFinan(financialOperations.Id, cancellationToken);

        // 3. بررسی وابستگی‌ها (Cheques)
        var hasRelatedCheques = await _context.Cheques
            .AnyAsync(c => c.FinancialOperationsId == existingOperation.Id, cancellationToken);

        if (hasRelatedCheques)
        {
            throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
        }
        _context.FinancialOperations.Update(financialOperations);
    }

    public Task<FinancialOperationsDtos> getById(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<FinancialOperations> getByIdFinan(int id, CancellationToken cancellationToken)
    {
        return await _context.FinancialOperations.IgnoreAutoIncludes().Where(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);

    }

    public async Task<PagedResult<FinancialOperationsDtos>> GetFinancialOperations(int? projectId, int? bankId, string search, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        // ایجاد کوئری پایه
        var query = (IQueryable<FinancialOperations>)_context.FinancialOperations
        .Include(s => s.Bank)
        .Include(s => s.Financial)
        .Include(s => s.Project)
        .Include(s => s.User);
        //     .Where(s => s.IsDeleted == false);

        if (projectId != null)
        {
            query = query.Where(s => s.ProjectId == projectId);
        }
        if (bankId != null)
        {
            query = query.Where(s => s.BankId == bankId);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s => s.PaymentOrderNumber.ToString().Contains(search) ||
                                  s.Amount.ToString().Contains(search) ||
                                  s.AccountSideName.Contains(search)
                                  || s.AmountCash.ToString().Contains(search) || s.Bank.Name.Contains(search)
                                  || s.Project.Name.Contains(search)
                                  || s.Financial.Title.Contains(search)
                                  );
        }
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
                UserName = s.User != null ? s.User.Username : null,
                IsDeleted = s.IsDeleted,
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
        await CheckDoublicateSerialNumber(financialOperations.PaymentOrderNumber, cancellationToken);
        await _context.FinancialOperations.AddAsync(financialOperations, cancellationToken);
    }

    public async Task Update(FinancialOperations financialOperations, CancellationToken cancellationToken)
    {
        _context.FinancialOperations.Update(financialOperations);
    }

    public async Task<List<CompanyDtos>> GetComboCompany(string userId, CancellationToken cancellationToken)
    {
        return await _context.Companies.Where(s => s.IsDeleted == false && s.IsHolding == false)
            .Select(s => new CompanyDtos
            {
                Id = s.Id,
                Name = s.Name,

            })
            .ToListAsync(cancellationToken);
    }
    public async Task<List<Project>> GetComboProject(string userId, int comId, CancellationToken cancellationToken)
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
        return await _context.BankCompanies.Include(s => s.Bank).Where(s => s.Bank.IsDeleted == false && s.CompanyId == comId)
            .Select(s => new BankDtos
            {
                Id = s.Bank.Id,
                Name = s.Bank.Name,

            })
            .ToListAsync(cancellationToken);
    }
    public async Task<List<AccountSideDtos>> GetAccountSideCombo(string userId, int companyId, CancellationToken cancellationToken)

    {
        return await _context.AccountSides.Select(s => new AccountSideDtos
        {
            Id = s.Id,
            Name = s.Name,
        }).ToListAsync(cancellationToken);
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


    private async Task CheckDoublicateSerialNumber(int code, CancellationToken cancellationToken)
    {
        var check = await _context.FinancialOperations.AnyAsync(s => s.PaymentOrderNumber == code);
        if (check)
            throw new RestBasedException(ApiErrorCodeMessage.Error_RefrenceCode);

    }

    public async Task<DashboardsDtos> GetDashboardsDtosAsync(int id, string userId, CancellationToken cancellationToken)
    {
        var countUser = await _context.Users.CountAsync(cancellationToken);
        var countOp = await _context.FinancialOperations.Where(s => s.IsDeleted == false).CountAsync(cancellationToken);
        var sumOPIN = await _context.FinancialOperations.Where(s=>s.Financial.Financial_transactions== Financial_transactionsEnum.In).SumAsync(s => s.Amount);
        var sumOPChequeOUT = await _context.FinancialOperations.Where(s => s.Financial.Financial_transactions == Financial_transactionsEnum.Out).SumAsync(s => s.Amount);
        return new DashboardsDtos
        {
            CountOP = countOp,
            SumAmountIn = sumOPIN,
            SumAmountOut = sumOPChequeOUT,
            CountUser = countUser,
        };

    }

}

