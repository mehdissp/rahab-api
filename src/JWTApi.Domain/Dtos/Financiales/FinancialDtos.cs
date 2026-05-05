using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Financiales
{
    public class FinancialDtos
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Financial_transactionsEnum Financial_transactions { get; set; }
        public string Financial_transactions_Title { get; set; }
        public List<FinancialDtos> Children { get; set; } = new List<FinancialDtos>();
        public int? ParentId { get; set; }

    }
}
