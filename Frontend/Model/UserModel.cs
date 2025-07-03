using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frontend.Controllers;
using IntroSE.Kanban.Backend.ServiceLayer.Models;
namespace Frontend.Model
{
    internal class UserModel
    {
        public string Email { get; }

        internal UserModel(UserSL user) 
        {
            this.Email = user.email;
        }
    }
}
