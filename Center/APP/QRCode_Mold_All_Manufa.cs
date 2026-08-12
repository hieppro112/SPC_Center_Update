using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;

namespace Center.APP
{
    internal class QRCode_Mold_All_Manufa : App
    {

        //22473
        public QRCode_Mold_All_Manufa() { }
        public void SetUp_QRCode_Mold_All_Manufa( string PO)
        {
            this.PO = PO;
        }
        public async Task<bool> Start_QRCode_Mold_All_Manufa()
        {
            string appName = "QRCode_Mold_All_Manufa";
            string windowName = "QRCode_TaperPin";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 9;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            try {await Fill_App(h, windowName, class1, index1, PO); } catch { }

            //Code tự động nhấn intem
            //string classPrintQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //string classQty = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            //int indexQty = 2;
            //int index_classPrintQty = 3;
            //WindowInfo PrintItem = CaptureItemHandle.GetControlHandle(h, "Print", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);*/
            //WindowInfo Qty = CaptureItemHandle.GetControlHandle(h, classQty, indexQty);
            //int IntQty = 0;
            //if (Qty.Caption == "")
            //{
            //    Cancel_App(GlobalVariables.tcs_QRCode_Mold_All_Manufa);
            //    return false;
            //}
            //else
            //{
            //    int.TryParse(Qty.Caption, out IntQty);
            //}
            //ClickPrintManyTime(PrintItem.Handle, GlobalVariables.tcs_QRCode_Mold_All_Manufa);
            //await WaitPrint(h, classPrintQty, index_classPrintQty, GlobalVariables.tcs_QRCode_Mold_All_Manufa, IntQty + 1);

            Finish_App(GlobalVariables.tcs_QRCode_Mold_All_Manufa);
            return true;
        }

    }
}
