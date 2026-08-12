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
namespace Center.APP
{
    internal class QRCode_SprueBush_Manufa : App
    {
        public QRCode_SprueBush_Manufa()
        {

        }
        public void SetUp_QRCode_SprueBush_Manufa(string PO)
        {
            this.PO = PO;
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
        public async Task<bool> Start_QRCode_SprueBush_Manufa()
        {
            string appName = "QRCode_SprueBush_Manufa";
            string windowName = "QRCode_SprueBush - Manufa";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 7;
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
            
            WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
            //GlobalVariables.mousehook_QRCode_SprueBush_Manufa.Initialize(h, PrintItem.Handle, this, GlobalVariables.tcs_QRCode_SprueBush_Manufa);




            // Code tự động click in tem
            //string classPrintQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //int index_classPrintQty = 0;
            //string classQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //int indexQty = 6;
            //WindowInfo info_PrintQty = CaptureItemHandle.GetControlHandle(h, classPrintQty, index_classPrintQty);
            //WindowInfo info_Qty = CaptureItemHandle.GetControlHandle(h, classQty, indexQty);
            //ClickPrintManyTime(PrintItem.Handle, GlobalVariables.tcs_QRCode_SprueBush_Manufa);
            //WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, classQty, indexQty);
            //int IntQty = 0;
            //if (Qty.Caption == "")
            //{
            //    Cancel_App(tcs);
            //    return false;
            //}
            //else
            //{
            //    int.TryParse(info_Qty.Caption, out IntQty);
            //}
            //await WaitPrint(h, classPrintQty, index_classPrintQty, GlobalVariables.tcs_QRCode_SprueBush_Manufa, IntQty*2 + 1);

            Finish_App(GlobalVariables.tcs_QRCode_SprueBush_Manufa);
            return true;
        }
    }
}
