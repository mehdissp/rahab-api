using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Dtos.Company;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Companies;
using JWTApi.Domain.Interfaces.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Companies
{
    public class CompanyService
    {
        private ICompanyRepository _companyRepository;
        private IUnitOfWork _unitOfWork;
        public CompanyService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Insert(string name,string desc,string userId,CancellationToken cancellationToken)
        {
            Company company = new Company();
            company.create(name, desc, 1, userId);
            await _companyRepository.InsertCompany(company, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);

        }
        public async Task<PagedResult<CompanyDtos>> GetCompany(int pageNumber,int pageSize,CancellationToken cancellationToken)
        {
            return await _companyRepository.GetCompany(pageNumber, pageSize,cancellationToken);
        }
        public async Task Update(int id,string name, string desc, string userId,CancellationToken cancellationToken)
        {
            Company company = new Company();
            company.update(id, name, desc, userId);
            await _companyRepository.UpdateCompany(company, cancellationToken);
        
        }

        public async Task Delete(int id ,CancellationToken cancellationToken)
        {
            await _companyRepository.DeleteCompany(id, cancellationToken);
        }
         public async Task<List<CompanyDtos>> CompanyDtos(CancellationToken cancellationToken)
        {
            return await _companyRepository.CompanyDtos(cancellationToken);
        }

 

    }
}
