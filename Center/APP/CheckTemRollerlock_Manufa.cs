using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;
using System.Windows.Forms;
using static Center.CaptureItemHandle;
using static Center.WinAPI;
using System.Diagnostics;
using System.Windows.Automation;
namespace Center.APP
{
    internal class CheckTemRollerlock_Manufa : App
    {

        public CheckTemRollerlock_Manufa() { }
        public void SetUp_CheckTemRollerlock_Manufa(string MANV, string PO)
        {
            this.MANV = MANV;
            this.PO = PO;
        }
        public override async Task Fill_App(IntPtr hWnd, string windowName, string class1, int index1, string class2, int index2, string MSNV, string PO)
        {
            uint BM_CLICK = 0x00F5;

            await Task.Delay(50);
            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);

            WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "Clear", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
            //Click(Clear.Handle, 5, 5);
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
                else {
                    MessageBox.Show("Phần tử không hỗ trợ InvokePattern hoặc ValuePattern.");
                }

            }
            else
            {
                MessageBox.Show("Không tìm thấy phần tử.");
            }
            await Task.Delay(100);

            await Task.Delay(50);
            WindowInfo wc1 = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);
            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D);


            AutomationElement element_MaNV = AutomationElement.FromHandle(wc1.Handle);

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
                    await Task.Delay(200);
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
        public async Task<bool> Start_CheckTemRollerlock_Manufa()
        {
            string appName = "CheckTemRollerlock_Manufa";
            string windowName = "CheckTemRollerlock";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            string class2 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 1;
            int index2 = 4;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            try
            {
                await Fill_App(h, windowName, class1, index1, class2, index2, MANV, PO);
            }
            catch { }

            //WindowsForms10.EDIT.app.0.34f5582_r8_ad1     2

            WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 0);
            int IntQty = 0;
            if (Qty.Caption == "")
            {
                Cancel_App(GlobalVariables.tcs_CheckTemRollerlock_Manufa);
                return false;
            }
            else
            {
                int.TryParse(Qty.Caption, out IntQty);
            }
            await WaitPrint(h, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 2, GlobalVariables.tcs_CheckTemRollerlock_Manufa, IntQty);
            //Finish_App(GlobalVariables.tcs_CheckTemRollerlock_Manufa);
            return true;
        }

    }
}
