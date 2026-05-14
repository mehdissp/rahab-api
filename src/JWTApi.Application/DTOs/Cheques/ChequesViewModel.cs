using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTApi.Application.DTOs.Cheques
{
    public class ChequesViewModel
    {
        public int Id { get; set; }
        public DateTime? ChequeDateAccepts { get; set; }
        public string? ChequeDatePersianAccepts { get; set; }
        public string? Desc { get; set; }
        public int TypeResult { get; set; }

    }
}
