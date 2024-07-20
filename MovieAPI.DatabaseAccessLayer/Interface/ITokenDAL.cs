using MovieAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.DatabaseAccessLayer.Interface
{
    public interface ITokenDAL
    {
        TokenResponce GetToken(IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpieredToken(string token);
    }
}
