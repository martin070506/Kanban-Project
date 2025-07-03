using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Controllers
{
    class UserController
    {

        private UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        public UserModel Register(string email, string password) 
        {
            var ret = userService.Register(email, password);
            if (!string.IsNullOrEmpty(ret.ErrorMessage) || ret.ReturnValue==null) 
            {
                throw new Exception(ret.ErrorMessage);
            }
            return new UserModel((UserSL)ret.ReturnValue);
        }
        public UserModel Login(string email, string password) 
        {
            var ret = userService.Login(email, password);
            if (!string.IsNullOrEmpty(ret.ErrorMessage) || ret.ReturnValue == null)
            {
                throw new Exception(ret.ErrorMessage);
            }
            return new UserModel((UserSL)ret.ReturnValue);
        }

        public void Logout(string email) 
        {
            userService.Logout(email);
        }
    }
}
