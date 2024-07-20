using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.BusinessLogicLayer.Interface;
using MovieAPI.DatabaseAccessLayer.Context;
using MovieAPI.Models.DTOs;

namespace MovieAPI.PresentationLayer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenBLL _tokenBLL;
        private readonly DataBaseContext _context;
        public TokenController(ITokenBLL tokenBLL, DataBaseContext context)
        {
            this._tokenBLL = tokenBLL;
            this._context = context;
        }
        [HttpPost]
        public IActionResult Refresh(RefreshTokenRequest tokenApiModel)
        {
            if (tokenApiModel is null)
            {
                return BadRequest("Invalid client request");
            }

            var result = _tokenBLL.RefreshToken(tokenApiModel);

            if (result == null)
            {
                return BadRequest("Invalid client request");
            }

            return Ok(result);
            //if (tokenApiModel is null) 
            //{
            //    return BadRequest("Invalid client request");
            //}
            //string accessToken = tokenApiModel.AccessToken;
            //string refreshToken = tokenApiModel.RefreshToken;
            //var principal = _tokenBLL.GetPrincipalFromExpieredToken(accessToken);
            //var userName = principal.Identity.Name;//username
            //var user = _context.tokenInfos.SingleOrDefault(user=> user.UserName == userName );
            //if(user is null || user.RefreshToken!=refreshToken || user.RefreshTokenExpiry <= DateTime.Now)
            //    return BadRequest("Invalid Clint request");
            //var newAccessToken = _tokenBLL.GetToken(principal.Claims);
            //var newRefreshToken = _tokenBLL.GetRefreshToken();
            //user.RefreshToken = newRefreshToken;
            //_context.SaveChanges();
            //return Ok(new RefreshTokenRequest()
            //{
            //    AccessToken = newAccessToken.TokenString,
            //    RefreshToken = newRefreshToken
            //});
        }
        //revoke is used to remove the token from the database
        [HttpPost, Authorize]
        public IActionResult Revoke()
        {
            var userName = User.Identity.Name;

            var result = _tokenBLL.RevokeToken(userName);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(true);
            //try
            //{
            //    var userName = User.Identity.Name;
            //    var user = _context.tokenInfos.SingleOrDefault(user => user.UserName.Equals(userName));
            //    if (user is null)
            //    {
            //        return BadRequest();
            //    }
            //    user.RefreshToken = null;
            //    _context.SaveChanges();
            //    return Ok(true);
            //}
            //catch(Exception ex) 
            //{
            //    return BadRequest();
            //}
        }
    }
}
