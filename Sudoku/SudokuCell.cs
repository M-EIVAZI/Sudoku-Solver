using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SudokuCell:Button
    {   private int _value;
        private int _x;
        private int _y;
        private bool _islocked;


        public int Value { get => _value; set => _value = value; }
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }
        public bool IsLocked { get => _islocked; set => _islocked = value; }
        public void Clear()
        {
            this.Text = string.Empty;
            this.IsLocked = false;
        }
    }
}
