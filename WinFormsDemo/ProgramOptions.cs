using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using DDraw;
using Nini.Config;

namespace WinFormsDemo
{
    public class ProgramOptions
    {
        const string _INIFILE = "WinFormsDemo.ini";
        const string MAIN_SECTION = "Main";
        const string FORMRECT_OPT = "FormRect";
        const string FORMWINDOWSTATE_OPT = "FormWindowState";
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

            if (source.Configs[MAIN_SECTION] == null)
                source.AddConfig(MAIN_SECTION);
            FormRect = StrToRect(source.Configs[MAIN_SECTION].Get(FORMRECT_OPT, "50,50,750,550"));
            string formWindowStateStr = source.Configs[MAIN_SECTION].Get(FORMWINDOWSTATE_OPT, FormWindowState.Normal.ToString());
            FormWindowState = (FormWindowState)Enum.Parse(typeof(FormWindowState), formWindowStateStr, true);
            string zoomStr = source.Configs[MAIN_SECTION].Get(ZOOM_OPT, Zoom.Custom.ToString());
            Zoom = (Zoom)Enum.Parse(typeof(Zoom), zoomStr, true);
            Scale = source.Configs[MAIN_SECTION].GetDouble(SCALE_OPT, 1);
            AntiAlias = source.Configs[MAIN_SECTION].GetBoolean(ANTIALIAS_OPT, true);
        }

        public void WriteIni()
        {
            if (!File.Exists(IniFile))
                File.Create(IniFile).Close();
            IConfigSource source = new IniConfigSource(IniFile);

            if (source.Configs[MAIN_SECTION] == null)
                source.AddConfig(MAIN_SECTION);
            source.Configs[MAIN_SECTION].Set(FORMRECT_OPT, RectToStr(FormRect));
            source.Configs[MAIN_SECTION].Set(FORMWINDOWSTATE_OPT, FormWindowState);
            source.Configs[MAIN_SECTION].Set(ZOOM_OPT, Zoom);
            source.Configs[MAIN_SECTION].Set(SCALE_OPT, Scale);
            source.Configs[MAIN_SECTION].Set(ANTIALIAS_OPT, AntiAlias);

            source.Save();
        }
    }
}
