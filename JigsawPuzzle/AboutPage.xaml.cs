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

namespace JigsawPuzzle
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();//标题
            emailComposeTask.Subject = "拼图(JigsawPuzzle Feedback)用户 ";
            emailComposeTask.To = "suguoqiang@outlook.com";//收件人
            emailComposeTask.Show();
        }
    }
}