using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.Models.DTOs
{
    public class TokenResponce
    {
        public string? TokenString { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
