using System;
using System.IO;

namespace ClippingMarker
{
    class Program
    {
        static void Main(string[] args)
        {
            Clipper clipper = new Clipper();

            //Point[] subject = new Point[] { new Point(0, 0), new Point(50, 10), new Point(65, 45) };
            //Point[] clip = new Point[] { new Point(25, 10), new Point(35, 50), new Point(15, 15) };

            Point[] subject = new Point[] { new Point(105, 5), new Point(110, 65), new Point(35, 75) };
            Point[] clip = new Point[] { new Point(70, 10), new Point(125, 40), new Point(30, 60) };

            //Point[] clip = new Point[] { new Point(15, 10), new Point(45, 50), new Point(5, 40) };
            //Point[] subject = new Point[] { new Point(40, 15), new Point(70, 30), new Point(25, 40) };

            clipper.ShowClip(subject, clip);

            Console.WriteLine("Проверка результата");

            int i = 1;
            foreach (var edge in clipper.Result)
            {
                Console.WriteLine();
                Console.WriteLine("{0} ребро: {1} - {2} || {3} - {4}", i, edge.start.X, edge.start.Y, edge.finish.X, edge.finish.Y);
                Console.WriteLine("Маркер ребра - {0}", edge.Checked);
                i++;
            }
        }
    }
}

