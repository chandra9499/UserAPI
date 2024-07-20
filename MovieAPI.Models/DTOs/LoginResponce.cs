using MovieAPI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.Models.DTOs
{
    public class LoginResponce : Status
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Experation { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }
}
