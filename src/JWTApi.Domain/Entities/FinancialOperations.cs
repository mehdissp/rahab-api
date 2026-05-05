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
        public string AccountSideName { get; set; }
        public string DescriptionRows { get; set; }
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
        public DateTime LastModify { get; set; }
        public Guid UserIdModify { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Bank Bank { get; set; } = default!;
        public Project Project { get; set; } = default!;
        public Financial Financial { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
