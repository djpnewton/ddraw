using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Workbook
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // if program not already running then run
            if (Ipc.GlobalIpc.MutexUnauthorized && WorkBookArguments.GlobalWbArgs.Show)
            {
                if (WorkBookArguments.GlobalWbArgs.ScreenAnnotate)
                    Ipc.GlobalIpc.SendMessage(IpcMessage.ScreenAnnotate);
                else
                    Ipc.GlobalIpc.SendMessage(IpcMessage.Show);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                new MainForm();
                Application.Run();
            }
        }
    }
}