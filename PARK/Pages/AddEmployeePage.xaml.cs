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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PARK.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        public MainWindow mainWindow;
        public AddEmployeePage( MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.GoBack();
        }

        private void AddEmployeeButton_click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Сотрудник создан успешно!");
        }
    }
}
