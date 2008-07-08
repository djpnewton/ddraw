using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net;

using Nini.Config;
using DDraw;

namespace Workbook.PersonalToolbar
{

    public enum PersonalToolButtonType { CustomFigure, RunCmd, ShowDir, WebLink };

    public abstract class PersonalTool
    {
        public string Label;
        public bool ShowLabel;

        public PersonalTool(string label, bool showLabel)
        {
            Label = label;
            ShowLabel = showLabel;
        }
    }

    public class RunCmdTool : PersonalTool
    {
        public string Command;
        public string Arguments;

        public RunCmdTool(string label, bool showLabel, string cmd, string args) : base(label, showLabel)
        {
            Command = cmd;
            Arguments = args;
        }

        public override string ToString()
        {
            if (ShowLabel)
                return Label;
            else
                return string.Concat("Run Command: \"", Path.GetFileName(Command), "\"");
        }
    }

    public class ShowDirTool : PersonalTool
    {
        public string Dir;

        public ShowDirTool(string label, bool showLabel, string dir) : base(label, showLabel)
        {
            Dir = dir;
        }

        public override string ToString()
        {
            if (ShowLabel)
                return Label;
            else
                return string.Concat("Open Directory: \"", Dir, "\"");
        }
    }

    public class WebLinkTool : PersonalTool
    {
        public string Link;

        public WebLinkTool(string label, bool showLabel, string link) : base(label, showLabel)
        {
            Link = link;
        }

        public override string ToString()
        {
            if (ShowLabel)
                return Label;
            else
                return string.Concat("Open Link: \"", Link, "\"");
        }
    }

    public class CustomFigureTool : PersonalTool
    {
        public Type FigureClass;
        public DAuthorProperties Dap;
        public string Base64Icon;

        public CustomFigureTool(string label, bool showLabel, Type figureClass, DAuthorProperties dap, string base64Icon) : base(label, showLabel)
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

    public abstract class PersonalToolButton : ToolStripButton
    {
        public string Label
        {
            get { return Text; }
            set { Text = value; }
        }
        public bool ShowLabel
        {
            get { return DisplayStyle == ToolStripItemDisplayStyle.Text; }
            set
            {
                if (value)
                    DisplayStyle = ToolStripItemDisplayStyle.Text;
                else
                    DisplayStyle = ToolStripItemDisplayStyle.Image;
            }
        }

        public PersonalToolButton(PersonalTool t)
        {
            Label = t.Label;
            ShowLabel = t.ShowLabel;
        }
    }

    public class RunCmdToolButton : PersonalToolButton
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

        public RunCmdTool RunCmd
        {
            get { return new RunCmdTool(Label, ShowLabel, cmd, args); }
        }

        public RunCmdToolButton(RunCmdTool t) : base(t)
        {
            Image = Resource1.cog;
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

    public class ShowDirToolButton : PersonalToolButton
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

        public ShowDirTool ShowDir
        {
            get { return new ShowDirTool(Label, ShowLabel, dir); }
        }

        public ShowDirToolButton(ShowDirTool t) : base(t)
        {
            Image = Resource1.folder;
            Dir = t.Dir;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (System.IO.Directory.Exists(dir))
                System.Diagnostics.Process.Start(dir);
        }
    }

    public class WebLinkToolButton : PersonalToolButton
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

        public WebLinkTool WebLink
        {
            get { return new WebLinkTool(Label, ShowLabel, link); }
        }

        public WebLinkToolButton(WebLinkTool t) : base(t)
        {
            Image = Resource1.world_link;
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

    public class CustomFigureToolButton : PersonalToolButton
    {
        Type figureClass = null;
        public Type FigureClass
        {
            get { return figureClass; }
            set
            {
                figureClass = value;
                ToolTipText = CustomFigure.ToString();
            }
        }

        DAuthorProperties dap = null;
        public DAuthorProperties Dap
        {
            get { return dap; }
            set { dap = value; }
        }

        public CustomFigureTool CustomFigure
        {
            get { return new CustomFigureTool(Label, ShowLabel, figureClass, dap, WorkBookUtils.BitmapToBase64((Bitmap)Image)); }
        }

        public CustomFigureToolButton(CustomFigureTool t) : base(t)
        {
            Image = Resource1.technocolor;
            FigureClass = t.FigureClass;
            Dap = t.Dap;
            if (t.Base64Icon != null)
            {
                Bitmap bmp = WorkBookUtils.Base64ToBitmap(t.Base64Icon);
                Bitmap bmp2;
                Graphics g;
                if (bmp.Width > Height)
                {
                    bmp2 = new Bitmap(bmp.Width, bmp.Width);
                    g = Graphics.FromImage(bmp2);
                    g.DrawImage(bmp, new PointF(0, bmp2.Height / 2 - bmp.Height / 2));
                }
                else
                {
                    bmp2 = new Bitmap(bmp.Height, bmp.Height);
                    g = Graphics.FromImage(bmp2);
                    g.DrawImage(bmp, new PointF(bmp2.Width / 2 - bmp.Width / 2, 0));
                }
                g.Dispose();
                bmp.Dispose();
                Image = bmp2;
            }
        }
    }

    public static class PtUtils
    {
        const string _INIFILE = "PersonalToolbar.ini";
        const string TYPE_OPT = "Type";
        const string LABEL_OPT = "Label";
        const string SHOWLABEL_OPT = "ShowLabel";
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
                    string label = config.Get(LABEL_OPT, "");
                    bool showLabel = config.GetBoolean(SHOWLABEL_OPT, false);
                    switch (type)
                    {
                        case PersonalToolButtonType.CustomFigure:
                            Type figureClass = Type.GetType(config.Get(FIGURECLASS_OPT));
                            if (figureClass != null)
                            {
                                DAuthorProperties dap = new DAuthorProperties();
                                WorkBookUtils.ReadConfigToDap(config, dap);
                                string base64Icon = config.Get(BASE64ICON_OPT);
                                ts.Items.Add(new CustomFigureToolButton(new CustomFigureTool(label, showLabel, figureClass, dap, base64Icon)));
                            }
                            break;
                        case PersonalToolButtonType.RunCmd:
                            ts.Items.Add(new RunCmdToolButton(new RunCmdTool(label, showLabel, config.Get(RUNCMD_OPT),
                                config.Get(ARGS_OPT))));
                            break;
                        case PersonalToolButtonType.ShowDir:
                            ts.Items.Add(new ShowDirToolButton(new ShowDirTool(label, showLabel, config.Get(DIR_OPT))));
                            break;
                        case PersonalToolButtonType.WebLink:
                            ts.Items.Add(new WebLinkToolButton(new WebLinkTool(label, showLabel, config.Get(WEBLINK_OPT))));
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
                if (b is PersonalToolButton)
                {
                    config.Set(LABEL_OPT, ((PersonalToolButton)b).Label);
                    config.Set(SHOWLABEL_OPT, ((PersonalToolButton)b).ShowLabel);
                    if (b is CustomFigureToolButton)
                    {
                        CustomFigureTool t = ((CustomFigureToolButton)b).CustomFigure;
                        config.Set(TYPE_OPT, PersonalToolButtonType.CustomFigure);
                        config.Set(FIGURECLASS_OPT, t.FigureClass.AssemblyQualifiedName);
                        WorkBookUtils.WriteDapToConfig(t.Dap, config);
                        config.Set(BASE64ICON_OPT, t.Base64Icon);
                    }
                    else if (b is RunCmdToolButton)
                    {
                        RunCmdTool t = ((RunCmdToolButton)b).RunCmd;
                        if (t.Command != null && t.Arguments != null)
                        {
                            config.Set(TYPE_OPT, PersonalToolButtonType.RunCmd);
                            config.Set(RUNCMD_OPT, t.Command);
                            config.Set(ARGS_OPT, t.Arguments);
                        }
                    }
                    else if (b is ShowDirToolButton)
                    {
                        ShowDirTool t = ((ShowDirToolButton)b).ShowDir;
                        if (t.Dir != null)
                        {
                            config.Set(TYPE_OPT, PersonalToolButtonType.ShowDir);
                            config.Set(DIR_OPT, t.Dir);
                        }
                    }
                    else if (b is WebLinkToolButton)
                    {
                        WebLinkTool t = ((WebLinkToolButton)b).WebLink;
                        if (t.Link != null)
                        {
                            config.Set(TYPE_OPT, PersonalToolButtonType.WebLink);
                            config.Set(WEBLINK_OPT, t.Link);
                        }
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
