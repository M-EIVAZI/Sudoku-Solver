using System.Diagnostics;

namespace Sudoku
{
    public partial class MainMenu : Form
    {
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private int way;
        public MainMenu()
        {
            InitializeComponent();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            createCells();
        }
        SudokuCell[,] cells = new SudokuCell[9, 9];
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void createCells()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new SudokuCell();
                    cells[i, j].Font = new Font(SystemFonts.DefaultFont.FontFamily, 20);
                    cells[i, j].Size = new Size(80, 80);
                    cells[i, j].ForeColor = Color.White;
                    cells[i, j].Location = new Point(i * 80, j * 80);
                    cells[i, j].BackColor = ((i / 3) + (j / 3)) % 2 == 0 ? Color.DarkKhaki : Color.DodgerBlue;
                    cells[i, j].FlatStyle = FlatStyle.Flat;
                    cells[i, j].FlatAppearance.BorderColor = Color.Black;
                    cells[i, j].X = i;
                    cells[i, j].Y = j;
                    cells[i, j].KeyPress += cell_keyPressed;

                    panel1.Controls.Add(cells[i, j]);
                }
            }
        }

        private void cell_keyPressed(object sender, KeyPressEventArgs e)
        {
            var cell = sender as SudokuCell;

            
            if (cell.IsLocked)
                return;

            int value;

            if (int.TryParse(e.KeyChar.ToString(), out value))
            {
                cell.Text = value.ToString();
                cell.Value = value;
                cell.ForeColor = Color.White;
            }
        }
        public bool IsSafeInRow(int row, int column, int num)
        {
            if (num == 0) return true;
            for (int col = 0; col < 9; col++)
            {
                if (cells[row, col].Value == num && column != col)
                    return false;
            }
            return true;
        }
        public bool IsSafeInColumn(int Row, int column, int num)
        {
            if (num == 0) return true;
            for (int row = 0; row < 9; row++)
            {
                if (cells[row, column].Value == num && Row != row)
                    return false;
            }
            return true;
        }
        private bool IsSafeInGrid(int startRow, int startCol, int num, int Row, int column)
        {
            if (num == 0) return true;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (cells[row + startRow, col + startCol].Value == num && startCol + col != column && startRow + row != Row)
                        return false;
                }
            }
            return true;
        }
        private int GetLeastConstrainingValue(int row, int col, List<int> bl)
        {
            List<int> availableValues = new();

            for (int i = 1; i <= 9; i++)
            {
                if (IsSafe(row, col, i))
                {
                    availableValues.Add(i);
                }
            }

            int minValue = int.MaxValue;
            int leastConstrainingValue = -1;

            foreach (int value in availableValues)
            {
                int count = CountConstrainingValues(row, col, value);
                if (count < minValue)
                {
                    minValue = count;
                    leastConstrainingValue = value;
                }
            }

            return leastConstrainingValue;
        }

        private int CountConstrainingValues(int row, int col, int value)
        {
            int count = 0;

            for (int i = 0; i < 9; i++)
            {
                if (cells[row, i].Value == 0 && IsSafe(row, i, value))
                {
                    count++;
                }
                if (cells[i, col].Value == 0 && IsSafe(i, col, value))
                {
                    count++;
                }
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (cells[i, j].Value == 0 && IsSafe(i, j, value))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        //using Just MRV and Degree
        public bool SolveSudoku()
        {
            int row, col;
            if (!FindBest(out row, out col))
                return true;


            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num))
                {

                    cells[row, col].Value = num;
                    cells[row, col].Text = Convert.ToString(num);

                    if (SolveSudoku())
                        return true;

                    cells[row, col].Value = 0;
                }
            }
            return false;
        }
        //using three heuristics
        public bool SolveSudoku2()
        {
            int row, col;
            if (!FindBest(out row, out col))
                return true;

            bool check = true;
            int num = 0;
            List<int> blocklist = new();

            while (check)
            {
                num = GetLeastConstrainingValue(row, col, blocklist);

                if (num == -1) 
                {
                    cells[row, col].Value = 0;
                    return false;
                }

                if (IsSafe(row, col, num))
                {
                    check = false;
                    cells[row, col].Value = num;
                    cells[row, col].Text = num.ToString();

                    if (SolveSudoku2())
                        return true;

                    cells[row, col].Value = 0; 
                }
                else
                {
                    if (!blocklist.Contains(num))
                        blocklist.Add(num);
                }
            }

            return false;
        }
        //using mrv and lcv
        public bool SolveSudoku3()
        {
            int row, col;
            if (!FindBest2(out row, out col))
                return true;


            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(row, col, num))
                {

                    cells[row, col].Value = num;
                    cells[row, col].Text = Convert.ToString(num);

                    if (SolveSudoku())
                        return true;

                    cells[row, col].Value = 0;
                }
            }
            return false;
        }
        private bool IsSafe(int row, int col, int num)
        {
            return IsSafeInRow(row, col, num) && IsSafeInColumn(row, col, num) && IsSafeInGrid(row - row % 3, col - col % 3, num, row, col);
        }
        private bool FindBest(out int row, out int col)
        {
            int minRemainingValues = int.MaxValue;
            row = -1;
            col = -1;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (cells[r, c].Value == 0)
                    {
                        int remainingvalues = CountRemainingValues(r, c);
                        if (remainingvalues < minRemainingValues)
                        {
                            minRemainingValues = remainingvalues;
                            row = r;
                            col = c;

                        }
                        if (remainingvalues == minRemainingValues)
                        {
                            int mostdegree = CalculateDegree(row, col);
                            int degree = CalculateDegree(r, c);
                            if (mostdegree < degree)
                            {
                                row = r;
                                col = c;

                            }

                        }

                    }

                }

            }
            return row != -1 && col != -1;

        }
        private int CountRemainingValues(int row, int col)
        {
            bool[] used = new bool[10];
            int count = 0;
            for (int i = 0; i < 9; i++)
            {
                used[cells[row, i].Value] = true;
                used[cells[i, col].Value] = true;
            }
            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    used[cells[i, j].Value] = true;
                }
            }
            for (int i = 1; i <= 9; i++)
            {
                if (!used[i])
                    count++;
            }

            return count;
        }

        private int CalculateDegree(int row, int col)
        {
            int degree = 0;
            for (int i = 0; i < 9; i++)
            {
                if (cells[i, col].Value == 0)
                    degree += 1;

            }
            for (int j = 0; j < 9; j++)
                if (cells[row, j].Value == 0)
                    degree += 1;
            int startrow = row - row % 3;
            int startcol = col - col % 3;
            for (int r = startrow; r < startrow + 3; r++)
            {
                for (int c = startcol; c < startcol; c++)
                {
                    if (cells[r, c].Value == 0)
                        degree += 1;

                }

            }
            return degree;

        }
        private bool FindBest2(out int row, out int col)
        {
            int minRemainingValues = int.MaxValue;
            row = -1;
            col = -1;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (cells[r, c].Value == 0)
                    {
                        int remainingvalues = CountRemainingValues(r, c);
                        if (remainingvalues < minRemainingValues)
                        {
                            minRemainingValues = remainingvalues;
                            row = r;
                            col = c;

                        }
                        if (remainingvalues == minRemainingValues)
                        {
                            int minlcv = CalculateLCV(row, col, cells[row, col].Value);
                            int lcv = CalculateLCV(row, col, cells[row, col].Value);
                            if (minlcv < lcv)
                            {
                                row = r;
                                col = c;

                            }

                        }

                    }

                }

            }
            return row != -1 && col != -1;

        }
        private int CalculateLCV(int row, int col, int value)
        {
            int count = 0;

            List<(int, int)> affectedCells = new();

            for (int i = 0; i < 9; i++)
            {
                if (cells[row, i].Value == 0 && i != col && IsSafe(row, i, value))
                {
                    affectedCells.Add((row, i));
                }
                if (cells[i, col].Value == 0 && i != row && IsSafe(i, col, value))
                {
                    affectedCells.Add((i, col));
                }
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = startRow; i < startRow + 3; i++)
            {
                for (int j = startCol; j < startCol + 3; j++)
                {
                    if (cells[i, j].Value == 0 && i != row && j != col && IsSafe(i, j, value))
                    {
                        affectedCells.Add((i, j));
                    }
                }
            }
            foreach (var (r, c) in affectedCells)
            {
                count += CountConstrainingValues(r, c, cells[r, c].Value);
            }

            return count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (!IsSafe(i, j, cells[i, j].Value))
                    {
                        MessageBox.Show($"{i} row and {j} columnn are wrong and broken constraints");
                        return;
                    }
                }
            switch (way)
            {
                case 1:
                    SolveSudoku();
                    break;
                case 2:
                    SolveSudoku2();
                    break;
                case 3:
                    SolveSudoku3();
                    break;

            }
            //            SolveSudokuWithHeuristics();
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (cells[i, j].Value != 0)
                        cells[i, j].Text = cells[i, j].Value.ToString();
                    else
                    {
                        MessageBox.Show("Puzzle is not solvable");
                        return;

                    }
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    cells[i, j].IsLocked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j].Text = "";
                    cells[i, j].Value = 0;
                }
            }
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    cells[i, j].IsLocked = false;
        }
        public void GeneratePuzzle(int difficultyLevel)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j].Text = "";
                    cells[i, j].Value = 0;
                }
            }
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    cells[i, j].IsLocked = false;
            switch(way)
            {
                case 1:
                    SolveSudoku();
                    break;
                case 2:
                    SolveSudoku2();
                    break;
                case 3:
                    SolveSudoku3();
                    break;

            }
            RemoveCellsBasedOnDifficulty(difficultyLevel);

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (cells[i, j].Value != 0)
                        cells[i, j].Text = cells[i, j].Value.ToString();
                    else
                        cells[i, j].Text = "";
        }
        private void RemoveCellsBasedOnDifficulty(int difficultyLevel)
        {
            int cellsToRemove = 0;
            switch (difficultyLevel)
            {
                case 1: // easy
                    cellsToRemove = 45;
                    break;
                case 2: // medium
                    cellsToRemove = 55;
                    break;
                case 3: // hard
                    cellsToRemove = 60;
                    break;
            }

            Random rand = new Random();
            while (cellsToRemove > 0)
            {
                int row = rand.Next(0, 9);
                int col = rand.Next(0, 9);

                if (cells[row, col].Value != 0)
                {
                    cells[row, col].Value = 0;
                    cellsToRemove--;
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            GeneratePuzzle(1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GeneratePuzzle(2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            GeneratePuzzle(3);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Browse Text Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };
            string path = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < 9; i++)
                {
                    path = openFileDialog1.FileName;
                }
            }
            SudokuCell[,] sudokuGrid = new SudokuCell[9, 9];
            if (File.Exists(path))
            {


                using (StreamReader reader = new StreamReader(path))
                {
                    int row = 0;
                    while (!reader.EndOfStream && row < 9)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split('|', '-', '+', ' ');

                        int col = 0;
                        foreach (string value in values)
                        {
                            if (value != "." && value != "")
                            {
                                int cellValue = int.Parse(value);
                                sudokuGrid[row, col] = new SudokuCell
                                {
                                    Value = cellValue,
                                    X = row,
                                    Y = col
                                };
                                col++;
                            }
                            else if (value == ".")
                            {
                                sudokuGrid[row, col] = new SudokuCell
                                {
                                    Value = 0,
                                    X = row,
                                    Y = col
                                };
                                col++;
                            }
                        }
                        bool check = true;
                        for (int j = 0; j < 9; j++)
                            if (sudokuGrid[row, j] == null)
                                check = false;
                        if (check)
                            row++;
                    }
                }

            }
            for (int row = 0; row < 9; row++)
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuGrid[row, col].Value != 0)
                    {
                        cells[row, col].Value = sudokuGrid[row, col].Value;
                        cells[row, col].Text = sudokuGrid[row, col].Value.ToString();
                    }
                    else
                    {
                        cells[row, col].Value = 0;


                    }

                }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "MRV & Degree")
                way = 1;
            if (comboBox1.Text == "MRV & LCV ")
                way = 3;
            if (comboBox1.Text == "MRV & LCV & Degree")
                way = 2;

        }
    }

}
