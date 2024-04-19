using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Конвертирует число в перечисляемый BitmapImage. Используется для таймера и счётчика мин
    /// </summary>
    internal class NumberToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                if (intValue < 100 && intValue >= 0)
                {
                    if (intValue < 10) value = "0" + value;
                    return GetImageForNumber("0" + value);
                }
                if (intValue < 0)
                {
                    if (intValue > -10) value = "-0" + value.ToString()[1];
                    else if (intValue <= -100) value = -99;
                    return GetImageForNumber(value.ToString());
                }

                return GetImageForNumber(value.ToString());
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<BitmapImage> GetImageForNumber(string number)
        {
            foreach (var num in number)
                switch (num)
                {
                    case '-':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/-.png"))); break;
                    case '0':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/0.png"))); break;
                    case '1':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/1.png"))); break;
                    case '2':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/2.png"))); break;
                    case '3':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/3.png"))); break;
                    case '4':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/4.png"))); break;
                    case '5':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/5.png"))); break;
                    case '6':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/6.png"))); break;
                    case '7':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/7.png"))); break;
                    case '8':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/8.png"))); break;
                    case '9':
                        yield return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/timer/9.png"))); break;
                }
        }
    }

    /// <summary>
    /// Конвертирует число BitmapImage. Используется для выключенных кнопок на поле
    /// </summary>
    internal class CellValueToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CellValues cv)
                return GetImageForCellValues(cv);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage GetImageForCellValues(CellValues cellValue)
        {
            switch (cellValue)
            {
                case CellValues.Zero:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/0.png")));
                case CellValues.One:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/1.png")));
                case CellValues.Two:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/2.png")));
                case CellValues.Three:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/3.png")));
                case CellValues.Four:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/4.png")));
                case CellValues.Five:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/5.png")));
                case CellValues.Six:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/6.png")));
                case CellValues.Seven:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/7.png")));
                case CellValues.Eight:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/8.png")));
                case CellValues.Mine:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/Mine.png")));
                case CellValues.ExplodedMine:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/ExplodedMine.png")));
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Конвертирует EnabledButtonStatus в BitmapImage. Используется для включенных кнопок на поле
    /// </summary>
    internal class EnabledButtonStatusToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnabledButtonStatus bs)
                return GetImageForCellValues(bs);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private BitmapImage GetImageForCellValues(EnabledButtonStatus buttonStatus)
        {
            switch (buttonStatus)
            {
                case EnabledButtonStatus.Default:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/EnabledButton.png")));
                case EnabledButtonStatus.Flagged:
                    return new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/buttons/Flag.png")));
                default:
                    return null;
            }
        }
    }
}
