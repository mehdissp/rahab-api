using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Dtos.ProjectUsers;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Banks;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Banks
{
    public class BankRepository : IBankRepository
    {
        private readonly AppDbContext _context;
        public BankRepository(AppDbContext app)
        {
            _context = app;
        }
        public async Task Delete(int id, CancellationToken cancellationToken)
        {
            var bank = await GetById(id, cancellationToken);

            if (bank == null)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_NotFound);
            }

            var hasRelatedCompanies = await _context.BankCompanies
                .AnyAsync(s => s.BankId == id, cancellationToken);

            if (hasRelatedCompanies)
            {
                throw new RestBasedException(ApiErrorCodeMessage.Error_Refrence);
            }

            // Soft delete
            bank.IsDeleted = true;
     

            await _context.SaveChangesAsync(cancellationToken);
        }





        public async Task<PagedResult<BankDtos>> GetAllBank(
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default)
        {
            var query = _context.Banks.Where(s => s.IsDeleted == false);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Select(c => new BankDtos
                {
                    Name = c.Name,
                    DescriptionRows = c.DescriptionRows,
                    Id = c.Id,
                    Address = c.Address,
                    Phone = c.Phone,
                    CreatedAt = c.CreateAt
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<BankDtos>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Bank> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.Banks
        .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted, cancellationToken);
        }

        public async Task Insert(Bank bank, CancellationToken cancellationToken)
        {
            await _context.AddAsync(bank,cancellationToken);
        }

        public async Task Update(Bank bank, CancellationToken cancellationToken)
        {
            _context.Update(bank);
        }


        public async Task<PagedResult<BankCompanyDtos>> GetBankCompanyDtos(string userId, int bankId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = from u in _context.Companies.Where(s=>s.IsDeleted==false && s.IsHolding==false)
                         
                            select new BankCompanyDtos
                            {
                                Id = u.Id,
                                Name = u.Name,
                              
                                IsCheck = _context.BankCompanies.Any(pu => pu.CompanyId == u.Id && pu.BankId == bankId)
                            };

            // گرفتن تعداد کل رکوردها
            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // اعمال صفحه‌بندی
            var users = await baseQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<BankCompanyDtos>
            {
                Items = users,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public async Task InsertOrDeleteBankInCompany(List<BankCompany> bankCompanies, CancellationToken cancellationToken)
        {
            try
            {
                // گرفتن اولین ProjectId برای فیلتر کردن (فرض می‌کنیم همه آیتم‌ها ProjectId یکسان دارند)
                var firstProjectId = bankCompanies.First().BankId;

                // موجودی فعلی از دیتابیس
                var existingProjectUsers = await _context.BankCompanies
                    .Where(pu => pu.BankId == firstProjectId)
                    .ToListAsync(cancellationToken);

                // پیدا کردن مواردی برای حذف (در دیتابیس هستند ولی در لیست جدید نیستند)
                // اما فقط آنهایی که IsCreator == false دارند
                var toDelete = existingProjectUsers
                    .Where(epu => !bankCompanies.Any(npu =>
                        npu.BankId == epu.BankId && npu.CompanyId == epu.CompanyId)
) // فقط مواردی که IsCreator false هستند
                    .ToList();

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var toAdd = bankCompanies
                    .Where(npu => !existingProjectUsers.Any(epu =>
                        epu.BankId == npu.BankId && epu.CompanyId == npu.CompanyId))
                    .ToList();

                // اجرای عملیات
                if (toDelete.Any())
                {
                    _context.BankCompanies.RemoveRange(toDelete);
                }

                if (toAdd.Any())
                {
                    await _context.BankCompanies.AddRangeAsync(toAdd, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteBankInCompany(int bankId, CancellationToken cancellationToken)
        {
            try
            {

                // پیدا کردن مواردی برای اضافه کردن (در لیست جدید هستند ولی در دیتابیس نیستند)
                var deleteRoleMenu = _context.BankCompanies.Where(s => s.BankId == bankId)
                    .ToList();
                _context.BankCompanies.RemoveRange(deleteRoleMenu);

            }
            catch (Exception ex)
            {

                throw;
            }

        }


    }
}
