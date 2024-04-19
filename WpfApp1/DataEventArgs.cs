using System;

namespace WpfApp1
{
    public delegate void DataEventHandler(object sender, DataEventArgs e);

    /// <summary>
    /// Хранит значениня пользовательского ввода в окне параметров
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        public int HeightInput;
        public int WidthInput;
        public int MinesInput;

        public DataEventArgs(int height, int width, int mines)
        {
            HeightInput = height;
            WidthInput = width;
            MinesInput = mines;
        }
    }
}
