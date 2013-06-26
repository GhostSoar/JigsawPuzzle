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

namespace JigsawPuzzle
{
    public partial class TopPage : PhoneApplicationPage
    {
        IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
        public TopPage()
        {
            InitializeComponent();

            #region 3And3
            InitializeListBox("3And3", lst_3And3);
            #endregion

            #region 4And4
            InitializeListBox("4And4", lst_4And4);
            #endregion

            #region 5And5
            InitializeListBox("5And5", lst_5And5);
            #endregion
        }

        void InitializeListBox(string gameClass,ListBox listBox)
        {
            if (!isf.DirectoryExists("img"))
                isf.CreateDirectory("img");
            if (!isf.DirectoryExists("img/" + gameClass))
                isf.CreateDirectory("img/" + gameClass);
            string[] directoryNames = isf.GetDirectoryNames("img/" + gameClass + "/*");
            listBox.Items.Clear();//解决后一页按钮后退键再次加载出现的问题
            Grid grid = new Grid();
            for (int j = 0; j < directoryNames.Length + 1; j++)//加1避免只创建一列出现的布局错误
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
                    img.VerticalAlignment = VerticalAlignment.Top;
                    img.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(img, (i % 2));
                    Grid.SetRow(img, (i / 2));
                    img.Source = bmpImg;
                    img.Name = imgUri;
                    img.Margin = new Thickness(20, 5, 20, 5);
                    grid.Children.Add(img);
                }
                i++;
                try
                {
                    using (IsolatedStorageFileStream isfStream = isf.OpenFile("img/" + gameClass + "/" + str + "/RecordOwner.jpg", FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage bmpImg = new BitmapImage();
                        bmpImg.SetSource(isfStream);
                        Image img = new Image();
                        img.Width = 180;
                        img.Height = 180;
                        img.VerticalAlignment = VerticalAlignment.Top;
                        img.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetColumn(img, (i % 2));
                        Grid.SetRow(img, (i / 2));
                        img.Source = bmpImg;
                        img.Name = imgUri;
                        img.Margin = new Thickness(20, 5, 20, 5);
                        grid.Children.Add(img);
                    }
                    TextBlock txtb_Record = new TextBlock();
                    txtb_Record.Text = GetRecord("img/" + gameClass + "/" + str);
                    txtb_Record.HorizontalAlignment = HorizontalAlignment.Center;
                    txtb_Record.VerticalAlignment = VerticalAlignment.Bottom;
                    txtb_Record.Margin = new Thickness(0, 0, 0, 30);
                    Grid.SetColumn(txtb_Record, (i % 2));
                    Grid.SetRow(txtb_Record, (i / 2));
                    grid.Children.Add(txtb_Record);
                }
                catch
                { }
                i++;
            }
            listBox.Items.Add(grid);
        }

        string GetRecord(string imgUri)
        {
            string str_RecordTime = "";
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
            return str_RecordTime;
        }
    }
}