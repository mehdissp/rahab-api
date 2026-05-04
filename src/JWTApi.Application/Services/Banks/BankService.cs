using JWTApi.Domain.Dtos;
using JWTApi.Domain.Dtos.Banks;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Domain.Interfaces.Banks;
using JWTApi.Domain.Interfaces.Companies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.Services.Banks
{
    public class BankService
    {
        private IBankRepository _bankRepository;
        private IUnitOfWork _unitOfWork;
        public BankService(IBankRepository bankRepository, IUnitOfWork unitOfWork)
        {
            _bankRepository = bankRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<PagedResult<BankDtos>> GetBankDtosAsync(int pageNumber ,
      int pageSize,
      CancellationToken cancellationToken)
        {
            return await _bankRepository.GetAllBank(pageNumber, pageSize, cancellationToken);
        }
        public async Task Insert(string name,string address,string phone,string desc,string userId,CancellationToken cancellationToken)
        {
            try
            {
                Bank bank = new Bank();
                bank.create(name, address, phone, desc, userId);
                await _bankRepository.Insert(bank, cancellationToken);
                await _unitOfWork.SaveChanges(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
  
        }
        public async Task Delete(int id,CancellationToken cancellationToken)
        {
            await _bankRepository.Delete(id, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public async Task Update(int id,string name,string address,string phone,string desc,string userId,CancellationToken cancellationToken)
        {
            Bank bank = new Bank();
            bank.update(id,name, address, phone, desc, userId);
            await _bankRepository.Update(bank, cancellationToken);
            await _unitOfWork.SaveChanges(cancellationToken);
        }
    }
}
