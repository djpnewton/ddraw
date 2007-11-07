// /home/daniel/Projects/ddraw/GTKDemo/Main.cs created with MonoDevelop
// User: daniel at 8:54 p 5/11/2007
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
// project created on 5/11/2007 at 8:54 p
using System;
using Gtk;

namespace GTKDemo
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.ShowAll ();
			Application.Run ();
		}
	}
}