using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Cheque
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime ChequeDate { get; set; }
        public string ChequeDate_Persion { get; set; }
        public int Amount { get; set; }
        public PaymentChequeStatusEnum PaymentChequeStatus { get; set; }
        public string BankName { get; set; }
        public string Desc { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UserEditor { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int FinancialOperationsId { get; set; }
        public FinancialOperations FinancialOperations { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;


        public void create(string serialNumber,DateTime chequeDate,string chequeDate_Persion
            ,int amount,PaymentChequeStatusEnum paymentChequeStatus,string desc,Guid userId,int financialOperationsId
            )
        {
            SerialNumber = serialNumber;
            ChequeDate = chequeDate;
            ChequeDate_Persion=chequeDate_Persion;
            Amount = amount;
            PaymentChequeStatus = paymentChequeStatus;
            Desc=desc;
            UserId = userId;
            FinancialOperationsId = financialOperationsId;
            CreatedAt = DateTime.Now;
        }
        public void update(int id,string serialNumber, DateTime chequeDate, string chequeDate_Persion
    , int amount, PaymentChequeStatusEnum paymentChequeStatus, string desc, Guid userId, int financialOperationsId
    )
        {
            Id= id;
            SerialNumber = serialNumber;
            ChequeDate = chequeDate;
            ChequeDate_Persion = chequeDate_Persion;
            Amount = amount;
            PaymentChequeStatus = paymentChequeStatus;
            Desc = desc;
            UserEditor = userId;
            FinancialOperationsId = financialOperationsId;
            UpdatedAt = DateTime.Now;
            
        }
        public void Delete(int id)
        {
            Id = id;
            IsDeleted = true;
            UpdatedAt= DateTime.Now;
                
        }
    }
}
