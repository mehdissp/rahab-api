using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class FinancialOperations
    {
        public int Id { get; set; }
        public int PaymentOrderNumber { get; set; }
        public string? AccountSideName { get; set; }
        public string? DescriptionRows { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string DateOfIssue_Persian { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDate_Persian { get; set; }
        public OperationCompletedEnum OperationCompleted { get; set; }
        public int ProjectId { get; set; }
        public int FinancialId { get;  set; }
        public int BankId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public DateTime? LastModify { get; set; }
        public Guid? UserIdModify { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int? CompanyId { get; set; }
        public int? AccountSideId { get; set; }
        public decimal? AmountCash { get; set; }
        public decimal? AmountCheque { get; set; }
        public Bank? Bank { get; set; } = default!;
        public AccountSide? AccountSide { get; set; } = default!;
        public Project? Project { get; set; } = default!;
        public Financial? Financial { get; set; } = default!;
        public User? User { get; set; } = default!;
        public ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
        
        public void create(int paymentOrderNumber,string accountSide,string desc, DateTime dateOfIssue
            ,string dateOfIssue_Persian, int paymentStatus,decimal amount, DateTime dueDate,
            string dueDate_Persian, int operationCompleted,int projectId,int financialId,
            int bankId, DateTime dateTime,string userId,int companyId,int accountSideId,decimal? amountCash,
            decimal? amountCheque

            )
        {
            PaymentOrderNumber = paymentOrderNumber;
            AccountSideName=accountSide;
            DescriptionRows = desc;
            DateOfIssue = dateOfIssue;
            DateOfIssue_Persian = dateOfIssue_Persian;
            PaymentStatus = (PaymentStatusEnum)paymentStatus;
            Amount = amount;
            DueDate = dueDate;
            UserId = Guid.Parse(userId);
            BankId = bankId;
            DueDate_Persian=dueDate_Persian;
            OperationCompleted = (OperationCompletedEnum)operationCompleted;
            ProjectId = projectId;
            FinancialId = financialId;
            CompanyId = companyId;
            AccountSideId= accountSideId;
            AmountCash= amountCash;
            AmountCheque=amountCheque;
        }
        public void update(int id,int paymentOrderNumber, string accountSide, string desc, DateTime dateOfIssue
    , string dateOfIssue_Persian, int paymentStatus, decimal amount, DateTime dueDate,
    string dueDate_Persian, int operationCompleted, int projectId, int financialId,
    int bankId, DateTime dateTime, string userId, int companyId, int accountSideId, decimal? amountCash,
            decimal? amountCheque

    )
        {
            Id = id;
            PaymentOrderNumber = paymentOrderNumber;
            AccountSideName = accountSide;
            DescriptionRows = desc;
            DateOfIssue = dateOfIssue;
            DateOfIssue_Persian = dateOfIssue_Persian;
            PaymentStatus = (PaymentStatusEnum)paymentStatus;
            Amount = amount;
            DueDate = dueDate;
            UserId = Guid.Parse(userId);
            BankId = bankId;
            DueDate_Persian = dueDate_Persian;
            OperationCompleted = (OperationCompletedEnum)operationCompleted;
            ProjectId = projectId;
            FinancialId = financialId;
            CompanyId = companyId;
            AccountSideId = accountSideId;
            AmountCash = amountCash;
            AmountCheque = amountCheque;
        }
        public void Delete(string userId)
        {
            IsDeleted = true;
            LastModify = DateTime.Now;
            UserIdModify = Guid.Parse(userId);
        }
    }
}
