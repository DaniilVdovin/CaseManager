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

namespace CaseManager.UI.BPMN
{
    /// <summary>
    /// Логика взаимодействия для BPMN_ask.xaml
    /// </summary>
    public partial class BPMN_ask : IElement
    {
        public List<Canvas_Propertis.Property> properties { get; set; }
        public BPMN_ask()
        {
            InitializeComponent();
            properties = new List<Canvas_Propertis.Property>
            {
                new Canvas_Propertis.Property("Основное","Текст", "<Текст>", "string",(v)=>{ context.Content = v.ToString(); }),
                new Canvas_Propertis.Property("Параметры","Да", "Да", "string",(v)=>{ context_yes.Content = v.ToString(); }),
                new Canvas_Propertis.Property("Параметры","Нет", "Нет", "string",(v)=>{ context_no.Content = v.ToString(); }),
            };
        }
    }
}
