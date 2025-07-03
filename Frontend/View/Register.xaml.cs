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
using Frontend.ViewModel;
using Frontend.Model;


namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private RegisterVM registerVM;
        public Register()
        {
            InitializeComponent();
            this.registerVM = new RegisterVM();
            this.DataContext = registerVM;
        }

        private void Register_Click(object sender, RoutedEventArgs e) 
        {
            UserModel? ret = registerVM.Register();
            if(ret!=null)
            {
                HomeWindow homeWindow = new HomeWindow(ret);
                homeWindow.Show();
                this.Close();
            }
            
        }
        private void GoToLogin(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterVM registerVM)
            {
                registerVM.Password = PasswordBox.Password;
            }
        }
        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterVM registerVM)
            {
                registerVM.RepeatPassword = RepeatPasswordBox.Password;
            }
        }
    }
}
