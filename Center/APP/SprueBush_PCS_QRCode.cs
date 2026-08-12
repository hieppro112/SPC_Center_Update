using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;
using static Center.CaptureItemHandle;
using static Center.WinAPI;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Automation;
namespace Center.APP
{
    internal class SprueBush_PCS_QRCode : App
    {

        public SprueBush_PCS_QRCode() { }
        public void SetUp_SprueBush_PCS_QRCode(string PO)
        {
            this.PO = PO;
        }

        #region remove
        //public async Task<IntPtr> OpenAndGetHandle()
        //{
        //    int time_find = 0;
        //    string appName = "SprueBush_PCS_QRCode_MANUFA";
        //    IntPtr h = IntPtr.Zero;
        //    if (await OpenApp(appName, "QRCode_SprueBush_PCS_MANUFA"))
        //    {
        //        h = findHandle(ref time_find, "QRCode_SprueBush_PCS_MANUFA");
        //    }

        //    return h;
        //}
        //public async Task Fill_App(IntPtr hWnd, string windowName,  string PO)
        //{
        //    // Kích hoạt cửa sổ ứng dụng
        //    await Task.Delay(200);
        //    WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.1a0e24_r8_ad1", 1);

        //    uint WM_SETTEXT = 0x000C;
        //    uint WM_KEYDOWN = 0x0100;
        //    uint WM_KEYUP = 0x0101;
        //    IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter

        //    IntPtr ptrMSNV2 = Marshal.StringToHGlobalUni(PO); // Sử dụng Unicode
        //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV2);

        //    Marshal.FreeHGlobal(ptrMSNV2);
        //    await Task.Delay(200);
        //    SendKeys.SendWait("{ENTER}");

        //    //SendMessage(wc.Handle, WM_KEYDOWN, VK_ENTER, IntPtr.Zero);  // Nhấn phím Enter
        //    //SendMessage(wc.Handle, WM_KEYUP, VK_ENTER, IntPtr.Zero);    // Nhản phím Enter


        //}
        #endregion

        public void Finish_SprueBush_PCS_QRCode()
        {
            Finish_App(GlobalVariables.tcs_SprueBush_PCS_QRCode); // Thông báo hoàn thành
        }
        public virtual async Task Fill_App(IntPtr hWnd, string windowName, string className, int index, string PO)
        {


            WindowInfo clear = CaptureItemHandle.GetControlHandle(hWnd, "Clear", "WindowsForms10.BUTTON.app.0.1a0e24_r8_ad1", 0);
            AutomationElement elementcl = AutomationElement.FromHandle(clear.Handle);

            if (elementcl == null)
            {
                Debug.Print("Không tìm thấy AutomationElement từ handle.");
                return;
            }

            // Kiểm tra nếu phần tử hỗ trợ InvokePattern (có thể click được)
            if (elementcl.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern2))
            {
                InvokePattern invokePattern = (InvokePattern)pattern2;

                // Thực hiện click vào phần tử
                invokePattern.Invoke();
                Debug.Print("Đã click vào phần tử.");
            }
            else
            {
                Debug.Print("Phần tử không thể click được.");
            }




            // Kích hoạt cửa sổ ứng dụng
            //hWnd = IntPtr.Zero;
            await Task.Delay(100);

            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, className, index);
            //wc.Handle = IntPtr.Zero;
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

                    //try {
                    //    valuePattern.SetValue("");
                    //    await Task.Delay(50);
                    //    valuePattern.SetValue(PO);

                    //} catch { }
                    // Thay đổi giá trị của TextBox
                    valuePattern.SetValue(PO);
                    SetForegroundWindow(hWnd);
                    element.SetFocus();
                    SendKeys.SendWait("{ENTER}");

                    //SendKeys.SendWait("{ENTER}");


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

            //SetWindowText(wc.Handle, "Dữ liệu của bạn");
            await Task.Delay(150);
            //SendKeys.SendWait("{ENTER}");
            //Control.MakeWindowTopMost(hWnd);
            //DoubleClick(wc.Handle, 5, 5);
            //SendKeys.Send("{BACKSPACE}");
            //await Task.Delay(200);
            //SendKeys.SendWait(PO);
            //await Task.Delay(200);
            //SendKeys.SendWait("{ENTER}");
            //Control.UNMakeWindowTopMost(hWnd);

            //Marshal.FreeHGlobal(ptrMSNV);
        }


        public async Task<bool> Start_SprueBush_PCS_QRCode()
        {
            string appName = "SprueBush_PCS_QRCode_MANUFA";
            string windowName = "QRCode_SprueBush_PCS_MANUFA";
            string class1 = "WindowsForms10.EDIT.app.0.1a0e24_r8_ad1";
            int index1 = 1;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            try
            {
                await Fill_App(h, windowName, class1, index1, PO);
            }
            catch { }
            
            WindowInfo Print_Ngoai = CaptureItemHandle.GetControlHandle(h, "Print_Ngoai", "WindowsForms10.BUTTON.app.0.1a0e24_r8_ad1", 0);
            WindowInfo Print_Trong = CaptureItemHandle.GetControlHandle(h, "Print_Trong", "WindowsForms10.BUTTON.app.0.1a0e24_r8_ad1", 0);

            Finish_App(GlobalVariables.tcs_SprueBush_PCS_QRCode);
            GlobalVariables.mousehook_SprueBush_PCS_QRCode.Initialize(h, Print_Ngoai.Handle, Print_Trong.Handle, this, GlobalVariables.tcs_SprueBush_PCS_QRCode);
            return true;
            //Finished?.Invoke();
        }










    }
}
