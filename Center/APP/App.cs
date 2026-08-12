using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Xml.Linq;
using static Center.Structs;
using static Center.WinAPI;

namespace Center
{
    public class App
    {
        public TaskCompletionSource<bool> tcs;
        public event Action<App, TaskCompletionSource<bool>> Canceled;
        public event Action<App, TaskCompletionSource<bool>> Finished;
        public string PO;
        public string MANV;
        //public event Action Finished;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        public string GetClassNameFromHandle(IntPtr handle)
        {
            StringBuilder className = new StringBuilder(256); // Tạo một StringBuilder để lưu ClassName
            GetClassName(handle, className, className.Capacity); // Gọi hàm GetClassName
            return className.ToString(); // Trả về ClassName
        }
        public IntPtr GetControlHandle(IntPtr hWnd,string controlTitle)
        {

            if (hWnd == IntPtr.Zero)
            {
                MessageBox.Show($"Không tìm thấy cửa sổ .");
                return IntPtr.Zero;
            }
            IntPtr controlHandle = IntPtr.Zero;
            // Lặp qua tất cả các control trong cửa sổ
            controlHandle = FindWindowEx(hWnd, controlHandle, "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", "In");
            return controlHandle;
        }
        public void DoubleClick(IntPtr hWnd, double a, double b)
        {
            // Lấy vị trí của cửa sổ
            if (GetWindowRect(hWnd, out RECT rect))
            {
                //MessageBox.Show(rect.Left.ToString() + ":" + rect.Top.ToString() + ":" + rect.Right.ToString() + ":" + rect.Bottom.ToString());
                // Tính tọa độ trung tâm cửa sổ
                int x = (int)((rect.Left + rect.Right) / a);
                int y = (int)((rect.Top + rect.Bottom) / b);
                //MessageBox.Show(x.ToString() + "   " + y.ToString());
                //int x = rect.Right - 200;
                //int y = rect.Bottom - 20;


                // Giả lập nhấn chuột trái
                Cursor.Position = new System.Drawing.Point(x, y);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0); // Lần 1

            }
        }
        public void DoubleClick(IntPtr hWnd, int a, int b)
        {
            // Lấy vị trí của cửa sổ
            if (GetWindowRect(hWnd, out RECT rect))
            {
                
                // Tính tọa độ trung tâm cửa sổ
                int x = rect.Left + a;
                int y = rect.Top + b;

                // Giả lập nhấn chuột trái
                Cursor.Position = new System.Drawing.Point(x, y);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0); // Lần 1
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
            }
        }
        public void Click(IntPtr hWnd, int a, int b)
        {
            // Lấy vị trí của cửa sổ
            if (GetWindowRect(hWnd, out RECT rect))
            {

                int x = rect.Left + a;
                int y = rect.Top + b;
                Cursor.Position = new System.Drawing.Point(x, y);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0); // Lần 1
            }
        }
        public string[] Find_App(string name) // giữ lại
        {
            try
            {
                // Các thư mục cần tìm kiếm
                string[] searchPaths = {
                    //Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
                    Environment.GetFolderPath(Environment.SpecialFolder.Programs)
                };

                // Danh sách tệp tìm thấy
                List<string> allAppFiles = new List<string>();

                // Duyệt qua các thư mục và tìm tệp có chứa "name"
                foreach (string path in searchPaths)
                {
                    var appFiles = Directory.GetFiles(path, "*.appref-ms", SearchOption.AllDirectories)
                                            .Where(f => f.Contains(name))
                                            .ToArray();

                    // Thêm các tệp tìm thấy vào danh sách
                    allAppFiles.AddRange(appFiles);
                }

                return allAppFiles.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
                return new string[0]; // Trả về mảng rỗng khi có lỗi
            }
        }
        public void CloseAppIfRunning(string appName)
        {
            // Lấy danh sách các tiến trình trùng tên
            var processes = Process.GetProcessesByName(appName);

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();  // Đóng tiến trình
                    process.WaitForExit();  // Đợi tiến trình kết thúc
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi đóng ứng dụng: {ex.Message}");
                }
            }
        }
        public IntPtr CheckRunning(string WindowName)
        {
            int a = 0;
            IntPtr h = findHandle(ref a, WindowName);
            return h;
        }
        public IntPtr findHandle(ref int time_find, string windowName)
        {
            IntPtr hWnd = IntPtr.Zero;
            do
            {
                time_find++;
                // Kiểm tra xem cửa sổ đã mở chưa
                Thread.Sleep(500); // Đợi nửa giây trước khi kiểm tra lại
                var windows = FindWindowsStartingWith(windowName); // Tìm các cửa sổ

                // Nếu tìm thấy ít nhất một cửa sổ, lấy handle của cửa sổ đầu tiên
                if (windows.Count > 0)
                {
                    hWnd = windows[0]; // Lấy handle của cửa sổ đầu tiên tìm thấy
                    return hWnd;
                }

            } while (hWnd == IntPtr.Zero && time_find <= 1); // Lặp lại cho đến khi tìm thấy cửa sổ
            if (time_find > 1)
            {
                //MessageBox.Show("Không tìm thấy cửa sổ.");
                return hWnd;
            }
            return hWnd;
        }
        public List<IntPtr> FindWindowsStartingWith(string windowNameStart)
        {
            List<IntPtr> foundWindows = new List<IntPtr>();

            // Gọi EnumWindows và truyền vào hàm kiểm tra
            EnumWindows((hWnd, lParam) =>
            {
                const int nChars = 256;
                StringBuilder titleBuffer = new StringBuilder(nChars);
                GetWindowText(hWnd, titleBuffer, nChars);
                string windowTitle = titleBuffer.ToString();

                // Kiểm tra nếu tiêu đề bắt đầu bằng windowNameStart
                if (windowTitle.StartsWith(windowNameStart, StringComparison.OrdinalIgnoreCase))
                {
                    foundWindows.Add(hWnd); // Thêm cửa sổ vào danh sách
                }
                return true; // Tiếp tục duyệt qua các cửa sổ
            }, IntPtr.Zero);
            return foundWindows; // Trả về danh sách các cửa sổ tìm thấy
        }
        public async Task<bool> OpenApp(string appName, string windowName) 
        {
            IntPtr h = CheckRunning(windowName);
            if (h == IntPtr.Zero)
            {
                string[] appFiles = Find_App(appName);

                if (appFiles.Length > 0)
                {
                    // Mở tệp ứng dụng đầu tiên tìm thấy
                    Process process = Process.Start(appFiles[0]);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ShowWindow(h, 9);
                SetForegroundWindow(h);
            }
            return true;
        }
        public async Task<IntPtr> OpenAndGetHandle(string appName, string windowName)
        {
            int time_find = 0;
            IntPtr h = IntPtr.Zero;
            if (await OpenApp(appName, windowName))
            {
                h = findHandle(ref time_find, windowName);
            }
            return h;
        }
        public virtual async Task Fill_App(IntPtr hWnd, string windowName,string class1, int index1, string class2, int index2, string MSNV, string PO)
        {
            await Task.Delay(50);
            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);

            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);

            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D);


            if (wc.Handle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy textbox MSNV (handle = 0).");
                return;
            }

            AutomationElement element_MaNV = null;
            try
            {
                element_MaNV = AutomationElement.FromHandle(wc.Handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo AutomationElement từ wc.Handle: " + ex.Message);
                return;
            }


            if (element_MaNV != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element_MaNV.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {

                    // Cast object thành ValuePattern
                    ValuePattern valuePattern = (ValuePattern)pattern;

                    // Thay đổi giá trị của TextBox
                    valuePattern.SetValue(MSNV);
                    //await Task.Delay(200);
                    SetForegroundWindow(hWnd);
                    element_MaNV.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                    Debug.Print("Giá trị mới đã được thiết lập.");
                }
                else
                {
                    MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy phần tử.");
            }


            AutomationElement element_PO = AutomationElement.FromHandle(wc2.Handle);

            if (element_PO != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {
                    // Cast object thành ValuePattern
                    ValuePattern valuePattern = (ValuePattern)pattern;

                    // Thay đổi giá trị của TextBox
                    valuePattern.SetValue(PO);
                    //await Task.Delay(200);
                    SetForegroundWindow(hWnd);
                    element_PO.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                    Debug.Print("Giá trị mới đã được thiết lập.");
                }
                else
                {
                    MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy phần tử.");
            }

        }


        public virtual async Task<bool> Fill_App1(IntPtr hWnd, string windowName, string class1, int index1, string class2, int index2, string MSNV, string PO)
        {
            await Task.Delay(50);

            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);

            if (wc.Handle == IntPtr.Zero || wc2.Handle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy một trong hai textbox (handle = 0).");
                return false;
            }

            bool success = true;

            try
            {
                AutomationElement element_MaNV = AutomationElement.FromHandle(wc.Handle);
                if (element_MaNV != null && element_MaNV.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern1))
                {
                    var valuePattern = (ValuePattern)pattern1;
                    valuePattern.SetValue(MSNV);
                    SetForegroundWindow(hWnd);
                    element_MaNV.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                    Debug.Print("MSNV đã được nhập.");
                }
                else
                {
                    MessageBox.Show("Không thể nhập MSNV: không tìm thấy phần tử hoặc không hỗ trợ ValuePattern.");
                    success = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý MSNV: " + ex.Message);
                success = false;
            }

            try
            {
                AutomationElement element_PO = AutomationElement.FromHandle(wc2.Handle);
                if (element_PO != null && element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern2))
                {
                    var valuePattern = (ValuePattern)pattern2;
                    valuePattern.SetValue(PO);
                    SetForegroundWindow(hWnd);
                    element_PO.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                    Debug.Print("PO đã được nhập.");
                }
                else
                {
                    MessageBox.Show("Không thể nhập PO: không tìm thấy phần tử hoặc không hỗ trợ ValuePattern.");
                    success = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xử lý PO: " + ex.Message);
                success = false;
            }

            return success;
        }


        public async void ClickPrintManyTime(IntPtr h, TaskCompletionSource<bool> tcs)
        {
            while (!tcs.Task.IsCompleted)
            {
                ClickPrintButton(h);
                await Task.Delay(2000);
            }
        }


        /// <summary>
        ///  Sử dụng AutomationElement để setvalue cho text box hoặc nhấn button
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="windowName"></param>
        /// <param name="className"></param>
        /// <param name="index"></param>
        /// <param name="PO"></param>
        /// <returns></returns>
        public virtual async Task Fill_App(IntPtr hWnd, string windowName, string className, int index, string PO)
        {
            await Task.Delay(100);
            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, className,index);
            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter
            // Điền dữ liệu vào textbox 1
            AutomationElement element = AutomationElement.FromHandle(wc.Handle);
            if (element != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {
                    // Cast object thành ValuePattern
                    ValuePattern valuePattern = (ValuePattern)pattern;
                    valuePattern.SetValue(PO);
                    SetForegroundWindow(hWnd);
                    element.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                }
                else
                {
                    MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy phần tử.");
            }
            await Task.Delay(150);
        }
        public Action<App, TaskCompletionSource<bool>> GetFinishedHandler()
        {
            return Finished; // Trả về delegate backing field
        }
        public Action<App, TaskCompletionSource<bool>> GetCanceledHandler()
        {
            return Canceled; // Trả về delegate backing field
        }
        public void ClickPrintButton(IntPtr h)
        {
            AutomationElement element_Print = AutomationElement.FromHandle(h);

            if (element_Print != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ InvokePattern không
                if (element_Print.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
                {
                    // Cast object thành InvokePattern
                    InvokePattern invokePattern = (InvokePattern)pattern;
                    // Thực hiện click
                    try { invokePattern.Invoke(); } catch { }
                }
                else
                {
                    MessageBox.Show("Phần tử không hỗ trợ InvokePattern hoặc ValuePattern.");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy phần tử.");
            }
        }
        public virtual async Task WaitPrint(IntPtr h, string classPrintQty, int index_classPrintQty, TaskCompletionSource<bool> tcs, int TemQty)
        {
            WindowInfo info_PrintQty = new WindowInfo();
            int qtyPrint = -10;
            int o = 0;
            while (!tcs.Task.IsCompleted)
            {
                o = o + 1;
                Debug.Print(o.ToString() + "Hahah" + o.ToString());
                info_PrintQty = CaptureItemHandle.GetControlHandle(h, classPrintQty, index_classPrintQty);
                if (info_PrintQty.Caption == "")
                {
                    qtyPrint = 0;
                }
                else
                {
                    int.TryParse(info_PrintQty.Caption, out qtyPrint);
                }
                if (qtyPrint == TemQty)
                {
                    Debug.Print("Đã in tem ");
                    Finish_App(tcs);
                }
                await Task.Delay(2000); // Giảm tải CPU, đợi thêm một chút
            }
        }
        public void Finish_App(TaskCompletionSource<bool> tcs )
        {
            Finished?.Invoke(this,tcs);
        }
        public void Cancel_App(TaskCompletionSource<bool> tcs)
        {
            Canceled?.Invoke(this,tcs);
        }
    }
}
