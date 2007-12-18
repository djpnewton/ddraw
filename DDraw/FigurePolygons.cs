using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public class PolygonFigure : RectFigure
    {
        DPoints points;

        public PolygonFigure(DPoints pts)
        {
            System.Diagnostics.Debug.Assert(pts.Count >= 3, "You need at least 3 points for a polygon");
            points = pts;
            points.Add(pts[0]);
        }

        protected override DHitTest _HitTest(DPoint pt)
        {
            DPoints pts = RealPoints();
            if (DGeom.PointInPolygon(pt, pts) || DGeom.PointInPolyline(pt, pts, StrokeWidth / 2))
                return DHitTest.Body;
            else
                return DHitTest.None;
        }

        DPoint Vertex(DPoint pt)
        {
            return new DPoint(X + Width * pt.X, Y + Height * pt.Y);
        }

        DPoints RealPoints()
        {
            DPoints pts = new DPoints();
            foreach (DPoint pt in points)
                pts.Add(Vertex(pt));
            return pts;
        }

        protected override void PaintBody(DGraphics dg)
        {
            DPoints pts = RealPoints();
            dg.FillPolygon(pts, Fill, Alpha);
            dg.DrawPolyline(pts, Stroke, Alpha, StrokeWidth);
        }
    }

    public class TriangleFigure : PolygonFigure
    {
        public TriangleFigure(): 
            base (DPoints.FromArray(new DPoint[3] { new DPoint(0, 1), new DPoint(0.5, 0), new DPoint(1, 1) }))
        { }
    }

    public class RightAngleTriangleFigure : PolygonFigure
    {
        public RightAngleTriangleFigure():
            base(DPoints.FromArray(new DPoint[3] { new DPoint(0, 0), new DPoint(1, 1), new DPoint(0, 1) }))
        { }
    }

    public class DiamondFigure : PolygonFigure
    {
        public DiamondFigure(): 
            base(DPoints.FromArray(new DPoint[4] { new DPoint(0.5, 0), new DPoint(1, 0.5), new DPoint(0.5, 1), new DPoint(0, 0.5) }))
        { }
    }

    public class PentagonFigure : PolygonFigure
    {
        public PentagonFigure():
            base(DPoints.FromArray(new DPoint[5] { new DPoint(0.5, 0), new DPoint(1, 0.3), new DPoint(0.8, 1), new DPoint(0.2, 1), new DPoint(0, 0.3) }))
        { }
    }
}