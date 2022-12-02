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

namespace CaseManager.UI.Notification
{
    /// <summary>
    /// Логика взаимодействия для NotificationItemUI.xaml
    /// </summary>
    public partial class NotificationItemUI : UserControl
    {
        public NotificationItemUI(int type,string text)
        {
            InitializeComponent();
            NatifText.Text = text;

        }
    }
}
