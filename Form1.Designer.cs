using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System;
using System.Globalization;
using System.Collections;



namespace PlanDifferences
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



        private void InitializeComponent(Patient this_patient, string[] args)
        //private void InitializeComponent()
        {
            this.PatientTxt = new System.Windows.Forms.Label();
            this.Updatebtn = new System.Windows.Forms.Button();
            this.PatientTxtbx = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // PatientTxt
            // 
            this.PatientTxt.AutoSize = true;
            this.PatientTxt.BackColor = System.Drawing.Color.SteelBlue;
            this.PatientTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.PatientTxt.ForeColor = System.Drawing.Color.White;
            this.PatientTxt.Location = new System.Drawing.Point(43, 27);
            this.PatientTxt.Name = "PatientTxt";
            this.PatientTxt.Size = new System.Drawing.Size(76, 22);
            this.PatientTxt.TabIndex = 23;
            this.PatientTxt.Text = "Patient: ";
            this.PatientTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Updatebtn
            // 
            this.Updatebtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);//, System.Drawing.FontStyle.Bold);
            this.Updatebtn.Location = new System.Drawing.Point(273, 960);
            this.Updatebtn.Name = "Updatebtn";
            this.Updatebtn.Size = new System.Drawing.Size(94, 38);
            this.Updatebtn.TabIndex = 25;
            this.Updatebtn.Text = "Compare plans";
            this.Updatebtn.UseVisualStyleBackColor = true;
            this.Updatebtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // PatientTxtbx
            // 
            this.PatientTxtbx.BackColor = System.Drawing.Color.SteelBlue;
            this.PatientTxtbx.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PatientTxtbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);//, System.Drawing.FontStyle.Bold);
            this.PatientTxtbx.ForeColor = System.Drawing.Color.White;
            this.PatientTxtbx.Location = new System.Drawing.Point(120, 28);
            this.PatientTxtbx.Name = "PatientTxtbx";
            this.PatientTxtbx.ReadOnly = true;
            this.PatientTxtbx.Size = new System.Drawing.Size(469, 20);
            this.PatientTxtbx.TabIndex = 32;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.SteelBlue;
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(660, 65);
            this.pictureBox1.TabIndex = 35;
            this.pictureBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(115, 97);
            this.comboBox1.MaxDropDownItems = 20;
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(252, 28);
            this.comboBox1.TabIndex = 36;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(373, 96);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(254, 28);
            this.comboBox2.TabIndex = 36;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(115, 135);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(252, 28);
            this.comboBox3.TabIndex = 36;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // comboBox4
            // 
            this.comboBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(373, 135);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(254, 28);
            this.comboBox4.TabIndex = 37;
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 180);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(599, 774);
            this.textBox1.TabIndex = 39;
            this.textBox1.Text = "";
            this.textBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(36, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 22);
            this.label1.TabIndex = 40;
            this.label1.Text = "Courses";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(58, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 22);
            this.label2.TabIndex = 40;
            this.label2.Text = "Plans";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 1010);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.PatientTxtbx);
            this.Controls.Add(this.Updatebtn);
            this.Controls.Add(this.PatientTxt);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Identify plan differences";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            InitData(this_patient);

        }
        public void InitData(Patient this_patient)
        {
            //InitData(this_patient); uncomment and copy this line into the function above if it gets deleted after editing the form. 

            GetCoursesList(this_patient);
            this.comboBox1.Items.AddRange(DATA.CourseList);
            DATA.this_patient = this_patient;
            this.comboBox2.Items.AddRange(DATA.CourseList);
            this.PatientTxtbx.Text = "  "+ this_patient.LastName + ", " + this_patient.FirstName + " ( " + this_patient.Id + " )";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)//, Patient this_patient)
        {
            DATA.SelectedCourseIndex = this.comboBox1.SelectedIndex;
            this.comboBox3.Text = "-----";
            this.comboBox3.Items.Clear();
            GetPlansList();            
            this.comboBox3.Items.AddRange(DATA.PlansList);

           


        }

        public DataBlock DATA;
        public void GetCoursesList(Patient this_patient)
        {
            IEnumerable<Course> courses = this_patient.Courses;
            var CrsNum = courses.ToList().Count;
            string[] CourseList = new String[CrsNum];
            int k = 0;
            while (k < CrsNum)
            {
                CourseList[k] = courses.ToList()[k].Id;
                k++;
            }
            DATA.CourseList = CourseList;
            DATA.CrsNum = CrsNum;
        }

        public void GetPlansList()
        {
            IEnumerable<Course> courses = DATA.this_patient.Courses;
            int PlanNum = courses.ToList()[DATA.SelectedCourseIndex].PlanSetups.ToList().Count;
            int k = 0;
            DATA.PlanNum = (int)0;
            if (PlanNum > 0)
            {
                string[] PlansList = new String[PlanNum];
                while (k < PlanNum)
                {
                    PlansList[k] = courses.ToList()[DATA.SelectedCourseIndex].PlanSetups.ToList()[k].Id;
                    k++;
                }
                DATA.PlansList = PlansList;
                DATA.PlanNum = PlanNum;
            }
            else
            // there are no plans listed for this course
            {
                string[] PlansList = new String[1];
                DATA.PlansList = PlansList;
                DATA.PlansList[0] = "<--No Plans Listed For This Course-->";
            }
        }

        public struct DataBlock
        {
            public string[] CourseList;
            public string CourseId;
            public int CrsNum;
            public string[] PlansList;
            public int PlanNum;
            public int SelectedCourseIndex;
            public Patient this_patient;
            public PlanSetup pln1;
            public PlanSetup pln2;
            

        }

        #endregion
        private System.Windows.Forms.Label PatientTxt;
        private System.Windows.Forms.Button Updatebtn;
        private System.Windows.Forms.TextBox PatientTxtbx;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.RichTextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;

        // initialize 
    }
}

