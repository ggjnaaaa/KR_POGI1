using System;
using System.Collections;
using System.Collections.Generic;

namespace WpfApp1
{
    /// <summary>
    /// Игровое поле. Создержит логику взаимодействия с полем (автоматическое открытие ячеек,
    /// расстановка мин, подсчет мин вокруг кнопки и тп)
    /// </summary>
    public class Field : ViewModelBase, IEnumerable<Cell>
    {
        public delegate void Game();
        public event Game GameOver;
        public event Game Win;

        public int Width => Cells == null ? 0 : Cells.GetLength(1);
        public int Height => Cells == null ? 0 : Cells.GetLength(0);
        public int Mines { get; }

        public int BtnHeight { get => Cells == null ? -1 : Cells[0, 0].BtnHeight; }
        public int BtnWidth { get => Cells == null ? -1 : Cells[0, 0].BtnWidth; }

        private Cell[,] Cells;
        private int NumberOfOpenCells;

        #region Конструкторы

        /// <summary>
        /// Конструктор для дефолтных сложностей.
        /// </summary>
        /// <param name="diff">Сложность (все кроме Customizable)</param>
        public Field(DifficultyLevel diff)
        {
            switch (diff)
            {
                case DifficultyLevel.Easy:
                    Cells = new Cell[10, 10];
                    Mines = 10;
                    break;
                case DifficultyLevel.Medium:
                    Cells = new Cell[16, 16];
                    Mines = 40;
                    break;
                case DifficultyLevel.Hard:
                    Cells = new Cell[16, 30];
                    Mines = 99;
                    break;
            }

            NumberOfOpenCells = 0;
            InitializeCells();
        }

        /// <summary>
        /// Конструктор для пользовательского режима.
        /// </summary>
        /// <param name="height">Высота поля</param>
        /// <param name="width">Ширина поля</param>
        /// <param name="mines">Количество мин</param>
        public Field(int height, int width, int mines)
        {
            Cells = new Cell[height, width];
            Mines = mines;

            NumberOfOpenCells = 0;
            InitializeCells();
        }

        #endregion

        #region Начало игры

        /// <summary>
        /// Задаёт массив пустых клеток.
        /// </summary>
        private void InitializeCells()
        {
            ProcessArray((i, j) =>
            {
                if (Cells[i, j] == null || Cells[i, j] != null && !Cells[i, j].HasMine)
                    Cells[i, j] = new Cell(IndexesToTag(j, i));
            });
        }

        /// <summary>
        /// Начинает игру чтобы первая нажатая кнопка на поле была нулём.
        /// </summary>
        /// <param name="startedIndex">Индекс начальной клетки</param>
        public void StartGame(int startedIndex)
        {
            // Клетки вокруг стартовой становятся нулями
            var row = startedIndex / Width;
            var column = startedIndex % Width;
            Cells[row, column].Value = CellValues.Zero;

            ProcessAroundButton((i, j) =>
            {
                var startCellX = column + i;
                var startCellY = row + j;

                if (startCellX >= 0 && startCellX < Width &&
                    startCellY >= 0 && startCellY < Height)
                {
                    Cells[startCellY, startCellX].Value = CellValues.Zero;
                }
            });

            RandomizeMines();
            // Подсчёт количества мин во всём поле
            ProcessArray((i, j) =>
            {
                if (Cells[i, j] == null || Cells[i, j] != null && !Cells[i, j].HasMine)
                    Cells[i, j].Value = CountAdjacentMines(i, j);
            });
        }

        /// <summary>
        /// Расставляет рандомные мины везде кроме клеток вокруг стартовой.
        /// </summary>
        private void RandomizeMines()
        {
            var minesLaid = 0;
            var rnd = new Random();

            while (minesLaid < Mines)
            {
                var randomColumn = rnd.Next(1, Width - 1);
                var randomRow = rnd.Next(1, Height - 1);

                if (!Cells[randomRow, randomColumn].HasMine && Cells[randomRow, randomColumn].Value != CellValues.Zero)
                {
                    Cells[randomRow, randomColumn].Value = CellValues.Mine;
                    minesLaid++;
                }
            }
        }

        /// <summary>
        /// Считает количество мин вокруг заданой клетки.
        /// </summary>
        /// <param name="x">Первый индекс</param>
        /// <param name="y">Второй индекс</param>
        /// <returns>Количество мин вокруг</returns>
        public CellValues CountAdjacentMines(int x, int y)
        {
            var count = 0;
            ProcessAroundButton((i, j) =>
            {
                var neighborX = x + i;
                var neighborY = y + j;
                if (neighborX >= 0 && neighborX < Height &&
                    neighborY >= 0 && neighborY < Width)
                    if (!(i == 0 && j == 0))
                        if (Cells[neighborX, neighborY] != null && Cells[neighborX, neighborY].HasMine)
                            count++;
            });

            return ((CellValues[])Enum.GetValues(typeof(CellValues)))[count + 1];
        }

        #endregion

        #region Нажатие на кнопки мыши

        /// <summary>
        /// Нажатие на ячейку ЛКМ.
        /// </summary>
        /// <param name="index">Индекс нажатой ячейки</param>
        public void CellLeftUp(int index)
        {
            var row = index / Width;
            var column = index % Width;

            if (!Cells[row, column].IsEnabled || Cells[row, column].ButtonStatus == EnabledButtonStatus.Flagged)
                return;

            var n = Cells[row, column].Value;

            if (n == CellValues.Mine)
                IsMine(row, column);
            else if (n == CellValues.Zero)
                AutoOpen(row, column);
            else
                IsNumber(row, column);

            if (NumberOfOpenCells == Width * Height - Mines)
            {
                Win?.Invoke();

                foreach (var cell in Cells)
                    if (cell.Value != CellValues.Mine)
                        cell.IsEnabled = false;
            }
        }

        /// <summary>
        /// Нажатие на ячейку ПКМ.
        /// </summary>
        /// <param name="index">Индекс нажатой ячейки</param>
        /// <returns>Был ли поставлен флаг</returns>
        public bool CellRightClick(int index)
        {
            var row = index / Width;
            var column = index % Width;

            if (Cells[row, column].ButtonStatus == EnabledButtonStatus.Default)
                Cells[row, column].ButtonStatus = EnabledButtonStatus.Flagged;
            else
                Cells[row, column].ButtonStatus = EnabledButtonStatus.Default;

            return Cells[row, column].ButtonStatus == EnabledButtonStatus.Flagged;
        }

        #endregion

        #region Действия на разные значения нажатой кнопки на поле

        /// <summary>
        /// Клетка оказалась миной.
        /// </summary>
        /// <param name="row">Первый индекс ячейки</param>
        /// <param name="column">Второй индекс ячейки</param>
        private void IsMine(int row, int column)
        {
            Cells[row, column].Value = CellValues.ExplodedMine;
            Cells[row, column].IsEnabled = false;
            foreach (var cell in Cells)
                if (cell.Value == CellValues.Mine)
                    cell.IsEnabled = false;

            GameOver?.Invoke();
        }

        /// <summary>
        /// Автооткрытие ячеек рекурсивно.
        /// </summary>
        /// <param name="row">Первый индекс ячейки</param>
        /// <param name="column">Второй индекс ячейки</param>
        private void AutoOpen(int row, int column)
        {
            if ((row < 0 || row >= Height || column < 0 || column >= Width || !Cells[row, column].IsEnabled))
                return;
            if (Cells[row, column].ButtonStatus == EnabledButtonStatus.Flagged)
                return;

            Cells[row, column].IsEnabled = false;
            NumberOfOpenCells++;

            if (Cells[row, column].Value == CellValues.Zero)
            {
                AutoOpen(row - 1, column - 1);
                AutoOpen(row - 1, column);
                AutoOpen(row - 1, column + 1);
                AutoOpen(row, column - 1);
                AutoOpen(row, column + 1);
                AutoOpen(row + 1, column - 1);
                AutoOpen(row + 1, column);
                AutoOpen(row + 1, column + 1);
            }
        }

        /// <summary>
        /// Клетка оказалась числом.
        /// </summary>
        /// <param name="row">Первый индекс ячейки</param>
        /// <param name="column">Второй индекс ячейки</param>
        private void IsNumber(int row, int column)
        {
            Cells[row, column].IsEnabled = false;
            NumberOfOpenCells++;
        }

        #endregion

        #region Перебор клеток массива для действия

        /// <summary>
        /// Перебор всего массива.
        /// </summary>
        /// <param name="action">Действия для массива</param>
        private void ProcessArray(Action<int, int> action)
        {
            for (var i  = 0; i < Height; i++)
            for (var j = 0; j < Width; j++)
                action(i, j);
        }

        /// <summary>
        /// Перебор кнопок вокруг одной.
        /// </summary>
        /// <param name="action"></param>
        private void ProcessAroundButton(Action<int, int> action)
        {
            for (var i = -1; i <= 1; i++)
            for (var j = -1; j <= 1; j++)
                action(i, j);
        }

        #endregion

        /// <summary>
        /// Перевод двух индексов в тэг.
        /// </summary>
        /// <param name="x">Первый индекс ячейки</param>
        /// <param name="y">Второй индекс ячейки</param>
        /// <returns>Итоговый тэг</returns>
        private int IndexesToTag(int x, int y) => y * Width + x;

        public IEnumerator<Cell> GetEnumerator()
        {
            foreach (var cell in Cells)
            {
                yield return cell;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
