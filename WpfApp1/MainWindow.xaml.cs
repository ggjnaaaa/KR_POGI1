using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;
        public MainWindow()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (sw.IsRunning)
            {
                TimeSpan ts = sw.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}",
                ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                timer.Content = currentTime;
            }
        }

        public int kNums; 

        public int i, j, m, q;

        public int[,] nums;

        public int status;
        //статус игры:
        //0-начало
        //1-активная игра
        //2-проигрыш или выигрыш

        public void eazy_click(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
            ugr.Children.Clear();
            timer.Content = "00:00:00";
            i = 10;
            j = 10;
            m = 10;
            nums = new int[12, 12];
            status = 0;
            mi();
        }

        public void medium_click(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
            ugr.Children.Clear();
            timer.Content = "00:00:00";
            i = 16;
            j = 16;
            m = 40;
            nums = new int[18, 18];
            status = 0;
            mi();
        }

        public void hard_click(object sender, RoutedEventArgs e)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }
            ugr.Children.Clear();
            timer.Content = "00:00:00";
            i = 30;
            j = 16;
            m = 99;
            nums = new int[32, 18];
            status = 0;
            mi();
        }

        void mi() 
        {
            int r, c; 
            int n = 0;  
            int k;      

            for (r = 0; r <= i + 1; r++)
            {
                nums[r, 0] = -3;
                nums[r, j + 1] = -3;
            }

            for (c = 0; c <= j + 1; c++)
            {
                nums[0, c] = -3;
                nums[i + 1, c] = -3;
            }

            for (r = 1; r <= i; r++)
                for (c = 1; c <= j; c++)
                    nums[r, c] = 0;

            Random rnd = new Random();
            do
            {
                r = rnd.Next(1, i);
                c = rnd.Next(1, j);
                k = 0;
                if (r <= i + 1 && c <= j + 1 && r != 0 && c != 0)
                {
                    if (nums[r, c] != 9)
                    {
                        if (nums[r - 1, c - 1] != 9) k++;
                        if (nums[r - 1, c] != 9) k++;
                        if (nums[r - 1, c + 1] != 9) k++;
                        if (nums[r, c - 1] != 9) k++;
                        if (nums[r, c + 1] != 9) k++;
                        if (nums[r + 1, c - 1] != 9) k++;
                        if (nums[r + 1, c] != 9) k++;
                        if (nums[r + 1, c + 1] != 9) k++; 
                    }
                }
                if (k != 0)
                {
                    nums[r, c] = 9;
                    n++;
                }
            }
            while (n != m);

            for (r = 1; r <= i + 1; r++)
            {
                for (c = 1; c <= j + 1; c++)
                {
                    k = 0;
                    if (nums[r, c] != 9 && nums[r, c] != -3)
                    {
                        k = 0;

                        if (nums[r - 1, c - 1] == 9) k++;
                        if (nums[r - 1, c] == 9) k++;
                        if (nums[r - 1, c + 1] == 9) k++; 
                        if (nums[r, c - 1] == 9) k++;
                        if (nums[r, c + 1] == 9) k++;
                        if (nums[r + 1, c - 1] == 9) k++;
                        if (nums[r + 1, c] == 9) k++;
                        if (nums[r + 1, c + 1] == 9) k++;

                        nums[r, c] = k;
                    }
                }
            }
            ugr.Width = 40 * i + i*2;
            ugr.Height = 40 * j + j*2;
            ugr.Rows = j;
            ugr.Columns = i;
            ugr.Margin = new Thickness(5);
            diff.Text = " ";

            for (int g = 1; g <= i; g++)
            {
                 for (int h = 1; h <= j; h++)
                 {
                    Button btn = new Button();
                    btn.Tag = nums[g, h];
                    btn.Width = 40;
                    btn.Height = 40;
                    btn.Content = " ";
                    btn.Margin = new Thickness(1);
                    ugr.Children.Add(btn);
                    btn.PreviewMouseDown += Btn_MouseDown;
                 }
            }

            kNums = 0;
        }

        private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                BitmapImage mine = new BitmapImage(new Uri(@"pack://application:,,,/img/Mine.jpg", UriKind.Absolute));
                int n = (int)((Button)sender).Tag;
                     
                if (n < 9 && n != -3)
                {
                    if (status == 0)
                    {
                        status = 1;
                    }
                    if (status == 2)
                    {
                        ((Button)sender).IsEnabled = false;
                        return;
                    }
                    ((Button)sender).Background = Brushes.White;
                    ((Button)sender).Foreground = Brushes.Red;
                    ((Button)sender).FontSize = 20;
                    ((Button)sender).Content = n.ToString();
                    kNums++;
                    ((Button)sender).IsEnabled = false;

                    if (!sw.IsRunning)
                    {
                        sw.Start();
                        dt.Start();
                    }
                }

                if (kNums == i * j - m)
                {
                    if (status == 2)
                    {
                        ((Button)sender).IsEnabled = false;
                        return;
                    } 
                    else if (status == 1)
                    {
                        status = 2;
                    }
                    sw.Stop();
                    MessageBox.Show("Поздравляем! вы выиграли! Выберите сложность, чтобы начать новую игру\n" + "Время: " + currentTime);
                }

                if (n == 9)
                {
                    if (status == 2)
                    {
                        ((Button)sender).IsEnabled = false;
                        return;
                    }
                    Image img = new Image();
                    img.Source = mine;
                    StackPanel stackPnl = new StackPanel();
                    stackPnl.Margin = new Thickness(1);
                    stackPnl.Children.Add(img);
                    ((Button)sender).Content = stackPnl;
                    if (sw.IsRunning)
                    {
                        sw.Stop();
                    }
                    MessageBox.Show("Вы проиграли. Выберите сложность, чтобы начать новую игру");
                    kNums = 0;
                    status = 2;
                    ((Button)sender).IsEnabled = false;
                }
            }
           
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (status == 2)
                {
                    ((Button)sender).IsEnabled = false;
                    return;
                }
                BitmapImage mark = new BitmapImage(new Uri(@"pack://application:,,,/img/Mark.png", UriKind.Absolute));
                Image img = new Image();
                img.Source = mark;
                StackPanel stackPnl = new StackPanel();
                stackPnl.Margin = new Thickness(1);
                stackPnl.Children.Add(img);
                ((Button)sender).Content = stackPnl;
            }
        }
    }
}
