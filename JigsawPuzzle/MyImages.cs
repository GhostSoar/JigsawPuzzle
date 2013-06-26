using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.IO.IsolatedStorage;

namespace JigsawPuzzle
{
    public class MyImages
    {
        private double _width;
        public double Width { get { return _width; } set { _width = value; } }
        private double _height;
        public double Height { get { return _height; } set { _height = value; } }
        public string str_RightAnswer;

        public List<int> listImgsSort = new List<int>();

        public MyImages(double width,double height)
        {
            _width = width;
            _height = height;
        }

        public List<Image> CreateImages(int count, string imageUri)
        {
            var listImages = new List<Image>();
            int j = 0;
            int z = 0;
            string imgUri = imageUri;
            int culomn = 0;
            int row = 0;
            if (count == 25)
            {
                culomn = 5;
                row = 5;
            }
            else if (count == 16)
            {
                culomn = 4;
                row = 4;
            }
            else if (count == 9)
            {
                culomn = 3;
                row = 3;
            }
            for (int i = 1; i <= count; i++)
            {
                #region 实现拼图随机排列
                Random rd = new Random();
                int dd = rd.Next(1, (count+1));
                while (listImgsSort.Contains(dd))
                {
                    dd = dd == count ? 1 : dd + 1;
                }
                listImgsSort.Add(dd);
                #endregion
                Image img = new Image();
                img.Width = _width;
                img.Height = _height;
                Canvas.SetLeft(img, (480 - (_width + 5) * culomn) / 2 + (_width + 5) * j);
                Canvas.SetTop(img, 30 + _height * z);
                #region 从存储过程中读取图片
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isf.DirectoryExists(imgUri))
                    isf.CreateDirectory(imgUri);
                string[] imgsNames = isf.GetFileNames(imgUri + "/a*.JPG");
                try
                {
                    using (IsolatedStorageFileStream isfStream = isf.OpenFile(imgUri + "/" + imgsNames[dd - 1], FileMode.Open, FileAccess.Read))
                    {
                        BitmapImage bmpImg = new BitmapImage();
                        bmpImg.SetSource(isfStream);
                        img.Source = bmpImg;
                    }
                }
                catch
                { }
                #endregion
                //BitmapImage bitmap = new BitmapImage(new Uri(string.Format(imgUri+@"\a{0}.jpg", i), UriKind.Relative));
                img.Name = string.Format("a{0}", dd);
                listImages.Add(img);
                str_RightAnswer += string.Format("a{0}", i);

                if (count == 25)
                {
                    if (j < 4)
                    {
                        j++;
                    }
                    else
                    {
                        z++;
                        j = 0;
                    }
                }

                if (count == 16)
                {
                    if (j < 3)
                    {
                        j++;
                    }
                    else
                    {
                        z++;
                        j = 0;
                    }
                }

                if (count == 9)
                {
                    if (j < 2)
                    {
                        j++;
                    }
                    else
                    {
                        z++;
                        j = 0;
                    }
                }
            }
            return listImages;
        }
    }
}
