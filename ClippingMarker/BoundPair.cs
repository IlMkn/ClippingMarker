using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClippingMarker
{
    //класс, определяющий пары из левой и правой границ полигона
    internal class BoundPair
    {
        public List<List<Point>> leftBounds;
        public List<List<Point>> rightBounds;
        public int kind;
        public int MinY { get; }
        public int MaxY { get; }

        public BoundPair(Point[] listOfPoints, int kind)
        {
            List<List<Point>> outputBounds = new List<List<Point>>();

            leftBounds = new List<List<Point>>();
            rightBounds = new List<List<Point>>();

            this.kind = kind;

            int minY = listOfPoints[0].Y;

            Point localMax = listOfPoints[0];
            Point localMin = listOfPoints[0];
            Point globalMin = listOfPoints[0];
            List<Point> localMaxList = new List<Point>();
            List<Point> localMinList = new List<Point>();
            List<int> indexList = new List<int>();

            int globalMinInd = 0;
            int localMinInd = 0;
            int localMaxInd = 0;

            for (int i = 1; i < listOfPoints.Length; i++)
            {
                if (listOfPoints[i].Y < minY)
                {
                    globalMin = listOfPoints[i];
                    globalMinInd = i;
                }
            }

            localMinList.Add(globalMin);
            bool consecLower = false;
            bool consecUpper = true;
            int j = globalMinInd + 1;
            indexList.Add(j - 1);

            while (!listOfPoints[j % (listOfPoints.Length)].Equals(globalMin))
            {
                if (listOfPoints[(j + 1) % (listOfPoints.Length)].Y > listOfPoints[j % listOfPoints.Length].Y)
                {
                    if (consecLower == true)
                    {
                        consecLower = false;
                        localMinList.Add(listOfPoints[j % (listOfPoints.Length)]);
                        indexList.Add(j % (listOfPoints.Length));
                    }
                    localMax = listOfPoints[(j + 1) % (listOfPoints.Length)];
                    localMaxInd = (j + 1) % (listOfPoints.Length);
                    consecUpper = true;
                    j++;
                }
                else
                {
                    if (listOfPoints[(j + 1) % (listOfPoints.Length)].Y < listOfPoints[j % listOfPoints.Length].Y)
                    {
                        if (consecUpper == true)
                        {
                            consecUpper = false;
                            localMaxList.Add(listOfPoints[j % (listOfPoints.Length)]);
                            indexList.Add(j % (listOfPoints.Length));
                        }
                        localMin = listOfPoints[(j + 1) % (listOfPoints.Length)];
                        localMinInd = (j + 1) % (listOfPoints.Length);
                        consecLower = true;
                        j++;
                    }
                }
            }

            int check = 0;
            int orderCheck = 0;

            for (int i = 0; i < (localMaxList.Count + localMinList.Count); i++)
            {
                List<Point> templist = new List<Point>();
                int tempInd = indexList[i];

                Point tempPoint = listOfPoints[(indexList[(i + 1) % indexList.Count()] + 1) % (listOfPoints.Length)];
                Point actualPoint = listOfPoints[tempInd % (listOfPoints.Length)];

                templist.Add(actualPoint);
                tempInd++;

                actualPoint = listOfPoints[tempInd % (listOfPoints.Length)];

                while (!tempPoint.Equals(actualPoint))
                {
                    templist.Add(actualPoint);
                    tempInd++;
                    actualPoint = listOfPoints[tempInd % (listOfPoints.Length)];
                }
                outputBounds.Add(templist);

                if (i == 0)
                {
                    if (templist[0].X < templist[1].X)
                    {
                        rightBounds.Add(templist);
                        orderCheck = 0;
                        check++;
                    }
                    else
                    {
                        leftBounds.Add(templist);
                        orderCheck = 1;
                    }
                }
                else
                {
                    if (check % 2 == 0)
                    {
                        rightBounds.Add(templist);
                        check++;
                    }
                    else
                    {
                        leftBounds.Add(templist);
                        check++;
                    }
                }
            }

            if (orderCheck == 0)
            {
                leftBounds.Reverse();
            }
            else if (orderCheck == 1)
            {
                rightBounds.Reverse();
            }

            foreach (var bound in rightBounds)
            {

                if (bound[0].Y > bound[1].Y)
                {
                    bound.Reverse();
                }
            }

            foreach (var bound in leftBounds)
            {
                if (bound[0].Y > bound[1].Y)
                {
                    bound.Reverse();
                }
            }

            foreach (var element in rightBounds)
            {
                foreach (var point in element)
                {
                    if (localMinList.Contains(point))
                    {
                        point.Type = 1;
                        point.Contributing = true;
                    }
                    else
                        if (localMaxList.Contains(point))
                    {
                        point.Type = 4;
                    }
                    else
                    {
                        point.Type = 2;
                    }
                }
            }

            foreach (var element in leftBounds)
            {
                foreach (var point in element)
                {
                    if (localMinList.Contains(point))
                    {
                        point.Type = 1;
                        point.Contributing = true;
                    }
                    else if (localMaxList.Contains(point))
                    {
                        point.Type = 4;
                    }
                    else
                    {
                        point.Type = 3;
                    }
                }
            }
        }
    }
}
