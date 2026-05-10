using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class AccountSide
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UserEditor { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<FinancialOperations> FinancialOperations { get; set; } = new List<FinancialOperations>();

    }
}
