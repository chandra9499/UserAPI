using MovieAPI.DatabaseAccessLayer.Repository;
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
        RefreshTokenRequest RefreshToken(RefreshTokenRequest tokenApiModel);
        bool RevokeToken(string userName);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        TokenResponce GetToken(IEnumerable<Claim> claims);
        string GetRefreshToken();
    }
}
