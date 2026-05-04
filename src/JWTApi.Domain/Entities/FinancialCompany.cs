using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class FinancialCompany
    {
        public int FinancialId { get; set; }
        public int CompanyId { get; set; }
        //public Guid PermissionId { get; set; }

        public Financial Financial { get; set; } = default!;
        public Company Company { get; set; } = default!;
        //public Permission Permission { get; set; } = default!;
    }
}
