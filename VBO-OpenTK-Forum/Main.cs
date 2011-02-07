
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace VBOOpenTKForum
{
	public class Application
	{
		static void Main (string[] args)
		{
			UIApplication.Main (args);
		}
	}

	// The name AppDelegate is referenced in the MainWindow.xib file.
	public partial class AppDelegate : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and is ready to run
		public override void FinishedLaunching (UIApplication app)
		{
			glView.Run (60.0);
		}

		public override void OnResignActivation (UIApplication app)
		{
			glView.Stop ();
			glView.Run (5.0);
		}

		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication app)
		{
			glView.Stop ();
			glView.Run (60.0);
		}
	}
}

