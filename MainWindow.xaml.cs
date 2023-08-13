using System;
using System.Data;
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
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Net.Mime.MediaTypeNames.Application;
using System.Security.Policy;

namespace WPF
{
    public partial class MainWindow : Window
    {
        int height;
        protected Grid gridWithButtons;
        StackPanel stackPanelForInfo;
        protected static bool[,] isClosedCeilsArray;
        protected static int quantityOfFlags = 0;
        protected static TextBlock bombsCountTextBlock;
        TextBlock timerTextBlock;
        protected static bool isGameOver = false;

        protected static DispatcherTimer timer;
        int seconds;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            timerTextBlock.Text = (seconds / 3600) + ":" + (seconds / 60) + ":" + (seconds % 60) + " ";
        }
        public void escButton_Click(object sender, RoutedEventArgs c) => this.Close();
        private void CreateFrame(int height)
        {
            isClosedCeilsArray = new bool[height, height];

            DockPanel dockPanel = new DockPanel();
            dockPanel.LastChildFill = true;

            stackPanelForInfo = new StackPanel();
            stackPanelForInfo.Orientation = Orientation.Horizontal;
            stackPanelForInfo.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanelForInfo.VerticalAlignment = VerticalAlignment.Center;
            stackPanelForInfo.MinHeight = 30;
            stackPanelForInfo.MaxHeight = 30;

            gridWithButtons = new Grid();

            for (int i = 0; i < height; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                gridWithButtons.RowDefinitions.Add(rowDef);
            }

            for (int j = 0; j < height; j++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                gridWithButtons.ColumnDefinitions.Add(colDef);
            }

            DockPanel.SetDock(stackPanelForInfo, Dock.Top);
            dockPanel.Children.Add(stackPanelForInfo);

            dockPanel.Children.Add(gridWithButtons);

            Content = dockPanel;
        }
        private void CreateField()
        {
            CreateFrame(height);

            timerTextBlock = new TextBlock();
            timerTextBlock.Text = "0:0:0 ";
            timerTextBlock.FontSize = 27;
            timerTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            timerTextBlock.VerticalAlignment = VerticalAlignment.Center;
            timerTextBlock.MinWidth = 15;

            bombsCountTextBlock = new TextBlock();
            bombsCountTextBlock.Text = "0";
            bombsCountTextBlock.FontSize = 27;
            bombsCountTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            bombsCountTextBlock.VerticalAlignment = VerticalAlignment.Center;
            bombsCountTextBlock.MinWidth = 15;

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri("C:\\Users\\alex5\\source\\repos\\WPF\\WPF\\images\\ukrainian_flag_white_background.png"));
            image.MinHeight = 30;

            stackPanelForInfo.Children.Add(timerTextBlock);
            stackPanelForInfo.Children.Add(bombsCountTextBlock);
            stackPanelForInfo.Children.Add(image);

            Button buttonEsc = new Button();
            buttonEsc.Click += (sender, e) => escButton_Click(sender, e);
            buttonEsc.IsCancel = true;
            buttonEsc.Width = 0;
            buttonEsc.Height = 0;

            for (int i = 0; i < gridWithButtons.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < gridWithButtons.ColumnDefinitions.Count; j++)
                {
                    int iCopy = i;
                    int jCopy = j;

                    Button button = new Button();
                    button.Background = new SolidColorBrush(Color.FromRgb(112, 225, 255));

                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    gridWithButtons.Children.Add(button);
                }
            }
            for (int i = 0; i < gridWithButtons.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < gridWithButtons.ColumnDefinitions.Count; j++)
                {
                    int iCopy = i;
                    int jCopy = j;

                    ((Button)gridWithButtons.Children[i * height + j]).Click += (sender, e) => MineSweeper.TransmitDataAboutFirstClick(iCopy, jCopy);
                    ((Button)gridWithButtons.Children[i * height + j]).Click += (sender, e) => MineSweeper.GenerateField(height);
                    ((Button)gridWithButtons.Children[i * height + j]).Click += (sender, e) => MineSweeper.OpenCeil(iCopy, jCopy, gridWithButtons.Children, true, false);
                    ((Button)gridWithButtons.Children[i * height + j]).Click += (sender, e) => UpdateDataAboutQuantityOfFlags();

                    ((Button)gridWithButtons.Children[i * height + j]).MouseRightButtonDown += (sender, e) => MakeFlag(iCopy, jCopy, gridWithButtons.Children);
                    ((Button)gridWithButtons.Children[i * height + j]).MouseRightButtonDown += (sender, e) => UpdateDataAboutQuantityOfFlags();
                }
            }

            gridWithButtons.Children.Add(buttonEsc);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            seconds = 0;

            timer.Start();
        }
        private void UpdateDataAboutQuantityOfFlags()
        {
            if (!isGameOver)
                bombsCountTextBlock.Text = (MineSweeper.QuantityOfBombs() - quantityOfFlags).ToString();
        }
        private void DataApprovalButtonClick(object sender, RoutedEventArgs e)
        {
            string inputDataStr = textBoxHeight.Text;
            int givenData;

            if (!int.TryParse(inputDataStr, out givenData) || Convert.ToInt32(inputDataStr) < 5 || Convert.ToInt32(inputDataStr) > 50)
                MessageBox.Show("Будь ласка, введіть ціле число від 5 до 50 включно");
            else
            {
                height = givenData;

                CreateField();
            }
        }
        private void MakeFlag(int i, int j, UIElementCollection children)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage(new Uri("C:\\Users\\alex5\\source\\repos\\WPF\\WPF\\images\\ukrainian_flag.png"));

            if (((SolidColorBrush)((Button)children[i * height + j]).Background).Color != Color.FromRgb(112, 225, 255) || isGameOver)
                return;

            if (isClosedCeilsArray[i, j] == false)
            {
                ((Button)children[i * height + j]).Content = image;
                isClosedCeilsArray[i, j] = true;
                quantityOfFlags++;
            }
            else
            {
                ((Button)children[i * height + j]).Content = null;
                isClosedCeilsArray[i, j] = false;
                quantityOfFlags--;
            }
        }
    }
}