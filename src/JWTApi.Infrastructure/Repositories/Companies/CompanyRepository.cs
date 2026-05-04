using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces.Companies;
using JWTApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Repositories.Companies
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;
        public CompanyRepository(AppDbContext app)
        {
            _context = app;
        }
        public async Task DeleteCompany(int id,CancellationToken cancellationToken)
        {
            var check = await _context.Companies.FindAsync(id, cancellationToken);
            if (check != null) {
          check.IsDeleted = true;
                await _context.SaveChangesAsync(cancellationToken);
            }

        }

        public async Task<PagedResult<CompanyDtos>> GetCompany(
           int pageNumber = 1,
           int pageSize = 10,
           CancellationToken cancellationToken = default)
        {
            var query = _context.Companies
                .AsNoTracking()
                .Where(s => s.IsDeleted == false && s.IsHolding==false);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Select(c => new CompanyDtos
                {
                    Name = c.Name,
                    DescriptionRows = c.Description,
                    Id = c.Id,
                    CreatedAt=c.CreateAt
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CompanyDtos>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CompanyDtos> GetCompanyById(int id, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (company != null)
            {
                return new CompanyDtos
                {
                    Name = company.Name,
                    DescriptionRows = company.Description,
                    Id = company.Id,
                };
            }
            return null;
        }


        public async Task InsertCompany(Company company,CancellationToken cancellationToken)
        {
          await _context.Companies.AddAsync(company,cancellationToken);
       
        }

        public async Task<Company> GetCompanyByIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _context.Companies.FindAsync(id, cancellationToken);
        }
        public async Task UpdateCompany(Company company,CancellationToken cancellationToken)
        {
             _context.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanyDtos>> CompanyDtos(CancellationToken cancellationToken)
        {
            return await _context.Companies.Where(s => s.IsDeleted == false && s.IsHolding == false).Select(c => new CompanyDtos
            {
                Name = c.Name,
                DescriptionRows = c.Description,
                Id = c.Id,
                CreatedAt = c.CreateAt
            }).ToListAsync(cancellationToken);
        }
    }
}
