using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Cheques;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Cheques;
using JWTApi.Domain.Shared;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using JWTApi.Infrastructure.Extentions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Cheques;

public class ChequeRepository:IChequesRepository
{
    private readonly AppDbContext _context;
    public ChequeRepository(AppDbContext app)
    {
        _context = app;
    }

    public async Task Create(List<Cheque> cheque,CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(cheque,cancellationToken);
    }
    public async Task Update(Cheque cheque, CancellationToken cancellationToken)
    {
         _context.Update(cheque);
    }
    public async Task Delete(Cheque cheque, CancellationToken cancellationToken)
    {
        _context.Update(cheque);
    }
    //public async Task CheckDoublicateSerialNumber(string code, CancellationToken cancellationToken)
    //{
    //    var check = await _context.Cheques.AnyAsync(s => s.SerialNumber == code);
    //    if (check)
    //        throw new RestBasedException(ApiErrorCodeMessage.Error_RefrenceCode);

    //}
    public async Task CheckDuplicateSerialNumbers(List<string> serialNumbers, CancellationToken cancellationToken)
    {
        var existingSerials = await _context.Cheques
            .Where(s => serialNumbers.Contains(s.SerialNumber))
            .Select(s => s.SerialNumber)
            .ToListAsync(cancellationToken);

        if (existingSerials.Any())
        {
            throw new RestBasedException($"شماره سریال‌های {string.Join(", ", existingSerials)} قبلاً ثبت شده است");
        }
    }


    public async Task<PagedResult<ChequesDtos>> ChequesDtosAsync(
        int? id,
        string userId,
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Cheques
            .Include(s => s.FinancialOperations)
            .Where(s => s.IsDeleted == false &&
                s.FinancialOperations.Financial.Financial_transactions == (Financial_transactionsEnum)id);

        // اعمال جستجو اختیاری
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s => s.BankName.Contains(searchTerm) ||
                                     s.SerialNumber.Contains(searchTerm) ||
                                     s.ChequeDate_Persion.Contains(searchTerm)
                                     ||s.FinancialOperations.PaymentOrderNumber.ToString().Contains(searchTerm)
                                     );
        }

        var projectedQuery = query.Select(s => new ChequesDtos
        {
            Id = s.Id,
            BankName = s.BankName,
            SerialNumber = s.SerialNumber,
            PaymentChequeStatus = s.PaymentChequeStatus,
            ChequeDate = s.ChequeDate,
            ChequeDate_Persion = s.ChequeDate_Persion,
            Amount = s.Amount,
            Desc = s.Desc,
            SerialFinancail=s.FinancialOperations.PaymentOrderNumber.ToString(),
        }).OrderBy(s => s.ChequeDate);

        return await projectedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<Cheque> GetChequeForEdit(int id,CancellationToken cancellationToken)
    {
        return await _context.Cheques.FindAsync(id, cancellationToken);
    }


    

}
