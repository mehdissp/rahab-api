using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHolding { get; set; }=false;
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Company> Children { get; set; } = new List<Company>(); // لیست 
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<FinancialCompany> FinancialCompanies { get; set; } = new List<FinancialCompany>();

        public ICollection<BankCompany> BankCompanies { get; set; } = new List<BankCompany>();

        public void create(string name,string desc,int parentId,string userId)
        {
            Name = name;
            Description = desc;
            ParentId = parentId;
            UserId = Guid.Parse(userId);
        }
        public void update(int id,string name, string desc 
           , string userId)
        {
            Name = name;
            Description = desc;
            UserId = Guid.Parse(userId);
            Id= id;
        }
    }
    
}
