using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClippingMarker
{
    //класс, определяющий точку полигона
    internal class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Contributing = false;
        }
        public int X
        {
            get; set;
        }
        public int Y
        {
            get; set;
        }
        public bool Contributing
        {
            get; set;
        }
        public int Type
        {
            get; set;
        }
    }
}
