using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Domain.Dtos.Dashboards
{
    public class DashboardsDtos
    {
        public int CountUser { get; set; }
        public int CountOP { get; set; }
        public decimal SumAmountOut { get; set; }
        public decimal SumAmountIn { get; set; }
    }
}
