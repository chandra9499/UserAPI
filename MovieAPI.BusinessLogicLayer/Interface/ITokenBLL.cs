using MovieAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.BusinessLogicLayer.Interface
{
    public interface ITokenBLL
    {
        RefreshTokenRequest RefreshToken(RefreshTokenRequest tokenApiModel);
        bool RevokeToken(string userName);
        TokenResponce GetToken(IEnumerable<Claim> claims);
        string GetRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpieredToken(string token);
    }
}
