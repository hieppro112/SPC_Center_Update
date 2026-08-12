using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Center.Structs;

namespace Center.APP
{
    internal class MTS_Epin_QRCode_Mold_All_Manufa : App
    {
        public MTS_Epin_QRCode_Mold_All_Manufa() { }
        public void SetUp_MTS_Epin_QRCode_Mold_All_Manufa(string PO)
        {
            this.PO = PO;
        }
        public async Task<bool> Start_MTS_Epin_QRCode_Mold_All_Manufa()
        {
            string appName = "QRCode_Mold_All_Manufa";
            string windowName = "QRCode_EjectorPin";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 7;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            try { await Fill_App(h, windowName, class1, index1, PO); } catch { }
            
            WindowInfo Print_Ngoai = CaptureItemHandle.GetControlHandle(h, "Print_Ngoai", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);
            WindowInfo Print_Trong = CaptureItemHandle.GetControlHandle(h, "Print_Trong", "WindowsForms10.BUTTON.app.0.34f5582_r8_ad1", 0);

            //GlobalVariables.mousehook_MTS_Epin_QRCode_Mold_All_Manufa.Initialize(h, Print_Ngoai.Handle, Print_Trong.Handle,this,GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa);
            Finish_App(GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa);
            return true;
        }

    }
}
