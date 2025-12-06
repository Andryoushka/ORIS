namespace GameAndDot.WinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            button1 = new Button();
            label1 = new Label();
            listBox1 = new ListBox();
            label2 = new Label();
            usernameLbl = new Label();
            colorLbl = new Label();
            label5 = new Label();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 14F);
            textBox1.Location = new Point(12, 50);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(176, 45);
            textBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 14F);
            button1.Location = new Point(214, 50);
            button1.Name = "button1";
            button1.Size = new Size(197, 49);
            button1.TabIndex = 1;
            button1.Text = "Начать игру";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(247, 38);
            label1.TabIndex = 2;
            label1.Text = "Введите никнейм:";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 25;
            listBox1.Location = new Point(547, 109);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(241, 329);
            listBox1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(547, 20);
            label2.Name = "label2";
            label2.Size = new Size(93, 25);
            label2.TabIndex = 4;
            label2.Text = "username:";
            // 
            // usernameLbl
            // 
            usernameLbl.AutoSize = true;
            usernameLbl.Location = new Point(646, 20);
            usernameLbl.Name = "usernameLbl";
            usernameLbl.Size = new Size(59, 25);
            usernameLbl.TabIndex = 5;
            usernameLbl.Text = "label3";
            // 
            // colorLbl
            // 
            colorLbl.AutoSize = true;
            colorLbl.Location = new Point(646, 64);
            colorLbl.Name = "colorLbl";
            colorLbl.Size = new Size(59, 25);
            colorLbl.TabIndex = 7;
            colorLbl.Text = "label4";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(547, 66);
            label5.Name = "label5";
            label5.Size = new Size(59, 25);
            label5.TabIndex = 6;
            label5.Text = "Color:";
            // 
            // panel1
            // 
            panel1.Location = new Point(12, 109);
            panel1.Name = "panel1";
            panel1.Size = new Size(514, 329);
            panel1.TabIndex = 8;
            panel1.MouseClick += panel1_MouseClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel1);
            Controls.Add(colorLbl);
            Controls.Add(label5);
            Controls.Add(usernameLbl);
            Controls.Add(label2);
            Controls.Add(listBox1);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(textBox1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Label label1;
        private ListBox listBox1;
        private Label label2;
        private Label usernameLbl;
        private Label colorLbl;
        private Label label5;
        private Panel panel1;
    }
}
