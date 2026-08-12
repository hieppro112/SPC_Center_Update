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
namespace Center.APP
{
    internal class QRCodeRollerlock_Manufa : App
    {
        public QRCodeRollerlock_Manufa()
        {

        }

        public void SetUp_QRCodeRollerlock_Manufa(string PO)
        {
            this.PO = PO;
        }

        public async Task<bool> Start_QRCodeRollerlock_Manufa()
        {
            string appName = "QRCodeRollerlock_Manufa";
            string windowName = "QRCode Rollerlock";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 8;
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


            // tự động click print code
            //string classPrintQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //int index_classPrintQty = 1;
            //string classQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //int indexQty = 7;
            //WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, classQty, indexQty);
            //int IntQty = 0;
            //if (Qty.Caption == "")
            //{
            //    Cancel_App(GlobalVariables.tcs_QRCodeRollerlock_Manufa);
            //    return false;
            //}
            //else
            //{
            //    int.TryParse(Qty.Caption, out IntQty);
            //}
            //ClickPrintManyTime(PrintItem.Handle, GlobalVariables.tcs_QRCodeRollerlock_Manufa);
            //await WaitPrint(h, classPrintQty, index_classPrintQty, GlobalVariables.tcs_QRCodeRollerlock_Manufa, IntQty + 1);

            Finish_App(GlobalVariables.tcs_QRCodeRollerlock_Manufa);
            return true;
        }
    }
}
