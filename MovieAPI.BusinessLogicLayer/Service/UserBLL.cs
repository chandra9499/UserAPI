using Microsoft.AspNetCore.Mvc;
using MovieAPI.BusinessLogicLayer.Interface;
using MovieAPI.DatabaseAccessLayer.Interface;
using MovieAPI.Models.DTOs;
using MovieAPI.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieAPI.BusinessLogicLayer.Service
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL _userDAL;
        public UserBLL(IUserDAL _userDAL) 
        {
            this._userDAL = _userDAL;
        }
        public Task<Status> ChangePassword(ChangePasswordModel model)
        {
            return _userDAL.ChangePassword(model);
        }

        public Task<LoginResponce> Login(LoginModel model)
        {
            return _userDAL.Login(model);
        }

        public Task<Status> RegistraterAdmin(RegistrationModel model)
        {
            return _userDAL.RegistraterAdmin(model);
        }

        public Task<Status> RegistraterUser(RegistrationModel model)
        {
            return _userDAL.RegistraterUser(model);
        }
    }
}
