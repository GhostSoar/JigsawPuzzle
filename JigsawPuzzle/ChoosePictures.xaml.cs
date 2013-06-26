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
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Input.Touch; 

namespace JigsawPuzzle
{
    public partial class ChoosePictures : PhoneApplicationPage
    {
        string gameClass;
        Image lastTouchImage;
        string deleteImg;

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GameClassChoosePage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Touch.FrameReported -= Touch_FrameReported;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            #region 接受页面传值
            IDictionary<string, string> para = this.NavigationContext.QueryString;
            if (para.Count > 0)
            {
                gameClass = (para["gameClass"]).ToString().Trim();
            }
            #endregion
            Update();
        }

        void Update()
        {
            Carrier.Items.Clear();
            #region 从存储过程中读取图片
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isf.DirectoryExists("img"))
            {
                isf.CreateDirectory("img");
            }
            if (!isf.DirectoryExists("img/" + gameClass))
            {
                isf.CreateDirectory("img/" + gameClass);
            }
            string[] directoryNames = isf.GetDirectoryNames("img/" + gameClass + "/*");
            //List<TopValue> tv = new List<TopValue> { };

            #region 创建ListBox中的Grid
            Carrier.Items.Clear();//解决后一页按钮后退键再次加载出现的问题
            Grid grid = new Grid();
            for (int j = 0; j < directoryNames.Length; j++)
            {
                ColumnDefinition ColumnDefinition = new ColumnDefinition();//定义Grid列属性对象
                ColumnDefinition.Width = new GridLength(240, GridUnitType.Pixel);//初始化GridLength并设置指定的值
                grid.ColumnDefinitions.Add(ColumnDefinition);//添加ColumnDefinition到Grid的列属性集合中
            }

            for (int j = 0; j < directoryNames.Length; j++)
            {
                RowDefinition RowDefinition = new RowDefinition();//定义Grid列属性对象
                RowDefinition.Height = new GridLength(240, GridUnitType.Pixel);//初始化GridLength并设置指定的值
                grid.RowDefinitions.Add(RowDefinition);//添加ColumnDefinition到Grid的列属性集合中
            }
            #endregion
            int i = 0;
            string imgUri;
            foreach (string str in directoryNames)
            {
                imgUri = string.Format("img/{0}/{1}", gameClass, str);
                using (IsolatedStorageFileStream isfStream = isf.OpenFile("img/" + gameClass + "/" + str + "/originalImage.jpg", FileMode.Open, FileAccess.Read))
                {
                    BitmapImage bmpImg = new BitmapImage();
                    bmpImg.SetSource(isfStream);
                    Image img = new Image();
                    img.Width = 200;
                    img.Height = 200;
                    Grid.SetColumn(img, (i % 2));
                    Grid.SetRow(img, (i / 2));
                    img.Source = bmpImg;
                    img.Name = imgUri;
                    img.Margin = new Thickness(20, 5, 20, 5);
                    grid.Children.Add(img);
                }
                i++;
            }
            Carrier.Items.Add(grid);
            #endregion

            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
        }

        public ChoosePictures()
        {
            InitializeComponent();

            TouchPanel.EnabledGestures = GestureType.Hold | GestureType.Tap | GestureType.DoubleTap | GestureType.Flick |
                 GestureType.FreeDrag | GestureType.HorizontalDrag | GestureType.VerticalDrag;

            InfoControl.btn_Delete.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(btn_Delete_ManipulationCompleted);
        }

        void btn_Delete_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (MessageBox.Show("确定删除？", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (isoStore.DirectoryExists(deleteImg))
                {
                    DeleteDirectory(isoStore,deleteImg);//自定义算法删除独立存储空间的文件夹
                }
                Touch.FrameReported -= Touch_FrameReported;//避免在函数Update中二次添加Touch
                Update();
            }
            else
            {
            }
            //取消同理 DialogResult.Cancel
        }

        public static void DeleteDirectory(IsolatedStorageFile store, String root)
        {
            String dir = root;

            //  delete file in current dir 
            foreach (String file in store.GetFileNames(dir + "/*"))
            {
                store.DeleteFile(dir + "/" + file);
            }

            //  delete sub-dir 
            foreach (String subdir in store.GetDirectoryNames(dir + "/*"))
            {
                DeleteDirectory(store, dir + "/" + subdir + "/");
            }

            //  delete current dir 
            store.DeleteDirectory(dir);
        } 

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);

            // Inhibit mouse promotion  
            if (primaryTouchPoint != null && primaryTouchPoint.Action == TouchAction.Down)
                e.SuspendMousePromotionUntilTouchUp();

            TouchPointCollection touchPoints = e.GetTouchPoints(null);

            foreach (var item in touchPoints)
            {
                while (TouchPanel.IsGestureAvailable)
                {
                    Image touchIsImage = item.TouchDevice.DirectlyOver as Image;
                    GestureSample gs = TouchPanel.ReadGesture();

                    switch (gs.GestureType)
                    {
                        case GestureType.Tap:
                            InfoControl.Visibility = Visibility.Collapsed;//隐藏用户控件CancelControl
                            if (lastTouchImage != null)
                            {
                                lastTouchImage.Width = 200;
                                lastTouchImage.Height = 200;
                            }
                            if (touchIsImage != null)
                            {
                                //触摸图片变小
                                touchIsImage.Width = 190;
                                touchIsImage.Height = 190;
                                NavigationService.Navigate(new Uri("/MainPage.xaml?gameClass=" + gameClass + "&imgUri=" + touchIsImage.Name, UriKind.Relative));
                                Touch.FrameReported -= Touch_FrameReported;
                            }
                            break;

                        case GestureType.DoubleTap:
                            break;

                        case GestureType.Hold:
                            if (touchIsImage != null)
                            {
                                //触摸图片变小
                                lastTouchImage = touchIsImage;
                                touchIsImage.Width = 150;
                                touchIsImage.Height = 150;
                                deleteImg = touchIsImage.Name;
                                InfoControl.Visibility = Visibility.Visible;
                                InfoControl.Margin = new Thickness(0, item.Position.Y, 0, 0);
                            }
                            break;

                        case GestureType.FreeDrag:
                            break;

                        case GestureType.HorizontalDrag:
                            break;

                        case GestureType.VerticalDrag:
                            break;

                        case GestureType.Flick:
                            break;
                    }
                }
            }

        }
    }
}