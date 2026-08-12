using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static Center.WinAPI;
using System.Threading.Tasks;
using static Center.Structs;
using System.Windows.Forms;
using System.Windows.Automation;

namespace Center
{
    internal class ToolPrintLabelInspection : App
    {
        //public event Action Finished;
        //public event Action Canceled;

        //string PO;
        public ToolPrintLabelInspection() {}
        public void SetUp_ToolPrintLabelInspection(string MANV, string PO)
        {
            this.MANV = MANV;
            this.PO = PO;
        }
        #region remove
        //public async Task<IntPtr> OpenAndGetHandle()
        //{
        //    int time_find = 0;
        //    string appName = "ToolPrintLabelInspection";
        //    IntPtr h = IntPtr.Zero;
        //    if (await OpenApp(appName, "Tool Print Qty Label Inspection"))
        //    {
        //        h = findHandle(ref time_find, "Tool Print Qty Label Inspection");
        //    }

        //    return h;
        //}


        //public bool OpenApp(string appName, string windowName)
        //{
        //    IntPtr h = CheckRunning(windowName);
        //    if (h == IntPtr.Zero)
        //    {
        //        string[] appFiles = Find_App(appName);

        //        if (appFiles.Length > 0)
        //        {
        //            // Mở tệp ứng dụng đầu tiên tìm thấy
        //            Process.Start(appFiles[0]);
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        ShowWindow(h, 9);
        //        SetForegroundWindow(h);
        //    }
        //    return true;
        //}
        

        //public async Task Fill_App(IntPtr hWnd, string windowName, string MSNV, string PO)
        //{
        //    // Kích hoạt cửa sổ ứng dụng
        //    await Task.Delay(200);
        //    WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 2);

        //    uint WM_SETTEXT = 0x000C;
        //    uint WM_KEYDOWN = 0x0100;
        //    uint WM_KEYUP = 0x0101;
        //    IntPtr VK_ENTER = new IntPtr(0x0D); // Mã phím cho Enter

        //    // Điền dữ liệu vào textbox 1
        //    IntPtr ptrMSNV = Marshal.StringToHGlobalUni(MSNV); // Sử dụng Unicode
        //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);

        //    // Điền dữ liệu vào textbox 2
        //    WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.EDIT.app.0.34f5582_r8_ad1", 3);
        //    IntPtr ptrMSNV2 = Marshal.StringToHGlobalUni(PO); // Sử dụng Unicode
        //    SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV2);

        //    // Giải phóng bộ nhớ sau khi gửi dữ liệu
        //    Marshal.FreeHGlobal(ptrMSNV);
        //    Marshal.FreeHGlobal(ptrMSNV2);

        //    // Gửi thông điệp để nhấn phím Enter cho textbox thứ 2
        //    SendMessage(wc2.Handle, WM_KEYDOWN, VK_ENTER, IntPtr.Zero);  // Nhấn phím Enter
        //    SendMessage(wc2.Handle, WM_KEYUP, VK_ENTER, IntPtr.Zero);    // Nhả phím Enter




        //}
        #endregion
        public void Finish_ToolPrintLabelInspection()
        {
            //Finished?.Invoke(); // Thông báo hoàn thành
        }



        public async Task FillControls()
        {

            string appName = "ToolPrintLabelInspection";
            string windowName = "Tool Print Qty Label Inspection";


            int index1 = 2;
            int index2 = 3;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ ứng dụng '" + windowName + "'");
            }
            string class1 = CaptureItemHandle.GetClassNameByPartial(h, "EDIT.app", index1);
            string class2 = CaptureItemHandle.GetClassNameByPartial(h, "EDIT.app", index2);

            if (string.IsNullOrEmpty(class1) || string.IsNullOrEmpty(class2))
            {
                System.Windows.Forms.MessageBox.Show("Không tìm thấy class tương ứng.");
            }

            try
            {
                await Fill_App(h, windowName, class1, index1, class2, index2, MANV, PO);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Lỗi khi fill: " + ex.Message);
            }

            // 👉 Lấy control Print Label theo class partial
            WindowInfo PrintItem = CaptureItemHandle.GetControlHandleByClassPartial(h, "BUTTON.app", 0);
            if (PrintItem.Handle == IntPtr.Zero)
            {
                System.Windows.Forms.MessageBox.Show("Không tìm thấy nút Print Label.");
            }
        }

      
        public async Task<bool> Start_ToolPrintLabelInspection()
        {

            string appName = "ToolPrintLabelInspection";
            string windowName = "Tool Print Qty Label Inspection";


            int index1 = 2;
            int index2 = 3;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ ứng dụng '" + windowName + "'");
                return false;
            }

            //string class1 = CaptureItemHandle.GetClassNameByPartial(h, "EDIT.app", index1);
            //string class2 = CaptureItemHandle.GetClassNameByPartial(h, "EDIT.app", index2);

            //if (string.IsNullOrEmpty(class1) || string.IsNullOrEmpty(class2))
            //{
            //    System.Windows.Forms.MessageBox.Show("Không tìm thấy class tương ứng.");
            //    return false;
            //}

            //try
            //{
            //    await Fill_App(h, windowName, class1, index1, class2, index2, MANV, PO);
            //    await Task.Delay(500); // chờ app fill xong
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("Lỗi khi fill: " + ex.Message);
            //    return false;
            //}

            // 👉 Lấy control Print Label theo class partial
            WindowInfo PrintItem = CaptureItemHandle.GetControlHandleByClassAndText(h, "BUTTON.app", "Print Label");
            //if (PrintItem.Handle == IntPtr.Zero)
            //{
            //    System.Windows.Forms.MessageBox.Show("Không tìm thấy nút Print Label.");
            //    return false;
            //}
            //GlobalVariables.mousehook_ToolPrintLabelInspection.Initialize(h, PrintItem.Handle, this, GlobalVariables.tcs_ToolPrintLabelInspection);

            //const uint BM_CLICK = 0x00F5;
            //SendMessage(PrintItem.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);

            //WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print Label", "WindowsForms10.BUTTON.app.0.34f5582_r7_ad1", 0);
            //await Task.Delay(1000);
            if (PrintItem.Handle == IntPtr.Zero)
            {
                System.Windows.Forms.MessageBox.Show("Không tìm thấy nút Print Label.");
                return false;
            }
            const uint BM_CLICK = 0x00F5;
            SendMessage(PrintItem.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            Finish_App(GlobalVariables.tcs_ToolPrintLabelInspection); // Thông báo hoàn thành
            return true;
        }

    }
}
