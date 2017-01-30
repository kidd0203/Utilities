namespace AsyncProgramming
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
            this.btnWhenAny = new System.Windows.Forms.Button();
            this.btnWhenAll = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.btnWaitAll = new System.Windows.Forms.Button();
            this.btnWaitAny = new System.Windows.Forms.Button();
            this.btnNormal = new System.Windows.Forms.Button();
            this.btnNoAwait = new System.Windows.Forms.Button();
            this.btnTaskParallel = new System.Windows.Forms.Button();
            this.btnTaskParallelNew = new System.Windows.Forms.Button();
            this.btnTaskParallelAwait = new System.Windows.Forms.Button();
            this.btnStartAsyncLoop = new System.Windows.Forms.Button();
            this.btnCancelAsyncLoop = new System.Windows.Forms.Button();
            this.btnMessage = new System.Windows.Forms.Button();
            this.btnTaskFactory = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnWhenAny
            // 
            this.btnWhenAny.Location = new System.Drawing.Point(61, 57);
            this.btnWhenAny.Name = "btnWhenAny";
            this.btnWhenAny.Size = new System.Drawing.Size(75, 23);
            this.btnWhenAny.TabIndex = 0;
            this.btnWhenAny.Text = "WhenAny";
            this.btnWhenAny.UseVisualStyleBackColor = true;
            this.btnWhenAny.Click += new System.EventHandler(this.btnWhenAny_Click);
            // 
            // btnWhenAll
            // 
            this.btnWhenAll.Location = new System.Drawing.Point(154, 57);
            this.btnWhenAll.Name = "btnWhenAll";
            this.btnWhenAll.Size = new System.Drawing.Size(75, 23);
            this.btnWhenAll.TabIndex = 1;
            this.btnWhenAll.Text = "WhenAll";
            this.btnWhenAll.UseVisualStyleBackColor = true;
            this.btnWhenAll.Click += new System.EventHandler(this.btnWhenAll_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(61, 125);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(180, 185);
            this.txtResult.TabIndex = 2;
            // 
            // btnWaitAll
            // 
            this.btnWaitAll.Location = new System.Drawing.Point(154, 86);
            this.btnWaitAll.Name = "btnWaitAll";
            this.btnWaitAll.Size = new System.Drawing.Size(75, 23);
            this.btnWaitAll.TabIndex = 3;
            this.btnWaitAll.Text = "WaitAll";
            this.btnWaitAll.UseVisualStyleBackColor = true;
            this.btnWaitAll.Click += new System.EventHandler(this.btnWaitAll_Click);
            // 
            // btnWaitAny
            // 
            this.btnWaitAny.Location = new System.Drawing.Point(61, 86);
            this.btnWaitAny.Name = "btnWaitAny";
            this.btnWaitAny.Size = new System.Drawing.Size(75, 23);
            this.btnWaitAny.TabIndex = 4;
            this.btnWaitAny.Text = "WaitAny";
            this.btnWaitAny.UseVisualStyleBackColor = true;
            this.btnWaitAny.Click += new System.EventHandler(this.btnWaitAny_Click);
            // 
            // btnNormal
            // 
            this.btnNormal.Location = new System.Drawing.Point(61, 28);
            this.btnNormal.Name = "btnNormal";
            this.btnNormal.Size = new System.Drawing.Size(75, 23);
            this.btnNormal.TabIndex = 5;
            this.btnNormal.Text = "Normal";
            this.btnNormal.UseVisualStyleBackColor = true;
            this.btnNormal.Click += new System.EventHandler(this.btnNormal_Click);
            // 
            // btnNoAwait
            // 
            this.btnNoAwait.Location = new System.Drawing.Point(154, 28);
            this.btnNoAwait.Name = "btnNoAwait";
            this.btnNoAwait.Size = new System.Drawing.Size(75, 23);
            this.btnNoAwait.TabIndex = 6;
            this.btnNoAwait.Text = "NoAwait";
            this.btnNoAwait.UseVisualStyleBackColor = true;
            this.btnNoAwait.Click += new System.EventHandler(this.btnNoAwait_Click);
            // 
            // btnTaskParallel
            // 
            this.btnTaskParallel.Location = new System.Drawing.Point(283, 27);
            this.btnTaskParallel.Name = "btnTaskParallel";
            this.btnTaskParallel.Size = new System.Drawing.Size(109, 23);
            this.btnTaskParallel.TabIndex = 7;
            this.btnTaskParallel.Text = "TaskParallel";
            this.btnTaskParallel.UseVisualStyleBackColor = true;
            this.btnTaskParallel.Click += new System.EventHandler(this.btnTaskParallel_Click);
            // 
            // btnTaskParallelNew
            // 
            this.btnTaskParallelNew.Location = new System.Drawing.Point(283, 56);
            this.btnTaskParallelNew.Name = "btnTaskParallelNew";
            this.btnTaskParallelNew.Size = new System.Drawing.Size(109, 23);
            this.btnTaskParallelNew.TabIndex = 8;
            this.btnTaskParallelNew.Text = "TaskParallelNew";
            this.btnTaskParallelNew.UseVisualStyleBackColor = true;
            this.btnTaskParallelNew.Click += new System.EventHandler(this.btnTaskParallelNew_Click);
            // 
            // btnTaskParallelAwait
            // 
            this.btnTaskParallelAwait.Location = new System.Drawing.Point(283, 86);
            this.btnTaskParallelAwait.Name = "btnTaskParallelAwait";
            this.btnTaskParallelAwait.Size = new System.Drawing.Size(109, 23);
            this.btnTaskParallelAwait.TabIndex = 9;
            this.btnTaskParallelAwait.Text = "TaskParallelAwait";
            this.btnTaskParallelAwait.UseVisualStyleBackColor = true;
            this.btnTaskParallelAwait.Click += new System.EventHandler(this.btnTaskParallelAwait_Click);
            // 
            // btnStartAsyncLoop
            // 
            this.btnStartAsyncLoop.Location = new System.Drawing.Point(263, 202);
            this.btnStartAsyncLoop.Name = "btnStartAsyncLoop";
            this.btnStartAsyncLoop.Size = new System.Drawing.Size(75, 23);
            this.btnStartAsyncLoop.TabIndex = 10;
            this.btnStartAsyncLoop.Text = "StartLoop";
            this.btnStartAsyncLoop.UseVisualStyleBackColor = true;
            this.btnStartAsyncLoop.Click += new System.EventHandler(this.btnStartAsyncLoop_Click);
            // 
            // btnCancelAsyncLoop
            // 
            this.btnCancelAsyncLoop.Location = new System.Drawing.Point(263, 232);
            this.btnCancelAsyncLoop.Name = "btnCancelAsyncLoop";
            this.btnCancelAsyncLoop.Size = new System.Drawing.Size(75, 23);
            this.btnCancelAsyncLoop.TabIndex = 11;
            this.btnCancelAsyncLoop.Text = "CancelLoop";
            this.btnCancelAsyncLoop.UseVisualStyleBackColor = true;
            this.btnCancelAsyncLoop.Click += new System.EventHandler(this.btnCancelAsyncLoop_Click);
            // 
            // btnMessage
            // 
            this.btnMessage.Location = new System.Drawing.Point(263, 320);
            this.btnMessage.Name = "btnMessage";
            this.btnMessage.Size = new System.Drawing.Size(75, 23);
            this.btnMessage.TabIndex = 12;
            this.btnMessage.Text = "Message";
            this.btnMessage.UseVisualStyleBackColor = true;
            this.btnMessage.Click += new System.EventHandler(this.btnMessage_Click);
            // 
            // btnTaskFactory
            // 
            this.btnTaskFactory.Location = new System.Drawing.Point(61, 320);
            this.btnTaskFactory.Name = "btnTaskFactory";
            this.btnTaskFactory.Size = new System.Drawing.Size(75, 23);
            this.btnTaskFactory.TabIndex = 13;
            this.btnTaskFactory.Text = "TaskFactoryTest";
            this.btnTaskFactory.UseVisualStyleBackColor = true;
            this.btnTaskFactory.Click += new System.EventHandler(this.btnTaskFactory_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 349);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 400);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTaskFactory);
            this.Controls.Add(this.btnMessage);
            this.Controls.Add(this.btnCancelAsyncLoop);
            this.Controls.Add(this.btnStartAsyncLoop);
            this.Controls.Add(this.btnTaskParallelAwait);
            this.Controls.Add(this.btnTaskParallelNew);
            this.Controls.Add(this.btnTaskParallel);
            this.Controls.Add(this.btnNoAwait);
            this.Controls.Add(this.btnNormal);
            this.Controls.Add(this.btnWaitAny);
            this.Controls.Add(this.btnWaitAll);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnWhenAll);
            this.Controls.Add(this.btnWhenAny);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWhenAny;
        private System.Windows.Forms.Button btnWhenAll;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnWaitAll;
        private System.Windows.Forms.Button btnWaitAny;
        private System.Windows.Forms.Button btnNormal;
        private System.Windows.Forms.Button btnNoAwait;
        private System.Windows.Forms.Button btnTaskParallel;
        private System.Windows.Forms.Button btnTaskParallelNew;
        private System.Windows.Forms.Button btnTaskParallelAwait;
        private System.Windows.Forms.Button btnStartAsyncLoop;
        private System.Windows.Forms.Button btnCancelAsyncLoop;
        private System.Windows.Forms.Button btnMessage;
        private System.Windows.Forms.Button btnTaskFactory;
        private System.Windows.Forms.Button button1;
    }
}

