using Center.FORM;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center
{
    public static class GlobalVariables
    {

        /// <summary>
        /// Apps và Groups là 2 list chính, khi mà ng dùng chọn 1 combo thì các app và group trong combo sẽ add vào 2 list này, có 2 list này rồi -> Control.SetUpApp -> Control.Run_Flow
        /// </summary>
        /// 
        public static List<Task> List_tasks = new List<Task>(); //List chứa các task chờ App finish
        public static List<string> Apps = new List<string>(); // List tên các App
        public static List<int> Groups = new List<int>(); // List Groups
        public static List<object> ListObj = new List<object>(); // List chứa các đối tượng app

        public static IBrowser browser;// Browser để chạy Puppeteer

        /// <summary>
        /// Các hook theo dõi sự kiện ng dùng click chuột
        /// </summary>
        public static MouseHook mousehook_ToolPrintLabelInspection = new MouseHook();
        public static MouseHook mousehook_SprueBush_PCS_QRCode = new MouseHook();
        public static MouseHook mousehook_QRCodeRollerlock_Manufa = new MouseHook();
        public static MouseHook mousehook_QRCode_Oilless_MANUFA = new MouseHook();
        public static MouseHook mousehook_QRCode_SprueBush_Manufa = new MouseHook();
        public static MouseHook mousehook_QRCode_Mold_All_Manufa = new MouseHook();
        public static MouseHook mousehook_MTS_Epin_QRCode_Mold_All_Manufa = new MouseHook();
        public static MouseHook mousehook_QRCode_Support_Pillar = new MouseHook();
        public static MouseHook mousehook_Spclt2_Program_Warehouse = new MouseHook();   
        public static void UnAllMouseHook()
        {
            mousehook_ToolPrintLabelInspection.Unhook();
            mousehook_SprueBush_PCS_QRCode.Unhook();
            mousehook_QRCodeRollerlock_Manufa.Unhook();
            mousehook_QRCode_Oilless_MANUFA.Unhook();
            mousehook_QRCode_SprueBush_Manufa.Unhook();
            mousehook_QRCode_Mold_All_Manufa.Unhook();
            mousehook_MTS_Epin_QRCode_Mold_All_Manufa.Unhook();
            mousehook_QRCode_Support_Pillar.Unhook();
            mousehook_Spclt2_Program_Warehouse.Unhook();
        }



        /// <summary>
        /// Các Task dùng để xác nhận 1 app Finish cũng như là để chờ 1 App Finish
        /// </summary>
        public static TaskCompletionSource<bool> tcs_Supro_Z200 = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_Supro_Z300 = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_ToolPrintLabelInspection = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_KJapaneseG = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_SpecialInspection = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_SprueBush_PCS_QRCode = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_CheckTemRollerlock_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_QRCodeRollerlock_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_QRCode_Oilless_MANUFA = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_QRCode_SprueBush_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_QRCode_Mold_All_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_MTS_Epin_QRCode_Mold_All_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_QRCode_Support_Pillar = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_Spclt2_Program_Warehouse = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_Template_Shipping= new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_CheckPart_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_InspectionKeshikomi_Manufa = new TaskCompletionSource<bool>();
        public static TaskCompletionSource<bool> tcs_CheckPart_Manufa_GetPart = new TaskCompletionSource<bool>();


        //Form
        public static TimeForm tf_Process = new TimeForm(1);
        public static TimeForm tf_Kcode = new TimeForm(1);
        public static OperatorForm opF = new OperatorForm();


        // Page supro
        //public static IPage Z200_Page;
        //public static IPage Z300_Page;


    }
}
