using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public static class DGeom
    {
        public const double HalfPi = Math.PI / 2;
        public const double QuartPi = Math.PI / 4;
        public const double OctPi = Math.PI / 8;

        public static DPoint RotatePoint(DPoint pt, DPoint origin, double angle)
        {
            if (angle == 0 || (pt.X == origin.X && pt.Y == origin.Y))
                return new DPoint(pt.X, pt.Y);
            // set to origin
            pt = pt.Offset(-origin.X, -origin.Y);
            // rotate point
            DPoint rotatedPt = new DPoint(pt.X * Math.Cos(angle) - pt.Y * Math.Sin(angle),
                                          pt.X * Math.Sin(angle) + pt.Y * Math.Cos(angle));
            // unset from origin
            rotatedPt = rotatedPt.Offset(origin.X, origin.Y);
            return rotatedPt;
        }

        public static double AngleBetweenPoints(DPoint p1, DPoint p2)
        {
            return -(Math.Atan2(p2.X - p1.X, p2.Y - p1.Y) - Math.PI);
        }

        public static DPoint PointFromAngle(DPoint origin, double angle, double r)
        {
            return new DPoint(origin.X + r * Math.Cos(angle), origin.Y + r * Math.Sin(angle));
        }

        public static double DistBetweenTwoPts(DPoint p1, DPoint p2)
        {
            /* Calculate the distance between to points */
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public static double DistBetweenPtAndLine(DPoint pt, DPoint linep1, DPoint linep2, out DPoint linept)
        {
            /* Calculate the distance between a point and a line */

            // below is based from http://www.allegro.cc/forums/thread/589720/644831#target

            double A = pt.X - linep1.X;
            double B = pt.Y - linep1.Y;
            double C = linep2.X - linep1.X;
            double D = linep2.Y - linep1.Y;

            double dot = A * C + B * D;
            double len_sq = C * C + D * D;
            double param = dot / len_sq;

            if (param < 0)
                linept = new DPoint(linep1.X, linep1.Y);
            else if (param > 1)
                linept = new DPoint(linep2.X, linep2.Y);
            else
                linept = new DPoint(linep1.X + param * C, linep1.Y + param * D);

            return DistBetweenTwoPts(linept, pt);
        }

        public static double DistXBetweenRects(DRect r1, DRect r2)
        {
            if (r1.Right < r2.Left)
                return r2.Left - r1.Right;
            else if (r1.Left > r2.Right)
                return r1.Left - r2.Right;
            else
                return 0;
        }

        public static double DistYBetweenRects(DRect r1, DRect r2)
        {
            if (r1.Bottom < r2.Top)
                return r2.Top - r1.Bottom;
            else if (r1.Top > r2.Bottom)
                return r1.Top - r2.Bottom;
            else
                return 0;
        }

        public static bool PointInRect(DPoint pt, DRect rect)
        {
            return (pt.X >= rect.X && pt.X <= rect.Right && pt.Y >= rect.Y && pt.Y <= rect.Bottom);
        }

        public static bool PointInEllipse(DPoint pt, DRect rect)
        {
            // see http://www.elists.org/pipermail/delphi-talk/2002-March/014690.html
            double Xo = rect.X + rect.Width / 2;
            double Yo = rect.Y + rect.Height / 2;
            double a = rect.Width / 2;
            double b = rect.Height / 2;
            double res = Math.Pow(pt.X - Xo, 2) / Math.Pow(a, 2) + Math.Pow(pt.Y - Yo, 2) / Math.Pow(b, 2);
            return res <= 1;
        }

        public static bool PointInCircle(DPoint pt, DPoint circleCenter, double radius)
        {
            return Math.Sqrt(Math.Pow(circleCenter.X - pt.X, 2) + Math.Pow(circleCenter.Y - pt.Y, 2)) <= radius;
        }

        public static bool PointInLine(DPoint pt, DPoint linePt1, DPoint linePt2, double width)
        {
            DPoint p;
            return DistBetweenPtAndLine(pt, linePt1, linePt2, out p) <= width;
        }

        public static bool PointInPolyline(DPoint pt, DPoints polyline, double width)
        {
            for (int i = 1; i < polyline.Count; i++)
                if (PointInLine(pt, polyline[i - 1], polyline[i], width))
                    return true;
            return false;
        }

        public static bool PointInPolygon(DPoint pt, DPoints polygon)
        {
            // see http://alienryderflex.com/polygon/
            int i, j = polygon.Count - 1;
            bool oddNodes = false;
            for (i = 0; i < polygon.Count; i++)
            {
                if (polygon[i].Y < pt.Y && polygon[j].Y >= pt.Y || polygon[j].Y < pt.Y && polygon[i].Y >= pt.Y)
                {
                    if (polygon[i].X + (pt.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < pt.X)
                        oddNodes = !oddNodes;
                }
                j = i;
            }
            return oddNodes;
        }

        public static DPoint IntersectionOfTwoLines(double m1, DPoint p1, double m2, DPoint p2)
        {
            // y - y0 = m(x - x0)
            double c1 = p1.Y - (m1 * p1.X);
            double c2 = p2.Y - (m2 * p2.X);
            double x = (c2 - c1) / (m1 - m2);
            double y = m1 * x + c1;
            return new DPoint(x, y);
        }

        public static DRect BoundingBoxOfRotatedRect(DRect rect, double angle, DPoint origin)
        {
            if (angle == 0)
                return rect;
            DPoint p1 = RotatePoint(rect.TopLeft, origin, angle);
            DPoint p2 = RotatePoint(rect.TopRight, origin, angle);
            DPoint p3 = RotatePoint(rect.BottomLeft, origin, angle);
            DPoint p4 = RotatePoint(rect.BottomRight, origin, angle);
            double x = Math.Min(Math.Min(p1.X, p2.X), Math.Min(p3.X, p4.X));
            double y = Math.Min(Math.Min(p1.Y, p2.Y), Math.Min(p3.Y, p4.Y));
            double right = Math.Max(Math.Max(p1.X, p2.X), Math.Max(p3.X, p4.X));
            double bottom = Math.Max(Math.Max(p1.Y, p2.Y), Math.Max(p3.Y, p4.Y));
            return new DRect(x, y, right - x, bottom - y);
        }

        public static DRect BoundingBoxOfRotatedRect(DRect rect, double angle)
        {
            return BoundingBoxOfRotatedRect(rect, angle, rect.Center);
        }

        static DPoint CalcPositionDeltaFromAngleAndCoordDelta(double angle, double dX, double dY)
        {
            // x/y modification for rotation
            if (angle == 0 || (dX == 0 && dY == 0))
                return new DPoint(0, 0);
            angle = angle / 2;
            double sintheta = Math.Sin(angle);
            double costheta = Math.Cos(angle);
            double r1 = sintheta * -dX;
            double r2 = sintheta * -dY;
            double modx = r1 * sintheta + r2 * costheta;
            double mody = -r1 * costheta + r2 * sintheta;
            return new DPoint(modx, mody);
        }

        public static void UpdateRotationPosition(Figure f, DRect oldRect, DRect newRect)
        {
            // update position of the figure depending on the change in rect and its rotation
            if (f.Rotation != 0)
            {
                DPoint dPt = CalcPositionDeltaFromAngleAndCoordDelta(f.Rotation, newRect.Right - oldRect.Right, newRect.Bottom - oldRect.Bottom);
                f.X += dPt.X;
                f.Y += dPt.Y;
                dPt = CalcPositionDeltaFromAngleAndCoordDelta(f.Rotation, newRect.Left - oldRect.Left, newRect.Top - oldRect.Top);
                f.X += dPt.X;
                f.Y += dPt.Y;
            }
        }

        public static DPoints SimplifyPolyline(DPoints original, double tolerance)
        {
            // using the Douglas-Peucker polyline simplification algorithm
            // see: http://geometryalgorithms.com/Archive/algorithm_0205/
            //      http://www.simdesign.nl/Components/DouglasPeucker.html

            if (original.Count <= 2)
                return original;
            // marker array
            bool[] markers = new bool[original.Count];
            // include first and last point
            markers[0] = true;
            markers[original.Count - 1] = true;
            // exclude intermediate points for now
            for (int i = 1; i < original.Count - 2; i++)
                markers[i] = false;
            // Simplify
            SimplifyDP(original, tolerance, 0, original.Count - 1, markers);
            // return simplified polyline
            DPoints res = new DPoints();
            for (int i = 0; i < original.Count; i++)
                if (markers[i])
                    res.Add(original[i]);
            return res;
        }

        static DPoint VecSubtract(DPoint a, DPoint b)
        {
            // return A - B
            return new DPoint(a.X - b.X, a.Y - b.Y);
        }

        static double DotProduct(DPoint a, DPoint b)
        {
            // return A * B
            return a.X * b.X + a.Y * b.Y;
        }

        static double NormSquared(DPoint a)
        {
            // Square of the norm |A|
            return a.X * a.X + a.Y * a.Y;
        }

        static double DistSquared(DPoint a, DPoint b)
        {
            // Square of the distance from A to B
            return NormSquared(VecSubtract(a, b));
        }

        static void SimplifyDP(DPoints original, double tolerance, int j, int k, bool[] markers)
        {
            //  The Douglas-Peucker recursive simplification routine.
            //  Mark each point that will be part of the simplified polyline.

            // check if there is anything to simplify
            if (k <= j + 1)
                return;

            DPoint p1 = original[j];
            DPoint p2 = original[k];
            DPoint u = VecSubtract(p2, p1); // segment direction vector
            double cu = DotProduct(u, u);   // segment length squared
            double maxD2 = 0;               // maximum value squared
            int maxI = 0;                   // index at maximum value
            double tol2 = tolerance * tolerance; // tolerance squared

            // test each vertex for max distance from segment (p1, p2)
            DPoint w;
            DPoint Pb = new DPoint(0, 0); // base of perpendicular from v[i] to S
            double b, cw, dv2;            // dv2 = distance original[i] to S squared

            // Loop through points and detect the one furthest away
            for (int i = j + 1; i < k; i++)
            {
                w = VecSubtract(original[i], p1);
                cw = DotProduct(w, u);

                // Distance of point original[i] from segment
                if (cw <= 0)
                    // Before segment
                    dv2 = DistSquared(original[i], p1);
                else
                {
                    if (cw > cu)
                        // Past segment
                        dv2 = DistSquared(original[i], p2);
                    else
                    {
                        // Fraction of the segment
                        try
                        {
                            b = cw / cu;
                        }
                        catch
                        {
                            b = 0; // in case CU = 0
                        }
                        Pb.X = Math.Round(p1.X + b * u.X);
                        Pb.Y = Math.Round(p1.Y + b * u.Y);
                        dv2 = DistSquared(original[i], Pb);
                    }
                }

                // test with current max distance squared
                if (dv2 > maxD2)
                {
                    // original[i] is a new max vertex
                    maxI = i;
                    maxD2 = dv2;
                }
            }

            // If the furthest point is outside tolerance we must split
            if (maxD2 > tol2)  // error is worse than the tolerance
            {
                // split the polyline at the farthest vertex from S
                markers[maxI] = true;  // mark original[maxI] for the simplified polyline

                // recursively simplify the two subpolylines at original[maxI]
                SimplifyDP(original, tolerance, j, maxI, markers); // polyline original[j] to original[maxI]
                SimplifyDP(original, tolerance, maxI, k, markers); // polyline original[maxI] to original[k]
            }
        }
    }
}
