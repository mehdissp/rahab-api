using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.FinancialOperationses
{
    public class FinancialOperationsDtos
    {
        public int Id { get; set; }
        public int PaymentOrderNumber { get; set; }
        public string AccountSideName { get; set; }
        public string DescriptionRows { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string DateOfIssue_Persian { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string PaymentStatusTitle => PaymentStatus.GetTitle();
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDate_Persian { get; set; }
        public OperationCompletedEnum OperationCompleted { get; set; }
        public string OperationCompletedTitle => OperationCompleted.GetTitle();
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int FinancialId { get; set; }
        public string FinancialName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
