
namespace CatVision.Wpf.Views
{
    partial class DrawROIForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelPos = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonDisp = new System.Windows.Forms.Button();
            this.buttonClearRoi = new System.Windows.Forms.Button();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.buttonDrawRoi = new System.Windows.Forms.Button();
            this.buttonReadImg = new System.Windows.Forms.Button();
            this.textBoxLength2 = new System.Windows.Forms.TextBox();
            this.textBoxLength1 = new System.Windows.Forms.TextBox();
            this.textBoxPhi = new System.Windows.Forms.TextBox();
            this.textBoxColumn = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxRow = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 924F));
            this.tableLayoutPanel1.Controls.Add(this.labelPos, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelInfo, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1222, 814);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelPos
            // 
            this.labelPos.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPos.AutoSize = true;
            this.labelPos.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelPos.ForeColor = System.Drawing.Color.White;
            this.labelPos.Location = new System.Drawing.Point(301, 782);
            this.labelPos.Name = "labelPos";
            this.labelPos.Size = new System.Drawing.Size(46, 27);
            this.labelPos.TabIndex = 19;
            this.labelPos.Text = "Pos";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonDisp);
            this.groupBox1.Controls.Add(this.buttonClearRoi);
            this.groupBox1.Controls.Add(this.buttonConfirm);
            this.groupBox1.Controls.Add(this.buttonDrawRoi);
            this.groupBox1.Controls.Add(this.buttonReadImg);
            this.groupBox1.Controls.Add(this.textBoxLength2);
            this.groupBox1.Controls.Add(this.textBoxLength1);
            this.groupBox1.Controls.Add(this.textBoxPhi);
            this.groupBox1.Controls.Add(this.textBoxColumn);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxRow);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxType);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 772);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DrawRoi";
            // 
            // buttonDisp
            // 
            this.buttonDisp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.buttonDisp.Location = new System.Drawing.Point(15, 484);
            this.buttonDisp.Name = "buttonDisp";
            this.buttonDisp.Size = new System.Drawing.Size(144, 38);
            this.buttonDisp.TabIndex = 19;
            this.buttonDisp.Text = "DisplayRoi";
            this.buttonDisp.UseVisualStyleBackColor = false;
            this.buttonDisp.Click += new System.EventHandler(this.buttonDisp_Click);
            // 
            // buttonClearRoi
            // 
            this.buttonClearRoi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.buttonClearRoi.Location = new System.Drawing.Point(15, 528);
            this.buttonClearRoi.Name = "buttonClearRoi";
            this.buttonClearRoi.Size = new System.Drawing.Size(144, 38);
            this.buttonClearRoi.TabIndex = 18;
            this.buttonClearRoi.Text = "ClearRoi";
            this.buttonClearRoi.UseVisualStyleBackColor = false;
            this.buttonClearRoi.Click += new System.EventHandler(this.buttonClearRoi_Click);
            // 
            // buttonConfirm
            // 
            this.buttonConfirm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.buttonConfirm.Location = new System.Drawing.Point(15, 572);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(144, 38);
            this.buttonConfirm.TabIndex = 16;
            this.buttonConfirm.Text = "Confirm";
            this.buttonConfirm.UseVisualStyleBackColor = false;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            // 
            // buttonDrawRoi
            // 
            this.buttonDrawRoi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.buttonDrawRoi.Location = new System.Drawing.Point(15, 440);
            this.buttonDrawRoi.Name = "buttonDrawRoi";
            this.buttonDrawRoi.Size = new System.Drawing.Size(144, 38);
            this.buttonDrawRoi.TabIndex = 15;
            this.buttonDrawRoi.Text = "DrawRoi";
            this.buttonDrawRoi.UseVisualStyleBackColor = false;
            this.buttonDrawRoi.Click += new System.EventHandler(this.buttonDrawRoi_Click);
            // 
            // buttonReadImg
            // 
            this.buttonReadImg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.buttonReadImg.Location = new System.Drawing.Point(15, 396);
            this.buttonReadImg.Name = "buttonReadImg";
            this.buttonReadImg.Size = new System.Drawing.Size(144, 38);
            this.buttonReadImg.TabIndex = 14;
            this.buttonReadImg.Text = "ReadImage";
            this.buttonReadImg.UseVisualStyleBackColor = false;
            this.buttonReadImg.Click += new System.EventHandler(this.buttonReadImg_Click);
            // 
            // textBoxLength2
            // 
            this.textBoxLength2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxLength2.ForeColor = System.Drawing.Color.White;
            this.textBoxLength2.Location = new System.Drawing.Point(110, 313);
            this.textBoxLength2.Name = "textBoxLength2";
            this.textBoxLength2.Size = new System.Drawing.Size(141, 34);
            this.textBoxLength2.TabIndex = 13;
            // 
            // textBoxLength1
            // 
            this.textBoxLength1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxLength1.ForeColor = System.Drawing.Color.White;
            this.textBoxLength1.Location = new System.Drawing.Point(110, 273);
            this.textBoxLength1.Name = "textBoxLength1";
            this.textBoxLength1.Size = new System.Drawing.Size(141, 34);
            this.textBoxLength1.TabIndex = 12;
            // 
            // textBoxPhi
            // 
            this.textBoxPhi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxPhi.ForeColor = System.Drawing.Color.White;
            this.textBoxPhi.Location = new System.Drawing.Point(110, 233);
            this.textBoxPhi.Name = "textBoxPhi";
            this.textBoxPhi.Size = new System.Drawing.Size(141, 34);
            this.textBoxPhi.TabIndex = 11;
            // 
            // textBoxColumn
            // 
            this.textBoxColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxColumn.ForeColor = System.Drawing.Color.White;
            this.textBoxColumn.Location = new System.Drawing.Point(110, 193);
            this.textBoxColumn.Name = "textBoxColumn";
            this.textBoxColumn.Size = new System.Drawing.Size(141, 34);
            this.textBoxColumn.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 313);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 27);
            this.label7.TabIndex = 9;
            this.label7.Text = "Length2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 273);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 27);
            this.label6.TabIndex = 8;
            this.label6.Text = "Length1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 233);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 27);
            this.label5.TabIndex = 7;
            this.label5.Text = "Phi";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 27);
            this.label4.TabIndex = 6;
            this.label4.Text = "Column";
            // 
            // textBoxRow
            // 
            this.textBoxRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxRow.ForeColor = System.Drawing.Color.White;
            this.textBoxRow.Location = new System.Drawing.Point(110, 151);
            this.textBoxRow.Name = "textBoxRow";
            this.textBoxRow.Size = new System.Drawing.Size(141, 34);
            this.textBoxRow.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 151);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 27);
            this.label3.TabIndex = 4;
            this.label3.Text = "Row";
            // 
            // comboBoxType
            // 
            this.comboBoxType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.comboBoxType.ForeColor = System.Drawing.Color.White;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Location = new System.Drawing.Point(86, 80);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(165, 35);
            this.comboBoxType.TabIndex = 3;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 27);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // textBoxName
            // 
            this.textBoxName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.textBoxName.ForeColor = System.Drawing.Color.White;
            this.textBoxName.Location = new System.Drawing.Point(86, 40);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(165, 34);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelInfo.ForeColor = System.Drawing.Color.White;
            this.labelInfo.Location = new System.Drawing.Point(3, 782);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(50, 27);
            this.labelInfo.TabIndex = 17;
            this.labelInfo.Text = "Info";
            // 
            // DrawROIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.ClientSize = new System.Drawing.Size(1222, 814);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DrawROIForm";
            this.Text = "DRAWroiForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DrawROIForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLength2;
        private System.Windows.Forms.TextBox textBoxLength1;
        private System.Windows.Forms.TextBox textBoxPhi;
        private System.Windows.Forms.TextBox textBoxColumn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxRow;
        private System.Windows.Forms.Button buttonReadImg;
        private System.Windows.Forms.Button buttonDrawRoi;
        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonClearRoi;
        private System.Windows.Forms.Label labelPos;
        private System.Windows.Forms.Button buttonDisp;
    }
}