using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClippingMarker
{
    //класс, определяющий основной алгоритм
    internal class Clipper
    {
        public List<int> SBL;
        public List<BoundPair> LML;
        public List<BoundPair> permament;
        public List<Edge> AEL;
        public List<Edge> SEL;
        public List<NewNode> IL;
        public List<Edge> Result;

        public Clipper()
        {
            SBL = new List<int>();
            LML = new List<BoundPair>();
            AEL = new List<Edge>();

            SEL = new List<Edge>();
            IL = new List<NewNode>();
            permament = new List<BoundPair>();
            Result = new List<Edge>();
        }

        //основная функция
        public void ShowClip(Point[] subjectPoints, Point[] clipPoints)
        {
            int yb, yt;
            UpdateLMLandSBL(subjectPoints, 1);
            UpdateLMLandSBL(clipPoints, 2);

            SBL.Sort();
            yb = SBL[0];
            SBL.RemoveAt(0);

            int i = 0;
            while (SBL.Count() > 0)
            {
                ResetAEL();

                AddEdgesStartingAtYBtoAEL(yb);
                yt = SBL[0];
                SBL.RemoveAt(0);
                SBL.Sort();
                ProcessIntersections();
                Result.Clear();
                yb = yt;
                i++;
            }

            Result.Clear();
            ProcessNodes();
            UpdateEdges();
        }

        //сбрасывает параметр ребра, отвечающий за имеющиеся у него пересечения
        public void ResetAEL()
        {
            foreach (Edge e in AEL)
            {
                e.Intersection.Clear();
            }
        }

        //функция, обрабатывающая ребро с одним пересечением
        public List<Edge> CutEdgeWithOneNode(NewNode node, Edge edge)
        {
            Edge firstEdge = edge;
            Edge secondEdge = edge;
            List<Edge> edges = new List<Edge>();

            if ((edge.start.X == node.e1.start.X) && (edge.start.Y == node.e1.start.Y))
            {
                firstEdge = new Edge(edge.start, node.point, node.e1.Kind, node.e1.Side);
                firstEdge.Checked = node.e1.Checked;
                firstEdge.OuterSide = node.e1.OuterSide;
                firstEdge.InnerSide = node.e1.InnerSide;

                secondEdge = new Edge(node.point, node.e2.finish, node.e2.Kind, node.e2.Side);
                secondEdge.Checked = node.e2.Checked;
                secondEdge.OuterSide = node.e2.OuterSide;
                secondEdge.InnerSide = node.e2.InnerSide;
            }

            if ((edge.start.X == node.e1.finish.X) && (edge.start.Y == node.e1.finish.Y))
            {
                firstEdge = new Edge(edge.start, node.point, node.e1.Kind, node.e1.Side);
                firstEdge.Checked = node.e1.Checked;
                firstEdge.OuterSide = node.e1.OuterSide;
                firstEdge.InnerSide = node.e1.InnerSide;

                secondEdge = new Edge(node.point, node.e2.start, node.e2.Kind, node.e2.Side);
                secondEdge.Checked = node.e2.Checked;
                secondEdge.OuterSide = node.e2.OuterSide;
                secondEdge.InnerSide = node.e2.InnerSide;
            }


            if ((edge.start.X == node.e3.start.X) && (edge.start.Y == node.e3.start.Y))
            {
                firstEdge = new Edge(edge.start, node.point, node.e3.Kind, node.e3.Side);
                firstEdge.Checked = node.e3.Checked;
                firstEdge.OuterSide = node.e3.OuterSide;
                firstEdge.InnerSide = node.e3.InnerSide;

                secondEdge = new Edge(node.point, node.e4.finish, node.e4.Kind, node.e4.Side);
                secondEdge.Checked = node.e4.Checked;
                secondEdge.OuterSide = node.e4.OuterSide;
                secondEdge.InnerSide = node.e4.InnerSide;
            }

            if ((edge.start.X == node.e3.finish.X) && (edge.start.Y == node.e3.finish.Y))
            {
                firstEdge = new Edge(edge.start, node.point, node.e3.Kind, node.e3.Side);
                firstEdge.Checked = node.e3.Checked;
                firstEdge.OuterSide = node.e3.OuterSide;
                firstEdge.InnerSide = node.e3.InnerSide;

                secondEdge = new Edge(node.point, node.e4.start, node.e4.Kind, node.e4.Side);
                secondEdge.Checked = node.e4.Checked;
                secondEdge.OuterSide = node.e4.OuterSide;
                secondEdge.InnerSide = node.e4.InnerSide;
            }

            edges.Add(firstEdge);
            edges.Add(secondEdge);
            return edges;
        }

        internal struct INTSorter1 : IComparer<NewNode>
        {
            public int Compare(NewNode n1, NewNode n2)
            {

                return n1.point.X.CompareTo(n2.point.X);
            }
        }

        internal struct INTSorter2 : IComparer<NewNode>
        {
            public int Compare(NewNode n1, NewNode n2)
            {

                return n2.point.X.CompareTo(n1.point.X);
            }
        }

        //функция, обрабатывающая ребро с двумя пересечениями
        public List<Edge> CutEdgeWithTwoNodes(Edge edge)
        {
            if (((Angle(edge) >= 0) && (Angle(edge) <= 90)) || (Angle(edge) >= 270 && (Angle(edge) <= 360)))
                edge.Intersection.Sort(new INTSorter1());
            else
                edge.Intersection.Sort(new INTSorter2());
            Edge firstEdge = edge;
            Edge secondEdge = edge;
            List<Edge> edges = new List<Edge>();
            Edge trimmedEdge = edge;
            foreach (var node in edge.Intersection)
            {
                if ((trimmedEdge.start.X == node.e1.start.X) && (trimmedEdge.start.Y == node.e1.start.Y))
                {
                    firstEdge = new Edge(trimmedEdge.start, node.point, node.e1.Kind, node.e1.Side);
                    firstEdge.Checked = node.e1.Checked;
                    firstEdge.OuterSide = node.e1.OuterSide;
                    firstEdge.InnerSide = node.e1.InnerSide;

                    secondEdge = new Edge(node.point, node.e2.finish, node.e2.Kind, node.e2.Side);
                    secondEdge.Checked = node.e2.Checked;
                    secondEdge.OuterSide = node.e2.OuterSide;
                    secondEdge.InnerSide = node.e2.InnerSide;


                    trimmedEdge = secondEdge;
                    edges.Add(firstEdge);
                    continue;
                }

                if ((trimmedEdge.finish.X == node.e2.finish.X) && (trimmedEdge.finish.Y == node.e2.finish.Y))
                {
                    firstEdge = new Edge(trimmedEdge.start, node.point, node.e1.Kind, node.e1.Side);
                    firstEdge.Checked = node.e1.Checked;
                    firstEdge.OuterSide = node.e1.OuterSide;
                    firstEdge.InnerSide = node.e1.InnerSide;

                    secondEdge = new Edge(node.point, node.e2.finish, node.e2.Kind, node.e2.Side);
                    secondEdge.Checked = node.e2.Checked;
                    secondEdge.OuterSide = node.e2.OuterSide;
                    secondEdge.InnerSide = node.e2.InnerSide;

                    trimmedEdge = secondEdge;
                    edges.Add(firstEdge);
                    continue;
                }

                if ((trimmedEdge.start.X == node.e3.start.X) && (trimmedEdge.start.Y == node.e3.start.Y))
                {
                    firstEdge = new Edge(trimmedEdge.start, node.point, node.e3.Kind, node.e3.Side);
                    firstEdge.Checked = node.e3.Checked;
                    firstEdge.OuterSide = node.e3.OuterSide;
                    firstEdge.InnerSide = node.e3.InnerSide;

                    secondEdge = new Edge(node.point, node.e4.finish, node.e4.Kind, node.e4.Side);
                    secondEdge.Checked = node.e4.Checked;
                    secondEdge.OuterSide = node.e4.OuterSide;
                    secondEdge.InnerSide = node.e4.InnerSide;

                    trimmedEdge = secondEdge;
                    edges.Add(firstEdge);
                    continue;

                }

                if ((trimmedEdge.finish.X == node.e4.finish.X) && (trimmedEdge.finish.Y == node.e4.finish.Y))
                {
                    firstEdge = new Edge(trimmedEdge.start, node.point, node.e3.Kind, node.e3.Side);
                    firstEdge.Checked = node.e3.Checked;
                    firstEdge.OuterSide = node.e3.OuterSide;
                    firstEdge.InnerSide = node.e3.InnerSide;

                    secondEdge = new Edge(node.point, node.e4.finish, node.e4.Kind, node.e4.Side);
                    secondEdge.Checked = node.e4.Checked;
                    secondEdge.OuterSide = node.e4.OuterSide;
                    secondEdge.InnerSide = node.e4.InnerSide;

                    trimmedEdge = secondEdge;
                    edges.Add(firstEdge);
                    continue;
                }
            }
            edges.Add(trimmedEdge);
            return edges;
        }

        //добавляет ребра с обработанными пересечениями в выходной список ребер
        public void UpdateEdges()
        {
            Result.Clear();
            foreach (var edge in AEL)
            {
                if (edge.Intersection.Count() == 0)
                {
                    if (!Result.Contains(edge))
                        Result.Add(edge);
                }
                else
                {
                    if (edge.Intersection.Count() == 1)
                    {
                        NewNode node = edge.Intersection[0];

                        List<Edge> edges = CutEdgeWithOneNode(node, edge);

                        foreach (Edge element in edges)
                        {
                            if (!Result.Contains(element))
                            {
                                Result.Add(element);
                            }
                        }
                    }
                    else
                    {
                        List<Edge> edges = CutEdgeWithTwoNodes(edge);

                        foreach (Edge element in edges)
                        {
                            if (!Result.Contains(element))
                            {
                                Result.Add(element);
                            }
                        }
                    }
                }
            }
        }

        //функция, обрабатывающая пересечение в соответствии с правилом
        public void ProcessNodes()
        {
            foreach (var node in IL)
            {
                if ((node.e3.PolyType == "subject") && (node.e3.Side == 1) && (node.e1.PolyType == "clip") && (node.e1.Side == 2))
                {
                    if (Angle(node.e1) > Angle(node.e3))
                    {
                        node.e1.Checked = true;
                        node.e1.InnerSide = 3;
                        node.e1.OuterSide = 2;

                        node.e3.Checked = true;
                        node.e3.InnerSide = 3;
                        node.e3.OuterSide = 2;
                        continue;
                    }
                    else
                    {
                        node.e2.Checked = true;
                        node.e2.InnerSide = 3;
                        node.e2.OuterSide = 2;

                        node.e4.Checked = true;
                        node.e4.InnerSide = 3;
                        node.e4.OuterSide = 2;
                        continue;
                    }
                }
                else
                {
                    if ((node.e1.PolyType == "subject") && (node.e1.Side == 1) && (node.e3.PolyType == "clip") && (node.e3.Side == 2))
                    {
                        if (Angle(node.e1) < Angle(node.e3))
                        {
                            node.e1.Checked = true;
                            node.e1.InnerSide = 3;
                            node.e1.OuterSide = 2;

                            node.e3.Checked = true;
                            node.e3.InnerSide = 3;
                            node.e3.OuterSide = 2;
                            continue;
                        }
                        else
                        {
                            node.e2.Checked = true;
                            node.e2.InnerSide = 3;
                            node.e2.OuterSide = 2;

                            node.e4.Checked = true;
                            node.e4.InnerSide = 3;
                            node.e4.OuterSide = 2;
                            continue;
                        }
                    }
                }

                if ((node.e3.PolyType == "subject") && (node.e3.Side == 1) && (node.e1.PolyType == "clip") && (node.e1.Side == 1))
                {
                    node.e2.Checked = true;
                    node.e2.InnerSide = 3;
                    node.e2.OuterSide = 2;

                    node.e3.Checked = true;
                    node.e3.InnerSide = 3;
                    node.e3.OuterSide = 2;
                    continue;
                }
                else
                {
                    if ((node.e1.PolyType == "subject") && (node.e1.Side == 1) && (node.e3.PolyType == "clip") && (node.e3.Side == 1))
                    {
                        node.e2.Checked = true;
                        node.e2.InnerSide = 3;
                        node.e2.OuterSide = 2;

                        node.e3.Checked = true;
                        node.e3.InnerSide = 3;
                        node.e3.OuterSide = 2;
                        continue;
                    }
                }

                if ((node.e3.PolyType == "subject") && (node.e3.Side == 2) && (node.e1.PolyType == "clip") && (node.e1.Side == 2))
                {
                    node.e1.Checked = true;
                    node.e1.InnerSide = 3;
                    node.e1.OuterSide = 2;

                    node.e4.Checked = true;
                    node.e4.InnerSide = 3;
                    node.e4.OuterSide = 2;
                    continue;
                }
                else
                {
                    if ((node.e1.PolyType == "subject") && (node.e1.Side == 2) && (node.e3.PolyType == "clip") && (node.e3.Side == 2))
                    {
                        node.e1.Checked = true;
                        node.e1.InnerSide = 3;
                        node.e1.OuterSide = 2;

                        node.e4.Checked = true;
                        node.e4.InnerSide = 3;
                        node.e4.OuterSide = 2;
                        continue;
                    }
                }

                if ((node.e3.PolyType == "subject") && (node.e3.Side == 2) && (node.e1.PolyType == "clip") && (node.e1.Side == 1))
                {
                    if (Angle(node.e1) > Angle(node.e3))
                    {

                        node.e1.Checked = true;
                        node.e1.InnerSide = 3;
                        node.e1.OuterSide = 2;

                        node.e3.Checked = true;
                        node.e3.InnerSide = 3;
                        node.e3.OuterSide = 2;
                        continue;
                    }
                    else
                    {
                        node.e2.Checked = true;
                        node.e2.InnerSide = 3;
                        node.e2.OuterSide = 2;

                        node.e4.Checked = true;
                        node.e4.InnerSide = 3;
                        node.e4.OuterSide = 2;
                        continue;
                    }
                }
                else
                {
                    if ((node.e1.PolyType == "subject") && (node.e1.Side == 2) && (node.e3.PolyType == "clip") && (node.e3.Side == 1))
                    {
                        if (Angle(node.e1) < Angle(node.e3))
                        {
                            node.e1.Checked = true;
                            node.e1.InnerSide = 3;
                            node.e1.OuterSide = 2;

                            node.e3.Checked = true;
                            node.e3.InnerSide = 3;
                            node.e3.OuterSide = 2;
                            continue;
                        }
                        else
                        {
                            node.e2.Checked = true;
                            node.e2.InnerSide = 3;
                            node.e2.OuterSide = 2;

                            node.e4.Checked = true;
                            node.e4.InnerSide = 3;
                            node.e4.OuterSide = 2;
                            continue;
                        }
                    }
                }
            }
        }

        //функция, определяющая угол между ребром и горизонталью
        public double Angle(Edge e)
        {
            double angle = (Math.Atan2(e.finish.Y - e.start.Y, e.finish.X - e.start.X) - Math.Atan2(0, 10)) * 180 / Math.PI;

            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }

        //функция, создающая ребра из точки yb и добавляющая их в active edge list
        public void AddEdgesStartingAtYBtoAEL(int yb)
        {
            foreach (var bound in LML)
            {
                int rInd = 0;
                int lInd = 0;

                foreach (var element in bound.rightBounds)
                {
                    foreach (var rPoint in element)
                    {
                        if ((rPoint.Y == yb) && (rPoint.Type != 4))
                        {
                            Edge e1 = new Edge(element[rInd], element[rInd + 1], bound.kind);
                            if (bound.kind == 1)
                            {
                                e1.PolyType = "subject";
                                e1.Side = 1;
                            }
                            else if (bound.kind == 2)
                            {
                                e1.PolyType = "clip";
                                e1.Side = 1;
                            }
                            AEL.Add(e1);
                            AEL.Sort(new AELSorter());
                        }
                        rInd++;
                    }
                }

                foreach (var element in bound.leftBounds)
                {
                    foreach (var lPoint in element)
                    {
                        if ((lPoint.Y == yb) && (lPoint.Type != 4))
                        {
                            Edge e1 = new Edge(element[lInd], element[lInd + 1], bound.kind);
                            if (bound.kind == 1)
                            {
                                e1.PolyType = "subject";
                                e1.Side = 2;
                            }
                            else if (bound.kind == 2)
                            {
                                e1.PolyType = "clip";
                                e1.Side = 2;
                            }
                            AEL.Add(e1);
                            AEL.Sort(new AELSorter());
                        }
                        lInd++;
                    }
                }
            }
        }

        //функция, создающая local minima list и добавляющая все уникальные точки в scanbeam list
        protected void UpdateLMLandSBL(Point[] polyPoints, int kind)
        {
            BoundPair poly = new BoundPair(polyPoints, kind);
            LML.Add(poly);
            permament.Add(poly);

            foreach (var element in poly.rightBounds)
            {
                foreach (var point in element)
                {
                    if (!SBL.Contains(point.Y))
                    {
                        SBL.Add(point.Y);
                    }

                }
            }

            foreach (var element in poly.leftBounds)
            {
                foreach (var point in element)
                {
                    if (!SBL.Contains(point.Y))
                    {
                        SBL.Add(point.Y);
                    }

                }
            }

            SBL.Sort();
        }

        internal struct AELSorter : IComparer<Edge>
        {
            public int Compare(Edge e1, Edge e2)
            {
                return e1.start.X.CompareTo(e2.start.X);
            }
        }

        internal struct SELSorter : IComparer<Edge>
        {
            public int Compare(Edge e1, Edge e2)
            {
                return e1.finish.X.CompareTo(e2.finish.X);
            }
        }

        //функция, обрабатывающая ребра,
        public void ProcessIntersections()
        {
            if (AEL.Count() == 0) return;
            BuildIL();
        }

        //функция, обрабатывающая ребра и находящая точки пересечения
        public void BuildIL()
        {
            Edge temp1, temp2, temp3, temp4;
            int topX1;
            bool moreEdges;
            Point p;
            NewNode intersectNode;

            SEL.Add(AEL[0]);
            SEL.Sort(new SELSorter());
            IL.Clear();

            foreach (Edge e1 in AEL)
            {
                if (!AEL[0].Equals(e1))
                {
                    topX1 = e1.finish.X;
                    Edge e2 = SEL.Last();
                    moreEdges = true;

                    while (moreEdges && (topX1 < e2.finish.X))
                    {
                        if (e1.PolyType != e2.PolyType)
                        {
                            if (CheckInter(e1, e2))
                            {
                                p = IntersectionOf(e1, e2);

                                temp1 = new Edge(e1.start, p, e1.Kind, e1.Side);
                                temp1.PolyType = e1.PolyType;

                                temp2 = new Edge(p, e1.finish, e1.Kind, e1.Side);
                                temp2.PolyType = e1.PolyType;

                                temp3 = new Edge(e2.start, p, e2.Kind, e2.Side);
                                temp3.PolyType = e2.PolyType;

                                temp4 = new Edge(p, e2.finish, e2.Kind, e2.Side);
                                temp4.PolyType = e2.PolyType;

                                if (p != null)
                                {
                                    intersectNode = new NewNode(temp1, temp2, temp3, temp4, p);
                                    IL.Add(intersectNode);
                                    e1.Intersection.Add(intersectNode);

                                    AEL.ElementAt(AEL.IndexOf(e2)).Intersection.Add(intersectNode);
                                }
                            }

                        }
                        if (SEL[0].Equals(e2))
                        {
                            moreEdges = false;
                        }
                        else
                        {
                            e2 = SEL[SEL.IndexOf(e2) - 1];
                        }
                    }

                    if (moreEdges)
                    {
                        SEL.Insert(SEL.IndexOf(e2) + 1, e1);
                    }
                    else
                    {
                        SEL.Insert(0, e1);
                    }
                    SEL.Sort(new SELSorter());
                }
            }
        }

        //функция, указывающая наличие пересечения между двумя ребрами
        public bool CheckInter(Edge e1, Edge e2)
        {
            int dx0 = e1.finish.X - e1.start.X;
            int dx1 = e2.finish.X - e2.start.X;
            int dy0 = e1.finish.Y - e1.start.Y;
            int dy1 = e2.finish.Y - e2.start.Y;
            int p0 = dy1 * (e2.finish.X - e1.start.X) - dx1 * (e2.finish.Y - e1.start.Y);
            int p1 = dy1 * (e2.finish.X - e1.finish.X) - dx1 * (e2.finish.Y - e1.finish.Y);
            int p2 = dy0 * (e1.finish.X - e2.start.X) - dx0 * (e1.finish.Y - e1.start.Y);
            int p3 = dy0 * (e1.finish.X - e2.finish.X) - dx0 * (e1.finish.Y - e1.finish.Y);

            return (p0 * p1 <= 0) && (p2 * p3 <= 0);
        }

        //функция, вовращающая точку пересечения двух ребер
        public Point IntersectionOf(Edge e1, Edge e2)
        {
            double x1 = e1.start.X, y1 = e1.start.Y;
            double x2 = e1.finish.X, y2 = e1.finish.Y;

            double x3 = e2.start.X, y3 = e2.start.Y;
            double x4 = e2.finish.X, y4 = e2.finish.Y;

            double tolerance = 0.001;
            double x, y;

            if (Math.Abs(x1 - x2) < tolerance)
            {
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;
                x = x1;
                y = c2 + m2 * x1;
            }
            else if (Math.Abs(x3 - x4) < tolerance)
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                x = x3;
                y = c1 + m1 * x3;
            }
            else
            {
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;
            }
            return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
        }
    }
}
