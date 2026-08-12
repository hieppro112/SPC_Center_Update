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
using System.Runtime.ConstrainedExecution;
using System.Windows.Automation;
namespace Center.APP
{
    public class Spclt2_Program_Warehouse:App
    {
        public Spclt2_Program_Warehouse() { }  
        public void SetUp_Spclt2_Program_Warehouse(string MANV, string PO)
        {
            this.MANV = MANV;
            this.PO = PO;
        }
        public void Finish_Spclt2_Program_Warehouse()
        {
            Finish_App(GlobalVariables.tcs_Spclt2_Program_Warehouse);
        }


        public override async Task Fill_App(IntPtr hWnd, string windowName, string class1, int index1, string class2, int index2, string MSNV, string PO)
        {
            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D);
            uint BM_CLICK = 0x00F5;
            uint BM_GETCHECK = 0x00F0; // Lấy trạng thái checkbox
            uint BST_UNCHECKED = 0x0000; // Không được chọn
            uint BST_CHECKED = 0x0001;   // Được chọn
            uint BST_INDETERMINATE = 0x0002; // Trạng thái không xác định (nếu có)
            WindowInfo checkbox1 = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 2);
            WindowInfo checkbox2 = CaptureItemHandle.GetControlHandle(hWnd, "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 1);


            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);


            AutomationElement element_CheckBox1 = AutomationElement.FromHandle(checkbox1.Handle);
            //MessageBox.Show(checkbox1.Handle.ToString());
            TogglePattern invokePattern_CheckBox1 = null;
            TogglePattern invokePattern_CheckBox2 = null;
            AutomationElement elemen_wc = AutomationElement.FromHandle(wc.Handle);
            AutomationElement elemen_wc2 = AutomationElement.FromHandle(wc2.Handle);
            ValuePattern valuePattern = null;
            ValuePattern valuePattern2 = null;


            if (element_CheckBox1.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj))
            {
                invokePattern_CheckBox1 = (TogglePattern)togglePatternObj;

                // Thực hiện toggle (bật/tắt)
                invokePattern_CheckBox1.Toggle();
                Debug.Print("Đã click vào CheckBox.");
            }
            else
            {
                
                Debug.Print("Phần tử không hỗ trợ TogglePattern.");
                return;
            }
           
            await Task.Delay(300);
            bool isEnabled = false;
            isEnabled = IsWindowEnabled(wc.Handle);
            if (elemen_wc.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
            {
                valuePattern = (ValuePattern)pattern;
            }
            if (isEnabled)
            {
                if (elemen_wc != null)
                {
                    valuePattern.SetValue(MSNV);
                    SetForegroundWindow(hWnd);
                    elemen_wc.SetFocus();
                    SendKeys.SendWait("{ENTER}");
                    // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                    
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phần tử.");
                }
            }else
            {
                invokePattern_CheckBox1.Toggle();
                await Task.Delay(150);
                valuePattern.SetValue(MSNV);
                SetForegroundWindow(hWnd);
                elemen_wc.SetFocus();
                SendKeys.SendWait("{ENTER}");
            }
            await Task.Delay(200);

            AutomationElement element_CheckBox2 = AutomationElement.FromHandle(checkbox2.Handle);



            if (element_CheckBox2.TryGetCurrentPattern(TogglePattern.Pattern, out object togglePatternObj2))
            {
                invokePattern_CheckBox2 = (TogglePattern)togglePatternObj2;

                // Thực hiện toggle (bật/tắt)
                invokePattern_CheckBox2.Toggle();
                Debug.Print("Đã click vào CheckBox.");
            }
            else
            {

                Debug.Print("Phần tử không hỗ trợ TogglePattern.");
                return;
            }

            await Task.Delay(500);
            bool isEnabled2 = false;
            isEnabled2 = IsWindowEnabled(wc2.Handle);
            if (elemen_wc2.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern2))
            {
                valuePattern2 = (ValuePattern)pattern2;
            }
            if (isEnabled2)
            {
                if (elemen_wc2 != null)
                {
                    // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                    valuePattern2.SetValue(PO);
                    SetForegroundWindow(hWnd);
                    elemen_wc2.SetFocus();
                    SendKeys.SendWait("{ENTER}");

                }
                else
                {
                    MessageBox.Show("Không tìm thấy phần tử.");
                }
            }
            else
            {
                invokePattern_CheckBox2.Toggle();
                await Task.Delay(150);
                valuePattern2.SetValue(PO);
                SetForegroundWindow(hWnd);
                elemen_wc2.SetFocus();
                SendKeys.SendWait("{ENTER}");
            }
            //SendMessage(checkbox1.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //bool isEnabled = IsWindowEnabled(wc.Handle);
            //if (isEnabled)
            //{
            //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");
            //}
            //else
            //{
            //    SendMessage(checkbox1.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //    await Task.Delay(100);
            //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");

            //}

            //SendMessage(checkbox2.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //bool isEnabled2 = IsWindowEnabled(wc2.Handle);
            //if (isEnabled2)
            //{
            //    SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrPO);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");
            //}
            //else
            //{
            //    SendMessage(checkbox2.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //    await Task.Delay(100);
            //    SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrPO);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");

            //}
            //AutomationElement element = AutomationElement.FromHandle(wc2.Handle);

            //if (element != null)
            //{
            //    // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
            //    if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
            //    {
            //        // Cast object thành ValuePattern
            //        ValuePattern valuePattern = (ValuePattern)pattern;

            //        // Thay đổi giá trị của TextBox
            //        valuePattern.SetValue(PO);

            //        Debug.Print("Giá trị mới đã được thiết lập.");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Không tìm thấy phần tử.");
            //}


            //int state = SendMessage(checkbox1.Handle, BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
            //if (state == BST_CHECKED)
            //{
            //    //SendMessage(checkbox1.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //    Debug.Print("Checked");
            //}
            //else if (state == BST_UNCHECKED)
            //{
            //    Debug.Print("Checkbox is unchecked.");
            //    WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
            //    IntPtr ptrMSNV = Marshal.StringToHGlobalUni(MSNV);
            //    SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");
            //    await Task.Delay(150);
            //}
            //else if (state == BST_INDETERMINATE)
            //{
            //    Debug.Print("Checkbox is in an indeterminate state.");
            //}
            //else
            //{
            //    Debug.Print("Unknown state.");
            //}

            //await Task.Delay(100);
            //WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);
            //int state2 = SendMessage(checkbox2.Handle, BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);

            //if (state2 == BST_CHECKED)
            //{
            //    Debug.Print("Checked");
            //    IntPtr ptrPO = Marshal.StringToHGlobalUni(PO);
            //    SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrPO);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");
            //    await Task.Delay(150);
            //}
            //else if (state2 == BST_UNCHECKED)
            //{
            //    SendMessage(checkbox2.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //    IntPtr ptrPO = Marshal.StringToHGlobalUni(PO);
            //    SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrPO);
            //    await Task.Delay(100);
            //    SendKeys.SendWait("{ENTER}");
            //    await Task.Delay(150);
            //}
            //else if (state2 == BST_INDETERMINATE)
            //{
            //    Debug.Print("Checkbox is in an indeterminate state.");
            //}
            //else
            //{
            //    Debug.Print("Unknown state.");
            //}



            //IntPtr ptrMSNV = Marshal.StringToHGlobalUni(MSNV);
            //SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
            //IntPtr ptrPO = Marshal.StringToHGlobalUni(PO);

            //SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrPO);
            //await Task.Delay(100);
            ////SendKeys.SendWait("{ENTER}");



            //SendMessage(wc2.Handle, WM_KEYDOWN, VK_ENTER, IntPtr.Zero);  // Nhấn phím Enter
            //SendMessage(wc2.Handle, WM_KEYUP, VK_ENTER, IntPtr.Zero);    // Nhả phím Enter
            //await Task.Delay(150);



            //uint BM_CLICK = 0x00F5;
            //await Task.Delay(50);
            //WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);

            //WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "Clear", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
            ////Click(Clear.Handle, 5, 5);
            //SendMessage(Clear.Handle, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //await Task.Delay(100);

            //uint WM_SETTEXT = 0x000C;
            //uint WM_KEYDOWN = 0x0100;
            //uint WM_KEYUP = 0x0101;
            //IntPtr VK_ENTER = new IntPtr(0x0D);

            //IntPtr ptrMSNV = Marshal.StringToHGlobalUni(MSNV);
            //SendMessage(wc.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV);
            //await Task.Delay(50);
            //SendKeys.SendWait("{ENTER}");

            //await Task.Delay(50);
            //WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);
            //IntPtr ptrMSNV2 = Marshal.StringToHGlobalUni(PO);

            //SendMessage(wc2.Handle, WM_SETTEXT, IntPtr.Zero, ptrMSNV2);
            //await Task.Delay(50);

            //Marshal.FreeHGlobal(ptrMSNV);
            //Marshal.FreeHGlobal(ptrMSNV2);

            //SendKeys.SendWait("{ENTER}");
        }
        public async Task<bool> Start_Spclt2_Program_Warehouse()
        {
            string appName = "Spclt2-Program-Warehouse_Manufa";
            string windowName = "Spclt2_Program";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            string class2 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 0;
            int index2 = 2;
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
            
            Finish_App(GlobalVariables.tcs_Spclt2_Program_Warehouse);

            //WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Save", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
            //GlobalVariables.mousehook_Spclt2_Program_Warehouse.Initialize(h, PrintItem.Handle, this, GlobalVariables.tcs_Spclt2_Program_Warehouse);
            return true;
        }
    }
}
