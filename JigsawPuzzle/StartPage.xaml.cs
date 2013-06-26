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
using System.Windows.Threading;

namespace JigsawPuzzle
{
    public partial class StartPage : PhoneApplicationPage
    {
        DispatcherTimer dispatcherTimer;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("真的要退出？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            while (NavigationService.CanGoBack == true)
                NavigationService.RemoveBackEntry();
            base.OnBackKeyPress(e);
        }

        public StartPage()
        {
            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DIYPage.xaml", UriKind.Relative));
            dispatcherTimer.Stop();
        }

        void pcTask_Completed(object sender, PhotoResult e)
        {
            //如果照相成功则把照片保存到独立存储空间并显示出来
            if (e.TaskResult == TaskResult.OK && e.Error == null)
            {
                BitmapImage bmpimg = new BitmapImage();
                bmpimg.SetSource(e.ChosenPhoto);
                App app = Application.Current as App;
                app.bmpimg = bmpimg;
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
                dispatcherTimer.Start();
            }
        }

        void NavigateToPage()
        {
            //NavigationService.Navigate(new Uri("/DIYPage.xaml", UriKind.Relative));
        }

        private void btn_Start_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_Start.Width = 140;
            btn_Start.Height = 140;
        }

        private void btn_Start_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_Start.Width = 150;
            btn_Start.Height = 150;
            NavigationService.Navigate(new Uri("/GameClassChoosePage.xaml", UriKind.Relative));
        }

        private void btn_DIY_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_DIY.Width = 140;
            btn_DIY.Height = 140;
        }

        private void btn_DIY_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_DIY.Width = 150;
            btn_DIY.Height = 150;
            PhotoChooserTask pcTask = new PhotoChooserTask();
            pcTask.ShowCamera = true;
            pcTask.Completed += new EventHandler<PhotoResult>(pcTask_Completed);
            pcTask.Show();
        }

        private void btn_Record_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_Record.Width = 140;
            btn_Record.Height = 140;
        }

        private void btn_Record_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_Record.Width = 150;
            btn_Record.Height = 150;
            NavigationService.Navigate(new Uri("/TopPage.xaml", UriKind.Relative));
        }

        private void btn_Help_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_Help.Width = 140;
            btn_Help.Height = 140;

        }

        private void btn_Help_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_Help.Width = 150;
            btn_Help.Height = 150;
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        private void btn_GiveMeScore_Click(object sender, EventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show(); 
        }

        private void btn_Recommend_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.Show();
        }

        private void btn_Feedback_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();//标题
            emailComposeTask.Subject = "拼图意见反馈(JigsawPuzzle Feedback) "; 
            emailComposeTask.To = "suguoqiang@outlook.com";//收件人
            emailComposeTask.Show();
        }

        private void btn_AboutUs_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }
    }
}