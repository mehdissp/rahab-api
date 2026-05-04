using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class BankCompany
    {
        public int BankId { get; set; }
        public int CompanyId { get; set; }
        //public Guid PermissionId { get; set; }

        public Bank Bank { get; set; } = default!;
        public Company Company { get; set; } = default!;
        //public Permission Permission { get; set; } = default!;
    }
}
