using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.FinancialOperationses
{
    public class FinancialOperationsCrudDtos
    {
        public int Id { get; set; }
        public int PaymentOrderNumber { get; set; }
        public string AccountSideName { get; set; }
        public string DescriptionRows { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string DateOfIssue_Persian { get; set; }
        public int PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDate_Persian { get; set; }
        public int OperationCompleted { get; set; }
        public int ProjectId { get; set; }
        public int FinancialId { get; set; }
        public int BankId { get; set; }
        public int CompanyId { get; set; }
    }
}
