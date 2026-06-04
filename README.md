WinBooster
A system-level Windows utility built in C# to optimize memory allocation and process priority. Designed to ensure active applications get unrestricted access to physical RAM and CPU cycles.

How It Works
Memory Trimming: Uses EmptyWorkingSet (via psapi.dll) to force non-whitelisted background processes to page their memory out to the disk.

Power Automation: Interacts directly with powrprof.dll to dynamically switch to the High Performance power scheme.

Dynamic Whitelisting: Protects critical system processes and user-defined apps from being suspended or trimmed.

Priority Management: Demotes idle processes to free up CPU scheduler time.

The Engineering Reality (No "Magic" Cleaners)
This tool does not magically "create" RAM. Instead, it utilizes Windows' native memory management. By forcefully pushing inactive memory pages to the standby list/pagefile before launching a heavy application, it prevents mid-session Page Faults, reducing stutters and keeping active apps running smoothly.

Tech Stack
Language: C# (.NET WinForms)

Core Concepts: Win32 API Integration (PInvoke), Process Handle Management, Unmanaged Memory (IntPtr).

Future Roadmap
Implement Deep Process Suspension (Thread-level pausing).

Transition to a Background Service architecture.