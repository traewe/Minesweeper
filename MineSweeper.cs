using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace WPF
{
    public class MineSweeper : MainWindow
    {
        static int height;
        static int[,] field;
        static bool[,] visitedCeils;
        static int firstClickHeight = -1;
        static int firstClickWidth = -1;
        static int quantityOfBombs = 0;
        static int quantityOfOpenedCeils = 0;
        
        public static void GenerateField(int givenHeight)
        {
            if (field != null)
                return;

            height = givenHeight;
            field = new int[height, height];
            visitedCeils = new bool[height, height];

            Random rnd = new Random();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!(i == firstClickHeight && j == firstClickWidth) && rnd.Next(1, 8) == 3)
                    {
                        field[i, j] = 10;
                        quantityOfBombs++;
                    }
                }
            }

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (field[i, j] != 10)
                        field[i, j] = CountMines(i, j);
                }
            }
        }
        public static void TransmitDataAboutFirstClick(int height, int width)
        {
            if (firstClickHeight == -1 && firstClickWidth == -1)
            {
                firstClickHeight = height;
                firstClickWidth = width;
            }
        }
        public static int QuantityOfBombs() => quantityOfBombs;
        static int CountMines(int i, int j)
        {
            int counter = 0;
            int[] firstCoordinates = { i - 1, i, i + 1 };
            int[] secondCoordinates = { j - 1, j, j + 1 };

            foreach (int firstCoordinate in firstCoordinates)
            {
                foreach (int secondCoordinate in secondCoordinates)
                {
                    if (firstCoordinate < 0 || firstCoordinate > height - 1 || secondCoordinate < 0 || secondCoordinate > height - 1)
                        continue;
                    else if (field[firstCoordinate, secondCoordinate] == 10)
                        counter++;
                }
            }
            return counter;
        }
        public static void OpenCeil(int i, int j, UIElementCollection children, bool playerClicked = false, bool reversion = true)
        {
            if (i < 0 || i > height - 1 || j < 0 || j > height - 1 || visitedCeils[i, j] || (isClosedCeilsArray[i, j] && reversion == false) || isGameOver)
                return;

            visitedCeils[i, j] = true;

            if (field[i, j] == 10 && playerClicked)
            {
                for (int n = 0; n < height; n++)
                {
                    for (int m = 0; m < height; m++)
                    {
                        if (field[n, m] == 10)
                        {
                            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                            image.Source = new BitmapImage(new Uri("C:\\Users\\alex5\\source\\repos\\WPF\\WPF\\images\\bomb.png"));
                            ((Button)children[n * height + m]).Content = image;
                        }
                    }
                }
                bombsCountTextBlock.Text = "Поразка :( Спробуй ще раз і у тебе вийде";
                isGameOver = true;
                timer.Stop();
            }

            else if (field[i, j] != 10)
            {
                quantityOfOpenedCeils++;

                if (isClosedCeilsArray[i, j])
                    quantityOfFlags--;

                ((Button)children[i * height + j]).Background = new SolidColorBrush(Colors.White);
                
                if (field[i, j] == 0)
                {
                    ((Button)children[i * height + j]).Content = null;
                    OpenCeil(i - 1, j, children);
                    OpenCeil(i + 1, j, children);
                    OpenCeil(i, j - 1, children);
                    OpenCeil(i, j + 1, children);
                    OpenCeil(i - 1, j - 1, children);
                    OpenCeil(i + 1, j - 1, children);
                    OpenCeil(i - 1, j + 1, children);
                    OpenCeil(i + 1, j + 1, children);
                }
                else
                {
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = new BitmapImage(new Uri("C:\\Users\\alex5\\source\\repos\\WPF\\WPF\\images\\" + Convert.ToString(field[i, j]) + ".png"));
                    ((Button)children[i * height + j]).Content = image;

                    if (i != 0 && field[i - 1, j] == 0)
                        OpenCeil(i - 1, j, children);
                    if (i != height - 1 && field[i + 1, j] == 0)
                        OpenCeil(i + 1, j, children);
                    if (j != 0 && field[i, j - 1] == 0)
                        OpenCeil(i, j - 1, children);
                    if (j != height - 1 && field[i, j + 1] == 0)
                        OpenCeil(i, j + 1, children);
                }
            }
            visitedCeils[i, j] = true;

            if (quantityOfOpenedCeils == height * height - quantityOfBombs)
            {
                bombsCountTextBlock.Text = "Перемога! Пес патрон тобою пишається!";
                isGameOver = true;
                timer.Stop();
                for (int n = 0; n < height; n++)
                {
                    for (int m = 0; m < height; m++)
                    {
                        if (field[n, m] == 10)
                        {
                            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                            image.Source = new BitmapImage(new Uri("C:\\Users\\alex5\\source\\repos\\WPF\\WPF\\images\\bomb.png"));
                            ((Button)children[n * height + m]).Content = image;
                        }
                    }
                }
            }
        }
    }
}
