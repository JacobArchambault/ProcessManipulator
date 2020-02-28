using System;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static System.Diagnostics.Process;

namespace ProcessManipulator
{
    class Program
    {
        static void Main()
        {
            WriteLine("***** Fun with Processes *****\n");
            ListAllRunningProcesses();

            // Prompt user for a PID and print out the set of active threads.
            WriteLine("***** Enter PID of process to investigate *****");
            Write("PID: ");
            string pID = ReadLine();
            int theProcID = int.Parse(pID);

            GetSpecificProcess();
            EnumThreadsForPid(theProcID);
            EnumModsForPid(theProcID);
            StartAndKillProcess();
            ReadLine();
        }
        static void ListAllRunningProcesses()
        {
            // Get all the processes on the local machine, ordered by
            // PID.
            var runningProcs = from proc in GetProcesses(".") orderby proc.Id select proc;

            // Print out PID and name of each process.
            foreach (var p in runningProcs)
            {
                string info = $"-> PID: {p.Id}\tName: {p.ProcessName}";
                WriteLine(info);
            }
            WriteLine("************************************\n");
        }
        // If there is no process with the PID of 987, a runtime exception will be thrown.
        static void GetSpecificProcess()
        {
            try
            {
                Process theProc = GetProcessById(987);
            }
            catch (ArgumentException ex)
            {
                WriteLine(ex.Message);
            }
        }
        static void EnumThreadsForPid(int pID)
        {
            Process theProc;
            try
            {
                theProc = GetProcessById(pID);
            }
            catch (ArgumentException ex)
            {
                WriteLine(ex.Message);
                return;
            }

            // List out stats for each thread in the specified process.
            WriteLine("Here are the threads used by: {0}", theProc.ProcessName);
            ProcessThreadCollection theThreads = theProc.Threads;

            foreach (ProcessThread pt in theThreads)
            {
                string info =
                    $"-> Thread ID: {pt.Id}\tStart Time: {pt.StartTime.ToShortTimeString()}\tPriority: {pt.PriorityLevel}";
                WriteLine(info);
            }
            WriteLine("************************************\n");
        }
        static void EnumModsForPid(int pID)
        {
            Process theProc;
            try
            {
                theProc = GetProcessById(pID);
            }
            catch (ArgumentException ex)
            {
                WriteLine(ex.Message);
                return;
            }

            WriteLine("Here are the loaded modules for: {0}", theProc.ProcessName);
            ProcessModuleCollection theMods = theProc.Modules;
            foreach (ProcessModule pm in theMods)
            {
                string info = $"-> Mod Name: {pm.ModuleName}";
                WriteLine(info);
            }
            WriteLine("************************************\n");
        }
        static void StartAndKillProcess()
        {
            Process ffProc = null;

            // Launch Firefox, and go to Waystar!
            try
            {
                ProcessStartInfo startInfo =
                    new ProcessStartInfo("FireFox.exe", "www.jacobarchambault.azurewebsites.net")
                    {
                        WindowStyle = ProcessWindowStyle.Maximized
                    };

                ffProc = Start(startInfo);
            }
            catch (InvalidOperationException ex)
            {
                WriteLine(ex.Message);
            }

            Write("--> Hit enter to kill {0}...", ffProc.ProcessName);
            ReadLine();

            // Kill the FireFox.exe process.
            try
            {
                ffProc.Kill();
            }
            catch (InvalidOperationException ex)
            {
                WriteLine(ex.Message);
            }
        }

    }
}