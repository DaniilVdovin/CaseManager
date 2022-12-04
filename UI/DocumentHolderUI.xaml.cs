using CaseManager.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Xps.Packaging;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для PersonUI.xaml
    /// </summary>
    public partial class DocumentHolderUI : UserControl, IElement
    {
        public bool CanDelite { get; set; }
        public bool[] ControlPoints { get; set; }
        public List<OpenSpace_Propertis.Property> properties { get; set; }

        public DocumentHolderUI()
        {
            InitializeComponent();
            CanDelite = true;
            
            ControlPoints = new bool[4];
            for (int i = 0; i<4; i++) ControlPoints[i] = true;
            properties = new List<OpenSpace_Propertis.Property>
            {
                new OpenSpace_Propertis.Property("Основное","Файл", "-","File",(v)=>{LoadDock(v.ToString());})
            };
        }
        public void LoadDock(string file)
        {
             

        }
        public void Clear()
        {
            
        }
    }
}
