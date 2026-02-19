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
            panelDetalles = new Panel();
            lblCantidad = new Label();
            lblTamanio = new Label();
            lblExtension = new Label();
            lblTipo = new Label();
            lblNombreCompleto = new Label();
            panelDetalles.SuspendLayout();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Location = new Point(12, 48);
            listView1.Name = "listView1";
            listView1.Size = new Size(580, 390);
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
            // panelDetalles
            // 
            panelDetalles.BackColor = SystemColors.Info;
            panelDetalles.BorderStyle = BorderStyle.FixedSingle;
            panelDetalles.Controls.Add(lblCantidad);
            panelDetalles.Controls.Add(lblTamanio);
            panelDetalles.Controls.Add(lblExtension);
            panelDetalles.Controls.Add(lblTipo);
            panelDetalles.Controls.Add(lblNombreCompleto);
            panelDetalles.Location = new Point(598, 48);
            panelDetalles.Name = "panelDetalles";
            panelDetalles.Size = new Size(190, 390);
            panelDetalles.TabIndex = 5;
            // 
            // lblCantidad
            // 
            lblCantidad.Location = new Point(8, 154);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(170, 40);
            lblCantidad.TabIndex = 4;
            lblCantidad.Text = "Contenido:";
            // 
            // lblTamanio
            // 
            lblTamanio.Location = new Point(8, 134);
            lblTamanio.Name = "lblTamanio";
            lblTamanio.Size = new Size(170, 20);
            lblTamanio.TabIndex = 3;
            lblTamanio.Text = "Tamaño:";
            // 
            // lblExtension
            // 
            lblExtension.Location = new Point(8, 103);
            lblExtension.Name = "lblExtension";
            lblExtension.Size = new Size(170, 20);
            lblExtension.TabIndex = 2;
            lblExtension.Text = "Extensión:";
            // 
            // lblTipo
            // 
            lblTipo.Location = new Point(8, 58);
            lblTipo.Name = "lblTipo";
            lblTipo.Size = new Size(170, 20);
            lblTipo.TabIndex = 1;
            lblTipo.Text = "Tipo:";
            // 
            // lblNombreCompleto
            // 
            lblNombreCompleto.Location = new Point(8, 8);
            lblNombreCompleto.Name = "lblNombreCompleto";
            lblNombreCompleto.Size = new Size(170, 60);
            lblNombreCompleto.TabIndex = 0;
            lblNombreCompleto.Text = "Nombre:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelDetalles);
            Controls.Add(txtRutaInicial);
            Controls.Add(btnRegresar);
            Controls.Add(btnCargarSubcarpetas);
            Controls.Add(btnSeleccionarCarpeta);
            Controls.Add(listView1);
            Name = "Form1";
            Text = "Explorador de archivos";
            panelDetalles.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button btnSeleccionarCarpeta;
        private System.Windows.Forms.Button btnRegresar;
        private System.Windows.Forms.TextBox txtRutaInicial;
        private System.Windows.Forms.Button btnCargarSubcarpetas;
        private System.Windows.Forms.Panel panelDetalles;
        private System.Windows.Forms.Label lblNombreCompleto;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.Label lblTamanio;
        private System.Windows.Forms.Label lblCantidad;
    }
}
