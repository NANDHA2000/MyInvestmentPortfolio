using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentPortfolio.Models.DBContext
{
    public class DeleteMessage
    {
        [Key]
        public string? Message { get; set; }
    }

}
