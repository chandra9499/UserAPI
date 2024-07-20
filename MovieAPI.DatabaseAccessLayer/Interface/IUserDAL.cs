using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.DatabaseAccessLayer.Interface
{
    public interface IUserDAL
    {
        Task<IActionResult> RegistrationUser(RegistrationModel model);
        Task<IActionResult> RegistrationAdmin(RegistrationModel model);
        Task<IActionResult> Login(LoginModel model);
        Task<IActionResult> ChangePassword(ChangePasswordModel model);
        
        
    }
}
