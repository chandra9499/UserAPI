using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models.DTOs;
using MovieAPI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.BusinessLogicLayer.Interface
{
    public interface IUserBLL
    {
        Task<Status> RegistraterAdmin(RegistrationModel model);
        Task<Status> RegistraterUser(RegistrationModel model);
        Task<LoginResponce> Login(LoginModel model);
        Task<Status> ChangePassword(ChangePasswordModel model);
    }
}
