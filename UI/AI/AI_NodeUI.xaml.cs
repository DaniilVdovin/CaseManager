using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace CaseManager.UI
{
    /// <summary>
    /// Логика взаимодействия для AI_NodeUI.xaml
    /// </summary>
    public partial class AI_NodeUI : UserControl, IElement
    {
        public bool CanDelite {get;set;}
        public List<OpenSpace_Propertis.Property> properties { get ; set; }
        public AI_NodeUI()
        {
            InitializeComponent();
            CanDelite = true;
            properties = new List<OpenSpace_Propertis.Property>
            {
                new OpenSpace_Propertis.Property("Основное","Вес", "1,8", "string",(v)=>{ node_text.Content = v.ToString()+"w"; }),
                new OpenSpace_Propertis.Property("Основное","Цвет", "White","string",(v)=>{ node_color.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(v.ToString())); })
            };
        }

        public void Clear()
        {
           
        }
    }
}
