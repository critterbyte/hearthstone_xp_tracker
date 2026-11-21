using System;
using System.Threading;
using System.Windows;
using XpTracker.Core;

namespace XpTracker.App;

/// <summary>
/// Starts the app. Only one instance runs: starting it again opens the dashboard of the running one.
/// </summary>
/// <remarks>
/// Started by Windows at sign-in (argument <c>--autostart</c>), the app shows only its notification area icon.
/// </remarks>
internal static class Program
{
    private const string InstanceName = "HearthstoneXpTracker.SingleInstance";

    [STAThread]
    public static int Main(string[] args)
    {
        var autostart = WindowsStartup.IsAutostart(args);
        using var mutex = new Mutex(initiallyOwned: true, InstanceName + ".Mutex", out var firstInstance);
        if (!firstInstance)
        {
            if (!autostart)
            {
                SignalRunningInstance();
            }

            return 0;
        }

        var showDashboard = new EventWaitHandle(false, EventResetMode.AutoReset, InstanceName + ".Show");
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var tracker = new XpTrackerApp();

        // An unexpected error in the user interface is written to the log; the app keeps running.
        app.DispatcherUnhandledException += (_, e) =>
        {
            AppLog.Error(e.Exception);
            e.Handled = true;
        };

        RegisteredWaitHandle? wait = null;
        app.Startup += (_, _) =>
        {
            tracker.Start();
            if (!autostart)
            {
                tracker.OpenDashboard();
            }

            wait = ThreadPool.RegisterWaitForSingleObject(
                showDashboard,
                (_, _) => app.Dispatcher.BeginInvoke(new Action(tracker.OpenDashboard)),
                state: null,
                millisecondsTimeOutInterval: Timeout.Infinite,
                executeOnlyOnce: false);
        };

        app.Run();

        wait?.Unregister(null);
        showDashboard.Dispose();
        mutex.ReleaseMutex();
        return 0;
    }

    private static void SignalRunningInstance()
    {
        try
        {
            using var signal = EventWaitHandle.OpenExisting(InstanceName + ".Show");
            signal.Set();
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            // The running instance is just starting or closing; nothing to open.
        }
    }
}
