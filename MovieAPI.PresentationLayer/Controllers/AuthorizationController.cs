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
        private readonly DataBaseContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenBLL _tokenBLL;

        public AuthorizationController(DataBaseContext context, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ITokenBLL tokenBLL) 
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._tokenBLL = tokenBLL;
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var status = new Status();
            //check validation
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the fields";
                return Ok(status);
            }
            //lets find the user
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid user name";
                return Ok(status);
            }
            //check current password
            if (await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return Ok(status);
            }
            //change password hear
            var result = await userManager.ChangePasswordAsync(user,model.CurrentPassword,model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "failed to change the password";
                return Ok(status);
            }
            status.StatusCode = 1;
            status.Message = "password change successfull";
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
            var user = await userManager.FindByNameAsync(model.UserName);
            //to login to database
            if (user != null && await userManager.CheckPasswordAsync(user,model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name,user.UserName),//storing user name into this name claim
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())//hear we are taking the jwt token and passing the random string into it
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = _tokenBLL.GetToken(authClaims);
                var refreshToken = _tokenBLL.GetRefreshToken();
                var tokenInfo = _context.tokenInfos.FirstOrDefault(u => u.UserName.Equals(user.UserName));
                if (tokenInfo == null)
                {
                    var info = new TokenInfo()
                    {
                        UserName = user.UserName,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddDays(7)
                    };
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.RefreshTokenExpiry = DateTime.Now.AddDays(7);
                }
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex) 
                {
                    return BadRequest(ex.Message);
                }
                return Ok(new LoginResponce()
                {
                    Name= user.Name,
                    UserName= user.UserName,
                    Token=token.TokenString,
                    RefreshToken=refreshToken,
                    Experation=token.ValidTo,
                    StatusCode = 1,
                    Message = "Logged in"
                });

            }

            //login fail condition
            return Ok(
                new LoginResponce()
                {
                    StatusCode = 0,
                    Message = "invalid username or password",
                    Token = "",
                    Experation = null
                });
        }
        [HttpPost]
        public async Task<IActionResult> RegistrationUser([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please Pass all the required fields";
                return Ok(status);
            }
            //check if the user exist or not
            var userExist = userManager.FindByNameAsync(model.UserName);

            if(userExist != null) 
            { 
                status.StatusCode= 0;
                status.Message = "Invalid username";
                return Ok(status);
            }
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            //create the user hear
            var result = await userManager.CreateAsync(user , model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "user creation failed";
                return Ok(status);
            }

            //add roles hear
            //hear we are checking if the role exist or not
            //if i want to register for Admin use ADmin insted of User
            if (!await roleManager.RoleExistsAsync(UserRole.User))
                await roleManager.CreateAsync(new IdentityRole(UserRole.User));//if not we are adding the role hear

            //if it exist select hear
            if (await roleManager.RoleExistsAsync(UserRole.User))
            {
                await userManager.AddToRoleAsync(user,UserRole.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }
        //{
        //  "name": "Chandrashekar",
        //  "userName": "admin",
        //  "email": "Chandrashekar.6569@gmail.com",
        //  "password": "Chandra@7022"
        //}
        /*
         * {
              "statusCode": 1,
              "message": "Successfully registered."
            }
         */
        [HttpPost]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields.";
                return Ok(status);
            }

            // Check if there is already an admin in the system
            if (await roleManager.RoleExistsAsync(UserRole.Admin))
            {
                var adminUsers = await userManager.GetUsersInRoleAsync(UserRole.Admin);
                if (adminUsers.Count > 0)
                {
                    status.StatusCode = 0;
                    status.Message = "An admin user already exists.";
                    return Ok(status);
                }
            }

            // Check if the user exists
            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username.";
                return Ok(status);
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };

            // Create the user
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed.";
                return Ok(status);
            }

            // Add roles
            if (!await roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.Admin)); // If not, add the role
            }

            // If it exists, assign the role
            if (await roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRole.Admin);
            }

            status.StatusCode = 1;
            status.Message = "Successfully registered.";
            return Ok(status);
        }

    }
}
