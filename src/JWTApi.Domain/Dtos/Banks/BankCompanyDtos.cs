using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Banks
{
    public class BankCompanyDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsCheck { get; set; }
    }
}
