namespace Center
{
    partial class SetUpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetUpForm));
            this.cmb_Apps = new System.Windows.Forms.ComboBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btn_select = new System.Windows.Forms.Button();
            this.btn_remove = new System.Windows.Forms.Button();
            this.btn_Swap = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // cmb_Apps
            // 
            this.cmb_Apps.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmb_Apps.FormattingEnabled = true;
            this.cmb_Apps.Items.AddRange(new object[] {
            "Supro",
            "Inspection",
            "Print Label",
            "Group1",
            "Group2",
            "Group3"});
            this.cmb_Apps.Location = new System.Drawing.Point(12, 24);
            this.cmb_Apps.Name = "cmb_Apps";
            this.cmb_Apps.Size = new System.Drawing.Size(193, 40);
            this.cmb_Apps.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(12, 82);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(867, 537);
            this.treeView1.TabIndex = 3;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            // 
            // btn_select
            // 
            this.btn_select.Location = new System.Drawing.Point(513, 24);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(112, 38);
            this.btn_select.TabIndex = 4;
            this.btn_select.Text = "Select";
            this.btn_select.UseVisualStyleBackColor = true;
            this.btn_select.Click += new System.EventHandler(this.btn_select_Click);
            // 
            // btn_remove
            // 
            this.btn_remove.Location = new System.Drawing.Point(642, 24);
            this.btn_remove.Name = "btn_remove";
            this.btn_remove.Size = new System.Drawing.Size(109, 38);
            this.btn_remove.TabIndex = 5;
            this.btn_remove.Text = "Remove";
            this.btn_remove.UseVisualStyleBackColor = true;
            this.btn_remove.Click += new System.EventHandler(this.btn_remove_Click);
            // 
            // btn_Swap
            // 
            this.btn_Swap.Location = new System.Drawing.Point(770, 24);
            this.btn_Swap.Name = "btn_Swap";
            this.btn_Swap.Size = new System.Drawing.Size(109, 38);
            this.btn_Swap.TabIndex = 6;
            this.btn_Swap.Text = "Swap";
            this.btn_Swap.UseVisualStyleBackColor = true;
            this.btn_Swap.Click += new System.EventHandler(this.btn_Swap_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "security.png");
            // 
            // SetUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 648);
            this.Controls.Add(this.btn_Swap);
            this.Controls.Add(this.btn_remove);
            this.Controls.Add(this.btn_select);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.cmb_Apps);
            this.Name = "SetUpForm";
            this.Text = "SetUp";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_Apps;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btn_select;
        private System.Windows.Forms.Button btn_remove;
        private System.Windows.Forms.Button btn_Swap;
        private System.Windows.Forms.ImageList imageList1;
    }
}

