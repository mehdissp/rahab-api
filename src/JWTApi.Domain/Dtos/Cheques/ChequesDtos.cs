using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Cheques
{
    public class ChequesDtos
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string SerialFinancail { get; set; }
        public DateTime ChequeDate { get; set; }
        public string ChequeDate_Persion { get; set; }
        public int Amount { get; set; }
        public PaymentChequeStatusEnum PaymentChequeStatus { get; set; }
        public string PaymentChequeStatusTitle => PaymentChequeStatus.GetTitle();
        public string BankName { get; set; }
        public string Desc { get; set; }
    }
}
