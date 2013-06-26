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
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Input.Touch;

namespace JigsawPuzzle
{
    public partial class DIYPage : PhoneApplicationPage
    {

        Image _DIYImg;//图片
        Rectangle _cutImgRect;//截图框

        int _culomns;
        int _rows;
        double _bigImg_Width;
        double _bigImg_Height;
        double _smallImg_Width;
        double _smallImg_Height;

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Touch.FrameReported -= Touch_FrameReported;
            base.OnNavigatedFrom(e);
        }

        //protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    #region 接受页面传值
        //    //Uri para = this.NavigationService.Source;
        //    ////if (para.Count > 0)
        //    ////{
        //    ////    gameClass = (para["gameClass"]).ToString().Trim();
        //    ////}
        //    #endregion
        //}

        public DIYPage()
        {
            InitializeComponent();

            #region 显示图片
            _DIYImg = new Image();
            App app = Application.Current as App;
            if (app.bmpimg != null)
                _DIYImg.Source = app.bmpimg;
            if (app.bmpimg.PixelWidth > 480)
            {
                _DIYImg.Width = 480;
                _DIYImg.Height = app.bmpimg.PixelHeight * ((double)480 / app.bmpimg.PixelWidth);
                Canvas.SetLeft(_DIYImg, 0);
                Canvas.SetTop(_DIYImg, 0);
            }
            else
            {
                Canvas.SetLeft(_DIYImg, (480 - app.bmpimg.PixelWidth) / 2);
                Canvas.SetTop(_DIYImg, 0);
            }
            Carrier.Children.Add(_DIYImg);
            #endregion

            #region 截图框
            _cutImgRect = new Rectangle();
            _cutImgRect.Width = 480;
            _cutImgRect.Height = 480;
            _cutImgRect.Fill = new SolidColorBrush(Colors.Gray);
            _cutImgRect.Fill = new SolidColorBrush(Color.FromArgb(232, 23, 32, 44));
            _cutImgRect.Opacity = 0.8;
            Canvas.SetLeft(_cutImgRect, 0);
            Canvas.SetTop(_cutImgRect, 0);
            Canvas.SetZIndex(_cutImgRect, 99);
            Carrier.Children.Add(_cutImgRect);
            #endregion

            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
            TouchPanel.EnabledGestures = GestureType.Pinch;

        }

        double lastRectPositionX;
        double lastRectPositionY;
        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);

            // Inhibit mouse promotion  
            if (primaryTouchPoint != null && primaryTouchPoint.Action == TouchAction.Down)
                e.SuspendMousePromotionUntilTouchUp();

            TouchPointCollection touchPoints = e.GetTouchPoints(null);

            foreach (var item in touchPoints)
            {
                Rectangle touchIsRectangle = item.TouchDevice.DirectlyOver as Rectangle;
                while (TouchPanel.IsGestureAvailable)
                {
                    GestureSample gs = TouchPanel.ReadGesture();
                    switch (gs.GestureType)
                    {
                        case GestureType.Pinch:
                            //    Vector2 a = gs.Position;                      //取得第一個觸碰點
                            //Vector2 aOld = gs.Position - gs.Delta;//取得第一個觸碰點的起始位置
                            //Vector2 b = gs.Position2;                 //取得第二個觸碰點
                            //Vector2 bOld = gs.Position2 - gs.Delta2;//取得第二個觸碰點的起始位置
                            //float d = Vector2.Distance(a, b);           //計算兩個觸碰點之間的距離
                            //float dOld = Vector2.Distance(aOld, bOld);//計算兩個原始座標之間的距離
                            //float scaleChange = (d - dOld) * .01f;            //計算距離的變化量
                            //Scale += scaleChange;           //將距離變化量的 1/10 當做縮放的比例
                            if (touchIsRectangle != null)
                            {
                                double d1 = Math.Sqrt(Math.Abs(gs.Position.X - gs.Position2.X) * Math.Abs(gs.Position.X - gs.Position2.X) + Math.Abs(gs.Position.Y - gs.Position2.Y) * Math.Abs(gs.Position.Y - gs.Position2.Y));
                                double d2 = Math.Sqrt(Math.Abs((gs.Position.X - gs.Delta.X) - (gs.Position2.X - gs.Delta2.X)) * Math.Abs((gs.Position.X - gs.Delta.X) - (gs.Position2.X - gs.Delta2.X)) + Math.Abs((gs.Position.Y - gs.Delta.Y) - (gs.Position2.Y - gs.Delta2.Y)) * Math.Abs((gs.Position.Y - gs.Delta.Y) - (gs.Position2.Y - gs.Delta2.Y)));
                                double distance = (d1 - d2) * 0.5;
                                _cutImgRect.Width += distance;
                                _cutImgRect.Height += distance;
                            }
                            break;
                    }
                }


                if (item.Action == TouchAction.Down)
                {
                    if (touchIsRectangle != null)
                    {
                        lastRectPositionX = item.Position.X - Canvas.GetLeft(_cutImgRect);
                        lastRectPositionY = item.Position.Y - Canvas.GetTop(_cutImgRect);
                    }
                }

                if (item.Action == TouchAction.Move)
                {
                    if (touchIsRectangle != null)
                    {
                        Canvas.SetLeft(item.TouchDevice.DirectlyOver, item.Position.X - lastRectPositionX);
                        Canvas.SetTop(item.TouchDevice.DirectlyOver, item.Position.Y - lastRectPositionY);
                        Canvas.SetZIndex(item.TouchDevice.DirectlyOver, 999);//处于最上端
                    }
                }
            }
        }

        private void btn_3and3_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_3and3.Width = 140;
            btn_3and3.Height = 65;
        }

        private void btn_4and4_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_4and4.Width = 140;
            btn_4and4.Height = 65;
        }

        private void btn_5and5_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            btn_5and5.Width = 140;
            btn_5and5.Height = 65;
        }

        private void btn_3and3_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_3and3.Width = 150;
            btn_3and3.Height = 75;
            btn_ImgEnable();
            _culomns = 3;
            _rows = 3;
            CutImgAndRect("3And3");
        }

        private void btn_4and4_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_4and4.Width = 150;
            btn_4and4.Height = 75;
            btn_ImgEnable();
            _culomns = 4;
            _rows = 4;
            CutImgAndRect("4And4");
        }

        private void btn_5and5_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            btn_5and5.Width = 150;
            btn_5and5.Height = 75;
            btn_ImgEnable();
            _culomns = 5;
            _rows = 5;
            CutImgAndRect("5And5");
        }

        //开始截图后，按钮不可用
        void btn_ImgEnable()
        {
            btn_3and3.ManipulationStarted -= btn_3and3_ManipulationStarted;
            btn_3and3.ManipulationCompleted -= btn_3and3_ManipulationCompleted;
            btn_4and4.ManipulationStarted -= btn_4and4_ManipulationStarted;
            btn_4and4.ManipulationCompleted -= btn_4and4_ManipulationCompleted;
            btn_5and5.ManipulationStarted -= btn_5and5_ManipulationStarted;
            btn_5and5.ManipulationCompleted -= btn_5and5_ManipulationCompleted;
        }

        void CutImgAndRect(string gameClass)
        {
            progressBar.Visibility = Visibility.Visible;
            //存图片到独立存储空间
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isf.DirectoryExists("img"))
                isf.CreateDirectory("img");
            if (!isf.DirectoryExists("img/" + gameClass))
                isf.CreateDirectory("img/" + gameClass);
            string[] directoryNames = isf.GetDirectoryNames("img/" + gameClass + "/*");
            //if (filesName.Length == 0)
            //{
            //    if (!isf.DirectoryExists("img/" + gameClass + "/" + 1))
            //        isf.CreateDirectory("img/" + gameClass + "/" + 1);
            //}
            //else
            //{

            string currentUri;
            if (directoryNames.Length == 0)
            {
                if (!isf.DirectoryExists(string.Format("img/{0}/{1}", gameClass, (1).ToString().Trim())))
                    isf.CreateDirectory(string.Format("img/{0}/{1}", gameClass, (1).ToString().Trim()));
                currentUri = string.Format("img/{0}/{1}/", gameClass, (1).ToString().Trim());
            }
            else
            {
                int[] int_directoryNames = new int[directoryNames.Length];
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    int_directoryNames[i] = Convert.ToInt32(directoryNames[i]);
                }
                int MaxDirectoryName = int_directoryNames.Max();
                string strMaxDirectoryName=((MaxDirectoryName+1).ToString().Trim());
                if (!isf.DirectoryExists(string.Format("img/{0}/{1}", gameClass, strMaxDirectoryName)))
                    isf.CreateDirectory(string.Format("img/{0}/{1}", gameClass, strMaxDirectoryName));
                currentUri = string.Format("img/{0}/{1}/", gameClass, strMaxDirectoryName);
            }
            //}

            // 截屏
            WriteableBitmap screenshot = new WriteableBitmap(_DIYImg, null);
            screenshot = CutRect(screenshot);

            #region 保存原图
            //保存原图到图库
            //string filename = "originalImage.jpg";
            //MediaLibrary library = new MediaLibrary();
            //library.SavePicture(filename, EncodeToJpeg(screenshot));
            //保存原图到独立存储空间
            using (Stream storagefilestream = isf.OpenFile(currentUri+"originalImage.jpg", FileMode.OpenOrCreate, FileAccess.Write))
            {
                WriteableBitmap wb = new WriteableBitmap(screenshot);
                wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
            }
            #endregion

            _bigImg_Width = screenshot.PixelWidth;
            _bigImg_Height = screenshot.PixelHeight;
            _smallImg_Width = _bigImg_Width / _culomns;
            _smallImg_Height = _bigImg_Height / _rows;

            WriteableBitmap bmpimg;

            //开始循环切图
            for (int i = 0; i < _culomns * _rows; i++)
            {
                Image img = new Image();
                img.Width = _smallImg_Width; img.Height = _smallImg_Height;//小图的长宽
                //传递参数到裁剪方法中(大图的位图,小图的起点x和y,长和宽)
                bmpimg = CutImage(screenshot, (i % _culomns) * _smallImg_Width, Math.Floor(i / _rows) * _smallImg_Height, _smallImg_Width, _smallImg_Height);
                img.Source = bmpimg;
                //显示截图图片
                Canvas.SetLeft(img, (i % _culomns) * _smallImg_Width + Canvas.GetLeft(_cutImgRect));
                Canvas.SetTop(img, Math.Floor(i / _rows) * _smallImg_Height + Canvas.GetTop(_cutImgRect));
                Carrier.Children.Add(img);

                #region 保存截图
                //存图片到图库
                //library = new MediaLibrary();
                //byte[] bb = EncodeToJpeg(CutImage(screenshot, (i % _culomns) * _smallImg_Width, Math.Floor(i / _rows) * _smallImg_Height, _smallImg_Width, _smallImg_Height));
                //library.SavePicture("a" + (i + 1), bb);
                //存图片到独立存储空间
                using (Stream storagefilestream = isf.OpenFile(currentUri+"a" + (i + 1) + ".jpg", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    WriteableBitmap wb = new WriteableBitmap(bmpimg);
                    wb.SaveJpeg(storagefilestream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                }
                #endregion
            }

            _DIYImg.Visibility = Visibility.Collapsed;
            MessageBox.Show("成功");
            progressBar.Visibility = Visibility.Collapsed;
        }

        private WriteableBitmap CutRect(WriteableBitmap wb)
        {
            // 裁剪方法
            Image img1 = new Image();
            img1.Stretch = Stretch.None;
            //移动大图的起点到小图起点
            img1.RenderTransform = new TranslateTransform { X = -1 * Canvas.GetLeft(_cutImgRect) + 1, Y = -1 * Canvas.GetTop(_cutImgRect) + 1 };
            img1.Source = wb;

            Canvas C = new Canvas();
            C.Width = _cutImgRect.Width + 1;
            C.Height = _cutImgRect.Height + 1;
            C.Children.Add(img1);
            return new WriteableBitmap(C, null);

        }

        public byte[] EncodeToJpeg(WriteableBitmap wb)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveJpeg(stream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                return stream.ToArray();
            }
        }

        private WriteableBitmap CutImage(WriteableBitmap wb, double x, double y, double width, double height)
        {
            // 裁剪方法

            Image img1 = new Image();
            img1.Stretch = Stretch.None;
            //移动大图的起点到小图起点
            img1.RenderTransform = new TranslateTransform { X = -1 * x, Y = -1 * y };
            img1.Source = wb;

            Canvas C = new Canvas();
            C.Width = width;
            C.Height = height;
            C.Children.Add(img1);

            return new WriteableBitmap(C, null);

        }

    }
}