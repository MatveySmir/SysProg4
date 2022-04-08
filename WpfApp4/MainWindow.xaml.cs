using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int ThreadCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetEnabled(bool state)
        {
            StartWithThreadLimitButton.IsEnabled = state;
            StartWithoutThreadButton.IsEnabled = state;
            StartWithThreadButton.IsEnabled = state;
            TimeOperation.IsEnabled = state;
            System.Windows.Forms.Application.DoEvents();
        }

        //прверка чисел
        private long getNum()
        {
            try
            {
                return long.Parse(FirstNumTextBox.Text);
            }
            catch (FormatException)
            {
                ResultsListBox.Items.Add("Ошибка ввода начального значения");
                return -1;
            }
        }
        private int getCount()
        {
            try
            {
                return int.Parse(CountTextBox.Text);
            }
            catch (FormatException)
            {
                ResultsListBox.Items.Add("Ошибка ввода количества");
                return -1;
            }
        }
        //без потоков
        private void StartWithoutThreadButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();
            long num = getNum();
            int count = getCount();
            if (num < 0 || count < 0)
                return;
            SetEnabled(false);
            DateTime now = DateTime.Now;
            while (count > 0)
            {
                if (isNumPrime(num))
                {
                    ResultsListBox.Items.Add(num.ToString());
                    count--;
                }
                num++;
                System.Windows.Forms.Application.DoEvents();
            }
            TimeOperation.Text = DateTime.Now.Subtract(now).ToString();
            SetEnabled(true);
        }

        //с потоками
        private void StartWithThreadButton_Click(object sender, RoutedEventArgs e)
        {

            ResultsListBox.Items.Clear();
            long num = getNum();
            int count = getCount();
            if (num < 0 || count < 0)
                return;
            SetEnabled(false);
            DateTime now = DateTime.Now;
            ThreadCount = 0;
            while (ResultsListBox.Items.Count < count)
            {
                if (ThreadCount < 100 && ThreadCount < count - ResultsListBox.Items.Count)
                {
                    Thread thread = new Thread(ExecuteThread);
                    ThreadParams threadParams = new ThreadParams();
                    threadParams.num = num;
                    threadParams.count = count;
                    ThreadCount++;
                    num++;
                    thread.Start(threadParams);
                }
                System.Windows.Forms.Application.DoEvents();
            }
            TimeOperation.Text = DateTime.Now.Subtract(now).ToString();
            SetEnabled(true);

        }


        private void ExecuteThread(object threadParams)
        {
            if (isNumPrime((threadParams as ThreadParams).num))
            {
                Dispatcher.BeginInvoke(new Action(
                    () =>
                    {

                        if (ResultsListBox.Items.Count < (threadParams as ThreadParams).count)
                        {
                            ResultsListBox.Items.Add((threadParams as ThreadParams).num.ToString());
                        }

                    }));
            }
            Dispatcher.BeginInvoke(new Action(
                    () =>
                    {
                        ThreadCount--;
                    }));

        }

        private void StartWithThreadLimitButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();
            long num = getNum();
            int count = getCount();
            if (num < 0 || count < 0)
            {
                return;
            }
            SetEnabled(false);
            DateTime now = DateTime.Now;
            ThreadCount = 0;
            while (ResultsListBox.Items.Count < count)
            {
                if (ThreadCount < 15)
                {
                    Thread thread = new Thread(ExecuteThread);
                    ThreadParams threadParams = new ThreadParams();
                    threadParams.num = num;
                    threadParams.count = count;
                    ThreadCount++;
                    num++;
                    thread.Start(threadParams);
                }
                System.Windows.Forms.Application.DoEvents();
            }
            TimeOperation.Text = DateTime.Now.Subtract(now).ToString();
            while (ThreadCount > 0)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            SetEnabled(true);
        }

        private bool isNumPrime(long num)
        {
            num = Math.Abs(num);
            if (num > 1)
            {
                long num2;
                for (num2 = 2; num % num2 != 0; num2++)
                {

                }
                return num == num2;
            }
            return true;
        }

        private void ClearResult_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();
            TimeOperation.Clear();
        }
    }
}
