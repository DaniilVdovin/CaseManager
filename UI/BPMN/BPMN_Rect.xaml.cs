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
    /// Логика взаимодействия для BPMN_Rect.xaml
    /// </summary>
    public partial class BPMN_Rect : IElement
    {
        public bool CanDelite { get; set; }
        public bool[] ControlPoints { get; set; }
        public List<OpenSpace_Propertis.Property> properties { get; set; }
        public BPMN_Rect()
        {
            InitializeComponent();
            CanDelite = true;
            ControlPoints = new bool[4];
            for (int i = 0; i<4; i++) ControlPoints[i] = true;
            properties = new List<OpenSpace_Propertis.Property>
            {
                new OpenSpace_Propertis.Property("Основное","Текст", "<Текст>", "string",(v)=>{ context.Content = v.ToString(); }),
                new OpenSpace_Propertis.Property("Основное","Цвет", "Gray","string",(v)=>{border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(v.ToString())); }),
                new OpenSpace_Propertis.Property("Основное","Связи", "1,1,1,1","string",SetConstrainPoints),
            };
        }
        public void SetConstrainPoints(object constrainPoints)
        {
            string[] s = (constrainPoints as string).Split(',');
            if(s.Length == 4) 
            for (int i = 0; i < s.Length; i++)
                    ControlPoints[i] = s[i]=="1"?true:false;
        }
        public void Clear()
        {
           
        }
    }
}
