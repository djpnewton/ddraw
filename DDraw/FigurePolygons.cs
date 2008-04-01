using System;
using System.Collections.Generic;
using System.Text;

namespace DDraw
{
    public class PolygonFigure : RectFigure
    {
        DPoints points;
        public DPoints Points
        {
            get { return points; }
            set { points = value; }
        }

        // default to triangle shape
        public PolygonFigure()
            : this(DPoints.FromArray(new DPoint[3] { new DPoint(0, 1), new DPoint(0.5, 0), new DPoint(1, 1) }))
        {
        }

        public PolygonFigure(DPoints pts)
        {
            System.Diagnostics.Debug.Assert(pts.Count >= 3, "You need at least 3 points for a polygon");
            points = pts;
        }

        protected override DHitTest BodyHitTest(DPoint pt)
        {
            DPoints pts = DrawPoints();
            if (DGeom.PointInPolygon(pt, pts) || DGeom.PointInPolyline(pt, pts, StrokeWidth / 2))
                return DHitTest.Body;
            else
                return DHitTest.None;
        }

        DPoint Vertex(DPoint pt)
        {
            return new DPoint(X + Width * pt.X, Y + Height * pt.Y);
        }

        DPoints DrawPoints()
        {
            DPoints pts = new DPoints();
            foreach (DPoint pt in points)
                pts.Add(Vertex(pt));
            // close the polygon
            pts.Add(pts[0]);
            // to draw the stroke join at the first vertex
            pts.Add(DGeom.PointFromAngle(pts[0], DGeom.AngleBetweenPoints(pts[0], pts[1]) - DGeom.HalfPi, 1)); 
            // return new points
            return pts;
        }

        protected override void PaintBody(DGraphics dg)
        {
            DPoints pts = DrawPoints();
            if (UseRealAlpha && Alpha != 1 && StrokeWidth > 0)
            {
                DBitmap bmp = GraphicsHelper.MakeBitmap(Width + StrokeWidth, Height + StrokeWidth);
                DGraphics bmpGfx = GraphicsHelper.MakeGraphics(bmp);
                bmpGfx.AntiAlias = dg.AntiAlias;
                bmpGfx.Translate(SwHalf - X, SwHalf - Y);
                bmpGfx.FillPolygon(pts, Fill, 1);
                bmpGfx.DrawPolyline(pts, Stroke, 1, StrokeWidth, StrokeStyle, StrokeJoin, StrokeCap);
                dg.DrawBitmap(bmp, new DPoint(X - SwHalf, Y - SwHalf), Alpha);
                bmpGfx.Dispose();
                bmp.Dispose();
            }
            else
            {
                dg.FillPolygon(pts, Fill, Alpha);
                dg.DrawPolyline(pts, Stroke, Alpha, StrokeWidth, StrokeStyle, StrokeJoin, StrokeCap);
            }
        }
    }

    public class TriangleFigure : PolygonFigure
    {
        public TriangleFigure(): 
            base ()
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