using Center.APP;
using Center.Apps;
using Center.FORM;
using Microsoft.SqlServer.Server;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using static Center.Structs;
using static Center.WinAPI;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Center
{
    internal class Control
    {
        public static OperatorForm opF;
        public static void ClearList<T>(List<T> list)
        {
            if (list != null)
            {
                list.Clear();
            }
        }
        public static void ClearAllLists()
        {
            GlobalVariables.Apps.Clear();
            GlobalVariables.Groups.Clear();
            GlobalVariables.ListObj.Clear();
        }

        public static void DeleteCheckedNode(TreeNodeCollection nodes)
        {

            for (int i = nodes.Count - 1; i >= 0; i--) // Duyệt ngược từ cuối lên đầu
            {
                TreeNode node = nodes[i];

                if (node.Checked)
                {
                    nodes.Remove(node);
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        DeleteCheckedNode(node.Nodes); // Đệ quy cho các node con
                    }
                }
            }
        }

        public static TreeNode GetCheckedNodes(TreeNodeCollection nodes, TreeNode rs)// rs = null
        {

            foreach (TreeNode node in nodes)
            {
                if (rs != null)
                {
                    return rs;
                }
                else
                {
                    if (node.Checked)
                    {
                        rs = node;
                    }
                    else
                    {
                        if (node.Nodes.Count > 0)
                        {
                            rs = GetCheckedNodes(node.Nodes, rs);
                        }
                    }
                }
            }
            return rs;
        }

        public static void AddToTreeView(string value, System.Windows.Forms.TreeView treeView1)
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            TreeNode t = null;
            TreeNode n = GetCheckedNodes(nodes, t);

            // Thêm giá trị thực sự vào đúng cấp độ
            if (n != null)
            {
                n.Nodes.Add(new TreeNode(value));
            }
            else
            {
                Debug.Print("Null");
                nodes.Add(new TreeNode(value));
            }

            treeView1.ExpandAll(); // Tùy chọn: Tự động mở rộng TreeView
        }

        public static void UncheckOtherNodes(TreeNode currentNode, TreeNode checkedNode)
        {
            // Nếu node hiện tại không phải là node được check, bỏ check
            if (currentNode != checkedNode)
            {
                currentNode.Checked = false;
            }

            // Duyệt qua các node con (nếu có)
            foreach (TreeNode childNode in currentNode.Nodes)
            {
                UncheckOtherNodes(childNode, checkedNode);
            }
        }

        static List<int> listKcode = new List<int> { 38, 43, 44 };
        static List<Button> buttons = new List<Button>();
        static List<CheckBox> checkboxes = new List<CheckBox>();
        static TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        static TextBox txt_MaNV = new TextBox();
        static ComboBox cmb_Shift = new ComboBox();
        static TextBox txt_MachineID_Z200 = new TextBox();
        static TextBox txt_MachineID_Z300 = new TextBox();
        static TextBox txt_PO = new TextBox();
        static Label lbl_Status = new Label();
        public static void Init_List(string tablet)
        {
            SQL tf = new SQL();
            string query = $"select app_name,group_name FROM F2Database.dbo.F2_ControlApp_Master where tablet_name = '{tablet}'";
            DataTable dt = new DataTable();
            tf.sqlDataAdapterFillDatatable(query, ref dt);
            foreach (DataRow row in dt.Rows)
            {
                string appName = row["app_name"].ToString(); // Ví dụ cột "Description" là tên ứng dụng
                int groupValue = Convert.ToInt32(row["group_name"]); // Giả sử cột "Group" là nhóm ứng dụng
                GlobalVariables.Apps.Add(appName);
                GlobalVariables.Groups.Add(groupValue);
            }
        }

        // hàm SetUpApp
        /// <summary>
        /// Tạo các control trong form operator
        /// Đưa các object (App) vào ListObj để lúc sau RunApp thì lấy các object đó ra để chạy
        /// </summary>
        /// 
        public static void SaveMachineConfig(string tablet, string z200, string z300)
        {
            SQL tf = new SQL();
            string checkQuery = $"SELECT COUNT(*) FROM F2Database.dbo.F2_ControlApp_Machine WHERE tablet_name = '{tablet}'";
            int count = tf.sqlExecuteScalar(checkQuery);

            string query;
            if (count == 0)
            {
                query = $"INSERT INTO F2Database.dbo.F2_ControlApp_Machine (tablet_name, MC_Inspection, MC_Packing) " +
                        $"VALUES ('{tablet}', '{z200}', '{z300}')";
            }
            else
            {
                query = $"UPDATE F2Database.dbo.F2_ControlApp_Machine SET MC_Inspection = '{z200}', MC_Packing = '{z300}' " +
                        $"WHERE tablet_name = '{tablet}'";
            }

            tf.sqlExecuteNonQuery(query);
        }

        public static async void SetUpApp(OperatorForm opF)
        {
            int heightForm = 0;
            int AddHeight = 35;
            int r = 0;
            int c = 1;
            bool ctrl_MaNV = false;
            bool ctrl_PO = false;
            bool ctrl_Shift = false;

            //opF = new OperatorForm();
            //TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.RowCount = 2;  // Số dòng
            tableLayoutPanel.ColumnCount = 3; // Số cột
            tableLayoutPanel.Dock = DockStyle.Fill; // Để TableLayoutPanel chiếm toàn bộ form
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));

            Button btn_Setup = new Button();
            btn_Setup.Dock = DockStyle.Fill;
            btn_Setup.Text = "Setup Combo";
            btn_Setup.BackColor = Color.Orange;
            btn_Setup.Click += btn_Setup_Click;
            tableLayoutPanel.Controls.Add(btn_Setup, 2, r);
            tableLayoutPanel.SetColumnSpan(btn_Setup, 1);


            //----------------------------------------------
            Button btn_Refresh = new Button();
            btn_Refresh.Dock = DockStyle.Fill;
            btn_Refresh.Text = "Refresh";
            //btn_Refresh.BackColor = Color.Orange;
            //btn_Refresh.Click += btn_Setup_Click;
            btn_Refresh.Click += (s, e2) =>
            {
                opF.Controls.Clear(); // Xóa tất cả controls

                //opF.Load();
                opF.OperatorForm_Load(s, e2);
            };
            tableLayoutPanel.Controls.Add(btn_Refresh, 0, r);
            tableLayoutPanel.SetColumnSpan(btn_Refresh, 2);

            r++;

            Debug.Print("GlobalVariables.Apps.Count: "+ GlobalVariables.Apps.Count);

            // THÊM VÀO ĐỂ QUERY Z200, Z300 ĐỂ NÓ AUTO FILL 
            string z200 = "";
            string z300 = "";
            SQL tf = new SQL();
            string queryLoad = $"SELECT MC_Inspection, MC_Packing FROM [F2_ControlApp_Machine] WHERE tablet_name = '{Environment.MachineName}'";
            DataTable dt = new DataTable();
            tf.sqlDataAdapterFillDatatableMachineConfig(queryLoad, ref dt);
            if (dt.Rows.Count > 0)
            {
                z200 = dt.Rows[0]["MC_Inspection"].ToString();
                z300 = dt.Rows[0]["MC_Packing"].ToString();
            }

            for (int i = 0; i < GlobalVariables.Apps.Count; i++)
            {
  
                if (GlobalVariables.Apps[i] == "Supro-Z200")// Suppro Z-200---------------------------//
                {
                    Supro supro = new Supro();
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = supro;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(supro);
                    }
                    if (ctrl_Shift == false)
                    {
                        //Label supro Z-200
                        heightForm += AddHeight;
                        Label lbl_Shift = new Label();
                        lbl_Shift.Text = "Shift";
                        lbl_Shift.Font = new Font(lbl_Shift.Font.FontFamily, 12);
                        lbl_Shift.TextAlign = ContentAlignment.TopRight;
                        lbl_Shift.BorderStyle = BorderStyle.FixedSingle;
                        lbl_Shift.Dock = DockStyle.Fill;
                        tableLayoutPanel.Controls.Add(lbl_Shift, c - 1, r);
                        tableLayoutPanel.SetColumnSpan(lbl_Shift, 2);

                        //Combobox Shift
                        cmb_Shift = new ComboBox();
                        cmb_Shift.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmb_Shift.SelectedIndexChanged += FocusNextTextBox;
                        cmb_Shift.Dock = DockStyle.Top;
                        cmb_Shift.Items.Add("");
                        cmb_Shift.Items.Add("1");
                        cmb_Shift.Font = new Font(cmb_Shift.Font.FontFamily, 10);
                        cmb_Shift.Items.Add("1s");
                        cmb_Shift.Items.Add("2");
                        cmb_Shift.Items.Add("2s");
                        cmb_Shift.Items.Add("3");
                        cmb_Shift.Items.Add("ADM");
                        tableLayoutPanel.Controls.Add(cmb_Shift, c + 1, r);

                        ctrl_Shift = true;
                        r++;
                    }


                    //Label Machine
                    Label lbl_MachineID_Z200 = new Label();
                    lbl_MachineID_Z200.Text = "Inspection";
                    lbl_MachineID_Z200.Font = new Font(lbl_MachineID_Z200.Font.FontFamily, 11);
                    lbl_MachineID_Z200.BorderStyle = BorderStyle.FixedSingle;
                    lbl_MachineID_Z200.TextAlign = ContentAlignment.TopRight;
                    lbl_MachineID_Z200.Dock = DockStyle.Fill;
                    tableLayoutPanel.Controls.Add(lbl_MachineID_Z200, c - 1, r);
                    tableLayoutPanel.SetColumnSpan(lbl_MachineID_Z200, 2);
                    //Textbox Machine
                    //TextBox txt_MachineID_Z200 = new TextBox();
                    txt_MachineID_Z200 = new TextBox();
                    txt_MachineID_Z200.Font = new Font(txt_MachineID_Z200.Font.FontFamily, 12);
                    txt_MachineID_Z200.Dock = DockStyle.Fill;
                    //txt_MachineID_Z200.Text = "TRTB24009";
                    txt_MachineID_Z200.Text = z200;
                    tableLayoutPanel.Controls.Add(txt_MachineID_Z200, c + 1, r);

                    txt_MachineID_Z200.KeyDown += txt_KeyDown;
                    heightForm += AddHeight;
                    r++;


                }
                else if (GlobalVariables.Apps[i] == "Supro-Z300")// Suppro Z-200---------------------------
                {
                    Supro supro = new Supro();

                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = supro;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(supro);
                    }

                    //await supro.SetUp_SuproInControl();
                    //suproObj_Z300_Page = supro.page;
                    if (ctrl_Shift == false)
                    {
                        //Label
                        heightForm += AddHeight;
                        Label lbl_Shift = new Label();
                        lbl_Shift.Text = "Shift";
                        lbl_Shift.BorderStyle = BorderStyle.FixedSingle;
                        lbl_Shift.Font = new Font(lbl_Shift.Font.FontFamily, 11);
                        lbl_Shift.TextAlign = ContentAlignment.TopRight;
                        lbl_Shift.Dock = DockStyle.Fill;
                        tableLayoutPanel.Controls.Add(lbl_Shift, c - 1, r);
                        tableLayoutPanel.SetColumnSpan(lbl_Shift, 2);
                        //Combobox
                        cmb_Shift = new ComboBox();
                        cmb_Shift.Font = new Font(cmb_Shift.Font.FontFamily, 12);
                        cmb_Shift.Dock = DockStyle.Top;
                        cmb_Shift.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmb_Shift.SelectedIndexChanged += FocusNextTextBox;
                        cmb_Shift.Items.Add("");
                        cmb_Shift.Items.Add("1");
                        cmb_Shift.Items.Add("1s");
                        cmb_Shift.Items.Add("2");
                        cmb_Shift.Items.Add("2s");
                        cmb_Shift.Items.Add("3");
                        cmb_Shift.Items.Add("ADM");
                        tableLayoutPanel.Controls.Add(cmb_Shift, c + 1, r);

                        ctrl_Shift = true;
                        r++;
                    }

                    // Label Machine
                    Label lbl_MachineID_Z300 = new Label();
                    lbl_MachineID_Z300.Text = "Packing";
                    lbl_MachineID_Z300.Font = new Font(lbl_MachineID_Z300.Font.FontFamily, 10);
                    lbl_MachineID_Z300.BorderStyle = BorderStyle.FixedSingle;
                    lbl_MachineID_Z300.TextAlign = ContentAlignment.TopRight;
                    lbl_MachineID_Z300.Dock = DockStyle.Fill;
                    tableLayoutPanel.Controls.Add(lbl_MachineID_Z300, c - 1, r);
                    tableLayoutPanel.SetColumnSpan(lbl_MachineID_Z300, 2);
                    //TextBox Machine
                    //TextBox txt_MachineID_Z300 = new TextBox();
                    txt_MachineID_Z300 = new TextBox();
                    txt_MachineID_Z300.Dock = DockStyle.Fill;
                    txt_MachineID_Z300.Font = new Font(txt_MachineID_Z300.Font.FontFamily, 12);
                    //txt_MachineID_Z300.Text = "TRTB25151";
                    txt_MachineID_Z300.Text = z300;
                    tableLayoutPanel.Controls.Add(txt_MachineID_Z300, c + 1, r);
                    txt_MachineID_Z300.KeyDown += txt_KeyDown;
                    heightForm += AddHeight;
                    r++;
                }
                else if (GlobalVariables.Apps[i] == "Inspection_Standard")// Inspection_Standard---------------------------
                {
                    Debug.Print("Hello: Inspection_Standard");
                    KJapaneseG star = new KJapaneseG();
                    //star.Start_Star_Ex();
                    star.OpenStar();
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = star;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(star);
                    }
                }
                else if (GlobalVariables.Apps[i] == "Inspection_Special")// Inspection_Special---------------------------
                {
                    SpecialInspection spI = new SpecialInspection();
                    string appName = "InspectionSpecialMaterial_Manufa";
                    await spI.OpenApp(appName, "Inspection");
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = spI;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(spI);
                    }


                }
                else if (GlobalVariables.Apps[i] == "Print_Label")// Print Label---------------------------
                {
                    ToolPrintLabelInspection pr = new ToolPrintLabelInspection();
                    string appName = "ToolPrintLabelInspection";
                    string windowName = "Tool Print Qty Label Inspection ";
                    await pr.OpenApp(appName,windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = pr;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(pr);
                    }
                }
                else if (GlobalVariables.Apps[i] == "SprueBush_PCS_QRCode")
                {
                    SprueBush_PCS_QRCode sPQR = new SprueBush_PCS_QRCode();
                    string appName = "SprueBush_PCS_QRCode_MANUFA";
                    string windowName = "QRCode_SprueBush_PCS_MANUFA";
                    await sPQR.OpenApp(appName,windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = sPQR;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(sPQR);
                    }
                }
                else if (GlobalVariables.Apps[i] == "CheckTemRollerlock_Manufa")
                {

                    CheckTemRollerlock_Manufa checkRL = new CheckTemRollerlock_Manufa();
                    string appName = "CheckTemRollerlock_Manufa";
                    string windowName = "CheckTemRollerlock";
                    await checkRL.OpenApp(appName,windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = checkRL;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(checkRL);
                    }
                }
                else if (GlobalVariables.Apps[i] == "QRCodeRollerlock_Manufa")
                {
                    QRCodeRollerlock_Manufa QrRl = new QRCodeRollerlock_Manufa();
                    string appName = "QRCodeRollerlock_Manufa";
                    string windowName = "QRCode Rollerlock";
                    await QrRl.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = QrRl;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(QrRl);
                    }
                }
                else if (GlobalVariables.Apps[i] == "QRCode_Oilless_MANUFA")
                {
                    QRCode_Oilless_MANUFA QrOM = new QRCode_Oilless_MANUFA();
                    string appName = "QRCode_Oilless_Manufa";
                    string windowName = "QRCode_Oilless";
                    await QrOM.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = QrOM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(QrOM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "QRCode_SprueBush_Manufa")
                {
                    QRCode_SprueBush_Manufa SpM = new QRCode_SprueBush_Manufa();
                    string appName = "QRCode_SprueBush_Manufa";
                    string windowName = "QRCode_SprueBush - Manufa";
                    await SpM.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = SpM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(SpM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "QRCode_Mold_All_Manufa")
                {
                    QRCode_Mold_All_Manufa QrMAM = new QRCode_Mold_All_Manufa();
                    string appName = "QRCode_Mold_All_Manufa";
                    string windowName = "QRCode_TaperPin";
                    await QrMAM.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = QrMAM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(QrMAM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "MTS_Epin_QRCode_Mold_All_Manufa")
                {
                    MTS_Epin_QRCode_Mold_All_Manufa MTSEQrM = new MTS_Epin_QRCode_Mold_All_Manufa();
                    string appName = "QRCode_Mold_All_Manufa";
                    string windowName = "QRCode_EjectorPin";
                    await MTSEQrM.OpenApp(appName,windowName);    
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = MTSEQrM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(MTSEQrM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "QRCode_Support_Pillar")
                {
                    QRCode_Support_Pillar QrSP = new QRCode_Support_Pillar();
                    string appName = "QRCode_Oilless_MANUFA";
                    string windowName = "QRCode_SupportPillar";
                    await QrSP.OpenApp(appName,windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = QrSP;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(QrSP);
                    }
                }
                else if (GlobalVariables.Apps[i] == "Spclt2_Program_Warehouse")
                {
                    Spclt2_Program_Warehouse Spclt2 = new Spclt2_Program_Warehouse();
                    string appName = "Spclt2-Program-Warehouse_Manufa";
                    string windowName = "Spclt2_Program";
                    await Spclt2.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = Spclt2;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(Spclt2);
                    }
                }
                else if (GlobalVariables.Apps[i] == "Template_Shipping")
                {
                    Template_Shipping tlS = new Template_Shipping();
                    tlS.Open_Excel("Template_ShippingLabel_VBEP_2K");
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = tlS;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(tlS);
                    }
                }
                else if (GlobalVariables.Apps[i] == "CheckPart_Manufa")
                {
                    CheckPart_Manufa cpM = new CheckPart_Manufa();
                    string appName = "CheckPart_Manufa";
                    string windowName1 = "MANUFA - Check Part QA -";
                    string windowName2 = "MANUFA - Check Part QA (Inspection)";
                    await cpM.OpenApp(appName, windowName1, windowName2 );
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = cpM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(cpM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "InspectionKeshikomi_Manufa")
                {
                    InspectionKeshikomi_Manufa IkM = new InspectionKeshikomi_Manufa();
                    string appName = "InspectionKeshikomi_Manufa";
                    string windowName = "Tram Inspection";
                    await IkM.OpenApp(appName, windowName);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = IkM;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(IkM);
                    }
                }
                else if (GlobalVariables.Apps[i] == "CheckPart_Manufa_GetPart")
                {
                    CheckPart_Manufa_GetPart CpMG = new CheckPart_Manufa_GetPart();
                    string appName = "CheckPart_Manufa";
                    string windowName1 = "MANUFA - Check Part QA -";
                    string windowName2 = "MANUFA - Check Part QA (Get part)";
                    await CpMG.OpenApp(appName, windowName1, windowName2);
                    if (i < GlobalVariables.ListObj.Count())
                    {
                        GlobalVariables.ListObj[i] = CpMG;
                    }
                    else
                    {
                        GlobalVariables.ListObj.Add(CpMG);
                    }
                }
            }


            // Label MaNV
            Label lbl_MaNV = new Label();
            lbl_MaNV.Text = "MaNV";
            lbl_MaNV.Font = new Font(lbl_MaNV.Font.FontFamily, 12);
            lbl_MaNV.TextAlign = ContentAlignment.TopRight;
            lbl_MaNV.BorderStyle = BorderStyle.FixedSingle;
            lbl_MaNV.Dock = DockStyle.Fill; // Để Label chiếm toàn bộ ô trong TableLayoutPanel
            tableLayoutPanel.Controls.Add(lbl_MaNV, c - 1, r);
            tableLayoutPanel.SetColumnSpan(lbl_MaNV, 2);
            // TextBox MaNV
            //TextBox txt_MaNV = new TextBox();
            txt_MaNV = new TextBox();
            txt_MaNV.Font = new Font(txt_MaNV.Font.FontFamily, 12);
            txt_MaNV.Dock = DockStyle.Fill;
            //txt_MaNV.Text = "22473";
            txt_MaNV.KeyDown += txt_KeyDown;
            tableLayoutPanel.Controls.Add(txt_MaNV, c + 1, r);

            r++;


            // Label PO
            Label lbl_PO = new Label();
            lbl_PO.Text = "PO";
            lbl_PO.Font = new Font(lbl_PO.Font.FontFamily, 12);
            lbl_PO.BorderStyle = BorderStyle.FixedSingle;
            lbl_PO.TextAlign = ContentAlignment.TopRight;
            lbl_PO.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(lbl_PO, c - 1, r);
            tableLayoutPanel.SetColumnSpan(lbl_PO, 2);

            //TextBox PO
            txt_PO = new TextBox();
            txt_PO.Font = new Font(txt_PO.Font.FontFamily, 12);
            txt_PO.Dock = DockStyle.Fill;
            //txt_PO.Text = "520003978430";

            
            //txt_PO.KeyDown += (sender, e) =>
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        if (txt_MaNV.Text == "" || (ctrl_Shift == true && cmb_Shift.SelectedIndex == -1))
            //        {
            //            MessageBox.Show("Vui long nhap day du thong tin");
            //            return;
            //        }
            //        Run_Flow(opF);// Hàm chạy chính (Chạy 1 combo)
            //    }
            //};

            //txt_PO.TextChanged += (sender, e) =>
            //{
            //    // Find KJapaneseG object in GlobalVariables.ListObj
            //    var kJapaneseObj = GlobalVariables.ListObj.FirstOrDefault(obj => obj is KJapaneseG) as KJapaneseG;
            //    if (kJapaneseObj != null)
            //    {
            //        int time_find = 0;
            //        IntPtr h = kJapaneseObj.ShowStar();
            //        // Update KJapaneseG with current MaNV and PO
            //        kJapaneseObj.Fill_Star(txt_MaNV.Text, txt_PO.Text, h);
            //        Debug.Print($"KJapaneseG updated with MaNV: {txt_MaNV.Text}, PO: {txt_PO.Text}");
            //    }
            //};

            txt_PO.KeyDown += async (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txt_MaNV.Text == "" || (ctrl_Shift == true && cmb_Shift.SelectedIndex == -1))
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                        return;
                    }

                    //tien hanh dua vao db 
                    string deviceName = Environment.MachineName;
                    bool checkIN = await tf.InsertUser(new Model.Users { ins = txt_MachineID_Z200.Text,msnv = txt_MaNV.Text,po = txt_PO.Text,machine = deviceName });
                    if (!checkIN)
                    {
                        MessageBox.Show("Loi khi them vao database user ");
                    }

                    var kJapaneseObj = GlobalVariables.ListObj.FirstOrDefault(obj => obj is KJapaneseG) as KJapaneseG;
                    if (kJapaneseObj != null)
                    {
                        IntPtr h = kJapaneseObj.ShowStar();
                        if (h == IntPtr.Zero)
                        {
                            MessageBox.Show("Không tìm thấy cửa sổ Supro");
                            return;
                        }

                        await Task.Delay(500); // đợi Supro ổn định trước khi fill
                        kJapaneseObj.Fill_Star(txt_MaNV.Text, txt_PO.Text, h);
                    }
                    var toolPrintLabelInspection = GlobalVariables.ListObj.FirstOrDefault(obj => obj is ToolPrintLabelInspection) as ToolPrintLabelInspection;
                    if (toolPrintLabelInspection != null)
                    {
                        toolPrintLabelInspection.SetUp_ToolPrintLabelInspection(txt_MaNV.Text, txt_PO.Text);
                        await toolPrintLabelInspection.FillControls();
                    }
                    //ReorderAppsForSpecialGroup();
                    Run_Flow(opF); // chạy sau khi fill PO xong
                    
                }
            };

            tableLayoutPanel.Controls.Add(txt_PO, c + 1, r);
            r++;


            //Label status
            lbl_Status.Text = "Stopping";
            lbl_Status.BorderStyle = BorderStyle.FixedSingle;
            lbl_Status.Dock = DockStyle.Fill;
            lbl_Status.Font = new Font(lbl_Status.Font.FontFamily, 15);
            lbl_Status.AutoSize = true;
            lbl_Status.TextAlign = ContentAlignment.MiddleCenter;
            tableLayoutPanel.Controls.Add(lbl_Status, c, r);
            tableLayoutPanel.SetColumnSpan(lbl_Status, 3);
            r++;

            //buttons và checkboxes : danh sách các button và check box của mỗi app
            buttons.Clear();
            checkboxes.Clear();

            for (int i = 0; i < GlobalVariables.Apps.Count; i++)
            {
                heightForm += AddHeight;

                // button mỗi app (Hiện chưa có sự kiện gì cho button
                Button btn = new Button();
                btn.Dock = DockStyle.Fill;
                btn.Text = GlobalVariables.Apps[i];
                tableLayoutPanel.Controls.Add(btn, 1, r);
                tableLayoutPanel.SetColumnSpan(btn, 2);
                buttons.Add(btn);

                // checkbox mỗi app
                CheckBox checkBox = new CheckBox();
                checkBox.Dock = DockStyle.Fill;
                checkBox.CheckState = CheckState.Checked;
                tableLayoutPanel.Controls.Add(checkBox, 0, r);
                checkboxes.Add(checkBox);
                checkBox.CheckedChanged += (sender, args) =>
                {
                    if (checkBox.Checked)// Nếu checkbox được check, enable button, ngược lại disable
                    {
                        btn.Enabled = true;
                        btn.BackColor = Color.White;
                    }
                    else
                    {
                        btn.Enabled = false;
                        btn.BackColor = Color.Gray;
                    }
                };
                r++;
            }

            if (ctrl_Shift == true)// nếu có Supro trong combo thêm phần Kcode và Cancel, Pause Button
            {
                heightForm += 70;
                int heigh = 10;
                Panel panel = new Panel();
                panel.BackColor = Color.LightYellow;
                panel.Dock = DockStyle.Fill;

                // Thêm Panel vào TableLayoutPanel tại cột 0, hàng r
                tableLayoutPanel.Controls.Add(panel, 0, r);
                tableLayoutPanel.SetColumnSpan(panel, 3);
                tableLayoutPanel.SetRowSpan(panel, heigh);

                //btn_Cancel PO---------------------------------------//
                Button btn_Cancel = new Button();
                btn_Cancel.Text = "Cancel PO";
                btn_Cancel.BackColor = Color.White;
                btn_Cancel.Click += async (s, e) =>
                {
                    if (suproObj_Z200_Page != null && suproObj_Z200_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                    {
                        await CancelClick(suproObj_Z200_Page);

                    }
                    else if (suproObj_Z300_Page != null && suproObj_Z300_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                    {
                        await CancelClick(suproObj_Z300_Page);
                    }
                    else
                    {
                        Debug.Print("Không có");
                    }
                };
                btn_Cancel.Location = new System.Drawing.Point(3, 0);
                btn_Cancel.Size = new Size(100, 25);
                panel.Controls.Add(btn_Cancel);

                //btn_Pause PO ---------------------------------------//
                Button btn_Pause = new Button();
                btn_Pause.Text = "Pause PO";
                btn_Pause.BackColor = Color.White;
                btn_Pause.Location = new System.Drawing.Point(150, 0);
                btn_Pause.Click += async (s, e) =>
                {
                    if (suproObj_Z200_Page != null && suproObj_Z200_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                    {
                        await PauseClick(suproObj_Z200_Page);


                    }
                    else if (suproObj_Z300_Page != null && suproObj_Z300_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                    {
                        await PauseClick(suproObj_Z300_Page);
                    }
                    else
                    {
                        Debug.Print("Không có");
                    }
                };
                btn_Pause.Size = new Size(100, 25);
                panel.Controls.Add(btn_Pause);

                //lbl_KCode
                Label lbl_Kcode = new Label();
                lbl_Kcode.Text = "KCode:";
                lbl_Kcode.Size = new Size(50, 15);
                panel.Controls.Add(lbl_Kcode);
                lbl_Kcode.Location = new System.Drawing.Point(0, 35);

                //lbl_Other
                Label lbl_Other = new Label();
                lbl_Other.Text = "Khác:";
                lbl_Other.Size = new Size(40, 15);
                panel.Controls.Add(lbl_Other);
                lbl_Other.Location = new System.Drawing.Point(50, 95);

                // Tạo một CheckedListBox 3 KCode có thể phổ biến nhất
                CheckedListBox checkedListBox = new CheckedListBox();
                checkedListBox.Items.Add("Di Ve Sinh");
                checkedListBox.Items.Add("Gio Nghi An Com");
                checkedListBox.Items.Add("Gio Nghi Giai Lao");
                checkedListBox.Size = new Size(150, 50);
                checkedListBox.Location = new System.Drawing.Point(50, 35);
                panel.Controls.Add(checkedListBox);



                // combobox Kcode
                ComboBox comboBox_Kcode = new ComboBox();
                string[] items = new string[]
                {
                    "0. Drilling hole Flange",
                    "1. Cutting rail",
                    "2. To roll rubber",
                    "3. Polishing",
                    "4. The first work-piece",
                    "5. Giao hang",
                    "6. Drill",
                    "7. Cimen",
                    "8. Kiem tra",
                    "9. Viec khac co su dung may",
                    "10. Chen bi cho truc Jig",
                    "11. Sua chua dao cat",
                    "12. Broaching cutter G",
                    "13. Sua lo Zaguri",
                    "14. Sua hang",
                    "15. Tap",
                    "16. Thay da mai",
                    "17. Thay nuoc,cham nuoc",
                    "18. To maintain mold",
                    "19. Don dep san nha xuong",
                    "20. Ve sinh san pham",
                    "21. Sua may",
                    "22. Thay doi truc",
                    "23. Xuat kho",
                    "24. Hoc may",
                    "25. Di hop (khong phai hop doi ca)",
                    "26. Ra da dan",
                    "27. Thay mui ra",
                    "28. Nhap du lieu",
                    "29. Viec khac khong su dung may",
                    "30. Viet Daily Report",
                    "31. Thay Jig",
                    "32. Thay san pham",
                    "33. Thay tool",
                    "34. Cai dat",
                    "35. Da tien",
                    "36. Giao ca",
                    "37. Sua doi du lieu",
                    "38. Di ve sinh",
                    "39. Cup dien",
                    "40. Het hang",
                    "41. Hoat dong kaizen",
                    "42. Lap rap san pham trong khi cho doi",
                    "43. Gio nghi an com",
                    "44. Gio nghi giai lao",
                    "45. Gia cong 1PC Flow",
                    "46. Gia cong nhieu may"
                };
                comboBox_Kcode.Items.AddRange(items);
                comboBox_Kcode.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox_Kcode.DropDownWidth = 400;
                comboBox_Kcode.Size = new Size(100, 30);
                comboBox_Kcode.Location = new System.Drawing.Point(90, 95);
                panel.Controls.Add(comboBox_Kcode);

                //btn_Start_KCode
                Button btn_startKcode = new Button();
                btn_startKcode.Text = "Start Kcode";
                btn_startKcode.Click += async (s, e) =>
                {
                    int Kcode_Index = -1;

                    // Ưu tiên checkedListBox
                    if (checkedListBox.CheckedIndices.Count == 1)
                    {
                        int selected = checkedListBox.CheckedIndices[0];
                        Kcode_Index = listKcode[selected];
                    }
                    else if (comboBox_Kcode.SelectedIndex != -1)
                    {
                        Kcode_Index = comboBox_Kcode.SelectedIndex;
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn một lý do KCode!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string name = "";

                    IPage page = null;
                    if (suproObj_Z200_Page != null && suproObj_Z200_Page.Url.StartsWith("http://10.4.24.117:8441"))
                        page = suproObj_Z200_Page;
                    else if (suproObj_Z300_Page != null && suproObj_Z300_Page.Url.StartsWith("http://10.4.24.117:8441"))
                        page = suproObj_Z300_Page;

                    if (page == null)
                    {
                        MessageBox.Show("Không tìm thấy trang để thao tác KCode", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Debug.Print($"[StartKcode_Click] MANV={txt_MaNV.Text}, Kcode_Index={Kcode_Index}");
                    bool result = await KCodeClick(page, 1, Kcode_Index, name, txt_MaNV.Text);

                    if (!result)
                    {
                        MessageBox.Show("Không thể thao tác KCode. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };


                btn_startKcode.BackColor = Color.White;
                btn_startKcode.Size = new Size(100, 25);
                btn_startKcode.Location = new System.Drawing.Point(3, 125);
                panel.Controls.Add(btn_startKcode);

                //btn_Finish_KCode
                Button btn_FinishKcode = new Button();
                btn_FinishKcode.Text = "Finish Kcode";
                btn_FinishKcode.BackColor = Color.White;
                btn_FinishKcode.Click += async (s, e) =>
                {
                    bool a = true;
                    try
                    {
                        if (suproObj_Z200_Page != null && !suproObj_Z200_Page.IsClosed && suproObj_Z200_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Kcode.aspx"))
                        {
                            a = await KCode_Finish_Click(suproObj_Z200_Page);
                        }
                        else if (suproObj_Z300_Page != null && !suproObj_Z300_Page.IsClosed && suproObj_Z300_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Kcode.aspx"))
                        {
                            a = await KCode_Finish_Click(suproObj_Z300_Page);
                        }

                        if (a == true)
                        {
                            if (suproObj_Z200_Page != null && !suproObj_Z200_Page.IsClosed && suproObj_Z200_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                            {
                                await suproObj_Z200_Page.BringToFrontAsync();
                            }
                            else if (suproObj_Z300_Page != null && !suproObj_Z300_Page.IsClosed && suproObj_Z300_Page.Url.StartsWith("http://10.4.24.117:8441/sp/Processing"))
                            {
                                await suproObj_Z300_Page.BringToFrontAsync();
                            }
                            for (int i = 0; i < checkedListBox.Items.Count; i++)
                            {
                                checkedListBox.SetItemChecked(i, false); 
                            }
                            comboBox_Kcode.SelectedIndex = -1;
                        }
                    }
                    catch { }

                };
                btn_FinishKcode.Size = new Size(100, 25);
                btn_FinishKcode.Location = new System.Drawing.Point(150, 125);
                panel.Controls.Add(btn_FinishKcode);

                r = r + heigh;

            }
            else
            {
                Label lbl_Space = new Label();
                lbl_Space.Dock = DockStyle.Fill;
                tableLayoutPanel.Controls.Add(lbl_Space, 0, r);
            }


            // Đưa tableLayoutPanel vào form opF
            opF.Controls.Add(tableLayoutPanel);
            opF.AutoSize = false;
            opF.Size = new Size(opF.Width, 220 + heightForm);
            opF.TopMost = true;
        }

        // Set Up combo




        private static void btn_Setup_Click(object sender, EventArgs e)
        {
            SetUp st = new SetUp();
            st.TopMost = true;
            st.Show();
            InitComboSetup(st.dgv_Setup);
        }


        // Focus next Empty TextBox
        static void FocusNextTextBox(object sender, EventArgs e)
        {
            // Lấy tất cả các TextBox trong TableLayoutPanel
            var textBoxes = tableLayoutPanel.Controls.OfType<System.Windows.Forms.TextBox>();

            // In ra số lượng TextBox tìm thấy
            //Debug.Print("kokkokokoko: " + textBoxes.Count().ToString());

            // Đặt focus vào TextBox đầu tiên (nếu tồn tại)
            foreach (var textBox in textBoxes)
            {
                if (textBox.Text.Length == 0)
                {
                    textBox.Focus();
                    return; // Focus vào TextBox đầu tiên và thoát
                }

            }
        }

        // Sự kiện TextBox Keydown
        static void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveMachineConfig(Environment.MachineName, txt_MachineID_Z200?.Text ?? "", txt_MachineID_Z300.Text);
                FocusNextTextBox(sender, e);
            }
        }

        static IPage suproObj_Z200_Page;
        static IPage suproObj_Z300_Page;
        //static KJapaneseG kJapaneseObj;
        //static ToolPrintLabelInspection toolPrintLabelObj;


        // Hàm chạy 1 App ---------------------------------------//
        public async static Task<bool> RunApp(int i, OperatorForm opF)
        {
            Action<App, TaskCompletionSource<bool>> finishHandler = null;
            finishHandler = (Obj, tcs) =>// Sự kiện Finish của App
            {
                if (!tcs.Task.IsCompleted)
                {
                    opF.Invoke(new Action(() =>
                    {
                        buttons[i].BackColor = Color.White;
                        buttons[i].Text = buttons[i].Text + ": Finished";
                       
                        Debug.Print("label Đã Finish");
                    }));
                    tcs.SetResult(true); // Set Finish Task của App
                    Obj.Finished -= finishHandler;
                }
            };

            Action<App, TaskCompletionSource<bool>> cancelHandler = null;
            cancelHandler = (Obj, tcs) => // Sự kiện Cancel của App
            {
                if (!tcs.Task.IsCompleted)
                {
                    opF.Invoke(new Action(() =>
                    {
                        buttons[i].Text = buttons[i].Text + ": Cancel";
                        SetTaskCancel();
                        for (int j = 0; j < buttons.Count; j++)
                        {
                            buttons[j].BackColor = Color.White;
                        }
                        lbl_Status.Text = "Stopping";
                        lbl_Status.BackColor = Color.White;
                    }));
                    SetTaskCancel(); // Set tất cả các Task của app này và các app khác cancel
                    Obj.Canceled -= cancelHandler;
                }

            };

            bool result = false;// biến kết quả khi chạy 1 app
                                // Kiểm tra index hợp lệ

            // MỚI THÊM VÀO 17/6/2025 9H54
            if (i >= GlobalVariables.ListObj.Count || GlobalVariables.ListObj[i] == null)
            {
                Debug.Print($"Invalid index or null object at index {i}");
                return false;
            }

            var obj = GlobalVariables.ListObj[i]; // object của App lấy từ list Obj

            if (obj is Supro suproObj)// Nếu Obj là Supro
            {
                Debug.Print("Hello Supro");

                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();//Khởi tạo 1 Task chờ
                if (GlobalVariables.Apps[i] == "Supro-Z200")
                {
                    GlobalVariables.tcs_Supro_Z200 = new TaskCompletionSource<bool>();
                    tcs = GlobalVariables.tcs_Supro_Z200;
                    suproObj.SetInputSupro(txt_MachineID_Z200.Text, cmb_Shift.SelectedIndex, txt_MaNV.Text, txt_PO.Text);// thiết lập các tham số cần thiết cho Supro
                }
                else if (GlobalVariables.Apps[i] == "Supro-Z300")
                {
                    GlobalVariables.tcs_Supro_Z300 = new TaskCompletionSource<bool>();
                    tcs = GlobalVariables.tcs_Supro_Z300;
                    suproObj.SetInputSupro(txt_MachineID_Z300.Text, cmb_Shift.SelectedIndex, txt_MaNV.Text, txt_PO.Text);
                }

                suproObj.tcs = tcs;

                // Xóa hết các sự kiện đang được gắn vào obj cho an tâm
                suproObj.Canceled -= cancelHandler;
                suproObj.Finished -= finishHandler;
                suproObj.Canceled += cancelHandler;
                suproObj.Finished += finishHandler;


                GlobalVariables.List_tasks.Add(tcs.Task);// Đưa Task của Supro vào List Task

                result = await suproObj.StartSupro(opF);// Start 1 App, mỗi App đều có 1 hàm start

                if (GlobalVariables.Apps[i] == "Supro-Z200")
                {
                    suproObj_Z200_Page = suproObj.page;// lấy page của obj Supro để tiện cho làm Kcode và Cancel or Pause 
                }
                else if (GlobalVariables.Apps[i] == "Supro-Z300")
                {
                    suproObj_Z300_Page = suproObj.page;// lấy page của obj Supro để tiện cho làm Kcode và Cancel or Pause 
                }

            }
            else if (obj is KJapaneseG kJapaneseObj)
            {
               
                GlobalVariables.tcs_KJapaneseG = new TaskCompletionSource<bool>();//khởi tạo Task của App]
                Debug.Print("txt_MaNV.Text: " + txt_MaNV.Text);
                Debug.Print("txt_PO.Text: " + txt_PO.Text);

                kJapaneseObj.SetUp_KJapaneseG(txt_MaNV.Text, txt_PO.Text); // SetUp App
                kJapaneseObj.Canceled -= cancelHandler;
                kJapaneseObj.Finished -= finishHandler;
                kJapaneseObj.Canceled += cancelHandler;
                kJapaneseObj.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_KJapaneseG.Task);// đưa task vào List_task
                result = kJapaneseObj.Start_Star(); // start_app
                Debug.Print($"HELLO  called at {DateTime.Now}");

            }
            else if (obj is ToolPrintLabelInspection toolPrintLabelObj)
            {
                toolPrintLabelObj = (ToolPrintLabelInspection)obj;  
                GlobalVariables.tcs_ToolPrintLabelInspection = new TaskCompletionSource<bool>();
                toolPrintLabelObj.SetUp_ToolPrintLabelInspection(txt_MaNV.Text, txt_PO.Text);
                toolPrintLabelObj.Canceled -= cancelHandler;
                toolPrintLabelObj.Finished -= finishHandler;
                toolPrintLabelObj.Canceled += cancelHandler;
                toolPrintLabelObj.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_ToolPrintLabelInspection.Task);
            
                result = await toolPrintLabelObj.Start_ToolPrintLabelInspection();
                //txt_PO.Text = "";
                //txt_PO.Focus();
                
            }
            else if (obj is SpecialInspection spI)
            {
                GlobalVariables.tcs_SpecialInspection = new TaskCompletionSource<bool>();
                spI.SetUp_SpecialInspection(txt_MaNV.Text, txt_PO.Text);
                spI.Canceled -= cancelHandler;
                spI.Finished -= finishHandler;
                spI.Canceled += cancelHandler;
                spI.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_SpecialInspection.Task);
                result = await spI.Start_SpecialInspection();
            }
            else if (obj is SprueBush_PCS_QRCode sPQR)
            {
                GlobalVariables.tcs_SprueBush_PCS_QRCode = new TaskCompletionSource<bool>();
                sPQR.SetUp_SprueBush_PCS_QRCode(txt_PO.Text);
                sPQR.Canceled -= cancelHandler;
                sPQR.Finished -= finishHandler;
                sPQR.Canceled += cancelHandler;
                sPQR.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_SprueBush_PCS_QRCode.Task);
                result = await sPQR.Start_SprueBush_PCS_QRCode();
            }
            else if (obj is CheckTemRollerlock_Manufa checkRL)
            {
                GlobalVariables.tcs_CheckTemRollerlock_Manufa = new TaskCompletionSource<bool>();
                checkRL.SetUp_CheckTemRollerlock_Manufa(txt_MaNV.Text, txt_PO.Text);

                checkRL.Canceled -= cancelHandler;
                checkRL.Finished -= finishHandler;
                checkRL.Canceled += cancelHandler;
                checkRL.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_CheckTemRollerlock_Manufa.Task);
                result = await checkRL.Start_CheckTemRollerlock_Manufa();
            }
            else if (obj is QRCodeRollerlock_Manufa QrRl)
            {
                GlobalVariables.tcs_QRCodeRollerlock_Manufa = new TaskCompletionSource<bool>();
                QrRl.SetUp_QRCodeRollerlock_Manufa(txt_PO.Text);

                QrRl.Canceled -= cancelHandler;
                QrRl.Finished -= finishHandler;
                QrRl.Canceled += cancelHandler;
                QrRl.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_QRCodeRollerlock_Manufa.Task);
                result = await QrRl.Start_QRCodeRollerlock_Manufa();
            }
            else if (obj is QRCode_Oilless_MANUFA QrOM)
            {
                GlobalVariables.tcs_QRCode_Oilless_MANUFA = new TaskCompletionSource<bool>();
                QrOM.SetUp_QRCode_Oilless_MANUFA(txt_PO.Text);

                QrOM.Canceled -= cancelHandler;
                QrOM.Finished -= finishHandler;
                QrOM.Canceled += cancelHandler;
                QrOM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_QRCode_Oilless_MANUFA.Task);
                result = await QrOM.Start_QRCode_Oilless_MANUFA();
            }
            else if (obj is QRCode_SprueBush_Manufa SpM)
            {
                GlobalVariables.tcs_QRCode_SprueBush_Manufa = new TaskCompletionSource<bool>();
                SpM.SetUp_QRCode_SprueBush_Manufa(txt_PO.Text);

                SpM.Canceled -= cancelHandler;
                SpM.Finished -= finishHandler;
                SpM.Canceled += cancelHandler;
                SpM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_QRCode_SprueBush_Manufa.Task);
                result = await SpM.Start_QRCode_SprueBush_Manufa();
            }
            else if (obj is QRCode_Mold_All_Manufa QrMAM)
            {
                GlobalVariables.tcs_QRCode_Mold_All_Manufa = new TaskCompletionSource<bool>();
                QrMAM.SetUp_QRCode_Mold_All_Manufa(txt_PO.Text);

                QrMAM.Canceled -= cancelHandler;
                QrMAM.Finished -= finishHandler;
                QrMAM.Canceled += cancelHandler;
                QrMAM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_QRCode_Mold_All_Manufa.Task);
                result = await QrMAM.Start_QRCode_Mold_All_Manufa();
            }
            else if (obj is MTS_Epin_QRCode_Mold_All_Manufa MTSEQrM)
            {
                GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa = new TaskCompletionSource<bool>();
                MTSEQrM.SetUp_MTS_Epin_QRCode_Mold_All_Manufa(txt_PO.Text);
                MTSEQrM.Canceled -= cancelHandler;
                MTSEQrM.Finished -= finishHandler;
                MTSEQrM.Canceled += cancelHandler;
                MTSEQrM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa.Task);
                result = await MTSEQrM.Start_MTS_Epin_QRCode_Mold_All_Manufa();
            }
            else if (obj is QRCode_Support_Pillar QrSP)
            {
                GlobalVariables.tcs_QRCode_Support_Pillar = new TaskCompletionSource<bool>();
                QrSP.SetUp_QRCode_Support_Pillar(txt_PO.Text);
                QrSP.Canceled -= cancelHandler;
                QrSP.Finished -= finishHandler;
                QrSP.Canceled += cancelHandler;
                QrSP.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_QRCode_Support_Pillar.Task);
                result = await QrSP.Start_QRCode_Support_Pillar();
            }
            else if (obj is Spclt2_Program_Warehouse Spclt2)
            {
                GlobalVariables.tcs_Spclt2_Program_Warehouse = new TaskCompletionSource<bool>();
                Spclt2.SetUp_Spclt2_Program_Warehouse(txt_MaNV.Text, txt_PO.Text);
                Spclt2.Canceled -= cancelHandler;
                Spclt2.Finished -= finishHandler;
                Spclt2.Canceled += cancelHandler;
                Spclt2.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_Spclt2_Program_Warehouse.Task);
                result = await Spclt2.Start_Spclt2_Program_Warehouse();
            }
            else if (obj is CheckPart_Manufa cpM)
            {
                GlobalVariables.tcs_CheckPart_Manufa = new TaskCompletionSource<bool>();
                cpM.SetUp_CheckPart_Manufa(txt_MaNV.Text, txt_PO.Text);
                cpM.Canceled -= cancelHandler;
                cpM.Finished -= finishHandler;
                cpM.Canceled += cancelHandler;
                cpM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_CheckPart_Manufa.Task);
                result = await cpM.Start_CheckPart_Manufa();
            }
            else if (obj is Template_Shipping tlS)
            {
                GlobalVariables.tcs_Template_Shipping = new TaskCompletionSource<bool>();
                tlS.SetUp_Template_Shipping(txt_PO.Text);
                tlS.Canceled -= cancelHandler;
                tlS.Finished -= finishHandler;
                tlS.Canceled += cancelHandler;
                tlS.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_Template_Shipping.Task);

                result = await tlS.Start_Template_Shipping();
            }
            else if (obj is InspectionKeshikomi_Manufa IkM)
            {
                GlobalVariables.tcs_InspectionKeshikomi_Manufa = new TaskCompletionSource<bool>();
                IkM.SetUp_InspectionKenshikomi_Manufa(txt_PO.Text);
                IkM.Canceled -= cancelHandler;
                IkM.Finished -= finishHandler;
                IkM.Canceled += cancelHandler;
                IkM.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_InspectionKeshikomi_Manufa.Task);
                result = await IkM.Start_InspectionKenshikomi_Manufa();
            }
            else if (obj is CheckPart_Manufa_GetPart CpMG)
            {
                GlobalVariables.tcs_CheckPart_Manufa_GetPart = new TaskCompletionSource<bool>();
                CpMG.SetUp_CheckPart_Manufa_GetPart(txt_MaNV.Text, txt_PO.Text);
                CpMG.Canceled -= cancelHandler;
                CpMG.Finished -= finishHandler;
                CpMG.Canceled += cancelHandler;
                CpMG.Finished += finishHandler;
                GlobalVariables.List_tasks.Add(GlobalVariables.tcs_CheckPart_Manufa_GetPart.Task);
                result = await CpMG.Start_CheckPart_Manufa_GetPart();
            }
            return result;
        }

        // Kiểm tra các Task trong GlobalVariables.List_tasks đã finish hêt chưa. True: Finished, False: Cancel
        public static bool CheckTask()
        {
            bool result = true;
            int CountFalse = 0;
            foreach (var task in GlobalVariables.List_tasks.ToList())
            {
                if (task.IsCompleted)
                {
                    // Tìm task tương ứng với TaskCompletionSource và gọi SetResult(false)
                    if (task == GlobalVariables.tcs_KJapaneseG.Task)
                    {
                        result = GlobalVariables.tcs_KJapaneseG.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_ToolPrintLabelInspection.Task)
                    {
                        result = GlobalVariables.tcs_ToolPrintLabelInspection.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_Supro_Z200.Task)
                    {
                        result = GlobalVariables.tcs_Supro_Z200.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_Supro_Z300.Task)
                    {
                        result = GlobalVariables.tcs_Supro_Z300.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_SpecialInspection.Task)
                    {
                        result = GlobalVariables.tcs_SpecialInspection.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_SprueBush_PCS_QRCode.Task)
                    {
                        result = GlobalVariables.tcs_SprueBush_PCS_QRCode.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_CheckTemRollerlock_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_CheckTemRollerlock_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_QRCodeRollerlock_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_QRCodeRollerlock_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Oilless_MANUFA.Task)
                    {
                        result = GlobalVariables.tcs_QRCode_Oilless_MANUFA.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_QRCode_SprueBush_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_QRCode_SprueBush_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Mold_All_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_QRCode_Mold_All_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Support_Pillar.Task)
                    {
                        result = GlobalVariables.tcs_QRCode_Support_Pillar.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_Spclt2_Program_Warehouse.Task)
                    {
                        result = GlobalVariables.tcs_Spclt2_Program_Warehouse.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_CheckPart_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_CheckPart_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }
                    }
                    else if (task == GlobalVariables.tcs_Template_Shipping.Task)
                    {
                        result = GlobalVariables.tcs_Template_Shipping.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;
                        }    
                    }
                    else if (task == GlobalVariables.tcs_InspectionKeshikomi_Manufa.Task)
                    {
                        result = GlobalVariables.tcs_InspectionKeshikomi_Manufa.Task.Result;
                        if (result == false)
                        {
                            CountFalse++;   
                        }
                    }
                    else if (task == GlobalVariables.tcs_CheckPart_Manufa_GetPart.Task)
                    {
                        result = GlobalVariables.tcs_CheckPart_Manufa_GetPart.Task.Result;
                        if(result == false)
                        {
                            CountFalse++;
                        }
                    }
                }
            }
            if (CountFalse > 0)
            {
                result = false;
            }
            return result;
        }

        //Cancel tất cả các Task trong GlobalVariables.List_tasks
        public static void SetTaskCancel()
        {
            // Duyệt qua từng task trong GlobalVariables.List_tasks
            foreach (var task in GlobalVariables.List_tasks.ToList())
            {
                // Kiểm tra nếu task chưa hoàn thành
                if (!task.IsCompleted)
                {
                    // Tìm task tương ứng với TaskCompletionSource và gọi SetResult(false)
                    if (task == GlobalVariables.tcs_KJapaneseG.Task)
                    {
                        GlobalVariables.tcs_KJapaneseG.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_ToolPrintLabelInspection.Task)
                    {
                        GlobalVariables.tcs_ToolPrintLabelInspection.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_Supro_Z200.Task)
                    {
                        GlobalVariables.tcs_Supro_Z200.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_Supro_Z300.Task)
                    {
                        GlobalVariables.tcs_Supro_Z300.SetResult(false);

                    }
                    else if (task == GlobalVariables.tcs_SpecialInspection.Task)
                    {
                        GlobalVariables.tcs_SpecialInspection.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_SprueBush_PCS_QRCode.Task)
                    {
                        GlobalVariables.tcs_SprueBush_PCS_QRCode.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_CheckTemRollerlock_Manufa.Task)
                    {
                        GlobalVariables.tcs_CheckTemRollerlock_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_QRCodeRollerlock_Manufa.Task)
                    {
                        GlobalVariables.tcs_QRCodeRollerlock_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Oilless_MANUFA.Task)
                    {
                        GlobalVariables.tcs_QRCode_Oilless_MANUFA.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_QRCode_SprueBush_Manufa.Task)
                    {
                        GlobalVariables.tcs_QRCode_SprueBush_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Mold_All_Manufa.Task)
                    {
                        GlobalVariables.tcs_QRCode_Mold_All_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa.Task)
                    {
                        GlobalVariables.tcs_MTS_Epin_QRCode_Mold_All_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_QRCode_Support_Pillar.Task)
                    {
                        GlobalVariables.tcs_QRCode_Support_Pillar.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_Spclt2_Program_Warehouse.Task)
                    {
                        GlobalVariables.tcs_Spclt2_Program_Warehouse.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_CheckPart_Manufa.Task)
                    {
                        GlobalVariables.tcs_CheckPart_Manufa.SetResult (false);
                    }
                    else if (task == GlobalVariables.tcs_Template_Shipping.Task)
                    {
                        GlobalVariables.tcs_Template_Shipping.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_InspectionKeshikomi_Manufa.Task)
                    {
                        GlobalVariables.tcs_InspectionKeshikomi_Manufa.SetResult(false);
                    }
                    else if (task == GlobalVariables.tcs_CheckPart_Manufa_GetPart.Task)
                    {
                        GlobalVariables.tcs_CheckPart_Manufa_GetPart.SetResult(false);
                    }
                }
            }
        }
        public static IntPtr findHandle(ref int time_find, string windowName)
        {
            IntPtr hWnd = IntPtr.Zero;
            do
            {
                time_find++;
                // Kiểm tra xem cửa sổ đã mở chưa
                Thread.Sleep(500); // Đợi nửa giây trước khi kiểm tra lại
                var windows = FindWindowsStartingWith(windowName); // Tìm các cửa sổ

                // Nếu tìm thấy ít nhất một cửa sổ, lấy handle của cửa sổ đầu tiên
                if (windows.Count > 0)
                {
                    hWnd = windows[0]; // Lấy handle của cửa sổ đầu tiên tìm thấy
                    return hWnd;
                }

            } while (hWnd == IntPtr.Zero && time_find <= 1); // Lặp lại cho đến khi tìm thấy cửa sổ
            if (time_find > 1)
            {
                //MessageBox.Show("Không tìm thấy cửa sổ.");
                return hWnd;
            }
            return hWnd;
        }
        public static List<IntPtr> FindWindowsStartingWith(string windowNameStart)
        {
            List<IntPtr> foundWindows = new List<IntPtr>();

            // Gọi EnumWindows và truyền vào hàm kiểm tra
            EnumWindows((hWnd, lParam) =>
            {
                const int nChars = 256;
                StringBuilder titleBuffer = new StringBuilder(nChars);
                GetWindowText(hWnd, titleBuffer, nChars);
                string windowTitle = titleBuffer.ToString();

                // Kiểm tra nếu tiêu đề bắt đầu bằng windowNameStart
                if (windowTitle.StartsWith(windowNameStart, StringComparison.OrdinalIgnoreCase))
                {
                    foundWindows.Add(hWnd); // Thêm cửa sổ vào danh sách
                }
                return true; // Tiếp tục duyệt qua các cửa sổ
            }, IntPtr.Zero);
            return foundWindows; // Trả về danh sách các cửa sổ tìm thấy
        }
        public static void EndCombo()
        {
            int tc = 0;
            IntPtr main = findHandle(ref tc, "OperatorForm");
            SetForegroundWindow(main);
            txt_PO.Text = "";
            txt_PO.Focus();
            SetTaskCancel();
            GlobalVariables.List_tasks.Clear();
        }

        public static void InitComboSetup(DataGridView dgv_Setup)
        {
         
            for (int i = 0; i < GlobalVariables.Apps.Count; i++)
            {
                foreach (DataGridViewRow row in dgv_Setup.Rows)
                {
                    try
                    {
                        if (row.Cells["AppName"].Value != null && row.Cells["AppName"].Value.ToString() == GlobalVariables.Apps[i])
                        {
                            row.Cells["STT"].Value = (i + 1).ToString();
                            row.Cells["Group"].Value = GlobalVariables.Groups[i].ToString();
                        }
                    }
                    catch { }

                }
            }
        }

        private static async Task WaitForElementAndClick(IPage page, string selector)
        {
            try
            {
                // Chờ cho đến khi phần tử xuất hiện với timeout 10 giây
                await page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = 3000 });
                await page.ClickAsync(selector);
            }
            catch (TimeoutException)
            {

                throw;
            }
        }

        public static async Task PauseClick(IPage page)
        {
            try
            {
                await page.BringToFrontAsync();

                // Chờ phần tử #btnOther và click vào nó
                await WaitForElementAndClick(page, "#btnOther");

                // Chờ phần tử #btnInterruption và click vào nó
                await WaitForElementAndClick(page, "#btnInterruption");

                await Task.Delay(500);
                await page.WaitForSelectorAsync($"#subFinishMsg_Suspension");
                await page.ClickAsync($"#subFinishMsg_Suspension");
            }
            catch 
            {

            }
           
        }
        public static async Task CancelClick(IPage page)
        {
            try
            {
                await page.BringToFrontAsync();
                // Chờ phần tử #btnOther và click vào nó
                await WaitForElementAndClick(page, "#btnOther");

                // Chờ phần tử #btnInterruption và click vào nó
                await WaitForElementAndClick(page, "#btnCancel");

                await Task.Delay(400);
                await page.WaitForSelectorAsync($"#btnCheckOK");
                await page.ClickAsync($"#btnCheckOK");
            }
            catch 
            {
                
            }
        }

       

        public static async Task<bool> KCodeClick(IPage page, int where, int Kcode_Index, string name, string MANV)
        {
            try
            {

                await page.BringToFrontAsync();
                //if (where == 2)
                //{
                //    await WaitForElementAndClick(page, "#btnOther");
                //    await Task.Delay(200);
                //}
                if (where == 1)
                {
                    await WaitForElementAndClick(page, "#btnOther");
                    await Task.Delay(200);
                }

                await WaitForElementAndClick(page, "#btnKcode");
                await Task.Delay(700);

                await page.TypeAsync("#lblDebug", MANV);
                await Task.Delay(500);

                await page.Keyboard.PressAsync("Enter");
                await Task.Delay(500);

                string id_Kcode = "#lstKcode_txtKcodeNm_" + Kcode_Index;
                var elementHandle = await page.QuerySelectorAsync(id_Kcode);

                if (elementHandle != null)
                {
                    await WaitForElementAndClick(page, "#btnStart");
                    await Task.Delay(500);
                    await WaitForElementAndClick(page, id_Kcode);
                    return true;
                    //tf_Process.PauseCount();
                }
                else
                {
                    await WaitForElementAndClick(page, "#btnErrorClose");
                    return false;
                    //hp.setProcess("App không thể start Kcode");
                }

            }
            catch 
            {
                return false;
            }
            
        }


        public static async Task<bool> KCode_Finish_Click(IPage page)
        {
            try
            {

                await page.BringToFrontAsync();
                var classExists = await page.EvaluateFunctionAsync<bool>(@"
            () => {
                return document.querySelector('.tblStyle_KcodeListSelected') !== null;
            }
        ");

                if (classExists)
                {
                    await WaitForElementAndClick(page, "#btnFinish");
                    await Task.Delay(500);
                    await WaitForElementAndClick(page, "#btnCancel");
                    return true;
                }
                return false;
            }
            catch
            {
                //hp.setProcess("App không thể Finish Kcode");
                return false;
            }

        }


        /// <summary>
        /// biến RunTimes dùng để xác định App hiện tại chạy lần bao nhiêu. 
        /// Để có thể Khi mà 1 combo đnag chạy nhưng ng dùng muốn chạy combo khác, thì biến này có thể giúp chương trình chạy combo khác và hủy combo trước đó
        /// </summary>
        static int RunTimes = 0;


        // Hàm chính chạy cả Combo


        public static void ReorderAppsForSpecialGroup()
        {
            List<string> newApps = new List<string>();
            List<int> newGroups = new List<int>();
            List<object> newListObj = new List<object>(); // Thêm dòng này

            // Thêm Supro-Z200 và KJapaneseG vào group 1
            if (GlobalVariables.Apps.Contains("Supro-Z200"))
            {
                int oldIndex = GlobalVariables.Apps.IndexOf("Supro-Z200");
                newApps.Add("Supro-Z200");
                newGroups.Add(1);
                newListObj.Add(GlobalVariables.ListObj[oldIndex]); // Giữ đúng object
            }

            if (GlobalVariables.Apps.Contains("KJapaneseG"))
            {
                int oldIndex = GlobalVariables.Apps.IndexOf("KJapaneseG");
                newApps.Add("KJapaneseG");
                newGroups.Add(1);
                newListObj.Add(GlobalVariables.ListObj[oldIndex]); // Giữ đúng object
            }

            int currentGroup = 2;
            var originalGroups = GlobalVariables.Groups.Distinct().OrderBy(x => x);

            foreach (int grp in originalGroups)
            {
                for (int i = 0; i < GlobalVariables.Apps.Count; i++)
                {
                    string app = GlobalVariables.Apps[i];
                    int group = GlobalVariables.Groups[i];

                    if ((app == "Supro-Z200" || app == "KJapaneseG"))
                        continue;

                    if (group == grp)
                    {
                        newApps.Add(app);
                        newGroups.Add(currentGroup);
                        newListObj.Add(GlobalVariables.ListObj[i]); // Giữ đúng object
                    }
                }
                currentGroup++;
            }

            // Gán lại tất cả danh sách
            GlobalVariables.Apps = newApps;
            GlobalVariables.Groups = newGroups;
            GlobalVariables.ListObj = newListObj; // Cập nhật ListObj
        }

     

        public async static void Run_Flow(OperatorForm opF)
        {
            RunTimes = RunTimes + 1;
            bool checkOpenPreviousApp = true;
            GlobalVariables.UnAllMouseHook();
            SetTaskCancel();
            GlobalVariables.List_tasks.Clear();
            lbl_Status.Text = "Running";
            lbl_Status.BackColor = Color.Green;
            for (int j = 0; j < buttons.Count; j++)
            {
                if (checkboxes[j].Checked == true)
                {
                    buttons[j].BackColor = Color.White;
                }
                buttons[j].Text = GlobalVariables.Apps[j];
            }
            int group = 1;

            // Nơi chạy combo
            for (int i = 0; i <= GlobalVariables.Apps.Count; i++)
            {
                if (i == GlobalVariables.Apps.Count)// Kết thúc combo
                {
                    await Task.WhenAll(GlobalVariables.List_tasks);// Chờ các task trogn list_Task finish

                    if (CheckTask() == false)// Nếu có task bị False
                    {
                        return;
                    }
                    opF.Invoke(new Action(() => //Nếu các task đều Finish
                    {
                        lbl_Status.Text = "Stopping";
                        lbl_Status.BackColor = Color.White;
                        EndCombo();
                     
                    }));
                }
                else if (GlobalVariables.Groups[i] == group + 1) // nếu app thuộc group kế tiếp
                {
                    await Task.WhenAll(GlobalVariables.List_tasks); // Chờ các task trogn list_Task finish
                    group = group + 1;
                    if (checkboxes[i].Checked == false)//nếu check box của app đó == false thì bỏ qua ko chạy
                    {
                        continue;
                    }
                    if (CheckTask() == false)
                    {
                        return;
                    }
                    GlobalVariables.List_tasks.Clear();
                    buttons[i].BackColor = Color.Green;

                    int tmpRunTimes = RunTimes;
                    checkOpenPreviousApp = await RunApp(i, opF);

                    if (checkOpenPreviousApp != true && tmpRunTimes == RunTimes)
                    {
                        lbl_Status.Text = "Stopping";
                        lbl_Status.BackColor = Color.White;
                        buttons[i].BackColor = Color.Red;
                        return;
                    }
                    else if (checkOpenPreviousApp != true && tmpRunTimes != RunTimes)
                    {
                        return;
                    }
                }
                else if (GlobalVariables.Groups[i] == group)
                {
                    if (checkboxes[i].Checked == false)
                    {
                        continue;
                    }
                    buttons[i].BackColor = Color.Green;

                    int tmpRunTimes = RunTimes;
                    checkOpenPreviousApp = await RunApp(i, opF);
                    if (checkOpenPreviousApp != true && tmpRunTimes == RunTimes)
                    {
                        lbl_Status.Text = "Stopping";
                        lbl_Status.BackColor = Color.White;
                        buttons[i].BackColor = Color.Red;
                        return;
                    }
                    else if (checkOpenPreviousApp != true && tmpRunTimes != RunTimes)
                    {
                        return;
                    }
                }
            }
        }

        public static void MakeWindowTopMost(IntPtr hWnd)
        {
            const int HWND_TOPMOST = -1; // Đặt trạng thái top-most
            //const int HWND_NOTOPMOST = -2; // Trả về trạng thái không top-most
            const uint SWP_NOMOVE = 0x0002;
            const uint SWP_NOSIZE = 0x0001;
            const uint SWP_SHOWWINDOW = 0x0040;

            // Đưa lên trên mọi thứ (bao gồm các cửa sổ top-most)
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

            Debug.Print("đã topmosts");

        }

        public static void UNMakeWindowTopMost(IntPtr hWnd)
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
    }
}
