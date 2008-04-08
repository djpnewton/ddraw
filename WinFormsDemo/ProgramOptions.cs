using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using DDraw;
using Nini.Config;
using WinFormsDemo.PersonalToolbar;

namespace WinFormsDemo
{
    public enum SidebarSide { Right, Left };

    public class ProgramOptions
    {
        const string _INIFILE = "WinFormsDemo.ini";
        const string MAIN_SECTION = "Main";
        const string FORMRECT_OPT = "FormRect";
        const string FORMWINDOWSTATE_OPT = "FormWindowState";
        const string SIDEBARSIDE_OPT = "SidebarSide";
        const string SIDEBARWIDTH_OPT = "SidebarWidth";
        const string ZOOM_OPT = "Zoom";
        const string SCALE_OPT = "Scale";
        const string ANTIALIAS_OPT = "AntiAlias";

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

        public void ReadIni()
        {
            IConfigSource source;
            if (File.Exists(IniFile))
                source = new IniConfigSource(IniFile);
            else
                source = new IniConfigSource();

            IConfig config = source.Configs[MAIN_SECTION];
            if (config == null)
                config = source.AddConfig(MAIN_SECTION);
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
        }

        public void WriteIni()
        {
            if (!File.Exists(IniFile))
                File.Create(IniFile).Close();
            IConfigSource source = new IniConfigSource(IniFile);

            IConfig config = source.Configs[MAIN_SECTION];
            if (config == null)
                config = source.AddConfig(MAIN_SECTION);
            config.Set(FORMRECT_OPT, RectToStr(FormRect));
            config.Set(FORMWINDOWSTATE_OPT, FormWindowState);
            config.Set(SIDEBARSIDE_OPT, SidebarSide);
            config.Set(SIDEBARWIDTH_OPT, SidebarWidth);
            config.Set(ZOOM_OPT, Zoom);
            config.Set(SCALE_OPT, Scale);
            config.Set(ANTIALIAS_OPT, AntiAlias);

            source.Save();
        }
    }
}
