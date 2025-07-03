using Frontend.Controllers;
using Frontend.Model;
using Frontend.ViewModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        private HomeWindowVM homeWindowVM;
        internal HomeWindow(UserModel userModel)
        {
            InitializeComponent();
            homeWindowVM = new HomeWindowVM(userModel);
            this.DataContext = homeWindowVM;
        }

        private void AddBoard_Click(object sender, RoutedEventArgs e)
        {
            AddBoardWindow win = new AddBoardWindow(homeWindowVM.User.Email, () =>
            {
                homeWindowVM.RefreshBoards();
            });
            win.Owner = this;
            win.ShowDialog();
        }

        private void DeleteBoard_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var board = button?.DataContext as BoardModel;
            if (board == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete board '{board.Name}'?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (homeWindowVM.DeleteBoard(homeWindowVM.User.Email, board.Name))
                {
                    homeWindowVM.RefreshBoards();
                }
            }
        }
        private void BoardName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hyperlink &&
                hyperlink.DataContext is BoardModel boardModel)
            {
                var vm = new BoardDetailsWindowVM(boardModel, homeWindowVM.User);
                var detailsWindow = new BoardDetailsWindow
                {
                    DataContext = vm,
                    Owner = this
                };

                this.Hide(); 

                detailsWindow.Show();
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            homeWindowVM.Logout();
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
