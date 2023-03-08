using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClippingMarker
{
    //класс, определяющий ребро полигона
    internal class Edge
    {
        public Point start { get; set; }

        public Point finish { get; set; }

        public int Side { get; set; }

        public int Kind { get; set; }

        public double Dx { get; set; }

        public bool Checked { get; set; }

        public string PolyType { get; set; }

        public int InnerSide { get; set; }

        public int OuterSide { get; set; }

        public bool Show { get; set; }

        public List<NewNode> Intersection { get; set; }

        public bool Intermediate { get; set; }


        public Edge(Point start, Point finish, int kind)
        {
            Kind = kind;
            this.start = start;
            this.finish = finish;
            if ((finish.X - start.X) == 0)
            {
                Dx = 0;
            }
            else
            {
                Dx = (finish.Y - start.Y) / (double)(finish.X - start.X);
            }
            InnerSide = 1;
            OuterSide = 0;
            Checked = false;
            Show = true;
            Intersection = new List<NewNode>();
            Intermediate = false;
        }

        public Edge(Point start, Point finish, int kind, int side)
        {
            Kind = kind;
            this.start = start;
            this.finish = finish;
            if ((finish.X - start.X) == 0)
            {
                Dx = 0;
            }
            else
            {
                Dx = (finish.Y - start.Y) / (double)(finish.X - start.X);
            }
            InnerSide = 1;
            OuterSide = 0;
            this.Side = side;
            Checked = false;
            Show = true;
            Intersection = null;
        }

        public void UpdateDX()
        {
            if ((finish.X - start.X) == 0)
            {
                Dx = 0;
            }
            else
            {
                Dx = (finish.Y - start.Y) / (double)(finish.X - start.X);
            }
        }
    }
}
