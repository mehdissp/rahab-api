using JWTApi.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Financial
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Financial_transactionsEnum Financial_transactions { get; set; }
        public Guid? UserId { get; set; }
        public int? ParentId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Financial> Children { get; set; } = new List<Financial>();
        public ICollection<FinancialCompany> FinancialCompanies { get; set; } = new List<FinancialCompany>();
    }
}
