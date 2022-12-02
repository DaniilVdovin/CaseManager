using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaseManager.UI.Notification
{
    /// <summary>
    /// Логика взаимодействия для NotificationUI.xaml
    /// </summary>
    public partial class NotificationUI : UserControl
    {
        int index = 0;
        List<NotificationItemUI> items;
        Storyboard myStoryboard;
        public NotificationUI()
        {
            InitializeComponent();
            items = new List<NotificationItemUI>();
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public void Add(int type, string text)
        {
            NotificationItemUI itemUI = new NotificationItemUI(type, text)
            {
                Name = "N" + index++ + "_" + type,
            };
            itemUI.RegisterName(itemUI.Name, itemUI);
            
            var c_type = GetNotifType(type);

            itemUI.NatifImage.Source = c_type.Item1;

            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            myDoubleAnimation.BeginTime = TimeSpan.FromSeconds(c_type.Item2);

            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            myStoryboard.Completed += MyStoryboard_Completed;
            Storyboard.SetTargetName(myDoubleAnimation, itemUI.Name);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("Opacity"));

            items.Add(itemUI);
            NatifHistory.Children.Add(itemUI);
            myStoryboard.Begin(itemUI);
        }

        private void MyStoryboard_Completed(object sender, EventArgs e)
        {
            items.RemoveAt(0);
            NatifHistory.Children.RemoveAt(0);
        }

        internal (ImageSource,int) GetNotifType(int type)
        {
            switch(type)
            {
                case 0://Уведомления
                    return (ImageSourceForBitmap(SystemIcons.Information.ToBitmap()), 1);
                case 1://Информация
                    return (ImageSourceForBitmap(SystemIcons.Information.ToBitmap()), 2);
                case 2://Внимание
                    return (ImageSourceForBitmap(SystemIcons.Warning.ToBitmap()), 2);
                case 3://Ошибка
                    return (ImageSourceForBitmap(SystemIcons.Error.ToBitmap()), 2);
                default://Прочее
                    return (ImageSourceForBitmap(SystemIcons.Application.ToBitmap()), 1);
            }
        }
        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch
            {
                DeleteObject(handle);
                return null;
            }
        }
        private void MyDoubleAnimation_Completed(object sender, EventArgs e)
        {
            Console.WriteLine(sender.GetType().Name);
            items.Remove(sender as NotificationItemUI);
            NatifHistory.Children.Remove(sender as NotificationItemUI);
        }
    }
}
