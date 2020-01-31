using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Configuration;
using System.Runtime.InteropServices;

namespace CRepublic.Restarter
{
    // The main restarter class.
    public class Restarter
    {
        public Restarter(string path)
        {
            RestartedTimes = new List<DateTime>();
            _path = path;
        }

        // Determines if the Restarter is running (returns _started).
        public bool Started => _started;

        // Determines if the process has crashed.
        public bool HasCrashed
        {
            get
            {
                if (GetWerFaultProcess() != null)
                    return true;
                return false;
            }
        }

        // Number of times the application was restarted.  => RestartedTimes.Count
        public int RestartCount => RestartedTimes.Count;

        // List of DateTime the application was restarted.
        public List<DateTime> RestartedTimes { get; private set; }    

        // DateTime of when the Restarter started.
        public DateTime StartTime { get; private set; }


        public TimeSpan AverageRunTime
        {
            get
            {
                // Avoid divide by 0 exception.
                if (RestartedTimes.Count == 0)
                    return default(TimeSpan);

                return TimeSpan.FromTicks((DateTime.Now - StartTime).Ticks / RestartedTimes.Count);
            }
        }

        // Thread that does the restarting stuff.
        private Thread _restarterThread;
        // Process of application to restart.
        private Process _process;
        // Path to .exe file.
        private readonly string _path;
        // Determines if the Restarter is running.
        private bool _started;

        // Starts the Restarter.
        public void Start()
        {
            _started = true;
            _process = Process.Start(_path);
            
            StartTime = DateTime.Now;

            _restarterThread = new Thread(HandleRestarting) {Name = "Restarter Thread"};
            _restarterThread.Start();
        }

        // Stops the Restarter.
        public void Stop()
        {
            if (_started)
            {
                _started = false;
                _restarterThread.Abort();
            }
            else throw new InvalidOperationException("Exception occurred!");
        }

        // Handles the restarting of stuff by checking if it crashes.
        private void HandleRestarting()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var title = "Clashers' Republic Restarter v" + Assembly.GetExecutingAssembly().GetName().Version + " Count: " + RestartedTimes.Count;
                        Console.Title = title;

                        // Check if hasCrashed.
                        var hasCrashed = HasCrashed;
                        if (hasCrashed)
                        {
                            ConsoleUtils.WriteLineCenterRed("||");
                            ConsoleUtils.WriteLineCenterRed("Detected that " + ConfigurationManager.AppSettings["FileName"] + " has been crashed.");
                            ConsoleUtils.WriteLineCenterYellow("-> Restarting " + ConfigurationManager.AppSettings["FileName"] + " at " + DateTime.Now);

                            // Kill WerFault.exe to cause the EXE to exit.
                            var werFault = GetWerFaultProcess();
                            werFault.Kill();

                            Restart();
                        }

                        // Check if it has exited.
                        var hasExited = _process.HasExited;
                        if (hasExited)
                        {
                            ConsoleUtils.WriteLineCenterRed("||");
                            ConsoleUtils.WriteLineCenterRed("Detected that " + ConfigurationManager.AppSettings["FileName"] + " has been closed.");
                            ConsoleUtils.WriteLineCenterYellow("-> Restarting " + ConfigurationManager.AppSettings["FileName"] + " at " + DateTime.Now);

                            Restart();
                        }

                        // Be sure to sleep because we don't want to kill the CPU.
                        Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        ConsoleUtils.WriteLineCenterRed("||");
                        ConsoleUtils.WriteLineCenterRed("Exception occurred while running.");

                        var exStr = ex.ToString().Split(Environment.NewLine.ToCharArray());
                        foreach (var str in exStr)
                        {
                            ConsoleUtils.WriteLineCenterRed(str);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // We don't care about these types of exceptions.
            }
        }

        private Process GetWerFaultProcess()
        {
            var processes = Process.GetProcessesByName("WerFault");
            for (int i = 0; i < processes.Length; i++)
            {
                var fileDescription = _process.MainModule.FileVersionInfo.FileDescription;
                if (processes[i].MainWindowTitle.Contains(fileDescription))
                    return processes[i];
            }
            return null;
        }

        private void Restart()
        {
            // Make sure it is not dead before killing it because we don't want to kill a dead process.
            if (!_process.HasExited)
            {
                _process.Kill();
            }

            _process = Process.Start(_path);
            RestartedTimes.Add(DateTime.Now);
            ConsoleUtils.WriteLineCenterGreen("||");
            ConsoleUtils.WriteLineCenterGreen("" + ConfigurationManager.AppSettings["FileName"] + " has been restarted successfully at " + DateTime.Now);
        }

    }
}
