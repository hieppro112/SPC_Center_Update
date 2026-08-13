using Center.FORM;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Center.WinAPI;
namespace Center.APP
{
    internal class Supro : App
    {
        int CheckKCODE = 0;
        string machinename;
        int shift;
        public IPage page = null;
        //string PO;
        string realPO;
        string checkTaskPO;
        public Supro() { }
        string PO_Date;

        public void SetInputSupro(string machinename, int shift, string MANV, string PO)
        {
            this.machinename = machinename;
            this.shift = shift;
            this.MANV = MANV;
            this.PO = PO;

        }

        //public void StartChromeIfNotRunning()
        //{
        //    var processes = Process.GetProcessesByName("chrome");
        //    bool isRunning = false;
        //    if (processes.Length > 0)
        //    {
        //        isRunning = true;
        //    }
        //    if (!isRunning)
        //    {
        //        Process.Start("chrome.exe", "--remote-debugging-port=9222");
        //    }
        //}





        public async Task TypeMachinename(IPage page, string machinename)
        {
            try
            {
                await Task.Delay(200);
                foreach (var c in machinename)
                {
                    await page.Keyboard.PressAsync(c.ToString());
                }
                await Task.Delay(200);
                await page.Keyboard.PressAsync("Enter");
            }
            catch { }

        }

        public async Task SelectShift(IPage page, int mappedValue)
        {
            try
            {
                await Task.Delay(200);
                await page.ClickAsync("#ddlShiftCD_lblMenuTop");
                string elementId = $"ddlShiftCD_ListViewMenuDtl_ListViewItem_{mappedValue}";
                await Task.Delay(200);
                await page.WaitForSelectorAsync($"#{elementId}");
                await page.ClickAsync($"#{elementId}");
            }
            catch { }
        }

        public async Task SetupPage(IBrowser browser)
        {
            try
            {
                var navigationTask = page.WaitForNavigationAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 },
                    Timeout = 30000
                });
                await Task.Delay(400);
                await TypeMachinename(page, machinename);
                await Task.Delay(400);
                // Chọn một shift
                await SelectShift(page, shift);
                await Task.Delay(400);
                // Nhấn login
                await page.ClickAsync("#btnLogin");
                await Task.Delay(300);
                await navigationTask;
            }
            catch { }

        }

        public void Check_Start_Stop_KCode(IPage page, OperatorForm opF)
        {
            // biến check KCODE để xem là đã gắn sự kiện này cho page chưa, để các lần khác ko cần gắn lại
            if (CheckKCODE == 0)
            {
                CheckKCODE = 1;
                EventHandler<PuppeteerSharp.RequestEventArgs> requestHandler2 = null;
                requestHandler2 = async (sender, e) =>
                {
                    var requestUrl = e.Request.Url;
                    var method = e.Request.Method;

                    // Kiểm tra yêu cầu HTTP POST với URL đặc biệt
                    if (requestUrl.StartsWith("http://10.4.24.117:8441/sp/Kcode") && method.ToString().ToUpper() == "POST")
                    {
                        // Lấy dữ liệu body của yêu cầu HTTP
                        string body = e.Request.PostData;
                        string rs = getScriptManager1(body);

                        // Xử lý các sự kiện bắt đầu hoặc kết thúc từ yêu cầu HTTP
                        if (rs == "btnStart")
                        {
                            try
                            {
                                opF.Invoke(new Action(() =>
                                {
                                    //GlobalVariables.tf_Kcode.Close();
                                    GlobalVariables.tf_Kcode = new TimeForm(100);
                                    GlobalVariables.tf_Kcode.Show();
                                    GlobalVariables.tf_Kcode.setProcess("KCODE");
                                    if (!GlobalVariables.tf_Process.IsDisposed)
                                    {
                                        Debug.Print("Pause count");
                                        GlobalVariables.tf_Process.PauseCount();
                                    }
                                }));

                            }
                            catch { }

                        }
                        else if (rs == "btnFinish")
                        {
                            string check = getKcodeStarted(body);

                            if (check != "")
                            {
                                try
                                {
                                    GlobalVariables.tf_Kcode.Invoke(new Action(() =>
                                    {
                                        if (GlobalVariables.tf_Kcode != null && !GlobalVariables.tf_Kcode.IsDisposed)
                                        {
                                            Debug.Print("Đóng tf_Kcode.");
                                            GlobalVariables.tf_Kcode.Close();
                                            GlobalVariables.tf_Kcode.Dispose();
                                            GlobalVariables.tf_Kcode = null;
                                        }
                                        else
                                        {
                                            Debug.Print("tf_Kcode không tồn tại hoặc đã bị đóng.");
                                        }

                                        if (GlobalVariables.tf_Process.Visible)
                                        {
                                            GlobalVariables.tf_Process.ContinueCount();
                                        }
                                    }));
                                }
                                catch { }
                            }

                        }
                    }
                };
                page.Request += requestHandler2;
            }

        }


        public string gethidUpdListControl(string body)
        {
            string btnStartValue = "";
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    var keyValuePairs = body.Split('&')
                    .Select(part => part.Split('='))
                    .Where(pair => Uri.UnescapeDataString(pair[0]) == "hidUpdListControl" || pair.Length > 1)
                    .ToDictionary(
                    pair => Uri.UnescapeDataString(pair[0]),
                    pair => pair.Length > 1 ? Uri.UnescapeDataString(pair[1]) : ""
                    );
                    // Lấy giá trị của hidUpdListControl
                    if (keyValuePairs.TryGetValue("hidUpdListControl", out var scriptManagerValue))
                    {
                        btnStartValue = scriptManagerValue;

                    }
                }
                return btnStartValue;
            }
            catch
            {
                return "";
            }
        }

        // Cách check POFinish cũ
        public async Task<bool> checkFinishByClick(int value, IPage page, string process)
        {
            //if (process == "Inspection")
            //{
            //    GlobalVariables.tf_Process = new TimeForm(value, hp);
            //    GlobalVariables.tf_Process.Show();
            //    GlobalVariables.tf_Process.setProcess("INSPECTION - Qty: " + value);
            //}
            //else
            //{
            //    GlobalVariables.tf_Process = new TimeForm(value, hp);
            //    GlobalVariables.tf_Process.Show();
            //    GlobalVariables.tf_Process.setProcess("Packing - Qty: " + value);
            //}
            int numsClick = 0;
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                await page.ExposeFunctionAsync("OnButtonClicked", () =>
                {
                    numsClick = numsClick + 1;
                });
            }
            catch
            {
            }

            await page.EvaluateFunctionAsync(@"() => {
                // Hàm gắn lại sự kiện
                const attachClickEvent = () => {
                    const button = document.getElementById('btnPOFinish');
                    if (button && !button.hasAttribute('data-event-attached')) {
                        button.setAttribute('data-event-attached', 'true');
                        button.addEventListener('pointerdown', () => {
                            
                            window.OnButtonClicked && window.OnButtonClicked();

                            console.log('Button clicked');
                        });
                    }
                };
                // Gắn sự kiện ban đầu
                attachClickEvent();

                // Quan sát sự thay đổi DOM
                const observer = new MutationObserver(() => {
                    //console.log('DOM changed, checking button...');
                    attachClickEvent();
                });

                // Bắt đầu quan sát cây DOM
                observer.observe(document.body, { childList: true, subtree: true });
            }");


            EventHandler<FrameNavigatedEventArgs> frameNavigatedHandler = null;
            EventHandler<PuppeteerSharp.RequestEventArgs> requestHandler = null;
            EventHandler<PuppeteerSharp.ResponseCreatedEventArgs> response = null;


            frameNavigatedHandler = async (sender2, e2) =>
            {
                var frameUrl = e2.Frame.Url;

                // Kiểm tra nếu URL bắt đầu bằng 
                if (frameUrl.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                {
                    await page.EvaluateFunctionAsync(@"() => {
                        // Hàm gắn lại sự kiện
                        const attachClickEvent = () => {
                            const button = document.getElementById('btnPOFinish');
                            if (button && !button.hasAttribute('data-event-attached')) {
                                button.setAttribute('data-event-attached', 'true');
                                button.addEventListener('pointerdown', () => {
                                    window.OnButtonClicked && window.OnButtonClicked();
                                    console.log('Button clicked');
                                });
                            }
                        };

                        // Gắn sự kiện ban đầu
                        attachClickEvent();

                        // Đảm bảo document.body đã sẵn sàng trước khi sử dụng MutationObserver
                        const observerTarget = document.body || document.documentElement;

                        if (observerTarget) {
                            // Quan sát sự thay đổi DOM
                            const observer = new MutationObserver(() => {
                                //console.log('DOM changed, checking button...');
                                attachClickEvent();
                            });

                            // Bắt đầu quan sát cây DOM
                            observer.observe(observerTarget, { childList: true, subtree: true });
                        } else {
                            console.log('document.body is not available');
                        }
                    }");




                }
            };

            response = async (sender, e) =>
            {

                if (e.Response.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                {
                    //value = await getValidityCnt(page);
                    try
                    {
                        var body = await e.Response.TextAsync();

                        // Sử dụng Regex để trích xuất giá trị validityCnt
                        var validityCnt = Regex.Match(body, @"<span[^>]*id=""lblvalidityCnt""[^>]*>(\d+)</span>");

                        if (validityCnt.Success)
                        {
                            // Trích xuất và in ra giá trị của lblvalidityCnt
                            value = int.Parse(validityCnt.Groups[1].Value);
                            //hp.setProText("1. Packing Supro: " + value);
                        }
                    }
                    catch { }
                    Debug.Print("---------------------------------" + value);
                }
            };

            requestHandler = (sender, e) =>
            {
                var requestUrl = e.Request.Url;
                var method = e.Request.Method;

                if (requestUrl.StartsWith("http://10.4.24.117:8441/sp/Standby") && method.ToString().ToUpper() == "GET")
                {
                    if (numsClick >= 1 && value > 0)
                    {
                        tcs.TrySetResult(true);
                        //hp.setProcess("Running");
                    }
                    else
                    {
                        tcs.TrySetResult(false);
                    }
                    page.Request -= requestHandler;
                    page.FrameNavigated -= frameNavigatedHandler;
                    page.Response -= response;



                    //hp.CloseTF(tf_Kcode, GlobalVariables.tf_Process);
                }
            };

            page.Response += response;
            page.Request += requestHandler;
            page.FrameNavigated += frameNavigatedHandler;

            return await tcs.Task;
        }


        // Cách check POFinish mới
        public void checkFinishWhenClick(int value, IPage page)
        {
            // Check Form đếm giờ của Supro có hiện lên ko, đề phòng lỗi
            if (GlobalVariables.tf_Process.Visible) // Có hiện lên
            {
                GlobalVariables.tf_Process.Invoke(new Action(() =>
                {
                    try
                    {
                        GlobalVariables.tf_Process.Close(); // đóng form
                    }
                    catch { }


                }));
            }

            // Mở form đếm giờ
            GlobalVariables.tf_Process = new TimeForm(value);
            GlobalVariables.tf_Process.Show();
            GlobalVariables.tf_Process.setProcess("Supro - Qty: " + value);


            int numsClick = 0;// Check số lần Click vào button POFinish
            EventHandler<PuppeteerSharp.RequestEventArgs> requestHandler = null; // lắng nghe request
            EventHandler<PuppeteerSharp.ResponseCreatedEventArgs> response = null;// lắng nghe reponse


            // reponse này mình sẽ lấy Validity của PO trên Supro gán vào biến Value
            response = async (sender, e) =>
            {
                if (e.Response.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                {
                    //value = await getValidityCnt(page);
                    try
                    {
                        var body = await e.Response.TextAsync();

                        // Sử dụng Regex để trích xuất giá trị validityCnt
                        var validityCnt = Regex.Match(body, @"<span[^>]*id=""lblvalidityCnt""[^>]*>(\d+)</span>");

                        if (validityCnt.Success)
                        {

                            value = int.Parse(validityCnt.Groups[1].Value);

                        }
                    }
                    catch { }

                }
            };
            // lắng nghe sự kiện request
            requestHandler = async (sender, e) =>
            {
                var requestUrl = e.Request.Url;
                var method = e.Request.Method;

                if (requestUrl.StartsWith("http://10.4.24.117:8441/sp/Standby") && method.ToString().ToUpper() == "GET")
                {
                    if (numsClick == 1 && value > 0)// Đã click vào button POFinish và có Validity Number > 0
                    {
                        Finish_App(tcs);
                    }
                    else
                    {
                        Cancel_App(tcs);
                    }
                    page.Request -= requestHandler;
                    page.Response -= response;

                    GlobalVariables.tf_Process.Invoke(new Action(() =>// đóng form đếm giờ
                    {
                        try
                        {
                            GlobalVariables.tf_Process.Close();
                        }
                        catch { }


                    }));
                }
                else if (requestUrl.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                {
                    string body = "";
                    try
                    {
                        body = e.Request.PostData; // lấy data trogn phương thức post 
                    }
                    catch { }

                    // lấy giá trị trường hidUpdListControl trong data
                    string rs = gethidUpdListControl(body);
                    if (rs == "POFinish")// nêu giá trị = POFinish thì ng dùng đã click vào button POFinish
                    {
                        numsClick = 1; // numsClick = 1
                    }
                }
            };
            page.Response += response;
            page.Request += requestHandler;
        }

        //Check máy có đúng là đã nhập từ trước không
        public async Task<bool> RightMachine(IPage page, string IDinTxt)
        {
            //#txtTerminalID
            bool check = false;
            var terminalIdValue = "";
            try
            {
                terminalIdValue = await page.EvaluateExpressionAsync<string>(
                "document.getElementById('txtTerminalID').value");
            }
            catch { }

            string IDinTxt_2 = IDinTxt.StartsWith("TR") ? IDinTxt.Substring(2) : IDinTxt;
            if (terminalIdValue == IDinTxt_2)
            {
                check = true;
            }

            return check;

        }
        public async void FillMaNVPo(IPage page, string MANV, string PO)
        {
            foreach (var c in MANV)
            {
                await page.Keyboard.PressAsync(c.ToString());
            }
            await Task.Delay(200);
            await page.Keyboard.PressAsync("Enter");
            await Task.Delay(200);

            foreach (var c in PO)
            {
                await page.Keyboard.PressAsync(c.ToString());
            }
            await Task.Delay(200);
            await page.Keyboard.PressAsync("Enter");
        }

        public async Task<string> WaitForUrlChangeAsyncPROCESSING(string check, IPage inspection, string nextInsUrl)
        {
            string a = "";
            while (check == PO_Date)
            {
                try
                {
                    // Kiểm tra nếu trang đã bị đóng
                    if (inspection.IsClosed)
                    {
                        Console.WriteLine("Trình duyệt đã bị đóng.");
                        break;
                    }

                    string currentUrl = inspection.Url;

                    // Kiểm tra nếu URL đã thay đổi
                    if (currentUrl != nextInsUrl && currentUrl.StartsWith("http://10.4.24.117:8441/sp/ProcessingLv1"))
                    {
                        if (check == PO_Date)
                        {
                            return currentUrl;
                        }
                    }

                    await Task.Delay(1000);
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                    break;
                }
            }
            return a;
        }


        public IntPtr GetHandleChrome()
        {
            var processes = Process.GetProcessesByName("chrome");
            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero) // Chỉ lấy tiến trình có cửa sổ chính
                    {
                        return process.MainWindowHandle;
                    }
                }
            }
            return IntPtr.Zero;
        }

        public static string getKcodeStarted(string body)
        {
            string result = "";
            if (!string.IsNullOrEmpty(body))
            {
                var keyValuePairs = body.Split('&')
                    .Select(part => part.Split('='))
                    .Where(pair => pair.Length > 1)
                    .ToDictionary(
                        pair => Uri.UnescapeDataString(pair[0]),
                        pair => Uri.UnescapeDataString(pair[1])
                    );

                // Duyệt qua các key trong Dictionary
                foreach (var kvp in keyValuePairs)
                {
                    // Kiểm tra nếu key bắt đầu bằng "lstKcode" và giá trị là "1"
                    if (kvp.Key.StartsWith("lstKcode$") && kvp.Key.Contains("$lblSelected") && kvp.Value == "1")
                    {
                        result = "ok";
                        break;
                    }
                }
            }
            return result;
        }

        public static string getScriptManager1(string body)
        {
            string btnStartValue = "";
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    var keyValuePairs = body.Split('&')
                    .Select(part => part.Split('='))
                    .Where(pair => Uri.UnescapeDataString(pair[0]) == "ScriptManager1" || pair.Length > 1)
                    .ToDictionary(
                    pair => Uri.UnescapeDataString(pair[0]),
                    pair => pair.Length > 1 ? Uri.UnescapeDataString(pair[1]) : ""
                    );

                    // Lấy giá trị của ScriptManager1
                    if (keyValuePairs.TryGetValue("ScriptManager1", out var scriptManagerValue))
                    {
                        btnStartValue = scriptManagerValue.Split('|').LastOrDefault();

                    }
                }
                return btnStartValue;
            }
            catch
            {
                return "";
            }

        }

        //public async Task SetUp_SuproInControl()
        //{
        //    if (page == null || page.IsClosed)
        //    {
        //        var screenWidth = Screen.PrimaryScreen.Bounds.Width;
        //        var screenHeight = Screen.PrimaryScreen.Bounds.Height;
        //        StartChromeIfNotRunning();
        //        if (GlobalVariables.browser == null)
        //        {
        //            Debug.Print("Browser null");
        //            GlobalVariables.browser = await Puppeteer.ConnectAsync(new ConnectOptions
        //            {

        //                BrowserURL = "http://localhost:9222"
        //            });
        //            try
        //            {
        //                var pages = await GlobalVariables.browser.PagesAsync();
        //                foreach (var page in pages)
        //                {
        //                    await page.SetViewportAsync(new ViewPortOptions
        //                    {
        //                        Width = (int)screenWidth,
        //                        Height = (int)screenHeight - 130
        //                    });
        //                }
        //            }
        //            catch { return; }
        //        }


        //        page = await GlobalVariables.browser.NewPageAsync();
        //        await page.SetViewportAsync(new ViewPortOptions
        //        {
        //            Width = (int)screenWidth,
        //            Height = (int)screenHeight - 130
        //        });

        //        await page.GoToAsync("http://10.4.24.117:8441/Login.aspx");
        //    }
        //}

        public async Task StartChromeIfNotRunning(string userProfilePath = @"C:\ChromeDebug2")
        {
            // Không quan tâm Chrome thường, chỉ mở instance debug riêng
            if (!Directory.Exists(userProfilePath))
                Directory.CreateDirectory(userProfilePath);

            // Kiểm tra cổng 9222 có Chrome đang nghe chưa
            if (await IsChromeDebuggingAvailableAsync())
            {
                Console.WriteLine("♻️ Chrome debug đã chạy, dùng lại.");
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = "chrome",
                Arguments = $"--remote-debugging-port=9222 " +
                            $"--user-data-dir=\"{userProfilePath}\" " +
                            "--no-first-run --no-default-browser-check " +
                            "--new-window",
                UseShellExecute = true
            };

            Process.Start(psi);
            Console.WriteLine("🚀 Đang khởi chạy Chrome riêng biệt...");

            // Chờ Chrome khởi động
            for (int i = 0; i < 10; i++)
            {
                if (await IsChromeDebuggingAvailableAsync())
                {
                    Console.WriteLine("✅ Chrome sẵn sàng tại http://localhost:9222");
                    return;
                }
                await Task.Delay(500);
            }

            Console.WriteLine("❌ Không thể kết nối tới http://localhost:9222");
        }

        private async Task<bool> IsChromeDebuggingAvailableAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var res = await client.GetAsync("http://localhost:9222/json/version");
                    return res.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task SetupSupro()
        {
            if (page == null || page.IsClosed)
            {
                var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = Screen.PrimaryScreen.Bounds.Height;

                // 🔹 Mở Chrome riêng (không ảnh hưởng Chrome người dùng)
                await StartChromeIfNotRunning(@"C:\ChromeDebug_Supro");

                if (GlobalVariables.browser == null || !GlobalVariables.browser.IsConnected)
                {
                    Debug.Print("🔌 Browser chưa kết nối, đang kết nối...");

                    try
                    {
                        string wsEndpoint;
                        using (var client = new HttpClient())
                        {
                            var json = await client.GetStringAsync("http://localhost:9222/json/version");
                            var doc = JsonDocument.Parse(json);
                            wsEndpoint = doc.RootElement.GetProperty("webSocketDebuggerUrl").GetString();
                        }

                        GlobalVariables.browser = await Puppeteer.ConnectAsync(new ConnectOptions
                        {
                            BrowserWSEndpoint = wsEndpoint
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("❌ Không thể kết nối browser: " + ex.Message);
                        return;
                    }
                }

                page = await GlobalVariables.browser.NewPageAsync();
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = (int)screenWidth,
                    Height = (int)screenHeight
                });

                //await page.EvaluateExpressionAsync("window.location.href = 'http://10.4.24.117:8441/Login.aspx';");
                //await page.WaitForNavigationAsync();

                await NavigateAndWaitAsync("http://10.4.24.117:8441/Login.aspx");

                await page.BringToFrontAsync();
                await SetupPage(GlobalVariables.browser);
            }

            if (page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                return;

            bool isPageValid = page.Url.StartsWith("http://10.4.24.117:8441/sp/Standby.aspx");
            if (!isPageValid)
            {
                await page.BringToFrontAsync();
                //await page.EvaluateExpressionAsync("window.location.href = 'http://10.4.24.117:8441/Login.aspx';");
                //await page.WaitForNavigationAsync();

                await NavigateAndWaitAsync("http://10.4.24.117:8441/Login.aspx");

                await SetupPage(GlobalVariables.browser);
            }

            await Task.Delay(500);
            bool Z = await RightMachine(page, machinename);
            if (!Z)
            {
                await page.BringToFrontAsync();
                //await page.EvaluateExpressionAsync("window.location.href = 'http://10.4.24.117:8441/Login.aspx';");
                //await page.WaitForNavigationAsync();

                await NavigateAndWaitAsync("http://10.4.24.117:8441/Login.aspx");

                await SetupPage(GlobalVariables.browser);
            }

            try
            {
                var currentTime = await page.EvaluateFunctionAsync<string>(@"() => {
            const now = new Date();
            return `${now.getHours()}${now.getMinutes()}`;
        }");
            }
            catch { }
        }
        private async Task NavigateAndWaitAsync(string url)
        {
            var navigationTask = page.WaitForNavigationAsync(new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }, // hoặc Load / DOMContentLoaded tùy nhu cầu
                Timeout = 30000
            });

            var navigateTask = page.Client.SendAsync("Page.navigate", new { url });

            await Task.WhenAll(navigationTask, navigateTask);
        }
        public async Task<int> getValidityCnt(IPage page)
        {
            try
            {
                await page.WaitForSelectorAsync("#lblvalidityCnt");
                var valueStr = await page.EvaluateFunctionAsync<string>(@"() => {
                const text = document.getElementById('lblvalidityCnt').innerText;
                return text === '' || text === null ? '0' : text; }");

                int value;
                if (!int.TryParse(valueStr, out value))
                {
                    value = 0; // Nếu không thể parse, gán giá trị mặc định là 0
                }
                return value;
            }
            catch
            {
                return 0; // Nếu có bất kỳ lỗi nào xảy ra, trả về 0
            }
        }

        public async Task<bool> StartSupro(OperatorForm opF)
        {
            await SetupSupro();
            await page.BringToFrontAsync();
            var currentTime = await page.EvaluateFunctionAsync<string>(@"() => {
                const now = new Date();
                const hours = now.getHours();
                const minutes = now.getMinutes();
                const seconds = now.getSeconds();
                return `${hours}${minutes}$`;
            }");

            PO_Date = PO + "_" + currentTime;
            string check = PO_Date;
            await Task.Delay(300);
            string currentInsUrl = page.Url;

            FillMaNVPo(page, MANV, PO);// Nhập mã nhân viên và PO
            Check_Start_Stop_KCode(page, opF);// Sự kiện lắng nghe người dùng có start Kcode hay Stop Kcode không
            await Task.Delay(1500);
            string nextInsUrl = page.Url;
            IntPtr cH = GetHandleChrome();

            //Xem thử đã vào màn hình processing chưa 
            if (currentInsUrl == nextInsUrl)// chưa vào Processing
            {
                //return false;
                await page.BringToFrontAsync();
                nextInsUrl = await WaitForUrlChangeAsyncPROCESSING(check, page, currentInsUrl);// Chờ ng dùng vào processing 
                if (nextInsUrl == "") // ko vào được processing -> hủy
                {
                    return false;
                }
                // Vào được Processing -> tiếp tục bên dưới
            }
            int value = await getValidityCnt(page);// lấy Validity number của PO
            checkFinishWhenClick(7, page); // sự kiện chờ Click PO Finish

            return true;
        }
    }
}
