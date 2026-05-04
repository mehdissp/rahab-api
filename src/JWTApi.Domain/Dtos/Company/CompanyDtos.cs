using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Company
{
    public class CompanyDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DescriptionRows { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtPersianRelative => CreatedAt.ToPersianRelativeDate();
    }
}
