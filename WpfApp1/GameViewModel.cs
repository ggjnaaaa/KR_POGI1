using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Вьюмодель для главного окна.
    /// </summary>
    public class GameViewModel : ViewModelBase
    {
        #region Свойства для связи с представлением

        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged();
            }
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                OnPropertyChanged();
            }
        }

        private bool _isWin;
        public bool IsWin
        {
            get => _isWin;
            set
            {
                _isWin = value;
                OnPropertyChanged();
            }
        }

        private bool _isLose;
        public bool IsLose
        {
            get => _isLose;
            set
            {
                _isLose = value;
                OnPropertyChanged();
            }
        }

        private bool _isWow;
        public bool IsWow
        {
            get => _isWow;
            set
            {
                _isWow = value;
                OnPropertyChanged();
            }
        }

        private int _countFlags;
        public int CountFlags
        {
            get => _countFlags;
            set
            {
                _countFlags = value;
                OnPropertyChanged();
            }
        }

        private int _timer;
        public int Timer
        {
            get => _timer;
            set
            {
                if (value > 999) _timer = value % 1000;
                else _timer = value;
                OnPropertyChanged();
            }
        }

        private Field _gameField;
        public Field GameField
        {
            get => _gameField;
            set
            {
                _gameField = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Команды для кнопок в представлении

        public ICommand SmileCommand { get; private set; }
        public ICommand RightMouseCellUpCommand { get; private set; }
        public ICommand LeftMouseCellDownCommand { get; private set; }
        public ICommand LeftMouseCellUpCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }

        #endregion

        #region Местные переменные

        private TimerViewModel GameTimer;
        private bool IsGameStarted;

        private DifficultyLevel GameDifficultyLevel;
        private int LastCustomizableHeight;
        private int LastCustomizableWidth;
        private int LastCustomizableMines;

        #endregion

        public GameViewModel()
        {
            GameDifficultyLevel = DifficultyLevel.Easy;

            GameTimer = new TimerViewModel();
            GameTimer.TimeChanged += Timer_TimeChanged;

            SmileCommand = new RelayCommand(SmileClick);
            RightMouseCellUpCommand = new RelayCommand(TileRightClick);
            LeftMouseCellDownCommand = new RelayCommand(TileLeftDown);
            LeftMouseCellUpCommand = new RelayCommand(TileLeftUp);
            SettingsCommand = new RelayCommand(SettingClick);

            NewGame();
        }

        #region Методы для обработки нажатий на кнопки

        /// <summary>
        /// Нажатие на главную кнопку или кнопку "Новая игра".
        /// </summary>
        /// <param name="parameter"></param>
        private void SmileClick(object parameter) => NewGame();

        /// <summary>
        /// Нажатие ЛКМ на кнопку на поле.
        /// </summary>
        /// <param name="parameter"></param>
        private void TileLeftDown(object parameter)
        {
            if (!IsWin && !IsLose)
            {
                IsWow = true;
                if (parameter is int index)
                    if (!IsGameStarted)
                    {
                        GameTimer.Start();
                        GameField.StartGame(index);
                        IsGameStarted = true;
                    }
            }
        }

        /// <summary>
        /// Отжатие ЛКМ на кнопке на поле.
        /// </summary>
        /// <param name="parameter"></param>
        private void TileLeftUp(object parameter)
        {
            if (!IsWin && !IsLose)
            {
                IsWow = false;

                if (parameter is int index)
                {
                    if (!IsGameStarted)
                    {
                        GameTimer.Start();
                        GameField.StartGame(index);
                        IsGameStarted = true;
                    }
                    else
                        GameField.CellLeftUp(index);
                }
            }
        }

        /// <summary>
        /// Нажатие ПКМ на кнопку на поле.
        /// </summary>
        /// <param name="parameter"></param>
        private void TileRightClick(object parameter)
        {
            if (!IsWin && !IsLose)
                if (parameter is int index)
                {
                    if (GameField.CellRightClick(index))
                        CountFlags--;
                    else
                        CountFlags++;
                }
        }

        /// <summary>
        /// Нажатие на кнопку "Параметры".
        /// </summary>
        /// <param name="parameter"></param>
        private void SettingClick(object parameter)
        {
            var wind = new MainMenu();
            var menuVM = new MenuViewModel();
            menuVM.DataEvent += OnDataReceived;
            wind.DataContext = menuVM;
            wind.Show();
        }

        #endregion

        /// <summary>
        /// Метод подписанный на закрытие окна параметров через кнопки "Ок" и "Отмена".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceived(object sender, DataEventArgs e)
        {
            if (sender is DifficultyLevel diff)
            {
                if (diff == DifficultyLevel.Customizable)
                {
                    GameDifficultyLevel = DifficultyLevel.Customizable;
                    LastCustomizableHeight = e.HeightInput;
                    LastCustomizableWidth = e.WidthInput;
                    LastCustomizableMines = e.MinesInput;
                }
                else GameDifficultyLevel = diff;

                NewGame();
            }
        }

        /// <summary>
        /// Создаёт новую игру в зависимости от GameDifficultyLevel.
        /// </summary>
        private void NewGame()
        {
            IsGameStarted = false;
            IsWin = false;
            IsLose = false;
            GameTimer.Stop();
            Timer = 0;

            if (GameDifficultyLevel == DifficultyLevel.Customizable)
                GameField = new Field(LastCustomizableHeight, LastCustomizableWidth, LastCustomizableMines);
            else
                GameField = new Field(GameDifficultyLevel);

            CountFlags = GameField.Mines;
            WindowHeight = GameField.Height * GameField.BtnHeight + 215;
            WindowWidth = GameField.Width * GameField.BtnWidth + 88;
            GameField.GameOver += LoseGame;
            GameField.Win += WinGame;
        }

        /// <summary>
        /// Метод подписанный на проигрыш в GameField.
        /// </summary>
        private void LoseGame()
        {
            IsGameStarted = false;
            GameTimer.Stop();
            IsLose = true;
            MessageBox.Show("Вы проиграли :( ");
        }

        /// <summary>
        /// Метод подписанный на выигрыш в GameField.
        /// </summary>
        private void WinGame()
        {
            IsGameStarted = false;
            GameTimer.Stop();
            IsWin = true;
            MessageBox.Show("Поздравляем! вы выиграли! Выберите сложность, чтобы начать новую игру\n" + "Время: " + Timer + " сек.");
        }

        /// <summary>
        /// Метод подписанный на изменение таймера GameTimer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_TimeChanged(object sender, EventArgs e) => Timer++;
    }

    public enum DifficultyLevel
    {
        [Description("Лёгкий (10 х 10, 10 мин)")]
        Easy,
        [Description("Средний (16 х 16, 40 мин)")]
        Medium,
        [Description("Сложный (30 х 16, 99 мин)")]
        Hard,
        [Description("Пользователььский")]
        Customizable
    }
}
