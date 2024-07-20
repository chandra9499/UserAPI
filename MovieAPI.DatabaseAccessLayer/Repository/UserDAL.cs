using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.DatabaseAccessLayer.Context;
using MovieAPI.DatabaseAccessLayer.Interface;
using MovieAPI.Models.DTOs;
using MovieAPI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Login(LoginModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> RegistrationAdmin(RegistrationModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> RegistrationUser(RegistrationModel model)
        {
            throw new NotImplementedException();
        }
    }
}
