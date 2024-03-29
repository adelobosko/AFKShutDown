﻿using System;
using System.Runtime.InteropServices;

namespace ShutDown
{
    internal static class WinAPI
    {
        public static class Advapi32
        {
            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool InitiateSystemShutdownEx(
                string lpMachineName,
                string lpMessage,
                uint dwTimeout,
                bool bForceAppsClosed,
                bool bRebootAfterShutdown,
                ShutdownReason dwReason);

            [Flags]
            public enum ShutdownReason : uint
            {
                // Microsoft major reasons.
                SHTDN_REASON_MAJOR_OTHER = 0x00000000,
                SHTDN_REASON_MAJOR_NONE = 0x00000000,
                SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,
                SHTDN_REASON_MAJOR_OPERATINGSYSTEM = 0x00020000,
                SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,
                SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,
                SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,
                SHTDN_REASON_MAJOR_POWER = 0x00060000,
                SHTDN_REASON_MAJOR_LEGACY_API = 0x00070000,

                // Microsoft minor reasons.
                SHTDN_REASON_MINOR_OTHER = 0x00000000,
                SHTDN_REASON_MINOR_NONE = 0x000000ff,
                SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,
                SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,
                SHTDN_REASON_MINOR_UPGRADE = 0x00000003,
                SHTDN_REASON_MINOR_RECONFIG = 0x00000004,
                SHTDN_REASON_MINOR_HUNG = 0x00000005,
                SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,
                SHTDN_REASON_MINOR_DISK = 0x00000007,
                SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,
                SHTDN_REASON_MINOR_NETWORKCARD = 0x00000000,
                SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000a,
                SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000b,
                SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000c,
                SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000d,
                SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000e,
                SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,
                SHTDN_REASON_MINOR_SERVICEPACK = 0x00000010,
                SHTDN_REASON_MINOR_HOTFIX = 0x00000011,
                SHTDN_REASON_MINOR_SECURITYFIX = 0x00000012,
                SHTDN_REASON_MINOR_SECURITY = 0x00000013,
                SHTDN_REASON_MINOR_NETWORK_CONNECTIVITY = 0x00000014,
                SHTDN_REASON_MINOR_WMI = 0x00000015,
                SHTDN_REASON_MINOR_SERVICEPACK_UNINSTALL = 0x00000016,
                SHTDN_REASON_MINOR_HOTFIX_UNINSTALL = 0x00000017,
                SHTDN_REASON_MINOR_SECURITYFIX_UNINSTALL = 0x00000018,
                SHTDN_REASON_MINOR_MMC = 0x00000019,
                SHTDN_REASON_MINOR_TERMSRV = 0x00000020,

                // Flags that end up in the event log code.
                SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,
                SHTDN_REASON_FLAG_PLANNED = 0x80000000,
                SHTDN_REASON_UNKNOWN = SHTDN_REASON_MINOR_NONE,
                SHTDN_REASON_LEGACY_API = (SHTDN_REASON_MAJOR_LEGACY_API | SHTDN_REASON_FLAG_PLANNED),

                // This mask cuts out UI flags.
                SHTDN_REASON_VALID_BIT_MASK = 0xc0ffffff
            }
        }

        public static class TaskbarProgress
        {
            public enum TaskbarStates
            {
                NoProgress = 0,
                Indeterminate = 0x1,
                Normal = 0x2,
                Error = 0x4,
                Paused = 0x8
            }

            [ComImport()]
            [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
            [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface ITaskbarList3
            {
                // ITaskbarList
                [PreserveSig]
                void HrInit();
                [PreserveSig]
                void AddTab(IntPtr hwnd);
                [PreserveSig]
                void DeleteTab(IntPtr hwnd);
                [PreserveSig]
                void ActivateTab(IntPtr hwnd);
                [PreserveSig]
                void SetActiveAlt(IntPtr hwnd);

                // ITaskbarList2
                [PreserveSig]
                void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

                // ITaskbarList3
                [PreserveSig]
                void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
                [PreserveSig]
                void SetProgressState(IntPtr hwnd, TaskbarStates state);
            }

            [ComImport()]
            [Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
            [ClassInterface(ClassInterfaceType.None)]
            private class TaskbarInstance
            {
            }
            private static ITaskbarList3 taskbarInstance;
            private static bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

            public static void SetState(IntPtr windowHandle, TaskbarStates taskbarState)
            {
                taskbarInstance = (ITaskbarList3)new TaskbarInstance();
                if (taskbarSupported) taskbarInstance.SetProgressState(windowHandle, taskbarState);
            }

            public static void SetValue(IntPtr windowHandle, double progressValue, double progressMax)
            {
                taskbarInstance = (ITaskbarList3)new TaskbarInstance();
                if (taskbarSupported) taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
            }
        }
    }
}