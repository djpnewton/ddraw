using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Nini.Config;
using DDraw;

namespace WinFormsDemo.PersonalToolbar
{

    public enum PersonalToolButtonType { CustomFigure, RunCmd, ShowDir };

    public struct RunCmdT
    {
        public string Command;
        public string Arguments;

        public RunCmdT(string cmd, string args)
        {
            Command = cmd;
            Arguments = args;
        }

        public override string ToString()
        {
            return string.Concat("Run Command: \"", Path.GetFileName(Command), "\"");
        }
    }

    public struct ShowDirT
    {
        public string Dir;

        public ShowDirT(string dir)
        {
            Dir = dir;
        }

        public override string ToString()
        {
            return string.Concat("Open Directory: \"", Dir, "\"");
        }
    }

    public struct CustomFigureT
    {
        public Type FigureClass;
        public DAuthorProperties Dap;

        public CustomFigureT(Type figureClass, DAuthorProperties dap)
        {
            FigureClass = figureClass;
            Dap = dap;
        }

        public override string ToString()
        {
            if (FigureClass != null)
                return string.Concat("Custom ", FigureClass.ToString());
            else 
                return base.ToString();
        }
    }

    public class RunCmdToolButton : ToolStripButton
    {
        string cmd;
        public string Cmd
        {
            get { return cmd; }
            set
            {
                cmd = value;
                ToolTipText = value;
            }
        }

        string args;
        public string Arguments
        {
            get { return args; }
            set { args = value; }
        }

        public RunCmdT RunCmdT
        {
            get { return new RunCmdT(cmd, args); }
        }

        public RunCmdToolButton()
        {
            Image = Resource1.cog;
        }

        public RunCmdToolButton(string cmd, string args) : this()
        {
            Cmd = cmd;
            Arguments = args;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (System.IO.File.Exists(cmd))
                System.Diagnostics.Process.Start(cmd, args);
        }
    }

    public class ShowDirToolButton : ToolStripButton
    {
        string dir;
        public string Dir
        {
            get { return dir; }
            set 
            { 
                dir = value;
                ToolTipText = value;
            }
        }

        public ShowDirT ShowDirT
        {
            get { return new ShowDirT(dir); }
        }

        public ShowDirToolButton()
        {
            Image = Resource1.folder;
        }

        public ShowDirToolButton(string dir) : this()
        {
            Dir = dir;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (System.IO.Directory.Exists(dir))
                System.Diagnostics.Process.Start(dir);
        }
    }

    public class CustomFigureToolButton : ToolStripButton
    {
        Type figureClass = null;
        public Type FigureClass
        {
            get { return figureClass; }
            set
            {
                figureClass = value;
                ToolTipText = CustomFigureT.ToString();
            }
        }

        DAuthorProperties dap = null;
        public DAuthorProperties Dap
        {
            get { return dap; }
            set { dap = value; }
        }

        public CustomFigureT CustomFigureT
        {
            get { return new CustomFigureT(figureClass, dap); }
        }

        public CustomFigureToolButton()
        {
            Image = Resource1.technocolor;
        }

        public CustomFigureToolButton(Type figureClass, DAuthorProperties dap) : this()
        {
            FigureClass = figureClass;
            Dap = dap;
        }
    }

    public static class PtUtils
    {
        const string _INIFILE = "PersonalToolbar.ini";
        const string TYPE_OPT = "Type";
        const string FIGURECLASS_OPT = "FigureClass";
        const string FILL_OPT = "Fill";
        const string STROKE_OPT = "Stroke";
        const string STROKEWIDTH_OPT = "StrokeWidth";
        const string STROKESTYLE_OPT = "StrokeStyle";
        const string ALPHA_OPT = "Alpha";
        const string STARTMARKER_OPT = "StartMarker";
        const string ENDMARKER_OPT = "EndMarker";
        const string FONTNAME_OPT = "FontName";
        const string BOLD_OPT = "Bold";
        const string ITALICS_OPT = "Italics";
        const string UNDERLINE_OPT = "Underline";
        const string STRIKETHROUGH_OPT = "Strikethrough";
        const string RUNCMD_OPT = "RunCmd";
        const string ARGS_OPT = "Args";
        const string DIR_OPT = "Dir";

        static string IniFile
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath) +
                    Path.DirectorySeparatorChar + _INIFILE;
            }
        }

        public static void LoadPersonalTools(ToolStripEx tsPersonal)
        {
            IConfigSource source;
            if (File.Exists(IniFile))
                source = new IniConfigSource(IniFile);
            else
                source = new IniConfigSource();

            foreach (IConfig config in source.Configs)
                if (config.Contains(TYPE_OPT))
                {
                    PersonalToolButtonType type = (PersonalToolButtonType)Enum.Parse(
                        typeof(PersonalToolButtonType), config.Get(TYPE_OPT), true);
                    switch (type)
                    {
                        case PersonalToolButtonType.CustomFigure:
                            Type figureClass = Type.GetType(config.Get(FIGURECLASS_OPT));
                            if (figureClass != null)
                            {
                                DAuthorProperties dap = new DAuthorProperties();
                                dap.Fill = DColor.FromString(config.Get(FILL_OPT));
                                dap.Stroke = DColor.FromString(config.Get(STROKE_OPT));
                                dap.StrokeWidth = config.GetInt(STROKEWIDTH_OPT);
                                dap.StrokeStyle = (DStrokeStyle)Enum.Parse(typeof(DStrokeStyle), config.Get(STROKESTYLE_OPT), true);
                                dap.Alpha = config.GetDouble(ALPHA_OPT);
                                dap.StartMarker = (DMarker)Enum.Parse(typeof(DMarker), config.Get(STARTMARKER_OPT), true);
                                dap.EndMarker = (DMarker)Enum.Parse(typeof(DMarker), config.Get(ENDMARKER_OPT), true);
                                dap.FontName = config.Get(FONTNAME_OPT);
                                dap.Bold = config.GetBoolean(BOLD_OPT);
                                dap.Italics = config.GetBoolean(ITALICS_OPT);
                                dap.Underline = config.GetBoolean(UNDERLINE_OPT);
                                dap.Strikethrough = config.GetBoolean(STRIKETHROUGH_OPT);
                                tsPersonal.Items.Add(new CustomFigureToolButton(figureClass, dap));
                            }
                            break;
                        case PersonalToolButtonType.RunCmd:
                            tsPersonal.Items.Add(new RunCmdToolButton(config.Get(RUNCMD_OPT),
                                config.Get(ARGS_OPT)));
                            break;
                        case PersonalToolButtonType.ShowDir:
                            tsPersonal.Items.Add(new ShowDirToolButton(config.Get(DIR_OPT)));
                            break;
                    }
                }
        }

        public static void SavePersonalTools(ToolStripEx tsPersonal)
        {
            if (!File.Exists(IniFile))
                File.Create(IniFile).Close();
            IConfigSource source = new IniConfigSource(IniFile);
            source.Configs.Clear();
            for (int i = 1; i < tsPersonal.Items.Count; i++)
            {
                IConfig config = source.AddConfig(i.ToString());
                ToolStripItem b = tsPersonal.Items[i];
                if (b is CustomFigureToolButton)
                {
                    CustomFigureT t = ((CustomFigureToolButton)b).CustomFigureT;
                    config.Set(TYPE_OPT, PersonalToolButtonType.CustomFigure);
                    config.Set(FIGURECLASS_OPT, t.FigureClass.AssemblyQualifiedName);
                    config.Set(FILL_OPT, DColor.FormatToString(t.Dap.Fill));
                    config.Set(STROKE_OPT, DColor.FormatToString(t.Dap.Stroke));
                    config.Set(STROKEWIDTH_OPT, t.Dap.StrokeWidth);
                    config.Set(STROKESTYLE_OPT, t.Dap.StrokeStyle.ToString());
                    config.Set(ALPHA_OPT, t.Dap.Alpha);
                    config.Set(STARTMARKER_OPT, t.Dap.StartMarker);
                    config.Set(ENDMARKER_OPT, t.Dap.EndMarker);
                    config.Set(FONTNAME_OPT, t.Dap.FontName);
                    config.Set(BOLD_OPT, t.Dap.Bold);
                    config.Set(ITALICS_OPT, t.Dap.Italics);
                    config.Set(UNDERLINE_OPT, t.Dap.Underline);
                    config.Set(STRIKETHROUGH_OPT, t.Dap.Strikethrough);
                }
                else if (b is RunCmdToolButton)
                {
                    RunCmdT t = ((RunCmdToolButton)b).RunCmdT;
                    if (t.Command != null && t.Arguments != null)
                    {
                        config.Set(TYPE_OPT, PersonalToolButtonType.RunCmd);
                        config.Set(RUNCMD_OPT, ((RunCmdToolButton)b).RunCmdT.Command);
                        config.Set(ARGS_OPT, ((RunCmdToolButton)b).RunCmdT.Arguments);
                    }
                }
                else if (b is ShowDirToolButton)
                {
                    ShowDirT t = ((ShowDirToolButton)b).ShowDirT;
                    if (t.Dir != null)
                    {
                        config.Set(TYPE_OPT, PersonalToolButtonType.ShowDir);
                        config.Set(DIR_OPT, (t.Dir));
                    }
                }
            }
            source.Save();
        }
    }
}
