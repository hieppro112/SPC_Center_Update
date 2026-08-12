using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Center.WinAPI;
namespace Center.FORM
{
    public partial class TimeForm : Form
    {

        int WarningTime = 300000;
        int tm = 60000;
        int elapsedSeconds = 0;
        int check = 0;
        int running = 1;
        public TimeForm(int value)
        {
            WarningTime = WarningTime * value;
            InitializeComponent();
        }
        public void StartForm()
        {

            //this.Show();
            StartCount();
            StartCount_Warning();

        }
        private void TimeForm_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            StartForm();
        }
        public void setProcess(string s)
        {
            lbl_Process.Text = s;
        }


        public void MakeWindowTopMost(IntPtr hWnd)
        {
            const int HWND_TOPMOST = -1; // Đặt trạng thái top-most
            //const int HWND_NOTOPMOST = -2; // Trả về trạng thái không top-most
            const uint SWP_NOMOVE = 0x0002;
            const uint SWP_NOSIZE = 0x0001;
            const uint SWP_SHOWWINDOW = 0x0040;

            // Đưa lên trên mọi thứ (bao gồm các cửa sổ top-most)
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

        }
        public void UNMakeWindowTopMost(IntPtr hWnd)
        {
            //const int HWND_TOPMOST = -1; // Đặt trạng thái top-most
            const int HWND_NOTOPMOST = -2; // Trả về trạng thái không top-most
            const uint SWP_NOMOVE = 0x0002;
            const uint SWP_NOSIZE = 0x0001;
            const uint SWP_SHOWWINDOW = 0x0040;



            // Trả lại trạng thái bình thường
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        private void tm_S_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            TimeSpan elapsedTime = TimeSpan.FromSeconds(elapsedSeconds);
            lbl_Time.Text = elapsedTime.ToString(@"hh\:mm\:ss");
        }

        public void StartCount_Warning()
        {
            if (WarningTime > 0)
            {
                tm_Warning.Interval = WarningTime;  // Tick mỗi giây
            }
            else
            {
                tm_Warning.Interval = 60000;  // Tick mỗi giây

            }
            running = 0;

            tm_Warning.Start();  // Bắt đầu Timer

        }

        public void StartCount()
        {
            elapsedSeconds = 0;
            tm_S.Interval = 1000;  // Tick mỗi giây
            tm_S.Start();  // Bắt đầu Timer
        }

        public void PauseCount()
        {
            tm_S.Stop();
            tm_Warning.Stop();
            //lbl_Time.Text = "PAUSING";

        }
        public void ContinueCount()
        {

            int newIterval = WarningTime - elapsedSeconds * 1000;
            if (newIterval > 0)
            {
                tm_Warning.Interval = newIterval;
            }
            tm_S.Start();
            tm_Warning.Start();

        }

        private void tm_Warning_Tick(object sender, EventArgs e)
        {
            //MakeWindowTopMost(this.Handle);
            if (check == 0)
            {
                if (running == 0)
                {
                    //hp.setProcess("RunningTimeLimit");
                }
                tm_ChangeColor.Interval = 500;
                tm_ChangeColor.Start();
                check = 1;
            }
        }

        private void tm_ChangeColor_Tick(object sender, EventArgs e)
        {
            if (this.BackColor == Color.Red)
            {
                this.BackColor = Color.White;
            }
            else
            {
                this.BackColor = Color.Red;
            }
        }


    }
}
