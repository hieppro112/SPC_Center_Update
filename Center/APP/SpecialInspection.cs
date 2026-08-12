using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;
using System.Windows.Forms;
using static Center.WinAPI;
using static Center.CaptureItemHandle;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Diagnostics;
namespace Center.APP
{
    internal class SpecialInspection : App
    {



        public SpecialInspection() { }
        public void SetUp_SpecialInspection(string MANV, string PO)
        {
            this.MANV = MANV;
            this.PO = PO;
        }
        public async Task Fill_InspectionSpecialMaterial_Manufa2(string MSNV, string PO)
        {
            //-------------
            string windowName = "Inspection";
            int time_find = 0;
            IntPtr hWnd = findHandle(ref time_find, windowName);
            if (hWnd == IntPtr.Zero)
            {
                return;
            }
            ShowWindow(hWnd, 9);
            SetForegroundWindow(hWnd);
            //--------------


            WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.Window.b.app.0.34f5582_r8_ad1", 4);
            AutomationElement element_Clear = AutomationElement.FromHandle(Clear.Handle);

            if (element_Clear != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ InvokePattern không
                if (element_Clear.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
                {
                    // Cast object thành InvokePattern
                    InvokePattern invokePattern = (InvokePattern)pattern;

                    // Thực hiện click
                    invokePattern.Invoke();
                    Debug.Print("Phần tử đã được click.");
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




            if (!string.IsNullOrEmpty(MSNV))
            {
                SendKeys.SendWait(MSNV);
                await Task.Delay(200);
                SendKeys.SendWait("{ENTER}");
                await Task.Delay(200);
            }
            if (!string.IsNullOrEmpty(PO))
            {
                SendKeys.SendWait(PO);
                await Task.Delay(200);
                SendKeys.SendWait("{ENTER}");
            }
        }

        public async Task Fill_InspectionSpecialMaterial_Manufa(string MSNV, string PO)
        {
            //-------------
            string windowName = "Inspection";
            int time_find = 0;
            IntPtr hWnd = findHandle(ref time_find, windowName);
            if (hWnd == IntPtr.Zero)
            {
                return;
            }
            ShowWindow(hWnd, 9);
            SetForegroundWindow(hWnd);
            //--------------




            WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.Window.b.app.0.34f5582_r8_ad1", 4);
            AutomationElement element_Clear = AutomationElement.FromHandle(Clear.Handle);

            if (element_Clear != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ InvokePattern không
                if (element_Clear.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
                {
                    // Cast object thành InvokePattern
                    InvokePattern invokePattern = (InvokePattern)pattern;

                    // Thực hiện click
                    invokePattern.Invoke();
                    Debug.Print("Phần tử đã được click.");
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






            // Kích hoạt cửa sổ ứng dụng
            await Task.Delay(200);
            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 3);

            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter

            AutomationElement element_MaNV = AutomationElement.FromHandle(wc.Handle);

            if (element_MaNV != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element_MaNV.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {
                    // Cast object thành ValuePattern
                    ValuePattern valuePattern = (ValuePattern)pattern;

                    // Thay đổi giá trị của TextBox
                    valuePattern.SetValue(MSNV);
                    await Task.Delay(200);
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


            //// Điền dữ liệu vào textbox 1
            //IntPtr ptrMSNV = Marshal.StringToHGlobalUni(MSNV); // Sử dụng Unicode
            //SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);


            //SendMessage(wc.Handle, WM_KEYDOWN, VK_ENTER, IntPtr.Zero);  // Nhấn phím Enter
            //SendMessage(wc.Handle, WM_KEYUP, VK_ENTER, IntPtr.Zero);    // Nhả phím Enter

            // Điền dữ liệu vào textbox 2
            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 4);
            AutomationElement element_PO = AutomationElement.FromHandle(wc2.Handle);

            if (element_PO != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {
                    // Cast object thành ValuePattern
                    ValuePattern valuePattern = (ValuePattern)pattern;

                    // Thay đổi giá trị của TextBox
                    try
                    {
                        valuePattern.SetValue(PO);
                        await Task.Delay(200);
                        SetForegroundWindow(hWnd);
                        element_PO.SetFocus();
                        SendKeys.SendWait("{ENTER}");
                        Debug.Print("Giá trị mới đã được thiết lập.");
                    }
                    catch { }

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

        public async Task<bool> Start_SpecialInspection()
        {
            string appName = "InspectionSpecialMaterial_Manufa";
            if(await OpenApp(appName, "Inspection"))
            {
                try
                {
                    await Fill_InspectionSpecialMaterial_Manufa(MANV, PO);
                }
                catch { }
               
                Finish_SpecialInspection();
                return true;
            }
            return false;
        }

        public void Finish_SpecialInspection()
        {
            Finish_App(GlobalVariables.tcs_SpecialInspection);
        }
    }
}
