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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для PersonUI.xaml
    /// </summary>
    public partial class PersonUI : UserControl
    {
        public List<Canvas_Propertis.Property> properties;
        public PersonUI()
        {
            InitializeComponent();
            properties = new List<Canvas_Propertis.Property>
            {
                new Canvas_Propertis.Property("Имя", "Daniil", typeof(string),(v)=>{ t_name.Text = v.ToString(); }),
                new Canvas_Propertis.Property("Возраст", 22, typeof(int),(v)=>{ t_age.Text = "Возраст: " + v.ToString(); })
            };
        }
        public new int GetHashCode()
        {
            return 1;
        }
    }
}
