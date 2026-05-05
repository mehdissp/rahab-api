using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Financiales;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Financiales;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Financiales
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly AppDbContext _context;
        public FinancialRepository(AppDbContext app)
        {
            _context = app;
        }
        public async Task Delete(int id, CancellationToken cancellationToken = default)
        {
            // 1. دریافت موجودیت با بررسی همزمان وجود آن
            var entity = await _context.Financials
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);

            // 2. بررسی وابستگی‌ها (وجود فرزند)
            var hasChildren = await _context.Financials
                .AnyAsync(x => x.ParentId == id && x.IsDeleted==false, cancellationToken);

            if (hasChildren)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            }

            // 3. حذف نرم (Soft Delete)
            entity.IsDeleted = true;


            // 4. ذخیره تغییرات
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Financial> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.Financials.FindAsync(id, cancellationToken);
        }

        public Task<List<FinancialDtos>> GetComboFinancial(int? id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<FinancialDtos>> GetFinancialDtos(
      int pageNumber = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default)
        {
            // دریافت همه داده‌ها از دیتابیس
            var allFinancials = await _context.Financials
                .Where(f => !f.IsDeleted)
                .ToListAsync(cancellationToken);

            // تبدیل به DTO
            var allDtos = allFinancials.Select(f => new FinancialDtos
            {
                Id = f.Id,
                Title = f.Title,
                Financial_transactions = f.Financial_transactions,
                Financial_transactions_Title = f.Financial_transactions.ToString(),
                ParentId = f.ParentId,
                Children = new List<FinancialDtos>()
            }).ToList();

            // ساخت ساختار درختی
            var lookup = allDtos.ToDictionary(x => x.Id);
            var allRoots = new List<FinancialDtos>();

            foreach (var dto in allDtos)
            {
                if (dto.ParentId.HasValue && lookup.ContainsKey(dto.ParentId.Value))
                {
                    lookup[dto.ParentId.Value].Children.Add(dto);
                }
                else
                {
                    allRoots.Add(dto);
                }
            }

            // صفحه‌بندی فقط روی ریشه‌ها
            var totalCount = allRoots.Count;
            var pagedRoots = allRoots
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<FinancialDtos>
            {
                Items = pagedRoots,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task Insert(Financial financial, CancellationToken cancellationToken)
        {
            await _context.Financials.AddAsync(financial, cancellationToken);
        }

        public async Task Update(Financial financial, CancellationToken cancellationToken)
        {
            _context.Financials.Update(financial);
        }
    }
}
