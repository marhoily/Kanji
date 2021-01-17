using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public static class GlobalKeyboardEvensInterceptor
    {
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KeyboardHookParam
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private delegate int HookProc(int code, IntPtr wParam, ref KeyboardHookParam lParam);
        private static HookProc myCallbackDelegate = null;

        public static void SetHook()
        {
            myCallbackDelegate = new HookProc(MyCallbackFunction);

            IntPtr hInstance = LoadLibrary("User32");
            SetWindowsHookEx(HookType.WH_KEYBOARD_LL,
                myCallbackDelegate, hInstance, 0);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KeyboardHookParam lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши

        private static int MyCallbackFunction(int code, IntPtr wParam, ref KeyboardHookParam lParam)
        {
            if (code < 0)
            {
                //you need to call CallNextHookEx without further processing
                //and return the value returned by CallNextHookEx
                return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }
            if (wParam != (IntPtr)WM_KEYDOWN)
                return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);

            // we can convert the 2nd parameter (the key code) to a System.Windows.Forms.Keys enum constant
            var keyPressed = lParam.vkCode;
            if (keyPressed == 91)
                return -1;
            Console.WriteLine(keyPressed);
            //return the value returned by CallNextHookEx
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }
    }
}
