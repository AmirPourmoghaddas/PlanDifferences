
this.PatientTxtbx.Text = args[0] + " (" + args[1] + ")";
this.PlanTxtbx.Text = args[2] + " - " + args[3];
this.OrientationTxtbx.Text = args[4];
this.UO_YTxtbx.Text = args[6];
this.UO_XTxtbx.Text = args[5];
this.UO_ZTxtbx.Text = args[7];






this.PatientTxtbx = new System.Windows.Forms.TextBox();

this.PatientTxtbx.Text = args[0] + " (" + args[1] + ")";      //initialize form with patient data
this.PlanTxtbx = new System.Windows.Forms.TextBox();

this.PlanTxtbx.Text = args[2] + " - " + args[3];
this.OrientationTxtbx = new System.Windows.Forms.TextBox();
this.OrientationTxtbx.Text = args[4];


SelectedEclipse_XTxtbx
SelectedEclipse_YTxtbx
SelectedEclipse_ZTxtbx


UO_Txtbx
UO_YTxtbx
UO_ZTxtbx

SC_XTxtbx
SC_YTxtbx
SC_ZTxtbx


decimal UOx; decimal UOy; decimal UOz;
bool flag;
flag = Decimal.TryParse(this.UO_XTxtbx.Text, out UOx); // decimal returns false if not able to convert successfully. good to catch non numbers. 
                                                       //decimal d = Decimal.Parse("1.2345E-02", System.Globalization.NumberStyles.Float);
if (!flag)
    flag = Decimal.TryParse(this.UO_XTxtbx.Text, out UOx, System.Globalization.NumberStyles.Float); // decimal returns false if not able to convert successfully. good to catch non numbers. 


Decimal.TryParse(this.UO_YTxtbx.Text, out UOy);
Decimal.TryParse(this.UO_ZTxtbx.Text, out UOz);



private static float ParseValue(string input, string varname)
{
    try // need to make this into a function so I can reuse it
    {
        UOx = Decimal.Parse(this.UO_XTxtbx.Text, System.Globalization.NumberStyles.Float); // decimal returns false if not able to convert successfully. good to catch non numbers. 
    }
    catch
    {
        MessageBox.Show("Error: Was not able to convert the User Origin X value : " + this.UO_XTxtbx.Text + " to a number.");
    }

}



this.PatientTxtbx.Text = args[0] + " (" + args[1] + ")";      //initialize form with patient data
this.PlanTxtbx = new System.Windows.Forms.TextBox();

this.PlanTxtbx.Text = args[2] + " - " + args[3];
this.OrientationTxtbx = new System.Windows.Forms.TextBox();
this.OrientationTxtbx.Text = args[4];
this.SuspendLayout();




namespace test_Wforms_app
{
    partial class Form1
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
        private void InitializeComponent()//string [] args)
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SelectedEclipse_XTxtbx = new System.Windows.Forms.TextBox();
            this.SelectedEclipse_YTxtbx = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectedEclipse_ZTxtbx = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.UO_ZTxtbx = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.UO_YTxtbx = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.UO_XTxtbx = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.OrientationTxt = new System.Windows.Forms.Label();
            this.PatientTxt = new System.Windows.Forms.Label();
            this.PlanTxt = new System.Windows.Forms.Label();
            this.Updatebtn = new System.Windows.Forms.Button();
            this.SC_ZTxtbx = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SC_YTxtbx = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.SC_XTxtbx = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.PatientTxtbx = new System.Windows.Forms.TextBox();

            this.PatientTxtbx.Text = args[0] + " (" + args[1] + ")";      //initialize form with patient data
            this.PlanTxtbx = new System.Windows.Forms.TextBox();

            this.PlanTxtbx.Text = args[2] + " - " + args[3];
            this.OrientationTxtbx = new System.Windows.Forms.TextBox();
            this.OrientationTxtbx.Text = args[4];
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label1.Location = new System.Drawing.Point(109, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "X (cm)";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // SelectedEclipse_XTxtbx
            // 
            this.SelectedEclipse_XTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.SelectedEclipse_XTxtbx.Location = new System.Drawing.Point(97, 205);
            this.SelectedEclipse_XTxtbx.Name = "SelectedEclipse_XTxtbx";
            this.SelectedEclipse_XTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SelectedEclipse_XTxtbx.TabIndex = 2;
            // 
            // SelectedEclipse_YTxtbx
            // 
            this.SelectedEclipse_YTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.SelectedEclipse_YTxtbx.Location = new System.Drawing.Point(97, 268);
            this.SelectedEclipse_YTxtbx.Name = "SelectedEclipse_YTxtbx";
            this.SelectedEclipse_YTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SelectedEclipse_YTxtbx.TabIndex = 4;
            this.SelectedEclipse_YTxtbx.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label2.Location = new System.Drawing.Point(109, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y (cm)";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // SelectedEclipse_ZTxtbx
            // 
            this.SelectedEclipse_ZTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.SelectedEclipse_ZTxtbx.Location = new System.Drawing.Point(97, 330);
            this.SelectedEclipse_ZTxtbx.Name = "SelectedEclipse_ZTxtbx";
            this.SelectedEclipse_ZTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SelectedEclipse_ZTxtbx.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label3.Location = new System.Drawing.Point(109, 307);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Z (cm)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.label4.Location = new System.Drawing.Point(75, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 48);
            this.label4.TabIndex = 7;
            this.label4.Text = "Point specified \r\nin Eclipse";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.label5.Location = new System.Drawing.Point(248, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 24);
            this.label5.TabIndex = 14;
            this.label5.Text = "User origin\r\n";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // UO_ZTxtbx
            // 
            this.UO_ZTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.UO_ZTxtbx.Location = new System.Drawing.Point(255, 330);
            this.UO_ZTxtbx.Name = "UO_ZTxtbx";
            this.UO_ZTxtbx.ReadOnly = true;
            this.UO_ZTxtbx.Size = new System.Drawing.Size(82, 26);
            this.UO_ZTxtbx.TabIndex = 13;
            this.UO_ZTxtbx.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            this.UO_ZTxtbx.Text = args[7];

            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label6.Location = new System.Drawing.Point(267, 307);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 20);
            this.label6.TabIndex = 12;
            this.label6.Text = "Z (cm)";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // UO_YTxtbx
            // 
            this.UO_YTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.UO_YTxtbx.Location = new System.Drawing.Point(255, 268);
            this.UO_YTxtbx.Name = "UO_YTxtbx";
            this.UO_YTxtbx.ReadOnly = true;
            this.UO_YTxtbx.Size = new System.Drawing.Size(82, 26);
            this.UO_YTxtbx.TabIndex = 11;
            this.UO_YTxtbx.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            this.UO_YTxtbx.Text = args[6];
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label7.Location = new System.Drawing.Point(267, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 20);
            this.label7.TabIndex = 10;
            this.label7.Text = "Y (cm)";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // UO_XTxtbx
            // 
            this.UO_XTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.UO_XTxtbx.Location = new System.Drawing.Point(255, 205);
            this.UO_XTxtbx.Name = "UO_XTxtbx";
            this.UO_XTxtbx.ReadOnly = true;
            this.UO_XTxtbx.Size = new System.Drawing.Size(82, 26);
            this.UO_XTxtbx.TabIndex = 9;
            this.UO_XTxtbx.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
            this.UO_XTxtbx.Text = args[5];

            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label8.Location = new System.Drawing.Point(267, 182);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 20);
            this.label8.TabIndex = 8;
            this.label8.Text = "X (cm)";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.label9.Location = new System.Drawing.Point(392, 127);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 48);
            this.label9.TabIndex = 21;
            this.label9.Text = "SunCheck\r\nCoordinates\r\n";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OrientationTxt
            // 
            this.OrientationTxt.AutoSize = true;
            this.OrientationTxt.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.OrientationTxt.Location = new System.Drawing.Point(87, 94);
            this.OrientationTxt.Name = "OrientationTxt";
            this.OrientationTxt.Size = new System.Drawing.Size(191, 24);
            this.OrientationTxt.TabIndex = 22;
            this.OrientationTxt.Text = "Identified Orientation: ";
            this.OrientationTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.OrientationTxt.Click += new System.EventHandler(this.label13_Click);
            // 
            // PatientTxt
            // 
            this.PatientTxt.AutoSize = true;
            this.PatientTxt.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.PatientTxt.Location = new System.Drawing.Point(87, 29);
            this.PatientTxt.Name = "PatientTxt";
            this.PatientTxt.Size = new System.Drawing.Size(76, 24);
            this.PatientTxt.TabIndex = 23;
            this.PatientTxt.Text = "Patient: ";
            this.PatientTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlanTxt
            // 
            this.PlanTxt.AutoSize = true;
            this.PlanTxt.Font = new System.Drawing.Font("Times New Roman", 13F);
            this.PlanTxt.Location = new System.Drawing.Point(87, 63);
            this.PlanTxt.Name = "PlanTxt";
            this.PlanTxt.Size = new System.Drawing.Size(57, 24);
            this.PlanTxt.TabIndex = 24;
            this.PlanTxt.Text = "Plan: ";
            this.PlanTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PlanTxt.Click += new System.EventHandler(this.label15_Click);
            // 
            // Updatebtn
            // 
            this.Updatebtn.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.Updatebtn.Location = new System.Drawing.Point(250, 381);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(94, 38);
            this.Updatebtn.TabIndex = 25;
            this.Updatebtn.Text = "Update";
            this.Updatebtn.UseVisualStyleBackColor = true;
            this.Updatebtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // SC_ZTxtbx
            // 
            this.SC_ZTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.SC_ZTxtbx.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.SC_ZTxtbx.Location = new System.Drawing.Point(405, 330);
            this.SC_ZTxtbx.Name = "SC_ZTxtbx";
            this.SC_ZTxtbx.ReadOnly = true;
            this.SC_ZTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SC_ZTxtbx.TabIndex = 31;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label10.Location = new System.Drawing.Point(417, 307);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 20);
            this.label10.TabIndex = 30;
            this.label10.Text = "Z (mm)";
            // 
            // SC_YTxtbx
            // 
            this.SC_YTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.SC_YTxtbx.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.SC_YTxtbx.Location = new System.Drawing.Point(405, 268);
            this.SC_YTxtbx.Name = "SC_YTxtbx";
            this.SC_YTxtbx.ReadOnly = true;
            this.SC_YTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SC_YTxtbx.TabIndex = 29;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label11.Location = new System.Drawing.Point(417, 245);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 20);
            this.label11.TabIndex = 28;
            this.label11.Text = "Y (mm)";
            // 
            // SC_XTxtbx
            // 
            this.SC_XTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.SC_XTxtbx.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.SC_XTxtbx.Location = new System.Drawing.Point(405, 205);
            this.SC_XTxtbx.Name = "SC_XTxtbx";
            this.SC_XTxtbx.ReadOnly = true;
            this.SC_XTxtbx.Size = new System.Drawing.Size(82, 26);
            this.SC_XTxtbx.TabIndex = 27;
            this.SC_XTxtbx.TextChanged += new System.EventHandler(this.SC_XTxtbx_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.label12.Location = new System.Drawing.Point(417, 182);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 20);
            this.label12.TabIndex = 26;
            this.label12.Text = "X (mm)";
            // 
            // PatientTxtbx
            // 
            this.PatientTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.PatientTxtbx.Location = new System.Drawing.Point(158, 29);
            this.PatientTxtbx.Name = "PatientTxtbx";
            this.PatientTxtbx.ReadOnly = true;
            this.PatientTxtbx.Size = new System.Drawing.Size(329, 26);
            this.PatientTxtbx.TabIndex = 32;
            // 
            // PlanTxtbx
            // 
            this.PlanTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.PlanTxtbx.Location = new System.Drawing.Point(158, 61);
            this.PlanTxtbx.Name = "PlanTxtbx";
            this.PlanTxtbx.ReadOnly = true;
            this.PlanTxtbx.Size = new System.Drawing.Size(329, 26);
            this.PlanTxtbx.TabIndex = 33;
            // 
            // OrientationTxtbx
            // 
            this.OrientationTxtbx.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold);
            this.OrientationTxtbx.Location = new System.Drawing.Point(275, 94);
            this.OrientationTxtbx.Name = "OrientationTxtbx";
            this.OrientationTxtbx.ReadOnly = true;
            this.OrientationTxtbx.Size = new System.Drawing.Size(212, 26);
            this.OrientationTxtbx.TabIndex = 34;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 478);
            this.Controls.Add(this.OrientationTxtbx);
            this.Controls.Add(this.PlanTxtbx);
            this.Controls.Add(this.PatientTxtbx);
            this.Controls.Add(this.SC_ZTxtbx);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.SC_YTxtbx);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.SC_XTxtbx);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.Updatebtn);
            this.Controls.Add(this.PlanTxt);
            this.Controls.Add(this.PatientTxt);
            this.Controls.Add(this.OrientationTxt);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.UO_ZTxtbx);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.UO_YTxtbx);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.UO_XTxtbx);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SelectedEclipse_ZTxtbx);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SelectedEclipse_YTxtbx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SelectedEclipse_XTxtbx);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SelectedEclipse_XTxtbx;
        private System.Windows.Forms.TextBox SelectedEclipse_YTxtbx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SelectedEclipse_ZTxtbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox UO_ZTxtbx;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox UO_YTxtbx;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox UO_XTxtbx;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label OrientationTxt;
        private System.Windows.Forms.Label PatientTxt;
        private System.Windows.Forms.Label PlanTxt;
        private System.Windows.Forms.Button Updatebtn;
        private System.Windows.Forms.TextBox SC_ZTxtbx;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox SC_YTxtbx;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox SC_XTxtbx;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox PatientTxtbx;
        private System.Windows.Forms.TextBox PlanTxtbx;
        private System.Windows.Forms.TextBox OrientationTxtbx;
    }
}


