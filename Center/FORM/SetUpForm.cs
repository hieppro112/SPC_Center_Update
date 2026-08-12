using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Center
{
    public partial class SetUpForm : Form
    {

        public SetUpForm()
        {
            InitializeComponent();
        }


        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                foreach (TreeNode node in treeView1.Nodes)
                {
                    Control.UncheckOtherNodes(node, e.Node);
                }
            }
        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            if (cmb_Apps.SelectedIndex != -1)
            {
                string nodename = cmb_Apps.SelectedItem.ToString();
                Control.AddToTreeView(nodename, treeView1);
            }
            
        }
        private void btn_remove_Click(object sender, EventArgs e)
        {
            TreeNodeCollection nodes = treeView1.Nodes;
            Control.DeleteCheckedNode(nodes);
        }

        private void btn_Swap_Click(object sender, EventArgs e)
        {
            if (cmb_Apps.SelectedIndex != -1)
            {
                TreeNode tn = null;
                tn = Control.GetCheckedNodes(treeView1.Nodes, tn);
                tn.Text = cmb_Apps.SelectedItem.ToString();
            }
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
           
        }
    }
}
