using Frontend.Controllers;
using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    internal class RegisterVM : Notifiable
    {
        UserController userController = ControllerFactory.Instance.userController;

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
            }
        }
        private string repeatPassword;
        public string RepeatPassword
        {
            get { return repeatPassword; }
            set
            {
                repeatPassword = value;
            }
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
        internal RegisterVM()
        {
            email = string.Empty;
            password = string.Empty;
            errorMessage = "";
        }
        internal UserModel? Register()
        {
            // Logic to handle login
            // For example, check if the username is valid
            if (string.IsNullOrEmpty(Email))
            {
                ErrorMessage = "Email cannot be empty";
                return null;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Password cannot be empty";
                return null;
            }
            if (!Password.Equals(RepeatPassword)) 
            {
                ErrorMessage = "Passwords Must Match";
                return null;
            }
            try
            {
                UserModel um = userController.Register(Email, Password);
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
