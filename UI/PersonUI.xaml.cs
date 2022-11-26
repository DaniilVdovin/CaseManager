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
        private readonly ImageSource image_default;
        public PersonUI()
        {
            InitializeComponent();
            properties = new List<Canvas_Propertis.Property>
            {
                new Canvas_Propertis.Property("Основное","Имя", "Daniil", "string",(v)=>{ t_name.Text = v.ToString(); }),
                new Canvas_Propertis.Property("Основное","Изображение", "-","File",(v)=>{LoadImage(v.ToString());}),
                new Canvas_Propertis.Property("Параметры","Возраст", 22, "string",(v)=>{ t_age.Text = "Возраст: " + v.ToString(); }),
                new Canvas_Propertis.Property("Параметры","Рост", 182, "string",(v)=>{ t_Height.Text = "Рост: " + v.ToString(); }),
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
        public new int GetHashCode()
        {
            return 1;
        }
    }
}
