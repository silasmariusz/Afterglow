using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Afterglow.Core;
using System.Net;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using Afterglow.Web.Host;

// trayIcon
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;
// show/hide console app
using System.Runtime.InteropServices;
using System.Threading;

namespace Afterglow.Web
{
    class Program
    {
        public static AfterglowRuntime Runtime
        {
            get
            {
                return _runtime;
            }
        }
        private static AfterglowRuntime _runtime;


        public static ContextMenu menu;
        public static MenuItem mnuExit;
        public static NotifyIcon notificationIcon;


        private static AppHost _appHost;

        
        static void Main(string[] args)
        {

            // Create Tray Icon
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            Stream iconResourceStream = currentAssembly.GetManifestResourceStream("Afterglow.Web.Resources.Icon.LED_light-16.ico");

            Thread notifyThread = new Thread(
                delegate()
                {
                    menu = new ContextMenu();
                    mnuExit = new MenuItem("E&xit");
                    menu.MenuItems.Add(0, mnuExit);

                    notificationIcon = new NotifyIcon()
                    {
                        Icon = new Icon(iconResourceStream),
                        ContextMenu = menu,
                        Text = typeof (Program).Assembly.GetName().Name
                    };

                    notificationIcon.MouseClick += new MouseEventHandler(notificationIcon_Click);
                    mnuExit.Click += new EventHandler(mnuExit_Click);

                    notificationIcon.Visible = true;
                    Application.Run();
                }
                );

            notifyThread.Start();
            // end: Tray Icon


            // Debug
#if DEBUG
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Debug.AutoFlush = true;
#endif
            //Debug.Indent();
            //Debug.Unindent();


            _runtime = new AfterglowRuntime();
            
            LogManager.LogFactory = new ConsoleLogFactory();

            //using (var appHost = new AppHost())
            _appHost = new AppHost();
            {
                Console.WriteLine("Starting Afterglow runtime...");
                //_runtime.Start();

                Console.WriteLine("Afterglow runtime started.");

                _appHost.Init();
                string host = String.Format("http://localhost:{0}/", _runtime.Setup.Port);
                if (args.Length > 0)
                    host = String.Format("http://{0}:{1}/", args[0], _runtime.Setup.Port);
                
                _appHost.Start(host);

                Console.WriteLine(host);
                
                //Console.WriteLine("Press <enter> to exit.");
                //Console.ReadLine();
                //appHost.Stop();

            }


            //_runtime.Setup = new AfterglowSetup();
            //Profile p = new Profile();
            //p.Setup = _runtime.Setup;
            //_runtime.Setup.ConfiguredPostProcessPlugins.Add(new ColourCorrectionPostProcess() { DisplayName = "frank" });
            ////_runtime.Setup.Profiles.Add(_runtime.Settings.Profiles.FirstOrDefault());
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredLightSetupPlugins.Add(a.OLDLightSetupPlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredCapturePlugins.Add(a.OLDCapturePlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredColourExtractionPlugins.Add(a.OLDColourExtractionPlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredPostProcessPlugins.AddRange(a.OLDPostProcessPlugins));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredOutputPlugins.AddRange(a.OLDOutputPlugins));

            //Profile profile = new Profile();
            //profile.LightSetupPlugins.AddRange(_runtime.Setup.DefaultLightSetupPlugins());
            //profile.CapturePlugins.AddRange(_runtime.Setup.DefaultCapturePlugins());
            //profile.ColourExtractionPlugins.AddRange(_runtime.Setup.DefaultColourExtractionPlugins());
            //profile.PostProcessPlugins.AddRange(_runtime.Setup.DefaultPostProcessPlugins());
            //profile.OutputPlugins.AddRange(_runtime.Setup.DefaultOutputPlugins());
            //_runtime.Setup.Profiles.Add(profile);
            //_runtime.Setup.ConfiguredLightSetupPlugins.Add(.FirstOrDefault().OLDPostProcessPlugins.FirstOrDefault());
            //_runtime.Save();

#if ! DEBUG 
            ShowWindow(ThisConsole, 0);
#endif
        }


        private static void mnuExit_Click(object sender, EventArgs e)
        {
            _appHost.Stop();

            notificationIcon.Visible = false;

//#if DEBUG
            ShowWindow(ThisConsole, 1);
            Console.WriteLine("Exiting...");
            //Console.WriteLine("Press <enter> to exit.");
            //Console.ReadLine();
//#endif

            Application.Exit();
            Environment.Exit(0);

            //Environment.Exit(1);
        }

        private static void notificationIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                //reserve right click for context menu
                showWindow = ++showWindow % 2;
                ShowWindow(ThisConsole, showWindow);
            }
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();

        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll")]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static Int32 showWindow = 0; //0 - SW_HIDE - Hides the window and activates another window.

    }
}
