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
    /// Interaction logic for AddBoardWindow.xaml
    /// </summary>
    public partial class AddBoardWindow : Window
    {
        private AddBoardWindowVM viewModel;
        private string email;
        private readonly Action refreshBoards;

        public AddBoardWindow(string email, Action refreshBoards)
        {
            this.email = email;
            this.refreshBoards = refreshBoards;
            InitializeComponent();
            viewModel = new AddBoardWindowVM(email);
            this.DataContext = viewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CreateBoard(email, viewModel.BoardName))
            {
                refreshBoards?.Invoke();
                this.Close();
            }
        }
    }
}
