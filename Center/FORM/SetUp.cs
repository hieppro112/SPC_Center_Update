using Center.FORM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Center
{
    public partial class SetUp : Form
    {

        public SetUp()
        {
            InitializeComponent();
        }
        SQL tf = new SQL();
        string tabletID = Environment.MachineName;
        string sql_query = $"Select app_name From F2Database.dbo.F2_ControlApp_Name ";

        private void SetUp_Load(object sender, EventArgs e)
        {
            //dgv_Setup.Rows.Add(false, "", "Supro-Z200","");
            //dgv_Setup.Rows.Add(false, "", "Supro-Z300", "");
            //dgv_Setup.Rows.Add(false, "", "Inspection_Standard", "");
            //dgv_Setup.Rows.Add(false, "", "Inspection_Special", "");
            //dgv_Setup.Rows.Add(false, "", "Print_Label", "");
            //dgv_Setup.Rows.Add(false, "", "SprueBush_PCS_QRCode", "");
            //dgv_Setup.Rows.Add(false, "", "CheckTemRollerlock_Manufa", "");
            //dgv_Setup.Rows.Add(false, "", "QRCodeRollerlock_Manufa", "");
            //dgv_Setup.Rows.Add(false, "", "QRCode_Oilless_MANUFA", "");
            //dgv_Setup.Rows.Add(false, "", "QRCode_SprueBush_Manufa", "");
            //dgv_Setup.CellValueChanged += dgv_Setup_CellValueChanged;

            DataTable dt = new DataTable();

            tf.sqlDataAdapterFillDatatable(sql_query, ref dt);

            dgv_Setup.Rows.Clear();

            foreach (DataRow row in dt.Rows)
            {
                dgv_Setup.Rows.Add(false, "", row["app_name"].ToString(), "");
            }
            dgv_Setup.CellValueChanged += dgv_Setup_CellValueChanged;

            txt_TabletID.Text = tabletID;
        }

        private void dgv_Setup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                //Debug.Print("hahaha");
                var sttValue = dgv_Setup.Rows[e.RowIndex].Cells["STT"].Value;
                if (sttValue != null && !string.IsNullOrEmpty(sttValue.ToString()) && int.TryParse(sttValue.ToString(), out _))
                {
                    dgv_Setup.Rows[e.RowIndex].Cells["Select"].Value = true;
                }
                else
                {
                    dgv_Setup.Rows[e.RowIndex].Cells["Select"].Value = false;
                    dgv_Setup.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    dgv_Setup.Rows[e.RowIndex].Cells["Group"].Value = "";
                }
            }


            #region remove
            //if (e.ColumnIndex == 1) // Cột STT
            //{
            //    var sttValue = dgv_Setup.Rows[e.RowIndex].Cells["STT"].Value;

            //    // Kiểm tra nếu giá trị là số hợp lệ
            //    if (sttValue != null && !string.IsNullOrEmpty(sttValue.ToString()) && int.TryParse(sttValue.ToString(), out int currentValue))
            //    {
            //        // Tìm số lớn nhất đã điền trước đó trong cột STT
            //        int maxValue = 0;
            //        for (int i = 0; i < dgv_Setup.Rows.Count; i++)
            //        {
            //            if (i != e.RowIndex) // Bỏ qua hàng hiện tại
            //            {
            //                var cellValue = dgv_Setup.Rows[i].Cells["STT"].Value;
            //                if (cellValue != null && int.TryParse(cellValue.ToString(), out int parsedValue))
            //                {
            //                    if (parsedValue > maxValue)
            //                    {
            //                        maxValue = parsedValue;
            //                    }
            //                }
            //            }
            //        }

            //        // Kiểm tra tính tuần tự (phải là số lớn nhất + 1)
            //        if (currentValue == maxValue + 1)
            //        {
            //            dgv_Setup.Rows[e.RowIndex].Cells["Select"].Value = true; // Giá trị hợp lệ
            //        }
            //        else
            //        {
            //            // Không tuần tự => Báo lỗi và reset giá trị
            //            //MessageBox.Show($"Số thứ tự phải là {maxValue + 1}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            dgv_Setup.Rows[e.RowIndex].Cells["Select"].Value = false;
            //            dgv_Setup.Rows[e.RowIndex].Cells["STT"].Value = "";
            //        }
            //    }
            //    else
            //    {
            //        // Giá trị không hợp lệ => Reset giá trị
            //       // MessageBox.Show("Vui lòng nhập số hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        dgv_Setup.Rows[e.RowIndex].Cells["Select"].Value = false;
            //        dgv_Setup.Rows[e.RowIndex].Cells["STT"].Value = "";
            //    }
            //}
            #endregion
        }

        private bool LoadAndSortData()
        {
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgv_Setup.Rows)
            {
                if (row.Cells["STT"].Value != null && !string.IsNullOrEmpty(row.Cells["STT"].Value.ToString()))
                {
                    rows.Add(row);
                }
            }
            var sortedRows = rows.OrderBy(row => Convert.ToInt32(row.Cells["STT"].Value)).ToList();

            int checkNum_STT = 0;
            int checkNum_Group = 0;
            foreach (var row in sortedRows)
            {
                int STT = Convert.ToInt32(row.Cells["STT"].Value);
                int Group = Convert.ToInt32(row.Cells["Group"].Value);
                int dif_STT = STT - checkNum_STT;
                int dif_Group = Group - checkNum_Group;
                if ( dif_STT == 1 && (dif_Group == 0 || dif_Group == 1))
                {
                    string appName = row.Cells["AppName"].Value.ToString(); // Ví dụ cột "Description" là tên ứng dụng
                    int groupValue = Convert.ToInt32(row.Cells["Group"].Value); // Giả sử cột "Group" là nhóm ứng dụng

                    GlobalVariables.Apps.Add(appName);
                    GlobalVariables.Groups.Add(groupValue);
                    checkNum_STT = STT;
                    checkNum_Group = Group;
                }
                else
                {
                    Control.ClearAllLists();
                    MessageBox.Show("Nhap sai STT hoac Group");
                    return false;
                }
                
            }

            if (GlobalVariables.Apps.Count > 1)
            {
                for (int i = 1; i < GlobalVariables.Apps.Count; i++)
                {
                    
                }
            }
            return true;
            
        }



        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (txt_TabletID.Text == "" || txt_UserID.Text == "")
            {
                MessageBox.Show("Vui lòng điền Mã tablet hoăc UserID");
                return;
            }
            Control.ClearAllLists();
            if (!LoadAndSortData())
            {
                return;
            }
            int check = 0;
            string query_checkTablet = $"select distinct tablet_name from F2Database.dbo.F2_ControlApp_Master where tablet_name = '{txt_TabletID.Text}'";
            string checkTablet = tf.sqlExecuteScalarString(query_checkTablet);
            if (checkTablet.Length > 0)
            {
                string query = $"Delete from F2Database.dbo.F2_ControlApp_Master where tablet_name = '{txt_TabletID.Text}'";
                tf.sqlExecuteNonQuery(query);
            }
            for (int i = 0; i < GlobalVariables.Apps.Count; i++)
            {
                string name = GlobalVariables.Apps[i];
                int group = GlobalVariables.Groups[i];
                string userid = txt_UserID.Text;
                string tabletID = txt_TabletID.Text;
                string query = $"INSERT INTO F2Database.dbo.F2_ControlApp_Master (stt, app_name,group_name, tablet_name, user_id,reg_date) VALUES ('{i+1}','{name}','{group}', '{tabletID}','{userid}',GETDATE())";
                int n = tf.sqlExecuteNonQuery(query);
                if (n > 0)
                {
                    check++;
                }
            }
            if (check > 0)
            {
                
                foreach (DataGridViewRow row in dgv_Setup.Rows)
                {
                    if (row.Cells["STT"].Value != null && !string.IsNullOrEmpty(row.Cells["STT"].Value.ToString()) || row.Cells["Group"].Value != null)
                    {
                        row.Cells["STT"].Value = "";
                        row.Cells["Group"].Value = "";
                    }
                }
                MessageBox.Show("Đã lưu vào Database");
            }

            

        }

    }
}
