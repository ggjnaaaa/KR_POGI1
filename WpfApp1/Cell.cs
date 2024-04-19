namespace WpfApp1
{
    /// <summary>
    /// Класс ячейки. Хранит информацию о значении и статусе кнопки.
    /// </summary>
    public class Cell : ViewModelBase
    {
        public int BtnHeight { get; } = 35;
        public int BtnWidth { get; } = 35;

        public bool HasMine;

        private CellValues _value;
        public CellValues Value
        {
            get => _value;
            set
            {
                _value = value;

                if (value == CellValues.Mine)
                    HasMine = true;

                OnPropertyChanged();
            }
        }

        private EnabledButtonStatus _buttonStatus;
        public EnabledButtonStatus ButtonStatus
        {
            get => _buttonStatus;
            set
            {
                _buttonStatus = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public int Tag { get; }

        public Cell(int tag)
        {
            IsEnabled = true;
            Tag = tag;
        }
    }

    /// <summary>
    /// Значения кнопок (0-9, мина и взорванная мина)
    /// </summary>
    public enum CellValues
    {
        Default,
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Mine,
        ExplodedMine
    }

    /// <summary>
    /// Статусы включенной кнопки (обычная и отмеченная)
    /// </summary>
    public enum EnabledButtonStatus
    {
        Default,
        Flagged
    }
}
