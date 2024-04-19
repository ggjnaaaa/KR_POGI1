using System;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    /// <summary>
    /// Вьюмодель для меню.
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        public event DataEventHandler DataEvent;

        #region Свойства для свясзи с представлением

        private string _heightInput;
        public string HeightInput
        {
            get => _heightInput;
            set
            {
                if (_heightInput != value)
                {
                    var intValue = int.Parse(value);
                    if (intValue >= 10 && intValue <= 50)
                    {
                        _heightInput = value;
                        OnPropertyChanged();
                        ErrorInputData = string.Empty;
                    }
                    else
                        ErrorInputData = "Значение высоты должно быть от 10 до 50";
                }
            }
        }

        private string _widthInput;
        public string WidthInput
        {
            get => _widthInput;
            set
            {
                if (_widthInput != value)
                {
                    var intValue = int.Parse(value);
                    if (intValue >= 10 && intValue <= 50)
                    {
                        _widthInput = value;
                        OnPropertyChanged();
                        ErrorInputData = string.Empty;
                    }
                    else
                        ErrorInputData = "Значение ширины должно быть от 10 до 50";
                }
            }
        }

        private string _minesInput;
        public string MinesInput
        {
            get => _minesInput;
            set
            {
                if (_minesInput != value)
                {
                    if (IsNumber(value))
                    {
                        var intValue = int.Parse(value);
                        if (intValue >= 5 && intValue <= Math.Ceiling(int.Parse(WidthInput) * int.Parse(HeightInput) * 0.5))
                        {
                            _minesInput = value;
                            OnPropertyChanged();
                            ErrorInputData = string.Empty;
                        }
                        else
                            ErrorInputData = "Количество мин должно быть от 5 до " + Math.Ceiling(int.Parse(WidthInput) * int.Parse(HeightInput) * 0.5);
                    }
                    
                }
            }
        }

        private bool _isCustomizable;
        public bool IsCustomizable
        {
            get => _isCustomizable;
            set
            {
                _isCustomizable = value;
                OnPropertyChanged();
            }
        }

        private string _errorInputData;
        public string ErrorInputData
        {
            get => _errorInputData;
            set
            {
                _errorInputData = value;
                OnPropertyChanged();
                if (_errorInputData == string.Empty)
                    ErrorBlockHeight = 0;
                else
                    ErrorBlockHeight = 20;
            }
        }

        private int _errorBlockHeight;
        public int ErrorBlockHeight
        {
            get => _errorBlockHeight;
            set
            {
                _errorBlockHeight = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Комманды для кнопок представления

        public ICommand SelectDifficultyCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        private DifficultyLevel SelectedDifficulty;

        public MenuViewModel()
        {
            SelectDifficultyCommand = new RelayCommand(SelectDifficultyClick);
            OkCommand = new RelayCommand(OkClick);
            CancelCommand = new RelayCommand(CancelClick);

            ErrorInputData = string.Empty;
        }

        private bool IsNumber(string value) => int.TryParse(value, out var result);

        private void SelectDifficultyClick(object parameter)
        {
            if (parameter is DifficultyLevel difficultyLevel)
            {
                SelectedDifficulty = difficultyLevel;
                if (SelectedDifficulty == DifficultyLevel.Customizable)
                    IsCustomizable = true;
                else
                    IsCustomizable = false;
            }
        }

        private void OkClick(object parameter)
        {
            if (parameter is Window window)
            {
                if (ErrorInputData == string.Empty)
                {
                    DataEventArgs e;
                    if (IsCustomizable)
                        e = new DataEventArgs(int.Parse(HeightInput), int.Parse(WidthInput), int.Parse(MinesInput));
                    else
                        e = null;

                    DataEvent?.Invoke(SelectedDifficulty, e);
                    window.Close();
                }
                
            }
        }

        private void CancelClick(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}
