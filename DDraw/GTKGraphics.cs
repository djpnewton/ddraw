using System;
using System.IO;
using Cairo;

namespace DDraw.GTK
{
	public class GTKBitmap : DBitmap
	{
	
	    ImageSurface surface
        {
            get { return (ImageSurface)nativeBmp; }
        }

		public GTKBitmap(int width, int height): base(width, height)
		{}
		
		protected override object MakeBitmap (int width, int height)
		{
			return new Cairo.ImageSurface(Format.ARGB32, width, height);
		}
		
		ImageSurface PixBufToImageSurface(Gdk.Pixbuf pb)
		{
		    Cairo.Format format = Format.A8;
		    if (pb.HasAlpha)
		    	format = Format.ARGB32;
		    int width = pb.Width;
    		int height = pb.Height;
    		ImageSurface image = new ImageSurface(format, width, height);
    		Cairo.Context cr = new Cairo.Context(image);
    		    		
    		Gdk.CairoHelper.SetSourcePixbuf(cr, pb, 0, 0);
    		cr.Paint();
    		
    		return image;
		}
		
		protected override object LoadBitmap (Stream s)
		{
			return PixBufToImageSurface(new Gdk.Pixbuf(s));
		}

		protected override object LoadBitmap (string filename)
		{
			return PixBufToImageSurface(new Gdk.Pixbuf(filename));
		}
		
		protected override void DisposeBitmap ()
		{
			surface.Destroy();
		}
		
        public override int Width
        {
            get { return surface.Width; }
        }

        public override int Height
        {
            get { return surface.Height; }
        } 
		
		public override void Save (string filename)
		{
			surface.WriteToPng(filename);
		}
	}
	
	public class GTKTextExtent : DTextExtent
	{
		Context cr;
		
		public GTKTextExtent()
		{
			// TODO: make this better???
			cr = new Context(new ImageSurface(Format.ARGB32, 10, 10));			
		}
		
	    public override DPoint MeasureText(string text, string fontName, double fontSize)
        {
        	cr.SelectFontFace(fontName, FontSlant.Normal, FontWeight.Normal);
        	cr.SetFontSize(fontSize);
        	TextExtents te = cr.TextExtents(text);
        	return new DPoint(te.Width, te.Height);
        }
	}
	
	public class GTKGraphics : DGraphics
	{
		Context cr;
		
		public GTKGraphics(Context cr)
		{
			this.cr = cr;
		}
		
		public GTKGraphics(DBitmap bmp)
		{
			cr = new Context((ImageSurface)bmp.NativeBmp);
		}
		
		// Helper Functions //
		
		Color MakeColor(DColor color)
		{
			return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
		}
		
		Color MakeColor(DColor color, double alpha)
		{
			return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f * alpha);
		}
		
		void CairoPenStyle(Context cr, DPenStyle penStyle)
		{
			switch (penStyle)
			{
				case DPenStyle.Solid:
					cr.SetDash(new double[] {}, 0);
					break;
				case DPenStyle.Dash:
					cr.SetDash(new double[] {2, 1}, 0);
					break;
				case DPenStyle.Dot:
					cr.SetDash(new double[] {1, 1}, 0);
					break;
			}
		}
		
		void CairoEllipse(Context cr, double x, double y, double width, double height)
		{
			cr.Save();
			cr.Translate(x + width / 2f, y + height / 2f);
			cr.Scale(width / 2f, height / 2f);
			cr.Arc(0, 0, 1, 0, 2 * Math.PI);
			cr.Restore();
		}
		
		DMatrix MakeMatrix(Matrix matrix)
		{
	       	return new DMatrix(matrix.Xx, matrix.Yx, matrix.Xy, matrix.Yy, matrix.X0, matrix.Y0); 
		}
		
		Matrix MakeMatrix(DMatrix matrix)
		{
			return new Matrix(matrix.A, matrix.B, matrix.C, matrix.D, matrix.E, matrix.F);

		}
		       
		// Drawing Functions //

        public override void FillRect(double x, double y, double width, double height, DColor color, double alpha)
        {
        	cr.Color = MakeColor(color, alpha);
        	cr.Rectangle(x, y, width, height);
        	cr.Fill();
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth, DPenStyle penStyle)
        {
        	cr.Color = MakeColor(color, alpha);
        	cr.LineWidth = strokeWidth;
        	CairoPenStyle(cr, penStyle);
        	cr.Rectangle(x, y, width, height);
        	cr.Stroke();
        }
        
        public override void DrawRect(double x, double y, double width, double height, DColor color)
        {
        	DrawRect(x, y, width, height, color, 1, 1);
        }

        public override void DrawRect(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
        	DrawRect(x, y, width, height, color, alpha, strokeWidth, DPenStyle.Solid);
        }

        public override void DrawRect(DRect rect, DColor color)
        {
        	DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, 1, 1);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha)
        {
        	DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1);
        }

        public override void DrawRect(DRect rect, DColor color, double alpha, DPenStyle penStyle)
        {
            cr.Color = MakeColor(color, alpha);
        	cr.LineWidth = 1;
        	CairoPenStyle(cr, penStyle);
        	cr.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        	cr.Stroke();
        }
        
        public override void FillEllipse(double x, double y, double width, double height, DColor color)
        {
        	FillEllipse(x, y, width, height, color, 1);
        }

        public override void FillEllipse(double x, double y, double width, double height, DColor color, double alpha)
        {
            cr.Color = MakeColor(color, alpha);
        	cr.LineWidth = 1;
        	CairoPenStyle(cr, DPenStyle.Solid);
        	CairoEllipse(cr, x, y, width, height);
        	cr.Fill();
        }

        public override void FillEllipse(DRect rect, DColor color)
        {
        	FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, 1);
        }

        public override void FillEllipse(DRect rect, DColor color, double alpha)
        {
        	FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha);
        }
        
        public override void DrawEllipse(double x, double y, double width, double height, DColor color)
        {
        	DrawEllipse(x, y, width, height, color, 1, 1);
        }

        public override void DrawEllipse(double x, double y, double width, double height, DColor color, double alpha, double strokeWidth)
        {
            cr.Color = MakeColor(color, alpha);
        	cr.LineWidth = strokeWidth;
        	CairoPenStyle(cr, DPenStyle.Solid);
        	CairoEllipse(cr, x, y, width, height);
        	cr.Stroke();
        }

        public override void DrawEllipse(DRect rect, DColor color)
        {
        	DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, 1, 1);
        }

        public override void DrawEllipse(DRect rect, DColor color, double alpha)
        {
        	DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color, alpha, 1);
        }
        
        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color)
        {
        	DrawLine(pt1, pt2, color, 1, DPenStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha)
        {
        	DrawLine(pt1, pt2, color, alpha, DPenStyle.Solid);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, DPenStyle penStyle)
        {
        	DrawLine(pt1, pt2, color, 1, penStyle);
        }

        public override void DrawLine(DPoint pt1, DPoint pt2, DColor color, double alpha, DPenStyle penStyle)
        {
            cr.Color = MakeColor(color, alpha);
        	cr.LineWidth = 1;
        	CairoPenStyle(cr, penStyle);
        	cr.MoveTo(pt1.X, pt1.Y);
        	cr.LineTo(pt2.X, pt2.Y);
        	cr.Stroke();
        }
        
        public override void DrawPolyline(DPoints pts, DColor color)
        {
            DrawPolyline(pts, color, 1, 1);
        }

        public override void DrawPolyline(DPoints pts, DColor color, double alpha, double strokeWidth)
        {
            if (pts.Count > 1)
            {
            	cr.Color = MakeColor(color, alpha);
	        	cr.LineWidth = 1;
	        	cr.LineJoin = LineJoin.Round;
	        	cr.MoveTo(pts[0].X, pts[0].Y);
	        	for (int i = 1; i < pts.Count; i++)
	        		cr.LineTo(pts[i].X, pts[i].Y);
	        	cr.Stroke();
            }
        }
        
        public override void DrawBitmap(DBitmap bitmap, DPoint pt)
        {
        	cr.SetSource((ImageSurface)bitmap.NativeBmp, pt.X, pt.Y);
        	cr.Paint();
        }

        public override void DrawBitmap(DBitmap bitmap, DRect rect)
        {
        	DrawBitmap(bitmap, rect, 1);
        }
        
        public override void DrawBitmap(DBitmap bitmap, DRect rect, double alpha)
        {
        	ImageSurface surf = (ImageSurface)bitmap.NativeBmp;
        	cr.Save();
        	cr.Translate(rect.X, rect.Y);
        	cr.Scale(surf.Width / rect.Width, surf.Height / rect.Height);
        	cr.SetSource(surf, 0, 0);
        	cr.PaintWithAlpha(alpha);
        	cr.Restore();
        }
        
        public override void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color)
        {
        	DrawText(text, fontName, fontSize, rect, color, 1);
        }

        public override void DrawText(string text, string fontName, double fontSize, DRect rect, DColor color, double alpha)
        {
        	cr.SelectFontFace(fontName, FontSlant.Normal, FontWeight.Normal);
        	cr.SetFontSize(fontSize);
        	cr.Color = MakeColor(color, alpha);
			TextExtents te = cr.TextExtents(text);
			cr.MoveTo(rect.X - te.XBearing, rect.Y - te.YBearing);
        	cr.ShowText(text);        	
        }
        
        public override DMatrix SaveTransform()
        {
        	return MakeMatrix(cr.Matrix);
        }

        public override void LoadTransform(DMatrix matrix)
        {
            cr.Matrix = MakeMatrix(matrix);
        }
        
        public override void Scale(double sx, double sy)
        {
        	cr.Scale(sx, sy);
        }

        public override void Rotate(double angle, DPoint center)
        {
            cr.Translate(center.X, center.Y);
            cr.Rotate(angle);
            cr.Translate(-center.X, -center.Y);
        }

        public override void Translate(DPoint offset)
        {
            cr.Translate(offset.X, offset.Y);
        }

        public override void ResetTransform()
        {
        	cr.Matrix = new Matrix(1, 0, 0, 0, 1, 0); // identity matrix
        }
        
        public override DCompositingMode CompositingMode
        {
            get
            {
            	if (cr.Operator == Operator.Source)
            		return DCompositingMode.SourceCopy;
            	else
            		return DCompositingMode.SourceOver;
            }
            set
            {
                if (value == DCompositingMode.SourceOver)
                    cr.Operator = Operator.Over;
                else
                    cr.Operator = Operator.Source;
            }
        }

        public override bool AntiAlias
        {
            get { return cr.Antialias == Cairo.Antialias.Subpixel; }
            set
            {
                if (value)
                    cr.Antialias = Cairo.Antialias.Subpixel;
                else
                    cr.Antialias = Cairo.Antialias.None;
            }
        }
	}
}
