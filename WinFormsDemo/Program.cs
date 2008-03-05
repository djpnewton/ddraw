using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinFormsDemo
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
            if (!Ipc.GlobalIpc.MutexUnauthorized)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                // send message to other program to show itself
                Ipc.GlobalIpc.SendMessage(IpcMessage.Show);
                if (WorkBookArguments.GlobalWbArgs.FloatingTools)
                    Ipc.GlobalIpc.SendMessage(IpcMessage.FloatingTools);
            }
        }
    }
}