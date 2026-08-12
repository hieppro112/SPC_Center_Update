using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Center.WinAPI;
using static Center.Structs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Runtime.InteropServices;
namespace Center
{
    internal class CaptureItemHandle
    {
        // Hàm lấy thông tin cửa sổ



        public static WindowInfo GetWindowInfo(IntPtr hWnd)
        {
            StringBuilder className = new StringBuilder(256);
            StringBuilder caption = new StringBuilder(256);
            string cap = "";
            GetClassName(hWnd, className, className.Capacity);
            
            GetWindowText(hWnd, caption, caption.Capacity);

            if (caption.ToString() == "")
            {
                cap = GetCaptionOfHandle(hWnd);
            }else
            {
                cap = caption.ToString();
            }
  
            return new WindowInfo
            {
                Handle = hWnd,
                ClassName = className.ToString(),
                Caption = cap,
                Children = new List<WindowInfo>()
            };
        }


        public static List<WindowInfo> EnumerateChildWindows(WindowInfo parentInfo)
        {
            var childWindows = new List<WindowInfo>();

            EnumChildWindows(parentInfo.Handle, (hWnd, lParam) =>
            {
                var childInfo = GetWindowInfo(hWnd);
                childWindows.Add(childInfo);
                return true;
            }, IntPtr.Zero);

            return childWindows;
        }

        public static void DisplayWindowTree(WindowInfo window, int level = 0)
        {
            string indent = new string(' ', level * 4); // Thụt đầu dòng theo cấp độ cây
            Debug.Print($"{indent}Handle: {window.Handle}, Class: {window.ClassName}, Caption: {window.Caption}");

            foreach (var child in window.Children)
            {
                DisplayWindowTree(child, level + 1); // Hiển thị các cửa sổ con
            }
        }
        public static List<WindowInfo> EnumerateChildWindows(WindowInfo parentInfo, string className, string caption)
        {
            var childWindows = new List<WindowInfo>();

            EnumChildWindows(parentInfo.Handle, (hWnd, lParam) =>
            {
                var child = GetWindowInfo(hWnd);
                if (child.ClassName.StartsWith(className) && child.Caption == caption)
                {
                    childWindows.Add(child);
                }

                return true;
            }, IntPtr.Zero);

            return childWindows;
        }
        public static List<WindowInfo> EnumerateChildWindows(WindowInfo parentInfo, string className)
        {
            var childWindows = new List<WindowInfo>();
            int o = 0;
            EnumChildWindows(parentInfo.Handle, (hWnd, lParam) =>
            {
                var child = GetWindowInfo(hWnd);
                //const int WM_GETTEXT = 0x000D;
                //StringBuilder caption = new StringBuilder(256); // Hoặc điều chỉnh kích thước nếu cần
                //SendMessage(child.Handle, WM_GETTEXT, caption.Capacity, caption);
                //if (caption.ToString() == "")
                //{
                //    Debug.Print("1");
                //}else { Debug.Print(caption.ToString()); }
                //MessageBox.Show(child.ClassName + " index : " + o.ToString() + " caption " + child.Caption);
                //o++;
                if (child.ClassName.StartsWith(className) )
                {

                    childWindows.Add(child);
                    //MessageBox.Show(child.ClassName + " index : " + o.ToString() + " caption " + child.Caption);
                    //o++;
                }

                return true;
            }, IntPtr.Zero);

            return childWindows;
        }
        public static WindowInfo GetControlHandle(IntPtr rH, string caption, string classname, int stt) // stt phàn tử con trong cây bắt đầu từ 0
        {
            
            WindowInfo root = GetWindowInfo(rH);
            
            List<WindowInfo> lW = EnumerateChildWindows(root, classname, caption);
            if (lW.Count() > 0)
            {
                return lW[stt];
            }
            else {  }

            return new WindowInfo();

        }

        public static WindowInfo GetControlHandle(IntPtr rH, string classname, int stt)
        {
            WindowInfo root = GetWindowInfo(rH);

            List<WindowInfo> lW = EnumerateChildWindows(root, classname);
            if (lW.Count > stt)
            {
                return lW[stt];
            }

            // Không tìm thấy, trả về handle rỗng (phải xử lý ở chỗ gọi)
            return new WindowInfo { Handle = IntPtr.Zero };
        }

        // Check xem có xuất hiện messagebox báo lỗi không 
        //Lấy các handle có className nhất định
        public static List<IntPtr> FindWindowsByClass(string className)
        {
            List<IntPtr> handles = new List<IntPtr>();
            EnumWindows((hWnd, lParam) =>
            {
                
                StringBuilder classText = new StringBuilder(256);
                GetClassName(hWnd, classText, classText.Capacity);
                if (classText.ToString() == className)
                {
                    handles.Add(hWnd);
                }
                return true; // Tiếp tục liệt kê các cửa sổ khác
            }, IntPtr.Zero);

            return handles;
        }



        //class name này là class name cha ví dụ #32770 (Dialog)
        public static bool isErrMess(string className, string ErrContent)
        {
            List<IntPtr> handles = FindWindowsByClass(className); // Lấy tất cả các handle với className
            bool found = false;
            foreach (IntPtr handle in handles)
            {
               if (found) break; // Dừng vòng lặp nếu đã tìm thấy

                WindowInfo wi = GetWindowInfo(handle);
                if (wi.Caption == ErrContent)
                {
                    return true;    
                }
            }

            return found;
        }


        public static string GetCaptionOfHandle(IntPtr hWnd)
        {
            const int WM_GETTEXT = 0x000D;
            StringBuilder caption = new StringBuilder(256); // Hoặc điều chỉnh kích thước nếu cần
            SendMessage(hWnd, WM_GETTEXT, caption.Capacity, caption);
            return caption.ToString();
        }

        public static void sendCtrTab(IntPtr hWnd)
        {
            SendKeys.SendWait("^{TAB}");
        }


        public static List<IntPtr> FindHandlebyCaption(string caption)
        {
            List<IntPtr> handles = new List<IntPtr>();

            // Duyệt qua tất cả các cửa sổ
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd)) // Chỉ kiểm tra cửa sổ hiển thị
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hWnd, windowText, 256);

                    // Kiểm tra nếu caption khớp
                    if (windowText.ToString().Equals(caption, StringComparison.OrdinalIgnoreCase))
                    {
                        handles.Add(hWnd);
                    }

                    // Duyệt qua các cửa sổ con của cửa sổ chính
                    EnumChildWindows(hWnd, (childHWnd, childLParam) =>
                    {
                        StringBuilder childWindowText = new StringBuilder(256);
                        GetWindowText(childHWnd, childWindowText, 256);

                        // Kiểm tra nếu caption khớp
                        if (childWindowText.ToString().Equals(caption, StringComparison.OrdinalIgnoreCase))
                        {
                            handles.Add(childHWnd);
                        }
                        return true; // Tiếp tục duyệt các cửa sổ con khác
                    }, IntPtr.Zero);
                }
                return true; // Tiếp tục duyệt qua các cửa sổ khác
            }, IntPtr.Zero);

            return handles;
        }

        public static IntPtr FindParentHandleByChildCaption(string caption)
        {
            IntPtr handle = IntPtr.Zero;

            bool found = false; // Biến để kiểm tra nếu tìm thấy

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd)) // Chỉ kiểm tra cửa sổ hiển thị
                {
                    // Duyệt qua các cửa sổ con của cửa sổ chính
                    EnumChildWindows(hWnd, (childHWnd, childLParam) =>
                    {
                        StringBuilder childWindowText = new StringBuilder(256);
                        GetWindowText(childHWnd, childWindowText, 256);

                        // Kiểm tra nếu caption khớp
                        if (childWindowText.ToString().Equals(caption, StringComparison.OrdinalIgnoreCase))
                        {
                            handle = childHWnd;
                            found = true; // Đánh dấu là đã tìm thấy
                            return false; // Dừng duyệt các cửa sổ con
                        }
                        return true; // Tiếp tục duyệt các cửa sổ con khác
                    }, IntPtr.Zero);

                    if (found) return false; // Dừng duyệt các cửa sổ chính nếu đã tìm thấy
                }
                return true; // Tiếp tục duyệt qua các cửa sổ khác
            }, IntPtr.Zero);

            return handle;
        }

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static WindowInfo GetControlHandleByClassPartial(IntPtr parentHandle, string partialClassName, int index)
        {
            List<WindowInfo> found = new List<WindowInfo>();
            EnumChildWindows(parentHandle, (hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                GetClassName(hWnd, sb, sb.Capacity);
                if (sb.ToString().Contains(partialClassName))
                    found.Add(new WindowInfo { Handle = hWnd });
                return true;
            }, IntPtr.Zero);

            if (found.Count > index)
                return found[index];
            else
                return default(WindowInfo);
        }

        public static WindowInfo GetControlHandleByClassAndText(IntPtr parentHandle, string partialClassName, string buttonText)
        {
            WindowInfo result = default;
            EnumChildWindows(parentHandle, (hWnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                StringBuilder text = new StringBuilder(256);
                GetWindowText(hWnd, text, text.Capacity);

                if (className.ToString().Contains(partialClassName) && text.ToString().Trim() == buttonText)
                {
                    result = new WindowInfo { Handle = hWnd };
                    return false; // dừng lại ngay khi tìm được
                }
                return true;
            }, IntPtr.Zero);
            return result;
        }

        public static string GetClassNameByPartial(IntPtr parentHandle, string partialClassName, int index)
        {
            List<string> matches = new List<string>();
            EnumChildWindows(parentHandle, (hWnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                if (className.ToString().Contains(partialClassName))
                    matches.Add(className.ToString());
                return true;
            }, IntPtr.Zero);

            return (matches.Count > index) ? matches[index] : string.Empty;
        }


    }




}
