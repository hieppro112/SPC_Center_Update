using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using static Center.WinAPI;
using System.Diagnostics;
using System.CodeDom;
namespace Center.APP
{
    internal class Template_Shipping : App
    {

        public Template_Shipping() { }  
        public void SetUp_Template_Shipping(string PO)
        {
            this.PO = PO;
        }
        public Excel.Workbook Open_Excel(string fileNameStart)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Excel.Workbook workbook;
            Excel.Application excelApp = null;

            // Kiểm tra nếu file Excel đã mở
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
            {
                try
                {
                    excelApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application") as Excel.Application;
                    if (excelApp != null)
                    {
                        foreach (Excel.Workbook wb in excelApp.Workbooks)
                        {
                            // Kiểm tra nếu tên Workbook bắt đầu bằng fileNameStart
                            if (wb.Name.StartsWith(fileNameStart, StringComparison.OrdinalIgnoreCase))
                            {
                                excelApp.Visible = true;
                                return wb; // Trả về Workbook đã mở
                            }
                        }
                    }
                }
                catch
                {
                    // Bỏ qua lỗi nếu không thể kết nối với Excel instance
                }
            }

            // Nếu chưa mở, tìm file trong thư mục Desktop
            string[] matchingFiles = System.IO.Directory.GetFiles(desktopPath, fileNameStart + "*", System.IO.SearchOption.TopDirectoryOnly);

            if (matchingFiles.Length > 0)
            {
                // Mở file đầu tiên khớp với fileNameStart
                string filePath = matchingFiles[0];
                if (excelApp == null)
                {
                    excelApp = new Excel.Application();
                }
                workbook = excelApp.Workbooks.Open(filePath);
                excelApp.Visible = true; // Hiển thị Excel
                return workbook;
            }
            else
            {
                throw new Exception($"Không tìm thấy file bắt đầu với '{fileNameStart}' trong thư mục Desktop. Vui lòng kiểm tra!");
            }
        }
        #region remove
        //public Excel.Workbook Open_Excel()
        //{
        //    string filePath = @"C:\Users\f2pc\Desktop\Template_ShippingLabel_VBEP_2K_Press_20240601_2nd.xls";
        //    Excel.Workbook workbook;
        //    // Khởi tạo ứng dụng Excel
        //    Excel.Application excelApp = null;

        //    // Kiểm tra nếu file Excel đã mở
        //    foreach (var process in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
        //    {
        //        excelApp = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application") as Excel.Application;
        //        if (excelApp != null)
        //        {
        //            foreach (Excel.Workbook wb in excelApp.Workbooks)
        //            {
        //                if (wb.FullName == filePath) // Kiểm tra nếu workbook đang mở
        //                {
        //                    excelApp.Visible = true;
        //                    //Fill_Excel(wb, "c", "10");
        //                    return wb; // Trả về workbook đã mở
        //                }
        //            }
        //        }
        //    }

        //    // Nếu chưa mở, tạo một ứng dụng Excel mới
        //    if (excelApp == null)
        //    {
        //        excelApp = new Excel.Application();
        //    }

        //    // Kiểm tra nếu file tồn tại
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        // Mở file Excel
        //        workbook = excelApp.Workbooks.Open(filePath);
        //        //Fill_Excel(workbook, "c", "10");
        //        // Hiển thị Excel
        //        excelApp.Visible = true;

        //        // Trả về đối tượng workbook
        //        return workbook;
        //    }
        //    else
        //    {
        //        throw new Exception("File không tồn tại. Vui lòng kiểm tra đường dẫn!");
        //    }





        //}
        //public void Open_Excel()
        //{
        //    string filePath = @"C:\Users\f2pc\Desktop\Book1.xlsx";

        //    try
        //    {
        //        // Khởi tạo ứng dụng Excel
        //        Excel.Application excelApp = new Excel.Application();

        //        // Kiểm tra nếu file tồn tại
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            // Mở file Excel
        //            Excel.Workbook workbook = excelApp.Workbooks.Open(filePath);

        //            // Hiển thị Excel cho người dùng
        //            excelApp.Visible = true;

        //            // Dọn dẹp đối tượng COM sau khi sử dụng
        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
        //            workbook = null;

        //            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        //            excelApp = null;

        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //        }
        //        else
        //        {
        //            MessageBox.Show("File không tồn tại. Vui lòng kiểm tra đường dẫn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Lỗi khi mở file Excel: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        #endregion remove
        public async Task Fill_TextBox(Excel.Workbook workbook, string sheetName, string textBoxName, string value)
        {
            // Lấy sheet từ workbook
            Excel.Worksheet worksheet = workbook.Sheets[sheetName];
            workbook.Application.Visible = true;
            // Lấy đối tượng TextBox thông qua OLEObjects
            Excel.OLEObject textBox = worksheet.OLEObjects(textBoxName) as Excel.OLEObject;

            if (textBox != null)
            {
                textBox.Object.GetType().InvokeMember("Text",
                    System.Reflection.BindingFlags.SetProperty,
                    null, textBox.Object, new object[] { value });
                IntPtr hwnd = new IntPtr(worksheet.Application.Hwnd);
                Control.MakeWindowTopMost(hwnd);
                ShowWindow(hwnd, 3);
                SetForegroundWindow(hwnd);
                try
                {
                    textBox.Activate();
                    //await Task.Delay(4000);
                    SendKeys.SendWait("{ENTER}");
                }
                catch { }
                Control.UNMakeWindowTopMost(hwnd);
            }
            else
            {
                throw new Exception($"Không tìm thấy TextBox với tên {textBoxName} trên sheet {sheetName}.");
            }

            // Dọn dẹp đối tượng COM
            System.Runtime.InteropServices.Marshal.ReleaseComObject(textBox);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public async Task<bool> Start_Template_Shipping()
        {
            Excel.Workbook workbook = Open_Excel("Template_ShippingLabel_VBEP_2K");
            await Fill_TextBox(workbook, "PrintControl", "Text_OrderNo", PO);
            Finish_App(GlobalVariables.tcs_Template_Shipping);
            return true;
        }
    }
}
