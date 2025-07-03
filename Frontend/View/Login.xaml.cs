using Frontend.Model;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LoginVM loginVM;
        public Login()
        {
            InitializeComponent();
            loginVM = new LoginVM();
            this.DataContext = loginVM;
        }

        private void NoAccount(object sender, RoutedEventArgs e) 
        {
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            UserModel? um = loginVM.Login();
            if (um != null) 
            {
                HomeWindow homeWindow = new HomeWindow(um);
                homeWindow.Show();
                this.Close();
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginVM loginVM)
            {
                loginVM.Password = PasswordBox.Password;
            }
        }

    }
}
