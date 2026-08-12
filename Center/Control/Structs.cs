using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Center
{
    public class Structs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPSTRUCT
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
        }


        public struct WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string ClassName { get; set; }
            public string Caption { get; set; }
            public List<WindowInfo> Children { get; set; }

            public WindowInfo(IntPtr handle, string className, string caption)
            {
                Handle = handle;
                ClassName = className;
                Caption = caption;
                Children = new List<WindowInfo>();
            }
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }
    }
}
