using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DescriptionRows { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<BankCompany> BankCompanies { get; set; } = new List<BankCompany>();

    }
}
