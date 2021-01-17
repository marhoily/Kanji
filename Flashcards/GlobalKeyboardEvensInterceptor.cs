using System;
using System.Runtime.InteropServices;
using DynamicData.Annotations;

namespace WpfApp1
{
    public static class GlobalKeyboardEvensInterceptor
    {
        private enum KeyCode { WinKey = 91 }
        private enum HookType { KeyboardLowLevel = 13 }
        private enum EventType { Keydown = 0x100 }
        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        private struct KeyboardHookParam
        {
#pragma warning disable 649
            public KeyCode KeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int DwExtraInfo;
#pragma warning restore 649
        }

        private delegate int HookProc(int code, EventType wParam, ref KeyboardHookParam lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, EventType  wParam, ref KeyboardHookParam lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private static readonly HookProc HookPtr = Hook;
        public static void SetHook()
        {
            SetWindowsHookEx(HookType.KeyboardLowLevel,
                HookPtr, LoadLibrary("User32"), 0);
        }
   
        private static int Hook(int code, EventType  eventType, ref KeyboardHookParam lParam)
        {
            if (code < 0 || eventType != EventType.Keydown)
            {
                return CallNextHookEx(IntPtr.Zero, code, eventType, ref lParam);
            }

            if (lParam.KeyCode == KeyCode.WinKey) 
                return -1;

            return CallNextHookEx(IntPtr.Zero, code, eventType, ref lParam);
        }
    }
}
