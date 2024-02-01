using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuNode
    {
        private int _row;
        private int _col;
        private int _value;

        public int Row { get => _row; set => _row = value; }
        public int Col { get => _col; set => _col = value; }
        public int Value { get => _value; set => _value = value; }
        public bool Domain()
        {   if(_value<=9 && _value >0)
                return true;
            else 
                return false;

        }
    }
}
