using System;
using System.Collections.Generic;
using System.IO;
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

namespace HraZivotaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd;
        int[,] pole, poleFuture;
        DispatcherTimer timer;

        public MainWindow()
        {
            rnd = new Random();
            pole = new int[20, 20];
            poleFuture = new int[20, 20];
            InitializeComponent();

            pole = NactiPole();
            if (pole == null)
            {
                pole = new int[20, 20];
                for (int i = 0; i < 100; i++)
                {
                    int x = rnd.Next(pole.GetLength(0)), y = rnd.Next(pole.GetLength(1));
                    if (pole[x, y] == 0)
                    {
                        pole[x, y] = 1;
                    }
                    else
                    {
                        i--;
                    }
                }
            }

            for (int x = 0; x < pole.GetLength(1); x++)
            {
                mrizka.ColumnDefinitions.Add(new ColumnDefinition());
            }


            for (int y = 0; y < pole.GetLength(0); y++)
            {
                mrizka.RowDefinitions.Add(new RowDefinition());
            }

            for (int x = 0; x < pole.GetLength(0); x++)
            {
                for (int y = 0; y < pole.GetLength(1); y++)
                {
                    Rectangle rec = new Rectangle();
                    rec.Fill = Brushes.Red;
                    rec.SetValue(Grid.RowProperty, x);
                    rec.SetValue(Grid.ColumnProperty, y);
                    rec.Tag = new int[] { x, y };
                    rec.MouseDown += Rec_MouseDown;
                    mrizka.Children.Add(rec);
                }
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick;
            Vypis();
        }

        private void Rec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rec)
            {
                if (rec.Fill == Brushes.Green)
                {
                    int[] arr = (int[])rec.Tag;
                    pole[arr[0], arr[1]] = 1;

                    Vypis();
                }
            }
        }

        private void startBTN_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
        }

        private void stopBTN_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int mezi = 0;
            for (int x = 0; x < pole.GetLength(0); x++)
            {
                for (int y = 0; y < pole.GetLength(1); y++)
                {
                    if (pole[x, y] == 1 && Okoli(x, y) < 2)
                    {
                        poleFuture[x, y] = 0;
                    }
                    if (pole[x, y] == 1 && Okoli(x, y) == 2 || pole[x, y] == 1 && Okoli(x, y) == 3)
                    {
                        poleFuture[x, y] = 1;
                    }
                    if (pole[x, y] == 1 && Okoli(x, y) > 3)
                    {
                        poleFuture[x, y] = 0;
                    }
                    if (pole[x, y] == 0 && Okoli(x, y) == 3)
                    {
                        poleFuture[x, y] = 1;
                    }
                    if (pole[x, y] == 1)
                    {
                        mezi++;
                    }
                }
            }

            pole = poleFuture;
            Vypis();

            poleFuture = new int[20, 20];
        }

        void Vypis()
        {
            foreach (var item in mrizka.Children)
            {
                if (item is Rectangle rec)
                {
                    int[] arr = (int[])rec.Tag;
                    if (pole[arr[0], arr[1]] == 1)
                    {
                        rec.Fill = Brushes.Blue;
                    }
                    else
                    {
                        rec.Fill = Brushes.Green;
                    }
                }
            }
        }

        int Okoli(int x, int y)
        {
            int soucet = 0;

            for (int x1 = -1; x1 <= 1; x1++)
            {
                for (int y1 = -1; y1 <= 1; y1++)
                {
                    if (x1 == 0 && y1 == 0)
                    {
                        continue;
                    }

                    if (x + x1 != -1 && x + x1 != pole.GetLength(0) && y + y1 != -1 && y + y1 != pole.GetLength(1))
                    {
                        if (pole[x + x1, y + y1] == 1)
                        {
                            soucet++;
                        }
                    }
                }
            }
            return soucet;
        }

        int[,] NactiPole()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "data files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (Stream soub = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sw = new StreamReader(soub, Encoding.UTF8))
                    {
                        string[] a = sw.ReadLine().Split(';');
                        int[,] arr = new int[20, 20];
                        for (int x = 0; x < 20; x++)
                        {
                            for (int y = 0; y < 20; y++)
                            {
                                arr[x, y] = Convert.ToInt32(a[20 * x + y]);
                            }
                        }
                        return arr;
                    }
                }
            }
            return null;
        }

        private void saveBTN_Click(object sender, RoutedEventArgs e)
        {
            ZapisPole();
        }

        void ZapisPole()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Filter = "data files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (Stream soub = new FileStream(saveFileDialog.FileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(soub, Encoding.UTF8))
                    {
                        string a = "";
                        for (int x = 0; x < 20; x++)
                        {
                            for (int y = 0; y < 20; y++)
                            {
                                a += $"{pole[x, y]};";
                            }
                        }
                        sw.Write(a);
                    }
                }
            }
        }
    }
}
