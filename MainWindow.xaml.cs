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

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isAdding = false;
        public MainWindow()
        {
            InitializeComponent();

            os_bt_create.Click += (s, e) => Op_Sp.Add_Element(new PersonUI());

            c_ui_person.MouseLeftButtonDown += (s, e) => _isAdding = true;
            c_ui_person.MouseLeftButtonUp += (s, e) => _isAdding = false;
            c_ui_person.MouseLeave += (s, e) =>
            {
                if (_isAdding)
                    Op_Sp.Add_Element(new PersonUI());
            };
        }
    }
}
