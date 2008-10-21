using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using DDraw;
using Nini.Config;
using Workbook.PersonalToolbar;

namespace Workbook
{
    public enum SidebarSide { Right, Left };

    public class ProgramOptions
    {
        const string _INIFILE = "Workbook.ini";
        const string MAIN_SECTION = "Main";
        const string FORMRECT_OPT = "FormRect";
        const string FORMWINDOWSTATE_OPT = "FormWindowState";
        const string SIDEBARSIDE_OPT = "SidebarSide";
        const string SIDEBARWIDTH_OPT = "SidebarWidth";
        const string ZOOM_OPT = "Zoom";
        const string SCALE_OPT = "Scale";
        const string ANTIALIAS_OPT = "AntiAlias";
        const string EDITTOOLBAR_OPT = "EditToolbar";
        const string PERSONALTOOLBAR_OPT = "PersonalToolbar";
        const string ENGINESTATETOOLBAR_OPT = "EngineStateToolbar";
        const string PROPERTYSTATETOOLBAR_OPT = "PropertyStateToolbar";
        const string PAGENAVIGATIONTOOLBAR_OPT = "PageNavigationToolbar";
        const string TOOLSTOOLBAR_OPT = "ToolsToolbar";
        const string RECENTDOCS_SECTION = "RecentDocuments";
        const string HIGHLIGHTSELECTION_OPT = "HighLightSelection";
        const string DEFAULTPAGESIZE_OPT = "DefaultPageSize";
        const string AUTOSAVEINTERVAL_OPT = "AutosaveInterval";
        const string AUTOROTATESNAP_OPT = "AutoRotateSnap";
        const string AUTOLOCKASPECTRATIO_OPT = "AutoLockAspectRatio";

        public string IniFile
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath) +
                    Path.DirectorySeparatorChar + _INIFILE;
            }
        }

        public Rectangle FormRect;
        public FormWindowState FormWindowState;
        public SidebarSide SidebarSide;
        public int SidebarWidth;
        public Zoom Zoom;
        public double Scale;
        public bool AntiAlias;
        public bool EditToolbar;
        public bool PersonalToolbar;
        public bool EngineStateToolbar;
        public bool PropertyStateToolbar;
        public bool PageNavigationToolbar;
        public bool ToolsToolbar;
        public bool HighlightSelection;
        public DPoint DefaultPageSize;
        public int AutoSaveInterval;
        public bool AutoRotateSnap;
        public bool AutoLockAspectRatio;

        public ProgramOptions()
        {
            ReadIni();
        }

        Rectangle StrToRect(string s)
        {
            Rectangle res = new Rectangle();
            string[] parts = s.Split(',');
            if (parts.Length >= 4)
            {
                int opt;
                if (int.TryParse(parts[0], out opt))
                    res.X = opt;
                if (int.TryParse(parts[1], out opt))
                    res.Y = opt;
                if (int.TryParse(parts[2], out opt))
                    res.Width = opt;
                if (int.TryParse(parts[3], out opt))
                    res.Height = opt;
            }
            return res;
        }

        string RectToStr(Rectangle r)
        {
            return string.Format("{0},{1},{2},{3}", r.X, r.Y, r.Width, r.Height);
        }

        IConfigSource _source = null;
        IConfigSource ConfigSource
        {
            get
            {
                if (_source == null)
                {
                    if (!File.Exists(IniFile))
                        File.Create(IniFile).Close();
                    _source = new IniConfigSource(IniFile);
                }
                return _source;
            }
        }

        public void ReadIni()
        {
            IConfig config = ConfigSource.Configs[MAIN_SECTION];
            if (config == null)
                config = ConfigSource.AddConfig(MAIN_SECTION);
            FormRect = StrToRect(config.Get(FORMRECT_OPT, "50,50,750,550"));
            string formWindowStateStr = config.Get(FORMWINDOWSTATE_OPT, FormWindowState.Normal.ToString());
            FormWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), formWindowStateStr, true);
            string SidebarSideStr = config.Get(SIDEBARSIDE_OPT, SidebarSide.Right.ToString());
            SidebarSide = (SidebarSide)Enum.Parse(typeof(SidebarSide), SidebarSideStr, true);
            SidebarWidth = config.GetInt(SIDEBARWIDTH_OPT, 100);
            string zoomStr = config.Get(ZOOM_OPT, Zoom.Custom.ToString());
            Zoom = (Zoom)Enum.Parse(typeof(Zoom), zoomStr, true);
            Scale = config.GetDouble(SCALE_OPT, 1);
            AntiAlias = config.GetBoolean(ANTIALIAS_OPT, true);
            EditToolbar = config.GetBoolean(EDITTOOLBAR_OPT, true);
            PersonalToolbar = config.GetBoolean(PERSONALTOOLBAR_OPT, true);
            EngineStateToolbar = config.GetBoolean(ENGINESTATETOOLBAR_OPT, true);
            PropertyStateToolbar = config.GetBoolean(PROPERTYSTATETOOLBAR_OPT, true);
            PageNavigationToolbar = config.GetBoolean(PAGENAVIGATIONTOOLBAR_OPT, true);
            ToolsToolbar = config.GetBoolean(TOOLSTOOLBAR_OPT, true);
            HighlightSelection = config.GetBoolean(HIGHLIGHTSELECTION_OPT, false);
            DefaultPageSize = DPoint.FromString(config.GetString(DEFAULTPAGESIZE_OPT, 
                DPoint.FormatToString(PageTools.FormatToSizeMM(PageFormat.Default))));
            AutoSaveInterval = config.GetInt(AUTOSAVEINTERVAL_OPT, 300000);
            AutoRotateSnap = config.GetBoolean(AUTOROTATESNAP_OPT, true);
            AutoLockAspectRatio = config.GetBoolean(AUTOLOCKASPECTRATIO_OPT, true);
        }

        public void WriteIni()
        {
            IConfig config = ConfigSource.Configs[MAIN_SECTION];
            if (config == null)
                config = ConfigSource.AddConfig(MAIN_SECTION);
            config.Set(FORMRECT_OPT, RectToStr(FormRect));
            config.Set(FORMWINDOWSTATE_OPT, FormWindowState);
            config.Set(SIDEBARSIDE_OPT, SidebarSide);
            config.Set(SIDEBARWIDTH_OPT, SidebarWidth);
            config.Set(ZOOM_OPT, Zoom);
            config.Set(SCALE_OPT, Scale);
            config.Set(ANTIALIAS_OPT, AntiAlias);
            config.Set(EDITTOOLBAR_OPT, EditToolbar);
            config.Set(PERSONALTOOLBAR_OPT, PersonalToolbar);
            config.Set(ENGINESTATETOOLBAR_OPT, EngineStateToolbar);
            config.Set(PROPERTYSTATETOOLBAR_OPT, PropertyStateToolbar);
            config.Set(PAGENAVIGATIONTOOLBAR_OPT, PageNavigationToolbar);
            config.Set(TOOLSTOOLBAR_OPT, ToolsToolbar);
            config.Set(HIGHLIGHTSELECTION_OPT, HighlightSelection);
            config.Set(DEFAULTPAGESIZE_OPT, DPoint.FormatToString(DefaultPageSize));
            config.Set(AUTOSAVEINTERVAL_OPT, AutoSaveInterval);
            config.Set(AUTOROTATESNAP_OPT, AutoRotateSnap);
            config.Set(AUTOLOCKASPECTRATIO_OPT, AutoLockAspectRatio);

            ConfigSource.Save();
        }

        const int MaxRecentDocs = 10;

        public List<string> GetRecentDocuments()
        {
            IConfig config = ConfigSource.Configs[RECENTDOCS_SECTION];
            if (config == null)
                config = ConfigSource.AddConfig(RECENTDOCS_SECTION);
            string[] values = config.GetValues();
            List<string> retValues = new List<string>();
            foreach (string value in values)
                if (File.Exists(value))
                    retValues.Add(value);
            if (retValues.Count > MaxRecentDocs)
                retValues.RemoveRange(10, retValues.Count - MaxRecentDocs);
            return retValues;
        }

        public void AddRecentDocument(string fileName)
        {
            List<string> values = GetRecentDocuments();
            IConfig config = ConfigSource.Configs[RECENTDOCS_SECTION];
            if (values.Count > 0 && !values[0].Equals(fileName))
            {
                int n = 1;
                config.Set(n.ToString(), fileName);
                foreach (string value in values)
                    if (!value.Equals(fileName))
                    {
                        n++;
                        config.Set(n.ToString(), value);
                    }
            }
            else
                config.Set(1.ToString(), fileName);
            ConfigSource.Save();
        }
    }
}
