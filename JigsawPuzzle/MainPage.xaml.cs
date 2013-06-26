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
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.IO.IsolatedStorage;

namespace JigsawPuzzle
{
    public partial class MainPage : PhoneApplicationPage
    {
        App app = Application.Current as App;
        bool a = false;

        string gameClass;//判断是哪种拼图
        string imgUri;

        Grid g;//表格
        int tableColumn;//创建方格的列数
        int tableRow;//创建方格的行数
        MyImages littleImages;//拼图集合
        double smallSquareWidth;//获取小拼图的宽
        double smallSquareHeight;//获取小拼图的高
        int imgsCount;//小拼图的总数

        vector2D[,] tableVectors;
        struct vector2D { public double x; public double y;}

        string[] arr_SequenceIsRight;
        string str_SequenceIsRight;

        DispatcherTimer dispatcherTimerChecked;//定义线程检测是否结束游戏

        //计时
        TextBlock txtb_Time;
        public DateTime StartTime;//记录开始计时的时间
        DispatcherTimer timer = new DispatcherTimer();//定义一个计时器，类似线程
        TimeSpan count = new TimeSpan(00, 00,00);//记录时间

        //显示原图
        Image originalImage = new Image();

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Touch.FrameReported -= Touch_FrameReported;
            NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass="+gameClass, UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            #region 接受页面传值
            IDictionary<string, string> para = this.NavigationContext.QueryString;
            if (para.Count > 0)
            {
                gameClass = (para["gameClass"]).ToString().Trim();
                imgUri = (para["imgUri"]).ToString().Trim();
            }
            #endregion

            #region 处理墓碑错误
            if (app.str_Uri != null)
            {
                NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass=" + gameClass, UriKind.Relative));
            }
            app.str_Uri = null;
            #endregion

            #region 处理表格和图片相关初始值
            switch (gameClass)
            {
                case "5And5":
                    InitializeAboutGameClass(5,5,75,75,25);//5*5
                    break;
                case "4And4":
                    InitializeAboutGameClass(4,4,90,90,16);//4*4
                    break;
                case "3And3":
                    InitializeAboutGameClass(3,3,120,120,9);//3*3
                    break;
                default: break;
            }
            #endregion

            #region 创建方格和加载图片
            ImageBrush berriesBrush = new ImageBrush();
            berriesBrush.ImageSource =
                new BitmapImage(
                    new Uri(@"bg_InGame.jpg", UriKind.Relative)
                );
            Carrier.Background = berriesBrush;
            //Image Map = new Image();
            //Map.Width = 480;
            //Map.Height = 800;
            //Map.Source = new BitmapImage((new Uri(@"bg_InGame.jpg", UriKind.Relative)));
            //Carrier.Children.Add(Map);
            //Map.SetValue(Canvas.ZIndexProperty, -1);//ZIndexProperty附加属性,这样它就相当于地图背景的作用

            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);

            Table t = new Table(tableColumn, tableRow, smallSquareWidth, smallSquareHeight);
            g = t.CreateTable();
            Carrier.Children.Add(g);


            littleImages = new MyImages(smallSquareWidth, smallSquareHeight);
            List<Image> listImages = littleImages.CreateImages(imgsCount,imgUri);
            try
            {
                foreach (var u in listImages)
                {
                    Carrier.Children.Add(u);
                }
            }
            catch
            { }

            tableVectors = new vector2D[tableRow + 1, tableColumn + 1];
            vector2D a = new vector2D();
            for (int i = 0; i < tableRow + 1; i++)
            {
                for (int j = 0; j < tableColumn + 1; j++)
                {
                    a.x = j * t.SmallSquareWidth + (480 - t.Column * t.SmallSquareWidth) / 2;
                    a.y = i * t.SmallSquareHeight;
                    tableVectors[i, j] = a;
                }
            }

            #region 原图

            originalImage.Name = "originalImage";
            #region 从存储过程中读取原图
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isf.DirectoryExists(imgUri))
                isf.CreateDirectory(imgUri);
            string[] imgsNames = isf.GetFileNames(imgUri + "/originalImage*.jpg");
            try
            {
                using (IsolatedStorageFileStream isfStream = isf.OpenFile(imgUri + "/" + imgsNames[0], FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bmpImg = new BitmapImage();
                    bmpImg.SetSource(isfStream);
                    originalImage.Source = bmpImg;
                }
            }
            catch
            { }
            #endregion
            //originalImage.Source = new BitmapImage(new Uri("img/5And5/1/originalImage.jpg", UriKind.Relative));
            originalImage.Width = g.Width;
            originalImage.Height = g.Height;
            Thickness gMargin = (Thickness)g.GetValue(MarginProperty);
            Canvas.SetLeft(originalImage, gMargin.Left);
            Canvas.SetTop(originalImage, gMargin.Top);

            Canvas.SetZIndex(originalImage, -1);
            try {
                Carrier.Children.Add(originalImage);
            }
            catch
            {

            }
            originalImage.Visibility = Visibility.Collapsed;

            #endregion

            #endregion

            #region 读取独立存储空间

            //switch (MusicName)
            //{
            //    case "Set Fire To The Rain": ReadIsolatedStorageFile(highPointCount[0], 1);//读取独立存储空间的歌曲高潮点文件,highPointCount[]歌曲名,1文件名
            //        break;
            //    case "What Doesn't Kill You": ReadIsolatedStorageFile(highPointCount[1], 2);//读取独立存储空间的歌曲高潮点文件
            //        break;
            //    default: break;
            //}
            #endregion

        }

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            app.str_Uri = null;

            #region 计时
            txtb_Time = new TextBlock();
            Canvas.SetLeft(txtb_Time, 350);
            Canvas.SetTop(txtb_Time, 0);
            Carrier.Children.Add(txtb_Time);
            this.txtb_Time.Text = count.ToString();//开始时为00:00:00
            this.timer.Tick += Timer_Tick;//时间控件timer，事件Tick，类似线程
            this.timer.Interval = TimeSpan.FromMilliseconds(100);//每100毫秒变化一次
            StartTime = DateTime.UtcNow;
            this.timer.Start();//控件timer开始计时，类似线程开始
            #endregion

            #region 判断是否游戏结束
            dispatcherTimerChecked = new DispatcherTimer();
            dispatcherTimerChecked.Tick += new EventHandler(dispatcherTimerChecked_Tick);
            dispatcherTimerChecked.Interval = TimeSpan.FromMilliseconds(50);
            dispatcherTimerChecked.Start();
            #endregion
        }

        //解决移动，目标的不确定性
        string downDirectlyOver= "";
        string moveDirectlyOver = "";

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);

            // Inhibit mouse promotion  
            if (primaryTouchPoint != null && primaryTouchPoint.Action == TouchAction.Down)
                e.SuspendMousePromotionUntilTouchUp();

            TouchPointCollection touchPoints = e.GetTouchPoints(null);

            foreach (var item in touchPoints)
            {
                if (item.Action == TouchAction.Down)
                {
                    //if (item.TouchDevice.DirectlyOver == image1)
                    //{
                        a = true;
                    //}
                        Image touchIsImage = item.TouchDevice.DirectlyOver as Image;
                        if (touchIsImage != null && touchIsImage.Name != "originalImage")
                        {
                            try
                            {
                                Carrier.Children.Remove(item.TouchDevice.DirectlyOver);//移掉Image控件在Carrier的子节点
                                g.Children.Remove(item.TouchDevice.DirectlyOver);//移掉Image控件在Grid的子节点
                                Carrier.Children.Add(item.TouchDevice.DirectlyOver);//移掉Image控件在Carrier的子节点
                            }
                            catch
                            { }
                            finally
                            { 
                            }
                            //try 
                            //{
                            //    Carrier.Children.Add(item.TouchDevice.DirectlyOver);//移掉Image控件在Carrier的子节点
                            //}
                            //catch { }
                            
                            downDirectlyOver = item.TouchDevice.DirectlyOver.GetValue(NameProperty).ToString();
                        }
                        originalImage.Visibility = Visibility.Collapsed;
                }

                if (item.Action == TouchAction.Move)
                {
                    moveDirectlyOver = item.TouchDevice.DirectlyOver.GetValue(NameProperty).ToString();
                    if (a == true && moveDirectlyOver == downDirectlyOver)
                    {
                        Canvas.SetLeft(item.TouchDevice.DirectlyOver, item.Position.X - smallSquareWidth / 1.8);
                        Canvas.SetTop(item.TouchDevice.DirectlyOver, item.Position.Y - smallSquareHeight / 1.8);
                        Canvas.SetZIndex(item.TouchDevice.DirectlyOver, 999);//处于最上端
                    }
                }

                if (item.Action == TouchAction.Up)
                {
                    a = false;
                    if (gameClass == "5And5")
                    {
                        #region 第一行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[0, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[1, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 0, 0);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 1, 1);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 2, 2);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 3, 3);
                            }
                            //5
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 4].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 5].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 4, 4);
                            }
                        }
                        #endregion

                        #region 第二行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[1, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[2, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 0, 5);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 1, 6);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 2, 7);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 3, 8);
                            }
                            //5
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 4].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 5].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 4, 9);
                            }
                        }
                        #endregion

                        #region 第三行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[2, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[3, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 0, 10);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 1, 11);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 2, 12);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 3, 13);
                            }
                            //5
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 4].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 5].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 4, 14);
                            }
                        }
                        #endregion

                        #region 第四行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[3, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[4, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 0, 15);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 1, 16);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 2, 17);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 3, 18);
                            }
                            //5
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 4].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 5].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 4, 19);
                            }
                        }
                        #endregion

                        #region 第五行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[4, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[5, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[4, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[4, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 4, 0, 20);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[4, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[4, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 4, 1, 21);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[4, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[4, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 4, 2, 22);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[4, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[5, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 4, 3, 23);
                            }
                            //5
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[4, 4].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[4, 5].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 4, 4, 24);
                            }
                        }
                        #endregion
                    }

                    if (gameClass == "4And4")
                    {
                        #region 第一行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[0, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[1, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 0, 0);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 1, 1);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 2, 2);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 3, 3);
                            }
                        }
                        #endregion

                        #region 第二行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[1, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[2, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 0, 4);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 1, 5);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 2, 6);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 3, 7);
                            }
                        }
                        #endregion

                        #region 第三行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[2, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[3, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 0, 8);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 1, 9);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 2, 10);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 3, 11);
                            }
                        }
                        #endregion

                        #region 第四行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[3, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[4, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 0, 12);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 1, 13);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 2, 14);
                            }
                            //4
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[3, 3].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[3, 4].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 3, 3, 15);
                            }
                        }
                        #endregion
                    }

                    if (gameClass == "3And3")
                    {
                        #region 第一行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[0, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[1, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 0, 0);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 1, 1);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[0, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[0, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 0, 2, 2);
                            }
                        }
                        #endregion

                        #region 第二行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[1, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[2, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 0, 3);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 1, 4);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[1, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[1, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 1, 2, 5);
                            }
                        }
                        #endregion

                        #region 第三行
                        if (Canvas.GetTop(item.TouchDevice.DirectlyOver) > (380 + tableVectors[2, 0].y) && Canvas.GetTop(item.TouchDevice.DirectlyOver) <= (380 + tableVectors[3, 0].y))
                        {
                            //1
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 0].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 1].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 0, 6);
                            }
                            //2
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 1].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 2].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 1, 7);
                            }
                            //3
                            if (Canvas.GetLeft(item.TouchDevice.DirectlyOver) >= tableVectors[2, 2].x && Canvas.GetLeft(item.TouchDevice.DirectlyOver) <= tableVectors[2, 3].x)
                            {
                                MagnetFunc((FrameworkElement)item.TouchDevice.DirectlyOver, 2, 2, 8);
                            }
                        }
                        #endregion
                    }

                    Canvas.SetZIndex(item.TouchDevice.DirectlyOver, 0);//使拼图返回到原来的层次
                }
            }

        }

        void MagnetFunc(FrameworkElement frameworkElement, int row, int culomn, int arr_SequenceIsRightIndex)
        {
            for (int i = 0; i < g.Children.Count; i++)
            {
                Image img = g.Children[i] as Image;
                if (img != null)
                {
                    if (Grid.GetRow(img) == row && Grid.GetColumn(img) == culomn)
                    {
                        return;
                    }
                }
            }
            try
            {
                Grid.SetRow(frameworkElement, row);
                Grid.SetColumn(frameworkElement, culomn);
                Carrier.Children.Remove(frameworkElement);//移掉Image控件在Carrier的子节点
                g.Children.Add(frameworkElement);//将在Carrier中移掉的Image控件添加到Grid中
            }
            catch
            {}

            arr_SequenceIsRight[arr_SequenceIsRightIndex] = frameworkElement.GetValue(NameProperty).ToString();

        }

        void dispatcherTimerChecked_Tick(object sender, EventArgs e)
        {
            str_SequenceIsRight = "";
            
            foreach (var z in arr_SequenceIsRight)
            {
                str_SequenceIsRight += z;
            }
            if (str_SequenceIsRight == littleImages.str_RightAnswer)
            {
                NavigationService.Navigate(new Uri("/GameResult.xaml?gameClass=" + gameClass + "&imgUri=" + imgUri + "&time=" + txtb_Time.Text.ToString().Trim(), UriKind.Relative));
                Touch.FrameReported -= Touch_FrameReported;
                App app = Application.Current as App;
                app.time = count;//将游戏总时间传递给App作为一个全局变量存储
                dispatcherTimerChecked.Stop();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            count = DateTime.UtcNow - StartTime;//记录时间间隔
            this.txtb_Time.Text = count.ToString();
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            originalImage.Visibility = Visibility.Visible;
        }

        void InitializeAboutGameClass(int _tableColumn, int _tableRow, int _smallSquareWidth, int _smallSquareHeight, int _arr_SequenceIsRightCount)
        {
            tableColumn = _tableColumn;
            tableRow = _tableRow;
            smallSquareWidth = _smallSquareWidth;
            smallSquareHeight = _smallSquareHeight;
            imgsCount = _tableColumn * _tableRow;
            arr_SequenceIsRight = new string[_arr_SequenceIsRightCount];
        }

    }
}