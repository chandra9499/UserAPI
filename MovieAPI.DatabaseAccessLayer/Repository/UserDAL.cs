using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DatabaseAccessLayer.Context;
using MovieAPI.DatabaseAccessLayer.Interface;
using MovieAPI.Models.DTOs;
using MovieAPI.Models.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.DatabaseAccessLayer.Repository
{
    public class UserDAL : IUserDAL
    {
        private readonly DataBaseContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenDAL _tokenDAL;

        public UserDAL(DataBaseContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITokenDAL tokenDAL)
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._tokenDAL = tokenDAL;
        }

        public async Task<Status> ChangePassword(ChangePasswordModel model)
        {
            var status = new Status();
            //lets find the user
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid user name";
                return status;
            }
            //check current password
            if (await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return status;
            }
            //change password 
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "failed to change the password";
                return status;
            }
            status.StatusCode = 1;
            status.Message = "password change successfull";
            return status;
        }

        public async Task<LoginResponce> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            //to login to database
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
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
                var token = _tokenDAL.GetToken(authClaims);
                var refreshToken = _tokenDAL.GetRefreshToken();
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
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while saving the token information.", ex);
                }
                return new LoginResponce()
                {
                    Name = user.Name,
                    UserName = user.UserName,
                    Token = token.TokenString,
                    RefreshToken = refreshToken,
                    Experation = token.ValidTo,
                    StatusCode = 1,
                    Message = "Logged in"
                };

            }

            //login fail condition
            return 
                new LoginResponce()
                {
                    StatusCode = 0,
                    Message = "invalid username or password",
                    
                };
        }

        public async Task<Status> RegistraterAdmin(RegistrationModel model)
        {
            var status = new Status();
            // Check if there is already an admin in the system
            if (await roleManager.RoleExistsAsync(UserRole.Admin))
            {
                var adminUsers = await userManager.GetUsersInRoleAsync(UserRole.Admin);
                if (adminUsers.Count > 0)
                {
                    await Console.Out.WriteLineAsync("the damin is allready present");
                    status.StatusCode = 0;
                    status.Message = "An admin user already exists.";
                    return status;
                }
            }

            // Check if the user exists
            var userExist = await userManager.FindByNameAsync(model.UserName);
            if (userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username.";
                return status;
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
                return status;
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
            return status;
        }

        public async Task<Status> RegistraterUser(RegistrationModel model)
        {
            var status = new Status();
            //check if the user exist or not
            var userExist = await userManager.FindByNameAsync(model.UserName);

            if (userExist != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            //create the user hear
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "user creation failed";
                return status;
            }

            //add roles hear
            //hear we are checking if the role exist or not
            //if i want to register for Admin use ADmin insted of User
            if (!await roleManager.RoleExistsAsync(UserRole.User))
                await roleManager.CreateAsync(new IdentityRole(UserRole.User));//if not we are adding the role hear

            //if it exist select hear
            if (await roleManager.RoleExistsAsync(UserRole.User))
            {
                await userManager.AddToRoleAsync(user, UserRole.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return status;
        }
    }
}
