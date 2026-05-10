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
        public int AccountSideId { get; set; }
        public decimal? AmountCash { get; set; }
        public decimal? AmountCheque { get; set; }
        public List<Cheques> Cheques { get; set; }

    }
    public class Cheques
    {
        public string SerialNumber { get; set; }
        public DateTime ChequeDate { get; set; }
        public string ChequeDate_Persion { get; set; }
        public int Amount { get; set; }
        public int? PaymentChequeStatus { get; set; }
        public string BankName { get; set; }
        public string Desc { get; set; }
    }
}
