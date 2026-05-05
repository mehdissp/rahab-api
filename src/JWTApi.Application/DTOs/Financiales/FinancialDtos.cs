using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Financiales
{
    public class FinancialCrudDtos
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Financial_transactions { get; set; }
        public int? ParentId { get; set; }


    }
}
