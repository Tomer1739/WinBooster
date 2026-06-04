using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//TODO - Program - Add better whitelist, more processes to close, and power plan

public class Booster{
    private Dictionary<Process, ProcessPriorityClass> boostedProcesses = new Dictionary<Process, ProcessPriorityClass>();
    private List<string> whiteList = new List<string>();
    private Guid originalGuid;
    private Guid highGuid = Guid.Parse("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
    // Importing Windows API(system functions) to force RAM clearance to the pagefile, and change performance mode
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hwProc);
    [DllImport("powrprof.dll")]
    static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, out IntPtr ActivePolicyGuid);
    [DllImport("powrprof.dll")]
    static extern uint PowerSetActiveScheme(IntPtr UserRootPowerKey, ref Guid SchemeGuid);


    public Booster()
    {
        File.WriteAllText("logErrorsBooster.txt", "Error log: \r\n");
        IntPtr pActiveGuid = IntPtr.Zero;
        uint result = PowerGetActiveScheme(IntPtr.Zero, out pActiveGuid);

        if (result == 0 && pActiveGuid != IntPtr.Zero)
        {
            originalGuid = Marshal.PtrToStructure<Guid>(pActiveGuid);
        }

        try {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path += "whitelist.txt";

            if (File.Exists(path)) {
                whiteList.AddRange(File.ReadAllLines(path));
            }
        }
        catch(Exception ex) { 
            File.AppendAllText("logErrorsBooster.txt", "Could not open whitelist file, error message: \r\n" + ex.Message);

            MessageBox.Show("Couldn't open whitelist file. Closing booster");

            Environment.Exit(1);
        }
    }

    public string StartBooster(){
        long clearedRam = 0;

        List<Process> close = ProcessesToClose();

        PowerSetActiveScheme(IntPtr.Zero, ref highGuid);

        (boostedProcesses, clearedRam) = CloseProcesses(close);

        clearedRam = (clearedRam / 1024) / 1024;//convert from bytes to MB

        return $"System Boosted, RAM Cleared = {clearedRam} MB, Processes put to sleep = {boostedProcesses.Count} :)";
    }

    public string CloseAndRestore(){
        foreach (var entry in boostedProcesses){
            try{
                entry.Key.PriorityClass = entry.Value;
            }
            catch (Exception ex)
            {
                File.AppendAllText("logErrorsBooster.txt", "Could not change priority for procces " + entry.Key.ProcessName + "\r\nerror message: " + ex.Message);
            }
        }

        boostedProcesses.Clear();

        return "System Restored :)";
    }

    private List<Process> ProcessesToClose(){
        Process[] processes = Process.GetProcesses();
        List<Process> toClose = new List<Process>();
        string set = "";
        int counter = 0;

        foreach (Process p in processes){
            string? path = "";
            try{
                path = p.MainModule?.FileName;
                if(path != null && !whiteList.Any(word => path.Contains(word)))
                {
                    toClose.Add(p);
                    set += "***Name*** = " + p.ProcessName;
                    set += "***MB*** = " + (p.WorkingSet64 / (1024 * 1024)).ToString() + "\r\n";
                }
            }
            catch (Exception ex)
            {
                counter++;
            }
        }

        File.AppendAllText("logErrorsBooster.txt", "Access denied to: " + counter + " Processes \r\n");


        File.WriteAllText("logBeforeBoost.txt", set);

        return toClose;
    }

    private (Dictionary<Process, ProcessPriorityClass>, long clearedRam) CloseProcesses(List<Process> toClose){
        Dictionary<Process, ProcessPriorityClass> closed = new Dictionary<Process, ProcessPriorityClass>();
        long clearedRam = 0;
        string set = "";
        int counter = 0;

        foreach(Process p in toClose){
            try{
                closed.Add(p, p.PriorityClass);
                clearedRam += p.WorkingSet64;
                p.PriorityClass = ProcessPriorityClass.Idle;
                EmptyWorkingSet(p.Handle);
                p.Refresh();
                set += "***Name*** = " + p.ProcessName;
                set += "***MB*** = " + (p.WorkingSet64 / (1024 * 1024)).ToString() + "\r\n";
            }
            catch (Exception ex)
            {
                counter++;
            }
        }

        File.AppendAllText("logErrorsBooster.txt", "Failed to close: " + counter + " Processes \r\n");

        File.WriteAllText("logAfterBoost.txt", set);

        return (closed, clearedRam);
    }
}
