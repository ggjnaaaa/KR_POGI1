using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        int numberOfOpenCells;
        int numberOfMines;
        Button[,] buttons;
        object originalContentValue; // Нужно для возвращения исходного значения контента при снятии флажка с кнопки
        object markContentValue; // Нужно для сравнения значения контента при проверке наличия флажка

        int mineValue = -9; // Как обозначаются мины в массиве
        int edgesValue = -3; // Как обозначаются края массива

        int status;
        //статус игры:
        //0-начало
        //1-активная игра
        //2-проигрыш или выигрыш

        // Выбрана лёгкая сложность
        public void eazy_click(object sender, RoutedEventArgs e)
        {
            NewGame();

            numberOfMines = 10;
            int[,] nums = new int[12, 12];
            MineGeneration(nums);
        }

        // Выбрана средняя сложность
        public void medium_click(object sender, RoutedEventArgs e)
        {
            NewGame();

            numberOfMines = 40;
            int[,] nums = new int[18, 18];
            MineGeneration(nums);
        }

        // Выбрана сложная сложность
        public void hard_click(object sender, RoutedEventArgs e)
        {
            NewGame();

            numberOfMines = 99;
            int[,] nums = new int[18, 32];
            MineGeneration(nums);
        }

        // Очищает поле и таймер
        void NewGame()
        {
            if (sw.IsRunning)
            {
                sw.Stop(); // Остановка секундомера
            }
            ugr.Children.Clear();
            timer.Content = "00:00:00";
            status = 0;
        }

        // Создание поля
        void MineGeneration(int[,] nums) 
        {
            // Создание поля чистого поля
            CreatingABlankField(nums);
            // Добавление мин
            AddingMines(nums);
            // Заполнение оставшихся ячеек количеством мин вокруг
            AddingMinesCount(nums);

            // Изменение размеров окна
            Window window = Application.Current.MainWindow;
            window.Width = 40 * nums.GetLength(1) + nums.GetLength(1) * 2 + 200;
            window.Height = 40 * nums.GetLength(0) + nums.GetLength(0) * 2 + 200;

            // Изменение размеров поля
            ugr.Width = 40 * nums.GetLength(1) + nums.GetLength(1) * 2;
            ugr.Height = 40 * nums.GetLength(0) + nums.GetLength(0) * 2;
            ugr.Rows = nums.GetLength(0) - 2;
            ugr.Columns = nums.GetLength(1) - 2;
            ugr.Margin = new Thickness(5);
            diff.Text = " ";

            // Добавление кнопок
            NewButtons(nums);

            numberOfOpenCells = 0; // Обнуление количества открытых ячеек
        }

        void CreatingABlankField(int[,] nums)
        {
            // Заполнение краёв массива
            for (int i = 0; i < nums.GetLength(1); i++) // Верх и низ
            {
                nums[0, i] = edgesValue;
                nums[nums.GetLength(0) - 1, i] = edgesValue;
            }

            for (int i = 0; i < nums.GetLength(0); i++) // Право и лево
            {
                nums[i, 0] = edgesValue;
                nums[i, nums.GetLength(1) - 1] = edgesValue;
            }

            for (int i = 1; i < nums.GetLength(0) - 1; i++)
                for (int j = 1; j < nums.GetLength(1) - 1; j++)
                    nums[i, j] = 0;
        }

        // Добавляет мины
        void AddingMines(int[,] nums)
        {
            Random rnd = new Random();

            int n = 0;

            // Продолжается пока не поставится нужное количество мин
            while (n != numberOfMines)
            {
                int randomColumn = rnd.Next(1, nums.GetLength(1) - 1);
                int randomRow = rnd.Next(1, nums.GetLength(0) - 1);

                // Если в ячейке нет мины, то добавляется новая
                if (nums[randomRow, randomColumn] != mineValue)
                {
                    nums[randomRow, randomColumn] = mineValue;
                    n++;
                }
            }
        }

        // Заполняет оставшиеся 
        void AddingMinesCount(int[,] nums)
        {
            for (int i = 1; i < nums.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < nums.GetLength(1) - 1; j++)
                {
                    if (nums[i, j] != mineValue && nums[i, j] != edgesValue)
                    {
                        nums[i, j] = SearchValueAround(nums, i, j, mineValue);
                    }
                }
            }
        }

        // Ищет количество мин вокруг заданной ячейки
        int SearchValueAround(int[,] nums, int row, int column, int value)
        {
            int k = 0;

            if (nums[row - 1, column - 1] == value) k++;
            if (nums[row - 1, column] == value) k++;
            if (nums[row - 1, column + 1] == value) k++; 
            if (nums[row, column - 1] == value) k++;
            if (nums[row, column + 1] == value) k++;
            if (nums[row + 1, column - 1] == value) k++;
            if (nums[row + 1, column] == value) k++;
            if (nums[row + 1, column + 1] == value) k++;

            return k;
        }

        // Добавляет новые кнопки
        void NewButtons(int[,] nums)
        {
            buttons = new Button[nums.GetLength(0) - 2, nums.GetLength(1) - 2];

            for (int i = 1; i < nums.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < nums.GetLength(1) - 1; j++)
                {
                    Button btn = new Button();
                    btn.Tag = nums[i, j];
                    btn.Width = 40;
                    btn.Height = 40;
                    btn.Content = " ";
                    btn.Margin = new Thickness(1);
                    ugr.Children.Add(btn);
                    btn.PreviewMouseDown += Btn_MouseDown;

                    buttons[i - 1, j - 1] = btn;
                }
            }

            originalContentValue = buttons[0, 0].Content;
        }

        // Обработка нажатия кнопки мышки
        private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Местоположение кнопки
            int row = 0;
            int column = 0;

            // Поиск нажатой кнопки в массиве кнопок
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                bool buttonFound = false;

                for (int j = 0; j < buttons.GetLength(1); j++)
                    if (buttons[i, j] == sender)
                    {
                        row = i;
                        column = j;
                        buttonFound = true;
                        break;
                    }

                if (buttonFound) break;
            }

            // Если нажата ЛКМ
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Если на кнопке стоит флажок, то кнопка не нажимается
                if (((Button)sender).Content != originalContentValue)
                    return;

                int n = (int)((Button)sender).Tag; // Значение кнопки

                if (n == mineValue) // Если мина
                    IsMine(row, column);
                else if (n == 0) // Если пусто
                    AutoOpen(row, column);
                else // Если число
                    IsNumber(row, column);

                if (numberOfOpenCells == ugr.Columns * ugr.Rows - numberOfMines) // Если все кнопки открыты
                {
                    status = 2;
                    sw.Stop();
                    MessageBox.Show("Поздравляем! вы выиграли! Выберите сложность, чтобы начать новую игру\n" + "Время: " + currentTime);

                    // Выключение всех кнопок
                    foreach (Button button in ugr.Children)
                    {
                        button.IsEnabled = false;
                    }
                }
            }

            // Если нажата ПКМ
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (((Button)sender).Content != originalContentValue) // Если флажок стоит
                    ((Button)sender).Content = originalContentValue;
                else // Если кнопка пустая
                    AddImage(((Button)sender), @"pack://application:,,,/img/Mark.png");

                markContentValue = ((Button)sender).Content;
            }
        }

        // Ставит картинку мины на кнопку и завершает игру
        void IsMine(int row, int column)
        {
            if (sw.IsRunning)
            {
                sw.Stop();
            }

            MessageBox.Show("Вы проиграли. Выберите сложность, чтобы начать новую игру");
            numberOfOpenCells = 0;
            status = 2;
            // Выключение всех кнопок
            foreach (Button button in ugr.Children)
            {
                button.IsEnabled = false;

                // Вывод мин
                if ((int)button.Tag == mineValue && button.Content == originalContentValue) // Если в кнопке была мина и не было флажка
                {
                    AddImage(button, @"pack://application:,,,/img/Mine.jpg");
                }
            }
        }

        // Устанавливает изображения на кнопку
        void AddImage(Button button, string path)
        {
            // Установка изображения мины на кнопку
            BitmapImage incorrectMark = new BitmapImage(new Uri(path, UriKind.Absolute));
            Image img = new Image();
            img.Source = incorrectMark;
            StackPanel stackPnl = new StackPanel();
            stackPnl.Margin = new Thickness(1);
            stackPnl.Children.Add(img);
            button.Content = stackPnl;

            button.Content = stackPnl;
        }

        // Открывает соседние ячейки если текущая оказалась 0
        void AutoOpen(int row, int column)
        {
            int[,] indexes = SearchValueButtonAround(row, column, 0);

            // Открытие нынешней кнопки
            IsNumber(row, column);

            // Открытие соседних кнопок, если они не нули
            if (row != 0 && column != 0 && indexes[0,0] == -1) IsNumber(row - 1, column - 1);
            if (row != 0 && indexes[1,0] == -1) IsNumber(row - 1, column);
            if (row != 0 && column != buttons.GetLength(1) - 1 && indexes[2,0] == -1) IsNumber(row - 1, column + 1);
            if (column != 0 && indexes[3,0] == -1) IsNumber(row, column - 1);
            if (column != buttons.GetLength(1) - 1 && indexes[4,0] == -1) IsNumber(row, column + 1);
            if (row != buttons.GetLength(0) - 1 && column != 0 && indexes[5,0] == -1) IsNumber(row + 1, column - 1);
            if (row != buttons.GetLength(0) - 1 && indexes[6,0] == -1) IsNumber(row + 1, column);
            if (row != buttons.GetLength(0) - 1 && column != buttons.GetLength(1) - 1 && indexes[7,0] == -1) IsNumber(row + 1, column + 1);

            // Вызов AutoOpen для соседних кнопок с нулевым значением
            for (int i = 0; i < indexes.GetLength(0); i++)
                if (indexes[i, 0] != -1)
                {
                    AutoOpen(indexes[i, 0], indexes[i, 1]);
                }
        }

        // Ищет индексы элементов с искомым значением вокруг заданной ячейки
        int[,] SearchValueButtonAround(int row, int column, int value)
        {
            int[,] indexes = new int[8, 2];

            // Заполнение массива, чтобы отследить изменение значений
            for (int i = 0; i < indexes.GetLength(0); i++)
                for (int j = 0; j < indexes.GetLength(1); j++)
                    indexes[i, j] = -1;

            if (row != 0 && column != 0) // Если кнопка не на границе массива
                if ((int)buttons[row - 1, column - 1].Tag == value && buttons[row - 1, column - 1].IsEnabled) // Если значение кнопки равно искомому и кнопка не нажата
                {
                    indexes[0, 0] = row - 1;
                    indexes[0, 1] = column - 1;
                }
            if (row != 0)
                if ((int)buttons[row - 1, column].Tag == value && buttons[row - 1, column].IsEnabled)
                {
                    indexes[1, 0] = row - 1;
                    indexes[1, 1] = column;
                }
            if (row != 0 && column != buttons.GetLength(1) - 1)
                if ((int)buttons[row - 1, column + 1].Tag == value && buttons[row - 1, column + 1].IsEnabled)
                {
                    indexes[2, 0] = row - 1;
                    indexes[2, 1] = column + 1;
                }
            if (column != 0)
                if ((int)buttons[row, column - 1].Tag == value && buttons[row, column - 1].IsEnabled)
                {
                    indexes[3, 0] = row;
                    indexes[3, 1] = column - 1;
                }
            if (column != buttons.GetLength(1) - 1)
                if ((int)buttons[row, column + 1].Tag == value && buttons[row, column + 1].IsEnabled)
                {
                    indexes[4, 0] = row;
                    indexes[4, 1] = column + 1;
                }
            if (row != buttons.GetLength(0) - 1 && column != 0)
                if ((int)buttons[row + 1, column - 1].Tag == value && buttons[row + 1, column - 1].IsEnabled)
                {
                    indexes[5, 0] = row + 1;
                    indexes[5, 1] = column - 1;
                }
            if (row != buttons.GetLength(0) - 1)
                if ((int)buttons[row + 1, column].Tag == value && buttons[row + 1, column].IsEnabled)
                {
                    indexes[6, 0] = row + 1;
                    indexes[6, 1] = column;
                }
            if (row != buttons.GetLength(0) - 1 && column != buttons.GetLength(1) - 1)
                if ((int)buttons[row + 1, column + 1].Tag == value && buttons[row + 1, column + 1].IsEnabled)
                {
                    indexes[7, 0] = row + 1;
                    indexes[7, 1] = column + 1;
                }

            return indexes;
        }

        // Открывает ячейку если в ней число и она не открыта
        void IsNumber(int row, int column)
        {
            // Если кнопка уже была открыта (чтобы не писать кучу условий в AutoOpen)
            if (buttons[row, column].IsEnabled == false)
                return;

            status = status == 0 ? 1 : status;
            int n = (int)buttons[row, column].Tag;

            // Добавление числа на кнопку
            buttons[row, column].Background = Brushes.White;
            buttons[row, column].Foreground = Brushes.Red;
            buttons[row, column].FontSize = 20;
            buttons[row, column].Content = n == 0 ? "" : n.ToString();
            buttons[row, column].IsEnabled = false;

            numberOfOpenCells++;

            // Запуск секундомера
            if (!sw.IsRunning)
            {
                sw.Start();
                dt.Start();
            }
        }
    }
}