using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Resources;
using System.Globalization;

namespace Workbook
{
    public class WbLocale
    {
        static ResourceManager resourceMgr;
        static CultureInfo cultureInfo;

        public static void Init(CultureInfo ci)
        {
            resourceMgr = new ResourceManager("Workbook.Localization.strings", System.Reflection.Assembly.GetCallingAssembly());
            cultureInfo = ci;
        }

        static string GetString(string name)
        {
            //System.Diagnostics.Debug.Assert(resourceMgr != null, "ERROR: WbLocale has not been initialized");
            if (resourceMgr != null && cultureInfo != null)
            {
                string res = resourceMgr.GetString(name, cultureInfo);
                System.Diagnostics.Debug.Assert(res != null, string.Format("ERROR: Did not find resource string \"{0}\"", name));
                if (res != null)
                    return res;
            }
            return name;
        }

        public static string Hello
        {
            get { return GetString("Hello"); }
        }

        #region shared strings
        public static string Ok
        {
            get { return GetString("Ok"); }
        }

        public static string Cancel
        {
            get { return GetString("Cancel"); }
        }

        public static string ApplyToAllPages
        {
            get { return GetString("ApplyToAllPages"); }
        }

        public static string Image
        {
            get { return GetString("Image"); }
        }

        public static string Select
        {
            get { return GetString("Select"); }
        }

        public static string Font
        {
            get { return GetString("Font"); }
        }

        public static string ScreenAnnotate
        {
            get { return GetString("ScreenAnnotate"); }
        }

        public static string Delete
        {
            get { return GetString("Delete"); }
        }

        public static string ScreenCapture
        {
            get { return GetString("ScreenCapture"); }
        }

        public static string Undo
        {
            get { return GetString("Undo"); }
        }

        public static string File
        {
            get { return GetString("File"); }
        }

        public static string Insert
        {
            get { return GetString("Insert"); }
        }

        public static string Format
        {
            get { return GetString("Format"); }
        }

        public static string Attachment
        {
            get { return GetString("Attachment"); }
        }

        public static string About
        {
            get { return GetString("About"); }
        }

        public static string Page
        {
            get { return GetString("Page"); }
        }

        public static string Text
        {
            get { return GetString("Text"); }
        }

        public static string RemoveLink
        {
            get { return GetString("RemoveLink"); }
        }

        public static string DeleteFigures
        {
            get { return GetString("DeleteFigures"); }
        }  
        #endregion

        #region AboutBox strings
        public static string Version
        {
            get { return GetString("Version"); }
        }
        #endregion

        #region AnnoToolsForm strings
        public static string Mouse
        {
            get { return GetString("Mouse"); }
        }

        public static string ImportArea
        {
            get { return GetString("ImportArea"); }
        }

        public static string ImportPage
        {
            get { return GetString("ImportPage"); }
        }

        public static string NoAnnotationsImported
        {
            get { return GetString("NoAnnotationsImported"); }
        }

        public static string NoAnnotationsImportedMsg
        {
            get { return GetString("NoAnnotationsImportedMsg"); }
        }
        #endregion

        #region Attachment strings
        public static string FileName
        {
            get { return GetString("FileName"); }
        }

        public static string Size
        {
            get { return GetString("Size"); }
        }

        public static string AttachmentExecuteError
        {
            get { return GetString("AttachmentExecuteError"); }
        }
        #endregion

        #region BackgroundForm strings
        public static string SetBackground
        {
            get { return GetString("SetBackground"); }
        }

        public static string SolidColor
        {
            get { return GetString("SolidColor"); }
        }

        public static string Color
        {
            get { return GetString("Color"); }
        }

        public static string Browse
        {
            get { return GetString("Browse"); }
        }
        #endregion

        #region CustomPageSizeForm strings
        public static string PageSize
        {
            get { return GetString("PageSize"); }
        }

        public static string Default
        {
            get { return GetString("Default"); }
        }

        public static string Custom
        {
            get { return GetString("Custom"); }
        }

        public static string WidthMM
        {
            get { return GetString("WidthMM"); }
        }

        public static string HeightMM
        {
            get { return GetString("HeightMM"); }
        }
        #endregion

        #region DimensionsForm strings
        public static string FigureDimensions
        {
            get { return GetString("FigureDimensions"); }
        }

        public static string Left
        {
            get { return GetString("Left"); }
        }

        public static string Top
        {
            get { return GetString("Top"); }
        }

        public static string Width
        {
            get { return GetString("Width"); }
        }

        public static string Height
        {
            get { return GetString("Height"); }
        }

        public static string Angle
        {
            get { return GetString("Angle"); }
        }

        public static string UnifyLeft
        {
            get { return GetString("UnifyLeft"); }
        }

        public static string UnifyTop
        {
            get { return GetString("UnifyTop"); }
        }

        public static string UnifyWidth
        {
            get { return GetString("UnifyWidth"); }
        }

        public static string UnifyHeight
        {
            get { return GetString("UnifyHeight"); }
        }

        public static string UnifyAngle
        {
            get { return GetString("UnifyAngle"); }
        }

        public static string LockAspect
        {
            get { return GetString("LockAspect"); }
        }
        #endregion

        #region ExportForm strings

        public static string Export
        {
            get { return GetString("Export"); }
        }

        public static string Pages
        {
            get { return GetString("Pages"); }
        }

        public static string PDF
        {
            get { return GetString("PDF"); }
        }

        public static string Current
        {
            get { return GetString("Current"); }
        }

        public static string All
        {
            get { return GetString("All"); }
        }
        
        public static string ERROR
        {
            get { return GetString("ERROR"); }
        }        

        public static string NoPagesSelected
        {
            get { return GetString("NoPagesSelected"); }
        }

        public static string ExportingToImages
        {
            get { return GetString("ExportingToImages"); }
        }     
        #endregion

        #region LinkForm strings
        public static string FigureLink
        {
            get { return GetString("FigureLink"); }
        }

        public static string LinkType
        {
            get { return GetString("LinkType"); }
        }

        public static string LinkMethod
        {
            get { return GetString("LinkMethod"); }
        }

        public static string WebPage
        {
            get { return GetString("WebPage"); }
        }

        public static string FileOnThisComputer
        {
            get { return GetString("FileOnThisComputer"); }
        }

        public static string PageInThisDocument
        {
            get { return GetString("PageInThisDocument"); }
        }

        public static string LinkIcon
        {
            get { return GetString("LinkIcon"); }
        }

        public static string FigureBody
        {
            get { return GetString("FigureBody"); }
        }

        public static string First
        {
            get { return GetString("First"); }
        }

        public static string Last
        {
            get { return GetString("Last"); }
        }

        public static string Next
        {
            get { return GetString("Next"); }
        }

        public static string Previous
        {
            get { return GetString("Previous"); }
        }

        public static string Address
        {
            get { return GetString("Address"); }
        }

        public static string CopyFileToAttachments
        {
            get { return GetString("CopyFileToAttachments"); }
        }
        #endregion

        #region PropertiesForm strings
        public static string FigureProperties
        {
            get { return GetString("FigureProperties"); }
        }
        #endregion

        #region ScreenCaptureForm strings
        public static string CaptureRectArea
        {
            get { return GetString("CaptureRectArea"); }
        }

        public static string CaptureFullScreen
        {
            get { return GetString("CaptureFullScreen"); }
        }

        public static string CaptureWindow
        {
            get { return GetString("CaptureWindow"); }
        }
        #endregion

        #region ToolStripState strings
        public static string Pen
        {
            get { return GetString("Pen"); }
        }

        public static string Eraser
        {
            get { return GetString("Eraser"); }
        }

        public static string Rectangle
        {
            get { return GetString("Rectangle"); }
        }

        public static string Ellipse
        {
            get { return GetString("Ellipse"); }
        }

        public static string Clock
        {
            get { return GetString("Clock"); }
        }

        public static string Triangle
        {
            get { return GetString("Triangle"); }
        }

        public static string RightAngledTriangle
        {
            get { return GetString("RightAngledTriangle"); }
        }

        public static string Diamond
        {
            get { return GetString("Diamond"); }
        }

        public static string Pentagon
        {
            get { return GetString("Pentagon"); }
        }

        public static string Line
        {
            get { return GetString("Line"); }
        }

        public static string Fill
        {
            get { return GetString("Fill"); }
        }

        public static string Stroke
        {
            get { return GetString("Stroke"); }
        }

        public static string StrokeWidth
        {
            get { return GetString("StrokeWidth"); }
        }

        public static string StrokeStyle
        {
            get { return GetString("StrokeStyle"); }
        }

        public static string StartMarker
        {
            get { return GetString("StartMarker"); }
        }

        public static string EndMarker
        {
            get { return GetString("EndMarker"); }
        }

        public static string Opacity
        {
            get { return GetString("Opacity"); }
        }

        public static string Bold
        {
            get { return GetString("Bold"); }
        }

        public static string Italic
        {
            get { return GetString("Italic"); }
        }

        public static string Underline
        {
            get { return GetString("Underline"); }
        }

        public static string Strikethrough
        {
            get { return GetString("Strikethrough"); }
        }

        public static string ChangeProperty
        {
            get { return GetString("ChangeProperty"); }
        }
        #endregion

        #region MainForm strings
        public static string Edit
        {
            get { return GetString("Edit"); }
        }

        public static string View
        {
            get { return GetString("View"); }
        }

        public static string Tools
        {
            get { return GetString("Tools"); }
        }

        public static string RecentDocuments
        {
            get { return GetString("RecentDocuments"); }
        }

        public static string ImportNotebook
        {
            get { return GetString("ImportNotebook"); }
        }

        public static string SendTo
        {
            get { return GetString("SendTo"); }
        }

        public static string MailRecipient
        {
            get { return GetString("MailRecipient"); }
        }

        public static string MailRecipientPDF
        {
            get { return GetString("MailRecipientPDF"); }
        }

        public static string Exit
        {
            get { return GetString("Exit"); }
        }

        public static string Zoom
        {
            get { return GetString("Zoom"); }
        }

        public static string FitToPage
        {
            get { return GetString("FitToPage"); }
        }

        public static string FitToWidth
        {
            get { return GetString("FitToWidth"); }
        }

        public static string _050Percent
        {
            get { return GetString("_050Percent"); }
        }

        public static string _100Percent
        {
            get { return GetString("_100Percent"); }
        }

        public static string _150Percent
        {
            get { return GetString("_150Percent"); }
        }

        public static string Antialias
        {
            get { return GetString("Antialias"); }
        }

        public static string Toolbars
        {
            get { return GetString("Toolbars"); }
        }

        public static string Personal
        {
            get { return GetString("Personal"); }
        }

        public static string ModeSelect
        {
            get { return GetString("ModeSelect"); }
        }

        public static string PropertySelect
        {
            get { return GetString("PropertySelect"); }
        }

        public static string PageNavigation
        {
            get { return GetString("PageNavigation"); }
        }

        public static string Order
        {
            get { return GetString("Order"); }
        }

        public static string ResetToolbars
        {
            get { return GetString("ResetToolbars"); }
        }

        public static string FlipLeftRight
        {
            get { return GetString("FlipLeftRight"); }
        }

        public static string FlipUpDown
        {
            get { return GetString("FlipUpDown"); }
        }

        public static string Background
        {
            get { return GetString("Background"); }
        }

        public static string BringForward
        {
            get { return GetString("BringForward"); }
        }

        public static string BringToFront
        {
            get { return GetString("BringToFront"); }
        }

        public static string ClearPage
        {
            get { return GetString("ClearPage"); }
        }

        public static string ClonePage
        {
            get { return GetString("ClonePage"); }
        }

        public static string Copy
        {
            get { return GetString("Copy"); }
        }

        public static string Cut
        {
            get { return GetString("Cut"); }
        }

        public static string DeletePage
        {
            get { return GetString("DeletePage"); }
        }

        public static string Dimensions
        {
            get { return GetString("Dimensions"); }
        }

        public static string Group
        {
            get { return GetString("Group"); }
        }

        public static string Link
        {
            get { return GetString("Link"); }
        }

        public static string Lock
        {
            get { return GetString("Lock"); }
        }

        public static string New
        {
            get { return GetString("New"); }
        }

        public static string NewPage
        {
            get { return GetString("NewPage"); }
        }

        public static string Open
        {
            get { return GetString("Open"); }
        }

        public static string Paste
        {
            get { return GetString("Paste"); }
        }

        public static string Print
        {
            get { return GetString("Print"); }
        }

        public static string Properties
        {
            get { return GetString("Properties"); }
        }

        public static string Redo
        {
            get { return GetString("Redo"); }
        }

        public static string RenamePage
        {
            get { return GetString("RenamePage"); }
        }

        public static string Save
        {
            get { return GetString("Save"); }
        }

        public static string SaveAs
        {
            get { return GetString("SaveAs"); }
        }

        public static string SelectAll
        {
            get { return GetString("SelectAll"); }
        }

        public static string SendBackward
        {
            get { return GetString("SendBackward"); }
        }

        public static string SendToBack
        {
            get { return GetString("SendToBack"); }
        }

        public static string UnlockFigure
        {
            get { return GetString("UnlockFigure"); }
        }

        public static string WebLinkError
        {
            get { return GetString("WebLinkError"); }
        }

        public static string FileLinkError
        {
            get { return GetString("FileLinkError"); }
        }

        public static string CouldNotFindFile
        {
            get { return GetString("CouldNotFindFile"); }
        }

        public static string PageLinkError
        {
            get { return GetString("PageLinkError"); }
        }

        public static string DoesNotExist
        {
            get { return GetString("DoesNotExist"); }
        }

        public static string AttachmentLinkError
        {
            get { return GetString("AttachmentLinkError"); }
        }

        public static string Ungroup
        {
            get { return GetString("Ungroup"); }
        }

        public static string MovePage
        {
            get { return GetString("MovePage"); }
        }

        public static string DragFigureToNewPage
        {
            get { return GetString("DragFigureToNewPage"); }
        }

        public static string ChangePageName
        {
            get { return GetString("ChangePageName"); }
        }

        public static string AddImage
        {
            get { return GetString("AddImage"); }
        }

        public static string AddAttachment
        {
            get { return GetString("AddAttachment"); }
        }

        public static string TextEdit
        {
            get { return GetString("TextEdit"); }
        }

        public static string SetGlobalPageSize
        {
            get { return GetString("SetGlobalPageSize"); }
        }

        public static string ChangePageSize
        {
            get { return GetString("ChangePageSize"); }
        }

        public static string SetGlobalBackground
        {
            get { return GetString("SetGlobalBackground"); }
        }

        public static string Figures
        {
            get { return GetString("Figures"); }
        }

        public static string Bitmap
        {
            get { return GetString("Bitmap"); }
        }

        public static string AttachmentIsLinked
        {
            get { return GetString("AttachmentIsLinked"); }
        }

        public static string AttachmentIsLinkedMsg
        {
            get { return GetString("AttachmentIsLinkedMsg"); }
        }

        public static string DeleteAttachments
        {
            get { return GetString("DeleteAttachments"); }
        }

        public static string NewDocument
        {
            get { return GetString("NewDocument"); }
        }

        public static string OpeningFile
        {
            get { return GetString("OpeningFile"); }
        }

        public static string OpenDocument
        {
            get { return GetString("OpenDocument"); }
        }

        public static string ErrorReadingFile
        {
            get { return GetString("ErrorReadingFile"); }
        }

        public static string ErrorWritingFile
        {
            get { return GetString("ErrorWritingFile"); }
        }

        public static string SavingFile
        {
            get { return GetString("SavingFile"); }
        }

        public static string SaveChangesTo
        {
            get { return GetString("SaveChangesTo"); }
        }

        public static string ImportingScreenCapture
        {
            get { return GetString("ImportingScreenCapture"); }
        }

        public static string ImportScreenCapture
        {
            get { return GetString("ImportScreenCapture"); }
        }

        public static string ChangeLink
        {
            get { return GetString("ChangeLink"); }
        }

        public static string ChangeFigureLock
        {
            get { return GetString("ChangeFigureLock"); }
        }

        public static string ChangeFigureDimensions
        {
            get { return GetString("ChangeFigureDimensions"); }
        }

        public static string ChangeFigureProperties
        {
            get { return GetString("ChangeFigureProperties"); }
        }

        public static string ImportingScreenAsPage
        {
            get { return GetString("ImportingScreenAsPage"); }
        }

        public static string ImportAnnotations
        {
            get { return GetString("ImportAnnotations"); }
        }

        public static string ImportingAreaAsImage
        {
            get { return GetString("ImportingAreaAsImage"); }
        }

        public static string AddAttachments
        {
            get { return GetString("AddAttachments"); }
        }

        public static string ImportingNotebookFile
        {
            get { return GetString("ImportingNotebookFile"); }
        }

        public static string Emailing
        {
            get { return GetString("Emailing"); }
        }

        public static string ErrorMailingPDF
        {
            get { return GetString("ErrorMailingPDF"); }
        }
        #endregion

        #region DEngine strings
        public static string SelectOperation
        {
            get { return GetString("SelectOperation"); }
        }
        
        public static string AddLine
        {
            get { return GetString("AddLine"); }
        }
        
        public static string AddText
        {
            get { return GetString("AddText"); }
        }
        
        public static string Add
        {
            get { return GetString("Add"); }
        }
        
        public static string FigureEdit
        {
            get { return GetString("FigureEdit"); }
        }
        
        public static string EraseOperation
        {
            get { return GetString("EraseOperation"); }
        }
        
        public static string Move
        {
            get { return GetString("Move"); }
        }
        #endregion
    }
}
