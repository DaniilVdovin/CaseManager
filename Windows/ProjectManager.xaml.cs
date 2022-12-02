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

namespace CaseManager.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProjectManager.xaml
    /// </summary>
    public partial class ProjectManager : UserControl
    {
        
        public EventHandler Open;
        public EventHandler Create;
        public EventHandler Close;
        public ProjectManager()
        {
            InitializeComponent();
            open.MouseLeftButtonDown += Open_MouseLeftButtonDown;
            create.MouseLeftButtonDown += Create_MouseLeftButtonDown;
            root.MouseLeftButtonDown += Root_MouseLeftButtonDown;

            
        }

        private void Root_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close.Invoke(sender,null);
        }

        private void Create_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Create.Invoke(sender, null);
        }

        private void Open_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Open.Invoke(sender, null);
        }
    }
}
