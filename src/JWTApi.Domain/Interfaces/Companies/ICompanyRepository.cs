using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Interfaces.Companies
{
    public interface ICompanyRepository
    {
        Task InsertCompany(Company company, CancellationToken cancellationToken);
        Task DeleteCompany(int id, CancellationToken cancellationToken);
        Task UpdateCompany(Company company, CancellationToken cancellationToken);
        Task<CompanyDtos> GetCompanyById(int id, CancellationToken cancellationToken);
        Task<PagedResult<CompanyDtos>> GetCompany(
           int pageNumber = 1,
           int pageSize = 10,
           CancellationToken cancellationToken = default);
        Task<Company> GetCompanyByIdAsync(int id, CancellationToken cancellationToken);
        Task<List<CompanyDtos>> CompanyDtos(CancellationToken cancellationToken);
    }
}
