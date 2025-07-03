using Frontend.Controllers;
using Frontend.Model;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Frontend.ViewModel
{
    internal class HomeWindowVM : Notifiable
    {
        ControllerFactory controllerFactory = ControllerFactory.Instance;

        private UserModel user;
        private ObservableCollection<BoardModel> boards;
        private string errorMessage;

        public ObservableCollection<BoardModel> Boards
        {
            get { return boards; }
            set
            {
                boards = value;
                RaisePropertyChanged("Boards");
            }
        }
        public UserModel User
        {
            get { return user; }
            set
            {
                user = value;
                RaisePropertyChanged("User");
            }
        }
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        public HomeWindowVM(UserModel user)
        {
            this.user = user;
            this.boards = controllerFactory.boardController.GetUserBoards(user.Email);
        }

        public void RefreshBoards()
        {
            Boards = new ObservableCollection<BoardModel>(
                controllerFactory.boardController.GetUserBoards(user.Email));
            RaisePropertyChanged("Boards");
        }

        public bool DeleteBoard(string email, string boardName)
        {
            try
            {
                controllerFactory.boardController.DeleteBoard(email, boardName);
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }
        }

        public void Logout()
        {
            controllerFactory.userController.Logout(user.Email);
            user = null;
            RaisePropertyChanged("User");
        }
    }
}
