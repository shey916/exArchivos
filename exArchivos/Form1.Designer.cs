namespace exArchivos
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
            listView1 = new ListView();
            btnSeleccionarCarpeta = new Button();
            btnRegresar = new Button();
            txtRutaInicial = new TextBox();
            btnCargarSubcarpetas = new Button();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Location = new Point(12, 48);
            listView1.Name = "listView1";
            listView1.Size = new Size(776, 370);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // btnSeleccionarCarpeta
            // 
            btnSeleccionarCarpeta.Location = new Point(12, 12);
            btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
            btnSeleccionarCarpeta.Size = new Size(140, 30);
            btnSeleccionarCarpeta.TabIndex = 1;
            btnSeleccionarCarpeta.Text = "Seleccionar carpeta...";
            btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
            // 
            // btnRegresar
            // 
            btnRegresar.Location = new Point(304, 12);
            btnRegresar.Name = "btnRegresar";
            btnRegresar.Size = new Size(100, 30);
            btnRegresar.TabIndex = 2;
            btnRegresar.Text = "Regresar";
            btnRegresar.UseVisualStyleBackColor = true;
            // 
            // txtRutaInicial
            // 
            txtRutaInicial.Location = new Point(410, 17);
            txtRutaInicial.Name = "txtRutaInicial";
            txtRutaInicial.ReadOnly = true;
            txtRutaInicial.Size = new Size(378, 23);
            txtRutaInicial.TabIndex = 3;
            // 
            // btnCargarSubcarpetas
            // 
            btnCargarSubcarpetas.Location = new Point(158, 12);
            btnCargarSubcarpetas.Name = "btnCargarSubcarpetas";
            btnCargarSubcarpetas.Size = new Size(140, 30);
            btnCargarSubcarpetas.TabIndex = 4;
            btnCargarSubcarpetas.Text = "Mostrar subcarpetas";
            btnCargarSubcarpetas.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkGray;
            ClientSize = new Size(800, 430);
            Controls.Add(txtRutaInicial);
            Controls.Add(btnRegresar);
            Controls.Add(btnCargarSubcarpetas);
            Controls.Add(btnSeleccionarCarpeta);
            Controls.Add(listView1);
            Name = "Form1";
            Text = "Explorador de archivos";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnSeleccionarCarpeta;
        private System.Windows.Forms.Button btnRegresar;
        private System.Windows.Forms.TextBox txtRutaInicial;
        private System.Windows.Forms.Button btnCargarSubcarpetas;
    }
}
