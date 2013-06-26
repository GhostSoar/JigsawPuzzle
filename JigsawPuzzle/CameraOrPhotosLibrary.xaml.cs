using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Threading;

namespace JigsawPuzzle
{
    public partial class CameraOrPhotosLibrary : PhoneApplicationPage
    {
        Thread thread;
        public CameraOrPhotosLibrary()
        {
            InitializeComponent();

            thread = new Thread(NavigateToPage);
        }

        private void btn_Camera_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            PhotoChooserTask pcTask = new PhotoChooserTask();
            pcTask.ShowCamera = true;
            pcTask.Completed += new EventHandler<PhotoResult>(pcTask_Completed);
            pcTask.Show();
        }

        void pcTask_Completed(object sender, PhotoResult e)
        {
            //如果照相成功则把照片保存到独立存储空间并显示出来
            if (e.TaskResult == TaskResult.OK && e.Error == null)
            {
                BitmapImage bmpimg = new BitmapImage();
                bmpimg.SetSource(e.ChosenPhoto);
                App app = Application.Current as App;
                Image img = new Image();
                img.Source = bmpimg;
                //app.DIYImg = img;
                //image1.Source = bmpimg;
                //IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                //if (!isf.DirectoryExists("UsersImage"))
                //{
                //    isf.CreateDirectory("UsersImage");
                //}
                //using (Stream storagefilestream = isf.OpenFile(MusicName + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))
                //{
                //    WriteableBitmap wb = new WriteableBitmap(bmpimg);
                //    wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                //}
                //NavigationService.Navigate(new Uri("/JigsawPuzzle;component/DIYPage.xaml", UriKind.Relative));
                thread.Start();
            }
        }

        void NavigateToPage()
        {
 
        }

        private void btn_PhotosLibrary_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DIYPage.xaml", UriKind.Relative));
        }
    }
}