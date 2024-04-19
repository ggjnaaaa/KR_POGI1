using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Наследник Button для пользовательской кнопки со смайликом
    /// </summary>
    internal class SmileButton : Button
    {
        #region Пользовательские свойства

        // Пользовательское свойство для смайлика с очками в случае победы
        public static readonly DependencyProperty IsWinProperty =
            DependencyProperty.Register(
                nameof(IsWin),
                typeof(bool),
                typeof(SmileButton),
                new PropertyMetadata(false, OnPropertyChanged));
        public bool IsWin
        {
            get => (bool)GetValue(IsWinProperty);
            set => SetValue(IsWinProperty, value);
        }

        // Пользовательское свойство для умершего смайлика в случае проигрыша
        public static readonly DependencyProperty IsLoseProperty =
            DependencyProperty.Register(
                nameof(IsLose),
                typeof(bool),
                typeof(SmileButton),
                new PropertyMetadata(false, OnPropertyChanged));
        public bool IsLose
        {
            get => (bool)GetValue(IsLoseProperty);
            set => SetValue(IsLoseProperty, value);
        }

        // Пользовательское свойство для удивлённого смайлика в случае нажатия кнопки на поле
        public static readonly DependencyProperty IsWowProperty =
            DependencyProperty.Register(
                nameof(IsWow),
                typeof(bool),
                typeof(SmileButton),
                new PropertyMetadata(false, OnPropertyChanged));
        public bool IsWow
        {
            get => (bool)GetValue(IsWowProperty);
            set => SetValue(IsWowProperty, value);
        }

        /// <summary>
        /// Общий метод для всех пользовательских свойств. Вызывается при изменении вышеописанных свойств.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (SmileButton)d;
            var value = (bool)e.NewValue;
            var propertyName = e.Property.Name;

            if (value)
                switch (propertyName)
                {
                    case "IsWin":
                        button.Background = new ImageBrush(
                            new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/Win.png"))));
                        break;
                    case "IsLose":
                        button.Background = new ImageBrush(
                            new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/Death.png"))));
                        break;
                    case "IsWow":
                        button.Background = new ImageBrush(
                            new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/Wow.png"))));
                        break;
                }
            else
                button.Background = button.SmileImg;
        }

        #endregion

        private readonly ImageBrush SmileImg;
        private readonly ImageBrush ClickedImg;

        public SmileButton()
        {
            SmileImg = new ImageBrush(
                new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/Smile.png"))));
            ClickedImg = new ImageBrush(
                new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img/Clicked.png"))));
            Background = SmileImg;
            Style = CreateCustomButtonStyle();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            Background = ClickedImg;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            Background = SmileImg;
        }

        private Style CreateCustomButtonStyle()
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
            
            // Создаем шаблон кнопки
            var template = new ControlTemplate(typeof(Button));
            var border = new FrameworkElementFactory(typeof(Border));
            border.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });
            border.SetBinding(Border.BorderBrushProperty, new Binding("BorderBrush") { RelativeSource = RelativeSource.TemplatedParent });
            border.SetBinding(Border.BorderThicknessProperty, new Binding("BorderThickness") { RelativeSource = RelativeSource.TemplatedParent });

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;
            
            // Добавляем триггер для наведения мыши
            var mouseOverTrigger = new Trigger { Property = IsMouseOverProperty, Value = true };
            mouseOverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, SmileImg));
            template.Triggers.Add(mouseOverTrigger);

            style.Setters.Add(new Setter(Control.TemplateProperty, template));
            return style;
        }
    }
}
