using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Nini.Config;

namespace WinFormsDemo.PersonalToolbar
{

    public enum PersonalToolbuttonType { RunCmd, ShowDir };

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

    public static class PtUtils
    {
        const string _INIFILE = "PersonalToolbar.ini";
        const string TYPE_OPT = "Type";
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
                    PersonalToolbuttonType type = (PersonalToolbuttonType)Enum.Parse(
                        typeof(PersonalToolbuttonType), config.Get(TYPE_OPT), true);
                    switch (type)
                    {
                        case PersonalToolbuttonType.RunCmd:
                            tsPersonal.Items.Add(new RunCmdToolButton(config.Get(RUNCMD_OPT),
                                config.Get(ARGS_OPT)));
                            break;
                        case PersonalToolbuttonType.ShowDir:
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
            for (int i = 1; i < tsPersonal.Items.Count; i++)
            {
                source.Configs.Clear();
                IConfig config = source.AddConfig(i.ToString());
                ToolStripItem b = tsPersonal.Items[i];
                if (b is RunCmdToolButton)
                {
                    config.Set(TYPE_OPT, PersonalToolbuttonType.RunCmd);
                    config.Set(RUNCMD_OPT, ((RunCmdToolButton)b).RunCmdT.Command);
                    config.Set(ARGS_OPT, ((RunCmdToolButton)b).RunCmdT.Arguments);
                }
                else if (b is ShowDirToolButton)
                {
                    config.Set(TYPE_OPT, PersonalToolbuttonType.ShowDir);
                    config.Set(DIR_OPT, ((ShowDirToolButton)b).ShowDirT.Dir);
                }
            }
            source.Save();
        }
    }
}
