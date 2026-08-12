using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using static Center.Structs;
using static Center.WinAPI;
namespace Center.APP
{
    internal class InspectionKeshikomi_Manufa : App
    {
        public InspectionKeshikomi_Manufa() { }
        public void SetUp_InspectionKenshikomi_Manufa(string PO)
        {
            this.PO = PO;   
        }
        public async Task<bool> Start_InspectionKenshikomi_Manufa()
        {
            string appName = "InspectionKeshikomi_Manufa";
            string windowName = "Tram Inspection";
            string class1 = "WindowsForms10.EDIT.app.0.34f5582_r8_ad1";
            int index1 = 1;
            IntPtr h = await OpenAndGetHandle(appName, windowName);
            if (h == IntPtr.Zero)
            {
                return false;
            }
            ShowWindow(h, 3);
            try { await Fill_App(h, windowName, class1, index1, PO); } catch { }
            Finish_App(GlobalVariables.tcs_InspectionKeshikomi_Manufa);
            return true;
        }
    }
}
