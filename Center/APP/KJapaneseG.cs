using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using static Center.Structs;
using static Center.WinAPI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
namespace Center.Apps
{
    internal class KJapaneseG : App
    {
        int check_MANV = 0;
        public KJapaneseG() {  }
        public void SetUp_KJapaneseG(string MANV,string PO) { this.MANV = MANV; this.PO = PO; }


        public void OpenStar()
        {
            string[] appFiles = Find_App("K情報技術G");
            if (appFiles.Length > 0)
            {
                bool isRunning = Process.GetProcessesByName("WorkSupportSystem").Any();

                if (!isRunning)
                {
                    try
                    {
                        Process.Start(appFiles[0]);
                    }
                    catch
                    {
                        try
                        {
                            Process.Start(appFiles[1]);
                        }
                        catch
                        {
                            try
                            {
                                Process.Start(appFiles[1]);
                            }
                            catch { }
                        }
                        
                    }
                    
                }
            }
        }
     

        public async void Fill_Star(string MaNV, string PO, IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 9);
                SetForegroundWindow(hWnd);

                //WindowInfo wi = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 0);
                //if (wi.Handle == IntPtr.Zero)
                //{
                //    wi = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.STATIC.app.0.34f5582_r7_ad1", 0);
                //    if (wi.Handle == IntPtr.Zero)
                //    {
                //        wi = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r9_ad1", 0);
                //    }
                //} 

                WindowInfo wi = CaptureItemHandle.GetControlHandleByClassPartial(hWnd, "EDIT.app", 0);
                if (wi.Handle == IntPtr.Zero)
                {
                    Debug.Print("Không tìm thấy control EDIT.app tại index 0.");
                    return;
                }


                uint WM_SETTEXT = 0x000C;
                uint WM_KEYDOWN = 0x0100;
                uint WM_KEYUP = 0x0101;
                IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter
                if (check_MANV == 0)
                {
                    AutomationElement element_MANV = AutomationElement.FromHandle(wi.Handle);

                    if (element_MANV != null)
                    {
                        try
                        {
                            if (!element_MANV.Current.IsEnabled)
                            {
                                Debug.Print("Phần tử MANV không được bật (IsEnabled = false). Bỏ qua.");
                            }
                            else if (element_MANV.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternObject))
                            {
                                ValuePattern valuePattern = (ValuePattern)valuePatternObject;
                                valuePattern.SetValue(MaNV);
                                Thread.Sleep(100);
                                SendKeys.SendWait("{ENTER}");
                                Debug.Print("Giá trị mới đã được thiết lập.");
                            }
                            else
                            {
                                Debug.Print("Phần tử MANV không hỗ trợ ValuePattern.");
                            }
                        }
                        catch (ElementNotEnabledException enex)
                        {
                            Debug.Print("ElementNotEnabledException khi set MaNV: " + enex.Message);
                        }
                        catch (Exception ex)
                        {
                            Debug.Print("Lỗi khác khi nhập MaNV: " + ex.Message);
                        }
                    }
                    else
                    {
                        Debug.Print("Không tìm thấy phần tử MANV.");
                    }

                    check_MANV = 1;
                }

                // Điền PO
                WindowInfo wi2 = CaptureItemHandle.GetControlHandleByClassPartial(hWnd, "EDIT.app", 0);
                if (wi2.Handle == IntPtr.Zero)
                {
                    Debug.Print("Không tìm thấy control EDIT.app tại index 1.");
                    return;
                }

                AutomationElement element_PO = AutomationElement.FromHandle(wi2.Handle);
                //AutomationElement element_PO = AutomationElement.FromHandle(wi.Handle);
                if (element_PO != null)
                {
                    try
                    {
                        if (!element_PO.Current.IsEnabled)
                        {
                            Debug.Print("Phần tử PO không được bật (IsEnabled = false). Bỏ qua.");
                        }
                        else if (element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternObject))
                        {
                            Debug.Print($"[Fill_Star] Điền PO: {PO}");
                            ValuePattern valuePattern = (ValuePattern)valuePatternObject;
                            valuePattern.SetValue(PO);
                            SendKeys.SendWait("{ENTER}");
                            Debug.Print("[Fill_Star] PO đã được thiết lập.");
                        }
                        else
                        {
                            Debug.Print("Phần tử PO không hỗ trợ ValuePattern.");
                        }
                    }
                    catch (ElementNotEnabledException enex)
                    {
                        Debug.Print("ElementNotEnabledException khi set PO: " + enex.Message);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Lỗi khác khi nhập PO: " + ex.Message);
                    }
                }
                else
                {
                    Debug.Print("Không tìm thấy phần tử PO.");
                }

            }
        }

        public IntPtr ShowStar()
        {
            IntPtr i = IntPtr.Zero;

            Process[] processes = Process.GetProcesses();



            foreach (Process process in processes)
            {
                try
                {

                    if (process.ProcessName == "WorkSupportSystem")
                    {
                        i = process.MainWindowHandle;
                    }
                }
                catch
                {
                    // Bỏ qua nếu không thể truy cập vào tiến trình
                }
            }

            return i;
        }

        public bool Start_Star( )
        {
            OpenStar();
            IntPtr h = ShowStar();
            if (h == IntPtr.Zero)
            {
                return false;
            }
            try
            {
                Fill_Star(MANV,PO, h);
            }
            catch { }
            Finish_Star();

            return true;
        }

        public void Finish_Star()
        {
            Debug.Print("Finish App Star");
            Finish_App(GlobalVariables.tcs_KJapaneseG); // Thông báo hoàn thành
        }
    }

}
