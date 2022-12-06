using CaseManager.UI;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для PersonUI.xaml
    /// </summary>
    public partial class PersonUI : UserControl, IElement
    {
        public bool CanDelite { get; set; }
        public bool[] ControlPoints { get; set; }
        public List<OpenSpace_Propertis.Property> properties { get; set; }
        private readonly ImageSource image_default;
        public PersonUI()
        {
            InitializeComponent();
            CanDelite=true;
            ControlPoints = new bool[4];
            for (int i = 0; i<4; i++) ControlPoints[i] = true;
            properties = new List<OpenSpace_Propertis.Property>
            {
                new OpenSpace_Propertis.Property("Основное","Имя", "Daniil", "string",(v)=>{ t_name.Text = v.ToString(); }),
                new OpenSpace_Propertis.Property("Основное","Изображение", "-","File",(v)=>{LoadImage(v.ToString());}),
                new OpenSpace_Propertis.Property("Параметры","Возраст", 22, "string",(v)=>{ t_age.Text = "Возраст: " + v.ToString(); }),
                new OpenSpace_Propertis.Property("Параметры","Рост", 182, "string",(v)=>{ t_Height.Text = "Рост: " + v.ToString(); }),
            };
            image_default = i_image.Source;
        }
        public void LoadImage(string path)
        {
            if(path!="" && path!="-")
            {
                try 
                { 
                    i_image.Source = new BitmapImage(new Uri(path));
                }
                catch
                {
                    i_image.Source = image_default;
                }
            }
        }
        public void Clear()
        {
            
        }
    }
}
