using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace RTEvents
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public delegate int RecoveryDelegate(IntPtr parameter);

        [DllImport("kernel32.dll")]
        private static extern int RegisterApplicationRecoveryCallback(
                RecoveryDelegate recoveryCallback,
                IntPtr parameter,
                uint pingInterval,
                uint flags);

        [DllImport("kernel32.dll")]
        private static extern void ApplicationRecoveryFinished(bool success);

        private static void RegisterForRecovery()
        {
            var callback = new RecoveryDelegate(p =>
            {
                Process.Start(Assembly.GetEntryAssembly().Location);
                ApplicationRecoveryFinished(true);
                return 0;
            });

            var interval = 100U;
            var flags = 0U;

            RegisterApplicationRecoveryCallback(callback, IntPtr.Zero, interval, flags);
        }

        private static void Recover()
        {
            //do the recovery and cleanup
            Process.Start(Assembly.GetEntryAssembly().Location);
        }

        

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler((s, e) =>
                {
                    Recover();
                    Environment.Exit(1);
                });

            RegisterForRecovery();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RTEventsMain());
        }
    }
}