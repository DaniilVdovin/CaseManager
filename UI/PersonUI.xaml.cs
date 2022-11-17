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
        public bool _isSelect = false;
        public bool _isMove = false;

        DropShadowEffect select_effect;

        public PersonUI()
        {
            InitializeComponent();
            select_effect = new DropShadowEffect
            {
                Color = Colors.LightBlue,
                Direction = 270,
                BlurRadius = 20,
                ShadowDepth = 10,
                Opacity = 0.8d
            };
            root.MouseDoubleClick += Root_MouseDoubleClick;
        }

        private void Root_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _isSelect = !_isSelect;
            SelectEffect();
        }

        private void SelectEffect()
        {
            if (_isSelect)
            {
                root.Effect = select_effect;
            }
            else
            {
                root.Effect = null;
            }
        }
    }
}
