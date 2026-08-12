using Capture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Center.FORM
{
    public partial class OperatorForm : Form
    {
        public OperatorForm()
        {
            InitializeComponent();
            //Control.ClearAllLists();
            //Control.Init_List(Environment.MachineName);
            //Control.SetUpApp(this);
        }

        public void OperatorForm_Load(object sender, EventArgs e)
        {
            ConfRegistry.CheckRegistry();// thiêt lặp chrome ở chế độ remmote debugging
            Control.ClearAllLists();  // Clear tất cả các list sử dụng để chứa APP
            Control.Init_List(Environment.MachineName); // lấy danh sách list của máy
            Control.SetUpApp(this); // setup App
            GlobalVariables.opF = this;
        }

        private void OperatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalVariables.UnAllMouseHook();
        }
    }
}
