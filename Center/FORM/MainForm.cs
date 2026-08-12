using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Center
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        SQL tf = new SQL();
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Control.Run_Flow();
        }

        private void selectComboToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Control.ClearAllLists();
            ////Control.Init_List();
            //Control.SetUpApp();


            SetUp st = new SetUp();
            st.TopMost = true;
            st.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //GlobalVariables.Apps.Add("Supro-Z200");
            //GlobalVariables.Apps.Add("Inspection_Standard");
            //GlobalVariables.Apps.Add("Supro-Z300");
            //GlobalVariables.Apps.Add("Print_Label");
            //GlobalVariables.Groups.Add(1);
            //GlobalVariables.Groups.Add(1);
            //GlobalVariables.Groups.Add(2);
            //GlobalVariables.Groups.Add(2);



            string query1 = $@"SELECT DISTINCT tablet_name
            FROM F2Database.dbo.F2_ControlApp_Master
            WHERE tablet_name = '{Environment.MachineName}'";

            string query = $@"SELECT DISTINCT tablet_name
            FROM F2Database.dbo.F2_ControlApp_Master";
            DataTable dt = new DataTable();
            tf.sqlDataAdapterFillDatatable(query1, ref dt);
            Debug.Print("count: " + dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                int yOffset = 30; // Khoảng cách giữa các Button trong GroupBox
                foreach (DataRow row in dt.Rows)
                {
                    string tabletName = row["tablet_name"].ToString();

                    // Kết hợp tablet_name và reg_date
                    string buttonText = tabletName;

                    // Tạo Button
                    Button button = new Button();
                    button.Text = buttonText;
                    button.Width = 300;     // Đặt chiều rộng cho Button
                    button.Height = 30;     // Đặt chiều cao cho Button
                    button.Location = new Point(10, yOffset); // Vị trí trong GroupBox

                    // Thêm sự kiện cho Button nếu cần
                    button.Click += (s, e2) =>
                    {
                        Control.ClearAllLists();
                        Control.Init_List(button.Text);
                        //Control.SetUpApp();
                    };

                    // Thêm Button vào GroupBox
                    grb_Combo.Controls.Add(button);

                    // Tăng yOffset để xếp các Button theo chiều dọc
                    yOffset += 40;
                }
            }


            //Control.ClearAllLists();
            //Control.Init_List(Environment.MachineName);
            //Control.SetUpApp();
            //this.Hide();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string query1 = $@"SELECT DISTINCT tablet_name
                FROM F2Database.dbo.F2_ControlApp_Master
                WHERE tablet_name = '{Environment.MachineName}'";

            string query = $@"SELECT DISTINCT tablet_name
                FROM F2Database.dbo.F2_ControlApp_Master";
            DataTable dt = new DataTable();
            tf.sqlDataAdapterFillDatatable(query, ref dt);
            Debug.Print("count: " + dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                int yOffset = 30; // Khoảng cách giữa các Button trong GroupBox
                foreach (DataRow row in dt.Rows)
                {
                    string tabletName = row["tablet_name"].ToString();

                    // Kết hợp tablet_name và reg_date
                    string buttonText = tabletName;

                    // Tạo Button
                    Button button = new Button();
                    button.Text = buttonText;
                    button.Width = 300;     // Đặt chiều rộng cho Button
                    button.Height = 30;     // Đặt chiều cao cho Button
                    button.Location = new Point(10, yOffset); // Vị trí trong GroupBox

                    // Thêm sự kiện cho Button nếu cần
                    button.Click += (s, e2) =>
                    {
                        Control.ClearAllLists();
                        Control.Init_List(button.Text);
                        //Control.SetUpApp();
                    };

                    // Thêm Button vào GroupBox
                    grb_Combo.Controls.Add(button);

                    // Tăng yOffset để xếp các Button theo chiều dọc
                    yOffset += 40;
                }
            }
        }
    }
}
