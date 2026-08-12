using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;
using System.Windows.Forms;
using static Center.WinAPI;
using static Center.CaptureItemHandle;
using System.Diagnostics;
using System.Windows.Automation;
namespace Center.APP
{
    internal class QRCode_Support_Pillar : App
    {

        public QRCode_Support_Pillar()
        {

        }
        public void SetUp_QRCode_Support_Pillar(string PO)
        {
            this.PO = PO;
        }
        public async Task Fill_App(IntPtr hWnd, string windowName, string className1, string className2, int index, string PO)
        {
            // Kích hoạt cửa sổ ứng dụng
            //hWnd = IntPtr.Zero;
            await Task.Delay(100);

            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, className1, index);
            if (wc.Handle == IntPtr.Zero || wc.Handle == null)
            {
                wc = CaptureItemHandle.GetControlHandle(hWnd, className2, index);
            }
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
                    valuePattern.SetValue(PO);
                    SetForegroundWindow(hWnd);
                    element.SetFocus();
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
            await Task.Delay(150);

        }

        #region remove
        //public async Task Fill_App(IntPtr hWnd, string windowName, string PO)
        //{
        //    // Kích hoạt cửa sổ ứng dụng
        //    await Task.Delay(50);
        //    WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 7);

        //    uint WM_SETTEXT = 0x000C;
        //    uint WM_KEYDOWN = 0x0100;
        //    uint WM_KEYUP = 0x0101;
        //    IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter

        //    // Điền dữ liệu vào textbox 1
        //    IntPtr ptrMSNV = Marshal.StringToHGlobalUni(PO); // Sử dụng Unicode
        //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
        //    await Task.Delay(50);
        //    SendKeys.SendWait("{ENTER}");
        //    Marshal.FreeHGlobal(ptrMSNV);
        //}
        #endregion
        public async Task<bool> Start_QRCode_Support_Pillar()
        {
            string appName = "QRCode_Oilless_MANUFA";
            string windowName = "QRCode_SupportPillar";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            string class2 = "WindowsForms10.EDIT.app.0.34f5582_r7_ad1";
            //WindowsForms10.EDIT.app.0.34f5582_r8_ad1
            int index1 = 7;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            //WindowsForms10.BUTTON.app.0.34f5582_r7_ad1
            try
            {
                await Fill_App(h, windowName, class1, class2, index1, PO);
            }
            catch { }
            

            string classPrintQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            string classPrintQty_2 = "WindowsForms10.EDIT.app.0.34f5582_r7_ad1";
            int index_classPrintQty = 0;
            string classQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            string classQty_2 = "WindowsForms10.EDIT.app.0.34f5582_r7_ad1";
            int indexQty = 6;



            WindowInfo info_PrintQty = CaptureItemHandle.GetControlHandle(h, classPrintQty, index_classPrintQty);
            if (info_PrintQty.Handle != IntPtr.Zero )
            {

                WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
                WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, classQty, indexQty);
                int IntQty = 0;
                if (Qty.Caption == "")
                {
                    Cancel_App(GlobalVariables.tcs_QRCode_Support_Pillar);
                    return false;
                }
                else
                {
                    int.TryParse(Qty.Caption, out IntQty);
                }
                ClickPrintButton(PrintItem.Handle);
                await WaitPrint(h, classPrintQty, index_classPrintQty, GlobalVariables.tcs_QRCode_Support_Pillar, IntQty + 1);
            }else
            {
                WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print", "WindowsForms10.BUTTON.app.0.34f5582_r7_ad1", 0);
                WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, classQty_2, indexQty);
                int IntQty = 0;
                if (Qty.Caption == "")
                {
                    Cancel_App(GlobalVariables.tcs_QRCode_Support_Pillar);
                    return false;
                }
                else
                {
                    int.TryParse(Qty.Caption, out IntQty);
                }
                ClickPrintButton(PrintItem.Handle);
                await WaitPrint(h, classPrintQty_2, index_classPrintQty, GlobalVariables.tcs_QRCode_Support_Pillar, IntQty + 1);
            }
            
            return true;
        }
    }
}
