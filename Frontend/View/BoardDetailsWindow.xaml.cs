using IntroSE.Kanban.Backend.ServiceLayer.Models;
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
    /// Interaction logic for BoardDetailsWindow.xaml
    /// </summary>
    public partial class BoardDetailsWindow : Window
    {
        BoardDetailsWindowVM boardDetailsWindowVM;
        public BoardDetailsWindow()
        {
            InitializeComponent();
            boardDetailsWindowVM = (BoardDetailsWindowVM)this.DataContext;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                Owner.Show();  
            }
            this.Close();
        }

    }
}
