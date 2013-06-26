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
using System.Windows.Resources;
using System.IO;
using System.IO.IsolatedStorage;

namespace JigsawPuzzle
{
    public partial class GameResult : PhoneApplicationPage
    {
        string gameClass;//判断是哪种拼图
        string imgUri;
        string str_RecordTime="00:00:00";
        TimeSpan recordTime = new TimeSpan(00, 00, 00);
        TimeSpan currentTime = new TimeSpan(00, 00, 00);

        int signalIsBack;//避免打开图库后重新加载OnNavigatedTo
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            #region 接受页面传值
            IDictionary<string, string> para = this.NavigationContext.QueryString;
            if (para.Count > 0)
            {
                gameClass = (para["gameClass"]).ToString().Trim();
                imgUri = (para["imgUri"]).ToString().Trim();
                //time = (para["time"]).ToString().Trim();
            }
            #endregion

            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (signalIsBack == 1)
            {
                #region 独立存储空间读取游戏图片
                if (!isf.DirectoryExists(imgUri))
                    isf.CreateDirectory(imgUri);
                try
                {
                    using (IsolatedStorageFileStream isfStream = isf.OpenFile(imgUri + "/originalImage.jpg", FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage bmpImg = new BitmapImage();
                        bmpImg.SetSource(isfStream);
                        GamePicture.Source = bmpImg;
                    }
                }
                catch
                { }
                #endregion

                #region 从存储过程中读取纪录保持者的图片
                if (!isf.DirectoryExists(imgUri))
                    isf.CreateDirectory(imgUri);
                string[] imgsNames = isf.GetFileNames(imgUri + "/RecordOwner*.jpg");
                try
                {
                    using (IsolatedStorageFileStream isfStream = isf.OpenFile(imgUri + "/" + imgsNames[0], FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage bmpImg = new BitmapImage();
                        bmpImg.SetSource(isfStream);
                        img_RecordOwner.Source = bmpImg;
                    }
                }
                catch
                { }
                #endregion

                #region 从存储过程中读取纪录保持者的时间
                #region 首次游戏时判断是否有Record.txt文件，无的话就创建
                if (!isf.FileExists(imgUri + "/Record.txt"))
                {
                    IsolatedStorageFileStream isoStream =
                        new IsolatedStorageFileStream(imgUri + "/Record.txt", System.IO.FileMode.Create, isf);
                    isoStream.Close();
                }
                #endregion
                try
                {
                    using (IsolatedStorageFileStream isfStream = isf.OpenFile(imgUri + "/Record.txt", FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader str = new StreamReader(isfStream))
                        {
                            str_RecordTime = Convert.ToString(str.ReadLine());
                        }
                    }
                }
                catch
                { }

                #endregion

                #region 比较并改写Record.txt的纪录
                App app = Application.Current as App;
                currentTime = app.time;
                txtb_CurrentTime.Text = currentTime.ToString().Trim();
                txtb_Record.Text = str_RecordTime;
                if (str_RecordTime != "")
                {
                    recordTime = TimeSpan.Parse(str_RecordTime);
                    if (recordTime >= app.time)
                    {
                        btn_Camera.Visibility = Visibility.Visible;
                        txtb_Info.Visibility = Visibility.Visible;
                        currentTime = app.time;
                        using (Stream storagefilestream = isf.OpenFile(imgUri + "/Record.txt", FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(storagefilestream))
                            {
                                sw.WriteLine(currentTime);
                            }
                        }
                        #region 如果用破纪录而不存储图片留念，就将系统的UnConfirmUser.png图片存储
                        Uri uri = new Uri("img/UnConfirmUser.png", UriKind.Relative);
                        StreamResourceInfo resourdeInfo = Application.GetResourceStream(uri);
                        BitmapImage bmpimg = new BitmapImage();
                        bmpimg.SetSource(resourdeInfo.Stream);
                        using (Stream storagefilestream = isf.OpenFile(imgUri + "/RecordOwner.jpg", FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            WriteableBitmap wb = new WriteableBitmap(bmpimg);
                            wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                        }
                        #endregion
                    }
                    else
                    {
                        btn_Camera.Visibility = Visibility.Collapsed;
                        txtb_Info.Visibility = Visibility.Visible;
                        txtb_Info.Text = "Keep Going";
                        txtb_Info.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
                else//第一个游戏者
                {
                    txtb_Info.Visibility = Visibility.Visible;
                    btn_Camera.Visibility = Visibility.Visible;
                    txtb_Info.Text = "You Are The First Player";
                    currentTime = app.time;
                    using (Stream storagefilestream = isf.OpenFile(imgUri + "/Record.txt", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(storagefilestream))
                        {
                            sw.WriteLine(currentTime);
                        }
                    }
                    #region 如果用破纪录而不存储图片留念，就将系统的UnConfirmUser.png图片存储
                    Uri uri = new Uri("img/UnConfirmUser.png", UriKind.Relative);
                    StreamResourceInfo resourdeInfo = Application.GetResourceStream(uri);
                    BitmapImage bmpimg = new BitmapImage();
                    bmpimg.SetSource(resourdeInfo.Stream);
                    using (Stream storagefilestream = isf.OpenFile(imgUri + "/RecordOwner.jpg", FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        WriteableBitmap wb = new WriteableBitmap(bmpimg);
                        wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                    }
                    #endregion
                }
                #endregion
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass=" + gameClass, UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        public GameResult()
        {
            InitializeComponent();
            signalIsBack = 1;
        }

        void pcTask_Completed(object sender, PhotoResult e)
        {
            //如果照相成功则把照片保存到独立存储空间并显示出来
            if (e.TaskResult == TaskResult.OK && e.Error == null)
            {
                BitmapImage bmpimg = new BitmapImage();
                bmpimg.SetSource(e.ChosenPhoto);
                img_Winer.Source = bmpimg;
                txtb_Info.Visibility = Visibility.Collapsed;
                txtb_Record = txtb_CurrentTime;
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isf.DirectoryExists(imgUri))
                {
                    isf.CreateDirectory(imgUri);
                }
                using (Stream storagefilestream = isf.OpenFile(imgUri+"/RecordOwner.jpg", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    WriteableBitmap wb = new WriteableBitmap(bmpimg);
                    wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                }
                signalIsBack = 0;
            }
        }

        private void btn_Camera_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            PhotoChooserTask pcTask = new PhotoChooserTask();
            pcTask.ShowCamera = true;
            pcTask.Completed += new EventHandler<PhotoResult>(pcTask_Completed);
            pcTask.Show();
        }

        private void btn_Menu_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
        }

        private void btn_Again_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml?gameClass=" + gameClass + "&imgUri=" + imgUri, UriKind.Relative));
        }
    }
}