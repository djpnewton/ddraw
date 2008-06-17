using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net;

using Nini.Config;
using DDraw;

namespace WinFormsDemo.PersonalToolbar
{

    public enum PersonalToolButtonType { CustomFigure, RunCmd, ShowDir, WebLink };

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

    public struct WebLinkT
    {
        public string Link;

        public WebLinkT(string link)
        {
            Link = link;
        }

        public override string ToString()
        {
            return string.Concat("Open Link: \"", Link, "\"");
        }
    }

    public struct CustomFigureT
    {
        public Type FigureClass;
        public DAuthorProperties Dap;
        public string Base64Icon;

        public CustomFigureT(Type figureClass, DAuthorProperties dap, string base64Icon)
        {
            FigureClass = figureClass;
            Dap = dap;
            Base64Icon = base64Icon;
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

        public RunCmdToolButton(RunCmdT t) : this()
        {
            Cmd = t.Command;
            Arguments = t.Arguments;
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

        public ShowDirToolButton(ShowDirT t) : this()
        {
            Dir = t.Dir;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (System.IO.Directory.Exists(dir))
                System.Diagnostics.Process.Start(dir);
        }
    }

    public class WebLinkToolButton : ToolStripButton
    {
        string link;
        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                ToolTipText = value;
            }
        }

        public WebLinkT WebLinkT
        {
            get { return new WebLinkT(link); }
        }

        public WebLinkToolButton()
        {
            Image = Resource1.world_link;
        }

        public WebLinkToolButton(WebLinkT t)
            : this()
        {
            Link = t.Link;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            try
            {
                UriBuilder ub = new UriBuilder(link);
                System.Diagnostics.Process.Start(ub.Uri.AbsoluteUri);
            }
            catch (Exception e2)
            { MessageBox.Show(e2.Message, "Web link error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            get { return new CustomFigureT(figureClass, dap, WorkBookUtils.BitmapToBase64((Bitmap)Image)); }
        }

        public CustomFigureToolButton()
        {
            Image = Resource1.technocolor;
        }

        public CustomFigureToolButton(CustomFigureT t) : this()
        {
            FigureClass = t.FigureClass;
            Dap = t.Dap;
            if (t.Base64Icon != null)
                Image = WorkBookUtils.Base64ToBitmap(t.Base64Icon);
        }
    }

    public static class PtUtils
    {
        const string _INIFILE = "PersonalToolbar.ini";
        const string TYPE_OPT = "Type";
        const string FIGURECLASS_OPT = "FigureClass";
        const string BASE64ICON_OPT = "base64Icon";
        const string RUNCMD_OPT = "RunCmd";
        const string ARGS_OPT = "Args";
        const string DIR_OPT = "Dir";
        const string WEBLINK_OPT = "WebLink";

        static string IniFile
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath) +
                    Path.DirectorySeparatorChar + _INIFILE;
            }
        }

        public static void LoadPersonalToolsFromSource(PersonalToolStrip ts, IConfigSource source)
        {
            ts.Clear();
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
                                WorkBookUtils.ReadConfigToDap(config, dap);
                                string base64Icon = config.Get(BASE64ICON_OPT);
                                ts.Items.Add(new CustomFigureToolButton(new CustomFigureT(figureClass, dap, base64Icon)));
                            }
                            break;
                        case PersonalToolButtonType.RunCmd:
                            ts.Items.Add(new RunCmdToolButton(new RunCmdT(config.Get(RUNCMD_OPT),
                                config.Get(ARGS_OPT))));
                            break;
                        case PersonalToolButtonType.ShowDir:
                            ts.Items.Add(new ShowDirToolButton(new ShowDirT(config.Get(DIR_OPT))));
                            break;
                        case PersonalToolButtonType.WebLink:
                            ts.Items.Add(new WebLinkToolButton(new WebLinkT(config.Get(WEBLINK_OPT))));
                            break;
                    }
                }
        }

        public static void LoadPersonalTools(PersonalToolStrip ts)
        {
            IConfigSource source;
            if (File.Exists(IniFile))
                source = new IniConfigSource(IniFile);
            else
                source = new IniConfigSource();
            LoadPersonalToolsFromSource(ts, source);
        }

        public static IniConfigSource CreatePersonalToolsSource(PersonalToolStrip ts)
        {
            IniConfigSource source = new IniConfigSource();
            for (int i = 1; i < ts.Items.Count; i++)
            {
                IConfig config = source.AddConfig(i.ToString());
                ToolStripItem b = ts.Items[i];
                if (b is CustomFigureToolButton)
                {
                    CustomFigureT t = ((CustomFigureToolButton)b).CustomFigureT;
                    config.Set(TYPE_OPT, PersonalToolButtonType.CustomFigure);
                    config.Set(FIGURECLASS_OPT, t.FigureClass.AssemblyQualifiedName);
                    WorkBookUtils.WriteDapToConfig(t.Dap, config);
                    config.Set(BASE64ICON_OPT, t.Base64Icon);
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
                else if (b is WebLinkToolButton)
                {
                    WebLinkT t = ((WebLinkToolButton)b).WebLinkT;
                    if (t.Link != null)
                    {
                        config.Set(TYPE_OPT, PersonalToolButtonType.WebLink);
                        config.Set(WEBLINK_OPT, (t.Link));
                    }
                }
            }
            return source;
        }

        public static void SavePersonalTools(PersonalToolStrip ts)
        {
            IniConfigSource source = CreatePersonalToolsSource(ts);
            source.Save(IniFile);
        }
    }
}
