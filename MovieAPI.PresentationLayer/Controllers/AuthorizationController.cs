using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.BusinessLogicLayer.Interface;
using MovieAPI.DatabaseAccessLayer.Context;
using MovieAPI.Models.DTOs;
using MovieAPI.Models.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MovieAPI.PresentationLayer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IUserBLL _userBLL;
        public AuthorizationController(IUserBLL userBLL) 
        {          
            this._userBLL = userBLL;
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new Status { StatusCode = 0, Message = "Please pass all the fields" });
            }
            var status = await _userBLL.ChangePassword(model);
            return Ok(status);
        }

        //Response body
        //Download
        //{
        //  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJqdGkiOiIwOTNlM2RhMC1mZDA2LTRhMzEtOGI2NS04YWUwNTdiZDBkZTEiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTcyMjA2MTE1MiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzA5NTtodHRwOi8vbG9jYWxob3N0OjUyMDMiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MDk1O2h0dHA6Ly9sb2NhbGhvc3Q6NTIwMyJ9.wqTD4xqxmqVbBLTUgPF8GjHU9408Rd_bBMTw1Li0Q1A",
        //  "refreshToken": "bnbd2RYPXSP9bbsx+/pfkL21TGZVOUtP+u9gwsedf48=",
        //  "experation": "2024-07-27T06:19:12Z",
        //  "name": "Chandrashekar",
        //  "userName": "admin",
        //  "statusCode": 1,
        //  "message": "Logged in"
        //}
        [HttpPost] 
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Status { StatusCode = 0, Message = "Please pass all the fields" });
            }
            var response = await _userBLL.Login(model);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrationUser([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Status { StatusCode = 0, Message = "Please pass all the fields" });
            }
            var status = await _userBLL.RegistraterUser(model);
            return Ok(status);
        }
        //{
        //  "name": "Chandrashekar",
        //  "userName": "admin",
        //  "email": "Chandrashekar.6569@gmail.com",
        //  "password": "Chandra@7022"
        //}
        
          //{
          //    "statusCode": 1,
          //    "message": "Successfully registered."
          //}
         
        [HttpPost]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Status { StatusCode = 0, Message = "Please pass all the fields" });
            }
            var status = await _userBLL.RegistraterAdmin(model);
            return Ok(status);
        }

    }
}
