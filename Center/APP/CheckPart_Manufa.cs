using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;
using System.Windows.Automation;
using System.Windows.Forms;
using static Center.WinAPI;
namespace Center.APP
{
    internal class CheckPart_Manufa : App
    {
        int check_FillMaNV = 0;
        public CheckPart_Manufa() { }
        public void SetUp_CheckPart_Manufa(string MANV, string PO)
        {
            this.MANV = MANV;
            this.PO = PO;
        }

        //public override async Task Fill_App(IntPtr hWnd, string windowName, string class1, int index1, string class2, int index2, string MSNV, string PO)
        //{
        //    uint BM_CLICK = 0x00F5;

        //    await Task.Delay(50);
        //    WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);

        //    WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "Back", "WindowsForms10.Window.b.app.0.13965fa_r8_ad1", 0);
        //    //Click(Clear.Handle, 5, 5);
        //    AutomationElement element_Clear = AutomationElement.FromHandle(Clear.Handle);

        //    if (element_Clear != null)
        //    {
        //        // Kiểm tra xem phần tử có hỗ trợ InvokePattern không
        //        if (element_Clear.TryGetCurrentPattern(InvokePattern.Pattern, out object pattern))
        //        {
        //            // Cast object thành InvokePattern
        //            InvokePattern invokePattern = (InvokePattern)pattern;

        //            // Thực hiện click
        //            invokePattern.Invoke();
        //            Debug.Print("Phần tử đã được click.");
        //        }
        //        else
        //        {
        //            MessageBox.Show("Phần tử không hỗ trợ InvokePattern hoặc ValuePattern.");
        //        }

        //    }
        //    else
        //    {
        //        MessageBox.Show("Không tìm thấy phần tử.");
        //    }
        //    await Task.Delay(100);

        //    await Task.Delay(50);
        //    WindowInfo wc1 = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
        //    WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);
        //    uint WM_SETTEXT = 0x000C;
        //    uint WM_KEYDOWN = 0x0100;
        //    uint WM_KEYUP = 0x0101;
        //    IntPtr VK_ENTER = new IntPtr(0x0D);


        //    AutomationElement element_MaNV = AutomationElement.FromHandle(wc1.Handle);

        //    if (element_MaNV != null)
        //    {
        //        // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
        //        if (element_MaNV.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
        //        {

        //            element_MaNV.SetFocus();
        //            SendKeys.Send(MANV);
        //            await Task.Delay(200);
        //            SendKeys.SendWait("{ENTER}");

        //        }
        //        else
        //        {
        //            MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Không tìm thấy phần tử.");
        //    }
        //    await Task.Delay(200);
        //    AutomationElement element_PO = AutomationElement.FromHandle(wc2.Handle);

        //    if (element_PO != null)
        //    {
        //        // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
        //        if (element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
        //        {

        //            element_PO.SetFocus();
        //            SendKeys.Send(PO);
        //            await Task.Delay(200);
        //            SendKeys.SendWait("{ENTER}");
        //        }
        //        else
        //        {
        //            MessageBox.Show("Phần tử không hỗ trợ ValuePattern.");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Không tìm thấy phần tử.");
        //    }
        //}

        public override async Task Fill_App(IntPtr hWnd, string windowName, string class1, int index1, string class2, int index2, string MSNV, string PO)
        {
            uint BM_CLICK = 0x00F5;

            await Task.Delay(50);
            WindowInfo wc = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);

            WindowInfo Clear = CaptureItemHandle.GetControlHandle(hWnd, "Back", "WindowsForms10.Window.b.app.0.13965fa_r8_ad1", 0);
            WindowInfo wc1 = CaptureItemHandle.GetControlHandle(hWnd, class1, index1);
            WindowInfo wc2 = CaptureItemHandle.GetControlHandle(hWnd, class2, index2);
            uint WM_SETTEXT = 0x000C;
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            IntPtr VK_ENTER = new IntPtr(0x0D);
            //Click(Clear.Handle, 5, 5);
            AutomationElement element_Clear = AutomationElement.FromHandle(Clear.Handle);

            if (check_FillMaNV == 0)
            {
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
                await Task.Delay(100);
                AutomationElement element_MaNV = AutomationElement.FromHandle(wc1.Handle);

                if (element_MaNV != null)
                {
                    if (element_MaNV.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                    {

                        element_MaNV.SetFocus();
                        SendKeys.Send(MANV);
                        await Task.Delay(200);
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
                await Task.Delay(200);
                check_FillMaNV = 1;
            }

            AutomationElement element_PO = AutomationElement.FromHandle(wc2.Handle);

            if (element_PO != null)
            {
                // Kiểm tra xem phần tử có hỗ trợ ValuePattern không
                if (element_PO.TryGetCurrentPattern(ValuePattern.Pattern, out object pattern))
                {
                    element_PO.SetFocus();
                    SendKeys.Send(PO);
                    await Task.Delay(200);
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
        }

        public async Task<bool> OpenApp(string appName, string windowName1, string windowName2)
        {
            IntPtr h1 = CheckRunning(windowName1);
            IntPtr h2  = CheckRunning(windowName2);
            int check = 0;
            if (h1 == IntPtr.Zero && h2 == IntPtr.Zero)
            {
                string[] appFiles = Find_App(appName);

                if (appFiles.Length > 0)
                {
                    Process process = Process.Start(appFiles[0]);
                    check = 1;
                }
                else
                {
                    return false;
                }
            }
            else if (h2 == IntPtr.Zero)
            {
                WindowInfo PrintSide = CaptureItemHandle.GetControlHandle(h1, "In tem QRCode", "WindowsForms10.Window.b.app.0.13965fa_r8_ad1", 0);
                ClickPrintButton(PrintSide.Handle);
            }
            else if (h2 != IntPtr.Zero)
            {

                ShowWindow(h2, 3);
                SetForegroundWindow(h2);
            }

            if (check == 1)
            {
                h1 = CheckRunning(windowName1);
                WindowInfo PrintSide = CaptureItemHandle.GetControlHandle(h1, "In tem QRCode", "WindowsForms10.Window.b.app.0.13965fa_r8_ad1", 0);
                ClickPrintButton(PrintSide.Handle);
            }

           
            return true;
        }
        public async Task WaitPrint( TaskCompletionSource<bool> tcs, string classError, string captionError)
        {
            while (!tcs.Task.IsCompleted)
            {
                bool errMess = CaptureItemHandle.isErrMess(classError, captionError);
                if (errMess == true)
                {
                    Finish_App(tcs);
                }
                await Task.Delay(2000); 
            }
        }
        public async Task<IntPtr> OpenAndGetHandle(string appName, string windowName1, string windowName2)
        {
            int time_find = 10;
            IntPtr h = IntPtr.Zero;
            if (await OpenApp(appName, windowName1, windowName2))
            {
                h = findHandle(ref time_find, windowName2);
                return h;
            }

            return IntPtr.Zero;
        }
        public async Task<bool> Start_CheckPart_Manufa()
        {
            string appName = "CheckPart_Manufa";
            string windowName1 = "MANUFA - Check Part QA -";
            string windowName2 = "MANUFA - Check Part QA (Inspection)";
            string class1 = "WindowsForms10.EDIT.app.0.13965fa_r8_ad1";
            string class2 = "WindowsForms10.EDIT.app.0.13965fa_r8_ad1";
            int index1 = 0;
            int index2 = 0;
            IntPtr h = await OpenAndGetHandle(appName, windowName1, windowName2);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            await Fill_App(h, windowName2, class1, index1, class2, index2, MANV, PO);
            WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print", "WindowsForms10.Window.b.app.0.13965fa_r8_ad1", 0);
            ClickPrintManyTime(PrintItem.Handle, GlobalVariables.tcs_CheckPart_Manufa);
            await WaitPrint( GlobalVariables.tcs_CheckPart_Manufa, "WindowsForms10.Window.8.app.0.13965fa_r8_ad1", "Error information");
            return true;
        }
    }
}
