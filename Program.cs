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
            int.TryParse(ReadLine(), out int processId);

            EnumThreadsForPid(processId);
            EnumModsForPid(processId);
            StartAndKillProcess();
            ReadLine();
        }
        static void ListAllRunningProcesses()
        {
            // Get all the processes on the local machine, ordered by
            // PID.
            var runningProcesses = from process in GetProcesses(".") orderby process.Id select process;

            // Print out PID and name of each process.
            foreach (Process p in runningProcesses)
                WriteLine($"-> PID: {p.Id}" +
                    $"\tName: {p.ProcessName}");

            WriteLine("************************************\n");
        }

        static Process GetSpecificProcess(int processId)
        {
            Process process = null;
            try
            {
                process = GetProcessById(processId);
            }
            catch (ArgumentException ex)
            {
                WriteLine(ex.Message);
            }
            return process;
        }
        static void EnumThreadsForPid(int processId)
        {
            Process process = GetSpecificProcess(processId);

            // List out stats for each thread in the specified process.
            WriteLine($"Here are the threads used by {process.ProcessName}");

            ProcessThreadCollection threads = process.Threads;

            foreach (ProcessThread thread in threads)
                WriteLine($"-> Thread ID: {thread.Id}" +
                    $"\tStart Time: {thread.StartTime.ToShortTimeString()}" +
                    $"\tPriority: {thread.PriorityLevel}");

            WriteLine("************************************\n");
        }

        static void EnumModsForPid(int processId)
        {
            Process process = GetSpecificProcess(processId);

            WriteLine($"Here are the loaded modules for {process.ProcessName}");

            ProcessModuleCollection modules = process.Modules;
            foreach (ProcessModule module in modules)
                WriteLine($"-> Mod Name: {module.ModuleName}");

            WriteLine("************************************\n");
        }
        static void StartAndKillProcess()
        {
            // Launch Firefox, and go to my site!
            try
            {
                ProcessStartInfo startInfo =
                    new ProcessStartInfo("FireFox.exe", "jacobarchambault.azurewebsites.net")
                    {
                        WindowStyle = ProcessWindowStyle.Maximized
                    };

                Process firefox = Start(startInfo);

                Write($"--> Hit enter to kill {firefox.ProcessName}");
                ReadLine();

                // Kill the FireFox.exe process.
                firefox.Kill();
            }
            catch (InvalidOperationException ex)
            {
                WriteLine(ex.Message);
            }
        }

    }
}