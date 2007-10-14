using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    static class DGeom
    {
        public static DPoint RotatePoint(DPoint pt, DPoint origin, double angle)
        {
            if (angle == 0)
                return pt;
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

        public static bool PointInPolyline(DPoint pt, DPoints polyline, double width)
        {
            DPoint p;
            for (int i = 1; i < polyline.Count; i++)
                if (DistBetweenPtAndLine(pt, polyline[i - 1], polyline[i], out p) <= width)
                    return true;
            return false;
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
    }
}
