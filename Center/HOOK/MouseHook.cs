using Center.Apps;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Center.Structs;
using static Center.WinAPI;
namespace Center
{
    public class MouseHook
    {
        private LowLevelMouseProc _hookCallbackDelegate;
        private ManualResetEvent _unhookedEvent = new ManualResetEvent(false);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        //private const int WM_LBUTTONDOWN = 0x0246;
        POINT mousePoint;
        private LowLevelMouseProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private IntPtr _hWnd; // Biến để lưu trữ hWnd
        private IntPtr lb; // Biến để lưu trữ hWnd
        string check = "";
        private IntPtr controlHandle;
        private IntPtr controlHandle1;
        private IntPtr controlHandle2;
        private int countClick1;
        private int countClick2;
        App a = new App();
        TaskCompletionSource<bool> tcs;
        //public event Action<App> Finished;

        public void Initialize( IntPtr rootHandle, IntPtr controlHandle,App a, TaskCompletionSource<bool> tcs)
        {
            if (_hookID != IntPtr.Zero)
            {
                Debug.Print("Đã unhook");
                Unhook();
            }
            //_proc = new LowLevelMouseProc(HookCallback);

            _proc = HookCallback_PrintLabel; 
            _hWnd = rootHandle;           
            this.controlHandle = controlHandle;
            this.a = a;
            this.tcs = tcs;
            Hook();
            //_unhookedEvent.WaitOne();
        }
        public void Initialize(IntPtr rootHandle, IntPtr controlHandle1,IntPtr controlHandle2, App a, TaskCompletionSource<bool> tcs)
        {
            if (_hookID != IntPtr.Zero)
            {
                Debug.Print("Đã unhook");
                Unhook();
            }
            //_proc = new LowLevelMouseProc(HookCallback);

            _proc = HookCallback_SprueBush_PCS_QRCode;
            _hWnd = rootHandle;
            this.controlHandle1 = controlHandle1;
            this.controlHandle2 = controlHandle2;
            this.a = a;
            this.tcs = tcs;
            countClick1 = 0;
            countClick2 = 0;
            Hook();
            //_unhookedEvent.WaitOne();
        }

        public MouseHook() { }

        public void Hook()
        {
            _hookID = SetHook(_proc);

            //_hookID = SetHook(_hookCallbackDelegate);
        }
        public void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
            countClick1 = 0;
            countClick2 = 0;
            // Gọi Set để tín hiệu sự kiện, thông báo rằng đã unhook
            //_unhookedEvent.Set();
        }
        private IntPtr SetHook(LowLevelMouseProc proc )
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private POINT GetMousePosition()
        {
            // Tạo biến kiểu POINT để chứa tọa độ
            POINT mousePosition;

            // Lấy vị trí chuột
            if (GetCursorPos(out mousePosition))
            {
                // Hiển thị tọa độ chuột
                return mousePosition;
            }
            else
            {
                MessageBox.Show("Không thể lấy tọa độ chuột.");
            }
            return mousePosition;
        }
        private IntPtr HookCallback_PrintLabel(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                Debug.Print("ọaoshdiasdoas");
                ClickPrintLabel(_hWnd,controlHandle);

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        private IntPtr HookCallback_SprueBush_PCS_QRCode(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                ClickPrintINandOUT(_hWnd, controlHandle1,controlHandle2);

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        private bool IsPointInSquare(POINT point, int squareX, int squareY, int squareSize)
        {
            return (point.x >= squareX && point.x <= squareX + squareSize &&
                    point.y >= squareY && point.y <= squareY + squareSize);
        }

        private bool IsPointInRectangle(POINT point, int rectX, int rectY, int rectWidth, int rectHeight)
        {
            return (point.x >= rectX && point.x <= rectX + rectWidth &&
                    point.y >= rectY && point.y <= rectY + rectHeight);
        }

        //Hàm FindXY dùng để tính số để độ vào code
        public void FindXY(POINT mouse) //R tọa độ của window , x là tọa độ đo được của điểm cần nhấp trong paint
        {
            if (mouse.x != 0 && mouse.y != 0)
            {
                POINT new_Mouse = GetMousePosition();
                int newX = new_Mouse.x - mouse.x;
                int newY = new_Mouse.y - mouse.y;
                Debug.Print("new X: " + newX.ToString() + " new Y: " + newY.ToString());
            }
        }
        public void FindXY(RECT mouse) //R tọa độ của window , x là tọa độ đo được của điểm cần nhấp trong paint
        {
            POINT new_Mouse = GetMousePosition();
            int newX = new_Mouse.x - mouse.Left;
            int newY = new_Mouse.y - mouse.Top;
            Debug.Print("new X: " + newX.ToString() + " new Y: " + newY.ToString());
        }
        public RECT GetFactRECT(RECT parentRect,RECT childRect)
        {
            int newChildLeft = childRect.Left;   // Cạnh trái của con
            int newChildTop = childRect.Top;     // Cạnh trên của con
            int newChildRight = childRect.Right; // Cạnh phải của con
            int newChildBottom = childRect.Bottom; // Cạnh dưới của con

            // Nếu kích thước của cửa sổ cha đã thay đổi
            if (parentRect.Right < newChildRight)
            {
                newChildRight = parentRect.Right;
            }
            if (parentRect.Bottom < newChildBottom)
            {
                newChildBottom = parentRect.Bottom;
            }
            if (parentRect.Left > newChildLeft)
            {
                newChildLeft = parentRect.Left; // Cập nhật cạnh trái
            }
            if (parentRect.Top > newChildTop)
            {
                newChildTop = parentRect.Top; // Cập nhật cạnh trên
            }
            RECT a = new RECT();
            a.Left = newChildLeft;
            a.Top = newChildTop;
            a.Right = newChildRight;
            a.Bottom = newChildBottom;
            return a;
        }
        public async void ClickPrintLabel(IntPtr h, IntPtr control)
        {
            mousePoint = GetMousePosition();

            RECT parentRect;
            RECT childRect;
            if (GetWindowRect(h, out parentRect) && GetWindowRect(control, out childRect))
            {
                RECT factRect = GetFactRECT(parentRect, childRect);

                int W = factRect.Right - factRect.Left;
                int H = factRect.Bottom - factRect.Top;

                // Kiểm tra xem chuột có nằm trong hình vuông không
                if (IsPointInRectangle(mousePoint, factRect.Left, factRect.Top, W, H))
                {
                    await Task.Delay(100);
                    IntPtr foregroundWindow = GetForegroundWindow();
                    if (h.ToString() == foregroundWindow.ToString() )
                    {
                        Debug.Print("Đã bắt được click chuột");

                        a.Finish_App(tcs);
                        Unhook();

                    }
                }
            }
        }
        public async void ClickPrintINandOUT(IntPtr h, IntPtr control1, IntPtr control2)
        {
            mousePoint = GetMousePosition();

            RECT parentRect;
            RECT childRect1;
            RECT childRect2;
            if (GetWindowRect(h, out parentRect) && GetWindowRect(control1, out childRect1) && GetWindowRect(control2, out childRect2))
            {
                RECT factRect1 = GetFactRECT(parentRect, childRect1);
                RECT factRect2 = GetFactRECT(parentRect, childRect2);
                int W1 = factRect1.Right - factRect1.Left;
                int H1 = factRect1.Bottom - factRect1.Top;
                int W2 = factRect2.Right - factRect2.Left;
                int H2 = factRect2.Bottom - factRect2.Top;
                // Kiểm tra xem chuột có nằm trong hình vuông không
                if (IsPointInRectangle(mousePoint, factRect1.Left, factRect1.Top, W1, H1))
                {
                    IntPtr foregroundWindow = GetForegroundWindow();
                    await Task.Delay(100);
                    
                    if (h.ToString() == foregroundWindow.ToString() )
                    {
                        if (countClick2 > 0)
                        {
                            a.Finish_App(tcs);
                            Unhook();
                        }
                        countClick1++;
                    }
                }
                else if (IsPointInRectangle(mousePoint, factRect2.Left, factRect2.Top, W2, H2))
                {
                    IntPtr foregroundWindow = GetForegroundWindow();
                    await Task.Delay(100);
                    if (h.ToString() == foregroundWindow.ToString())
                    {
                        if (countClick1 > 0)
                        {
                            a.Finish_App(tcs);
                            Unhook();
                        }
                        countClick2++;

                    }
                }
            }
            
        }

    }
}
