namespace TestNadaConfigWindow
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
        private void InitializeComponent()
        {
            this.Env = new System.Windows.Forms.TextBox();
            this.item = new System.Windows.Forms.TextBox();
            this.section = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.conn = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Key = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.val = new System.Windows.Forms.Label();
            this.server = new System.Windows.Forms.Label();
            this.service = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TO = new System.Windows.Forms.Label();
            this.servicename = new System.Windows.Forms.TextBox();
            this.srvname = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Env
            // 
            this.Env.Location = new System.Drawing.Point(95, 65);
            this.Env.Name = "Env";
            this.Env.Size = new System.Drawing.Size(150, 20);
            this.Env.TabIndex = 5;
            // 
            // item
            // 
            this.item.Location = new System.Drawing.Point(83, 82);
            this.item.Name = "item";
            this.item.Size = new System.Drawing.Size(150, 20);
            this.item.TabIndex = 3;
            // 
            // section
            // 
            this.section.Location = new System.Drawing.Point(83, 35);
            this.section.Name = "section";
            this.section.Size = new System.Drawing.Size(150, 20);
            this.section.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(267, 63);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Find Service";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // conn
            // 
            this.conn.AutoSize = true;
            this.conn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conn.ForeColor = System.Drawing.Color.Green;
            this.conn.Location = new System.Drawing.Point(26, 105);
            this.conn.Name = "conn";
            this.conn.Size = new System.Drawing.Size(117, 16);
            this.conn.TabIndex = 7;
            this.conn.Text = "Connected To : ";
            this.conn.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Environment";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Section";
            // 
            // Key
            // 
            this.Key.AutoSize = true;
            this.Key.Location = new System.Drawing.Point(27, 85);
            this.Key.Name = "Key";
            this.Key.Size = new System.Drawing.Size(27, 13);
            this.Key.TabIndex = 2;
            this.Key.Text = "Item";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(275, 82);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Get";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Value";
            // 
            // val
            // 
            this.val.AutoSize = true;
            this.val.Location = new System.Drawing.Point(80, 130);
            this.val.Name = "val";
            this.val.Size = new System.Drawing.Size(0, 13);
            this.val.TabIndex = 5;
            // 
            // server
            // 
            this.server.AutoSize = true;
            this.server.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.server.ForeColor = System.Drawing.Color.Green;
            this.server.Location = new System.Drawing.Point(149, 105);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(0, 16);
            this.server.TabIndex = 9;
            // 
            // service
            // 
            this.service.AutoSize = true;
            this.service.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.service.ForeColor = System.Drawing.Color.Green;
            this.service.Location = new System.Drawing.Point(26, 137);
            this.service.Name = "service";
            this.service.Size = new System.Drawing.Size(0, 16);
            this.service.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.item);
            this.groupBox1.Controls.Add(this.section);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.val);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Key);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(12, 180);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 177);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // TO
            // 
            this.TO.AutoSize = true;
            this.TO.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TO.ForeColor = System.Drawing.Color.Red;
            this.TO.Location = new System.Drawing.Point(138, 105);
            this.TO.Name = "TO";
            this.TO.Size = new System.Drawing.Size(91, 20);
            this.TO.TabIndex = 8;
            this.TO.Text = "Timed Out";
            this.TO.Visible = false;
            // 
            // servicename
            // 
            this.servicename.Location = new System.Drawing.Point(95, 12);
            this.servicename.Name = "servicename";
            this.servicename.Size = new System.Drawing.Size(296, 20);
            this.servicename.TabIndex = 1;
            this.servicename.Leave += new System.EventHandler(this.servicename_Leave);
            // 
            // srvname
            // 
            this.srvname.Location = new System.Drawing.Point(95, 38);
            this.srvname.Name = "srvname";
            this.srvname.Size = new System.Drawing.Size(150, 20);
            this.srvname.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Service";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Server";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 397);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.srvname);
            this.Controls.Add(this.servicename);
            this.Controls.Add(this.TO);
            this.Controls.Add(this.service);
            this.Controls.Add(this.server);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.conn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Env);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Test NadaConfig";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Env;
        private System.Windows.Forms.TextBox item;
        private System.Windows.Forms.TextBox section;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label conn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Key;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label val;
        private System.Windows.Forms.Label server;
        private System.Windows.Forms.Label service;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label TO;
        private System.Windows.Forms.TextBox servicename;
        private System.Windows.Forms.TextBox srvname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
    }
}

