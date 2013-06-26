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

namespace JigsawPuzzle
{
    public class Table
    {
        private int _column;//列数
        public int Column { get { return _column; } set { _column = value; } }
        private int _row;//行数
        public int Row { get { return _row; } set { _row = value; } }
        private double _smallSquareWidth;//小方格的宽度
        public double SmallSquareWidth { get { return _smallSquareWidth; } set { _smallSquareWidth = value; } }
        private double _smallSquareHeight;//小方格的高度
        public double SmallSquareHeight { get { return _smallSquareHeight; } set { _smallSquareHeight = value; } }

        public Table(int column, int row, double smallSquareWidth, double smallSquareHeight)
        {
            _column = column;
            _row = row;
            _smallSquareWidth = smallSquareWidth;
            _smallSquareHeight = smallSquareHeight;
        }

        //创建方格
        public Grid CreateTable()
        {
            Grid table = new Grid();

            table.ShowGridLines = true;
            table.Margin = new Thickness((480-_column*_smallSquareWidth)/2, 380, 0, 5);
            table.HorizontalAlignment = HorizontalAlignment.Center;
            table.Height = _column * _smallSquareHeight;
            table.Width = _row * _smallSquareWidth;

            for (int i = 0; i < _column; i++)
            {
                ColumnDefinition ColumnDefinition = new ColumnDefinition();//定义Grid列属性对象
                ColumnDefinition.Width = new GridLength(_smallSquareWidth, GridUnitType.Pixel);//初始化GridLength并设置指定的值
                table.ColumnDefinitions.Add(ColumnDefinition);//添加ColumnDefinition到Grid的列属性集合中
            }

            for (int i = 0; i < _row; i++)
            {
                RowDefinition RowDefinition = new RowDefinition();//定义Grid列属性对象
                RowDefinition.Height = new GridLength(_smallSquareHeight, GridUnitType.Pixel);//初始化GridLength并设置指定的值
                table.RowDefinitions.Add(RowDefinition);//添加ColumnDefinition到Grid的列属性集合中
            }
            for (int j = 0; j < _column; j++)
            {
                for (int i = 0; i < _row; i++)
                {
                    Border border = new Border();
                    border.BorderBrush = new SolidColorBrush(Colors.Red);
                    border.BorderThickness = new Thickness(1);
                    border.Width = _smallSquareWidth;
                    border.Height = _smallSquareHeight;
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                    table.Children.Add(border);
                }
            }

            return table;
        }
    }
}
