using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public class FieldChangedEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int to_X { get; private set; }
        public int to_Y { get; private set; }

        public FieldChangedEventArgs(int x, int y, int to_x, int to_y)
        {
            X = x;
            Y = y;
            to_X = to_x;
            to_Y = to_y;
        }
    }
}
