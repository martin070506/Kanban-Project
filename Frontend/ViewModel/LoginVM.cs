using Frontend.Controllers;
using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    internal class LoginVM : Notifiable
    {
        ControllerFactory controllerFactory = ControllerFactory.Instance;

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string password;
        public string Password 
        {
            get { return password; }
            set { password = value; }
        }
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public LoginVM() 
        {
            email = string.Empty;
            password = string.Empty;
            errorMessage = "";
        }

        public UserModel? Login() 
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) 
            {
                ErrorMessage = "You Have To Fill Both Fields";
            }
            try
            {
                UserModel um = controllerFactory.userController.Login(Email, Password);
                return um;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
        }
    }
}
