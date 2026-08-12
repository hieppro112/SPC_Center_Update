using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capture
{
    internal class ConfRegistry
    {
        public static void RunReg()
        {
            //string regFilePath = @"chrome_debug.reg"; // Đường dẫn tới tệp .reg trong thư mục đầu ra của dự án
            //string regFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chrome_debug.reg");
            //string regFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MyApp", "chrome_debug.reg");
            //string regFilePath = @"\\192.168.122.2\dtp-data\Diep\New folder\chrome_debug.reg";
            string regFilePath = @"\\192.168.122.2\Soft F2\Application\108. Capture_POFinish_K99282\chrome_debug.reg";
            
            //
            if (!File.Exists(regFilePath))
            {
                MessageBox.Show($"Looking for registry file at: {regFilePath}", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Registry file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Chạy tệp .reg
            ProcessStartInfo processStartInfo = new ProcessStartInfo("regedit.exe")
            {
                Arguments = $"/s \"{regFilePath}\"", // Tham số /s để thực thi mà không hiển thị hộp thoại xác nhận
                Verb = "runas", // Chạy với quyền Administrator
                CreateNoWindow = true,
                UseShellExecute = true
            };

            try
            {
                Process.Start(processStartInfo);
                //MessageBox.Show("Setup Complete");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing registry script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void CheckRegistry()
        {
            // Đường dẫn đến khóa registry
            string registryPath = @"HKEY_CLASSES_ROOT\ChromeHTML\shell\open\command";

            // Giá trị mới cho khóa registry
            string newValue = "\"C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe\" --remote-debugging-port=9222  --\"%1\"";

            // Lấy giá trị hiện tại của khóa registry
            string currentValue = (string)Registry.GetValue(registryPath, "", null);

            // Kiểm tra xem giá trị hiện tại có giống giá trị mới không
            if (currentValue == newValue)
            {
                //MessageBox.Show("Registry value is already set correctly.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    // Thiết lập giá trị mới cho khóa registry
                    //Registry.SetValue(registryPath, "", newValue, RegistryValueKind.String);
                    RunReg();
                    //MessageBox.Show("Registry value set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error setting registry value: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
