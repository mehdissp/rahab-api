using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Banks;
using JWTApi.Infrastructure.Data;
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
            var bank= await GetById(id,cancellationToken);
            bank.IsDeleted = true;

        }


        public async Task<PagedResult<BankDtos>> GetAllBank(
      int pageNumber = 1,
      int pageSize = 10,
      CancellationToken cancellationToken = default)
        {
            var query = _context.Banks
                .AsNoTracking()
                .Where(s => s.IsDeleted == false );

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Select(c => new BankDtos
                {
                    Name = c.Name,
                    DescriptionRows = c.DescriptionRows,
                    Id = c.Id,
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
                PageSize = pageSize
            };
        }

        public async Task<Bank> GetById(int id, CancellationToken cancellationToken)
        {
            return await _context.Banks.FindAsync(id, cancellationToken);
        }

        public async Task Insert(Bank bank, CancellationToken cancellationToken)
        {
            await _context.AddAsync(bank,cancellationToken);
        }

        public async Task Update(Bank bank, CancellationToken cancellationToken)
        {
            _context.Update(bank);
        }
    }
}
