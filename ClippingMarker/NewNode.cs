using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClippingMarker
{
    internal class NewNode
    {
        public Edge e1;
        public Edge e2;
        public Edge e3;
        public Edge e4;
        public Point point;

        public NewNode(Edge e1, Edge e2, Edge e3, Edge e4, Point point)
        {
            this.e1 = e1;
            this.e2 = e2;
            this.e3 = e3;
            this.e4 = e4;
            this.point = point;
        }
    }
}
