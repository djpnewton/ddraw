using System;
using System.Collections;
using System.Collections.Generic;

namespace DDraw
{
	struct FigureProperties
	{
		public Figure Figure;
		public int ListPosition;
		public DRect Rect;
		public double Rotation;
		public DColor Fill;
		public DColor Stroke;
		public double StrokeWidth;
		public double Alpha;
		public DBitmap Bitmap;
		public string Text;
		public string FontName;
		public double FontSize;
        public FigureProperties[] ChildFigureProps;
	}
	
	enum FigureChangeType { Removed, Added, PropertyChanged, Moved };		
	
	struct FigureChange
	{
		public Figure Figure;
		public FigureChangeType Type;
		public FigureProperties FigProps;
		
		public FigureChange(Figure figure, FigureChangeType figureChangeType,
			FigureProperties figProps)
		{
			Figure = figure;
			Type = figureChangeType;
			FigProps = figProps;
		}
	}
		
	class UndoFrame
	{
		string name;
		public string Name
		{
			get { return name; }
		}
		
		List<FigureChange> figureChanges;
        public List<FigureChange> FigureChanges
        {
            get { return figureChanges; }
        }
		
		public UndoFrame(string name, List<FigureChange> figureChanges)
		{
			this.name = name;
			this.figureChanges = figureChanges;
		}
	}
	
	public delegate void UndoRedoChangedDelegate(bool commitAction);
	
	public class UndoRedoManager
	{
		string undoName;
		Stack<UndoFrame> undos = new Stack<UndoFrame>();
		Stack<UndoFrame> redos = new Stack<UndoFrame>();
		List<Figure> figures;
		List<FigureProperties> figureProps = new List<FigureProperties>();
		
		public event UndoRedoChangedDelegate UndoRedoChanged;
		
		public bool CanUndo
		{
			get { return undos.Count > 0; }
		}		
		public string UndoName
		{
			get { return undos.Peek().Name; }
		}
		public bool CanRedo
		{
			get { return redos.Count > 0; }
		}
		public string RedoName
		{
			get { return redos.Peek().Name; }
		}
		
		public UndoRedoManager(List<Figure> figures)
		{
			this.figures = figures;
		}
		
		FigureProperties CreateFigureProps(Figure f, int listPosition)
		{
            FigureProperties fp = new FigureProperties();
			fp.Figure = f;
			fp.ListPosition = listPosition;
			fp.Rect = f.Rect;
			fp.Rotation = f.Rotation;
			if (f is IFillable)
				fp.Fill = ((IFillable)f).Fill;
			if (f is IStrokeable)
			{
				fp.Stroke = ((IStrokeable)f).Stroke;
				fp.StrokeWidth = ((IStrokeable)f).StrokeWidth;
			}
			if (f is IAlphaBlendable)
				fp.Alpha = ((IAlphaBlendable)f).Alpha;
			if (f is IBitmapable)
				fp.Bitmap = ((IBitmapable)f).Bitmap;
			if (f is ITextable)
			{
				fp.Text = ((ITextable)f).Text;
				fp.FontName = ((ITextable)f).FontName;
				fp.FontSize = ((ITextable)f).FontSize;
			}
            if (f is IChildFigureable)
            {
                IChildFigureable icf = (IChildFigureable)f;
                FigureProperties[] childFigProps = new FigureProperties[icf.ChildFigures.Length];
                for (int i = 0; i < icf.ChildFigures.Length; i++)
                    childFigProps[i] = CreateFigureProps(icf.ChildFigures[i], i);
                fp.ChildFigureProps = childFigProps;
            }
            return fp;			
		}
		
		public void Start(string name)
		{
			undoName = name;
			// store current figures state
			figureProps.Clear();
			int i = 0;
			foreach (Figure f in figures)
			{
	            figureProps.Add(CreateFigureProps(f, i));
				i += 1;				
			}
		}
        
        bool ContainsFigure(List<FigureProperties> figureProps, Figure f)
        {
            foreach (FigureProperties fp in figureProps)
                if (fp.Figure == f)
                    return true;
            return false;
        }
        
        bool FigureMatchesProps(FigureProperties fp)
        {
            Figure f = fp.Figure;
            
            if (!fp.Rect.Equals(f.Rect))
                return false;
            if (fp.Rotation != f.Rotation)
                return false;
            if (f is IFillable)
            {
                if (!fp.Fill.Equals(((IFillable)f).Fill))
                    return false;
            }
            if (f is IStrokeable)
            {
                if (!fp.Stroke.Equals(((IStrokeable)f).Stroke))
                    return false;
                if (fp.StrokeWidth != ((IStrokeable)f).StrokeWidth)
                    return false;
            }
            if (f is IAlphaBlendable)
            {
                if (fp.Alpha != ((IAlphaBlendable)f).Alpha)
                    return false;
            }
            if (f is IBitmapable)
            {
                if (!fp.Bitmap.Equals(((IBitmapable)f).Bitmap))
                    return false;
            }
            if (f is ITextable)
            {
                if (fp.Text != ((ITextable)f).Text)
                    return false;
                if (fp.FontName != ((ITextable)f).FontName)
                    return false;
                if (fp.FontSize != ((ITextable)f).FontSize)
                    return false;
            }
            if (f is IChildFigureable)
            {
                foreach (FigureProperties cfp in fp.ChildFigureProps)
                    if (!FigureMatchesProps(cfp))
                        return false;
            }
            return true;               
        }

        void ApplyFigureProps(Figure f, FigureProperties fp)
        {
            f.X = fp.Rect.X;
            f.Y = fp.Rect.Y;
            f.Width = fp.Rect.Width;
            f.Height = fp.Rect.Height;
            f.Rotation = fp.Rotation;
            if (f is IFillable)
                ((IFillable)f).Fill = fp.Fill;
            if (f is IStrokeable)
            {
                ((IStrokeable)f).Stroke = fp.Stroke;
                ((IStrokeable)f).StrokeWidth = fp.StrokeWidth;
            }
            if (f is IAlphaBlendable)
                ((IAlphaBlendable)f).Alpha = fp.Alpha;
            if (f is IBitmapable)
                ((IBitmapable)f).Bitmap = fp.Bitmap;
            if (f is ITextable)
            {
                ((ITextable)f).Text = fp.Text;
                ((ITextable)f).FontName = fp.FontName;
                ((ITextable)f).FontSize = fp.FontSize;
            }
            if (f is IChildFigureable)
            {
                foreach (FigureProperties cfp in fp.ChildFigureProps)
                    ApplyFigureProps(cfp.Figure, cfp);
            }
        }

        UndoFrame Snapshot(string name)
        {
            // record all the figures state
            List<FigureChange> fcs = new List<FigureChange>();
            if (figureProps.Count != figures.Count)
            {
                // find all removed figures
                foreach (FigureProperties fp in figureProps)
                {
                    if (!figures.Contains(fp.Figure))
                        fcs.Add(new FigureChange(fp.Figure, FigureChangeType.Removed, fp));
                }
                // find add added figures
                int i = 0;
                foreach (Figure f in figures)
                {
                    if (!ContainsFigure(figureProps, f))
                        fcs.Add(new FigureChange(f, FigureChangeType.Added, CreateFigureProps(f, i)));
                    i += 1;
                }
            }
            else
            {
                // find all figures with changed listposition
                foreach (FigureProperties fp in figureProps)
                    if (fp.Figure != figures[fp.ListPosition])
                        fcs.Add(new FigureChange(fp.Figure, FigureChangeType.Moved, fp));
                // find all changed figures
                foreach (FigureProperties fp in figureProps)
                    if (!FigureMatchesProps(fp))
                        fcs.Add(new FigureChange(fp.Figure, FigureChangeType.PropertyChanged, fp));
            }
            return new UndoFrame(name, fcs);
        }
		
		public void Commit()
		{
            UndoFrame uf = Snapshot(undoName);
            if (uf.FigureChanges.Count > 0)
            {
                // push figure state onto undo stack
                undos.Push(uf);
                redos.Clear();
                DoUndoRedoChanged(true);
            }
		}

        void ApplyUndoFrame(UndoFrame uf)
        {
            // apply undo frame changes
            foreach (FigureChange fc in uf.FigureChanges)
                switch (fc.Type)
                {
                    case FigureChangeType.Added:
                        figures.Remove(fc.Figure);
                        break;
                    case FigureChangeType.Removed:
                        figures.Insert(fc.FigProps.ListPosition, fc.Figure);
                        ApplyFigureProps(fc.Figure, fc.FigProps);
                        break;
                    case FigureChangeType.PropertyChanged:
                        ApplyFigureProps(fc.Figure, fc.FigProps);
                        break;
                    case FigureChangeType.Moved:
                        figures.Remove(fc.Figure);
                        figures.Insert(fc.FigProps.ListPosition, fc.Figure);
                        break;
                }
        }

        UndoFrame Invert(UndoFrame uf)
        {
            List<FigureChange> fcs = new List<FigureChange>();
            foreach (FigureChange fc in uf.FigureChanges)
                fcs.Add(Invert(fc));
            return new UndoFrame(uf.Name, fcs);
        }

        FigureChange Invert(FigureChange fc)
        {
            switch (fc.Type)
            {
                case FigureChangeType.Added:
                    fc.Type = FigureChangeType.Removed;
                    return fc;
                case FigureChangeType.Removed:
                    fc.Type = FigureChangeType.Added;
                    return fc;
                case FigureChangeType.Moved:
                    fc.FigProps.ListPosition = figures.IndexOf(fc.Figure);
                    return fc;
                default:
                    FigureChange res = new FigureChange();
                    res.Figure = fc.Figure;
                    res.Type = fc.Type;
                    res.FigProps = CreateFigureProps(fc.Figure, fc.FigProps.ListPosition);
                    return res;
            }
        }
		
		public void Undo()
		{
			if (undos.Count > 0)
			{
				UndoFrame uf = undos.Pop();
                redos.Push(Invert(uf));
                ApplyUndoFrame(uf);
                DoUndoRedoChanged(false);
			}
		}
		
		public void Redo()
		{
			if (redos.Count > 0)
			{
				UndoFrame uf = redos.Pop();
                undos.Push(Invert(uf));
                ApplyUndoFrame(uf);
                DoUndoRedoChanged(false);
			}
		}	
		
		void DoUndoRedoChanged(bool commitAction)
		{
			if (UndoRedoChanged != null)
				UndoRedoChanged(commitAction);
		}
	}
}
