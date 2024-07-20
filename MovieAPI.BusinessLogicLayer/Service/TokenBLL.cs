using MovieAPI.BusinessLogicLayer.Interface;
using MovieAPI.DatabaseAccessLayer.Interface;
using MovieAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.BusinessLogicLayer.Service
{
    public class TokenBLL : ITokenBLL
    {
        private readonly ITokenDAL _tokenDAL;
        public TokenBLL(ITokenDAL tokenDAL) {
            _tokenDAL = tokenDAL;
        }
        public ClaimsPrincipal GetPrincipalFromExpieredToken(string token)
        {
            return _tokenDAL.GetPrincipalFromExpieredToken(token);
        }

        public string GetRefreshToken()
        {
            return _tokenDAL.GetRefreshToken();
        }

        public TokenResponce GetToken(IEnumerable<Claim> claims)
        {
            return _tokenDAL.GetToken(claims);
        }
    }
}
