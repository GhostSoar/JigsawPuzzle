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

namespace JigsawPuzzle
{
    public partial class GameClassChoosePage : PhoneApplicationPage
    {
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/StartPage.xaml", UriKind.Relative));
            base.OnBackKeyPress(e);
        }

        public GameClassChoosePage()
        {
            InitializeComponent();
        }


        private void img_btn_3And3_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            img_btn_3And3.Width = 390;
            img_btn_3And3.Height = 190;
        }

        private void img_btn_3And3_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            img_btn_3And3.Width = 400;
            img_btn_3And3.Height = 200;
            NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass=3And3", UriKind.Relative));
        }

        private void img_btn_4And4_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            img_btn_4And4.Width = 390;
            img_btn_4And4.Height = 190;
        }

        private void img_btn_4And4_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            img_btn_4And4.Width = 400;
            img_btn_4And4.Height = 200;
            NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass=4And4", UriKind.Relative));
        }


        private void img_btn_5And5_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            img_btn_5And5.Width = 390;
            img_btn_5And5.Height = 190;
        }

        private void img_btn_5And5_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            img_btn_5And5.Width = 400;
            img_btn_5And5.Height = 200;
            NavigationService.Navigate(new Uri("/ChoosePictures.xaml?gameClass=5And5", UriKind.Relative));
        }
    }
}