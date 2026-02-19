using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace exArchivos
{
    public partial class Form1 : Form
    {
        private readonly ImageList _smallIcons;
        private readonly Dictionary<string, int> _iconIndexCache = new();
        private DirectoryInfo _currentDirectory;

        public Form1()
        {
            InitializeComponent();

            _smallIcons = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(16, 16)
            };

            listView1.SmallImageList = _smallIcons;
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Clear();
            listView1.Columns.Add("Nombre", 350);
            listView1.Columns.Add("Tipo", 150);
            listView1.Columns.Add("Tamaño", 100);
            listView1.Columns.Add("Última modificación", 160);

            listView1.DoubleClick += ListView1_DoubleClick;
            listView1.SelectedIndexChanged += ListView1_SelectedIndexChanged;
            btnSeleccionarCarpeta.Click += BtnSeleccionarCarpeta_Click;
            btnRegresar.Click += BtnRegresar_Click;
            btnCargarSubcarpetas.Click += BtnCargarSubcarpetas_Click;

            // Inicialmente deshabilitar regresar
            btnRegresar.Enabled = false;
            panelDetalles.Visible = false;
        }

        private void BtnSeleccionarCarpeta_Click(object? sender, EventArgs e)
        {
            using var dlg = new FolderBrowserDialog
            {
                Description = "Seleccione una carpeta raíz",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            txtRutaInicial.Text = dlg.SelectedPath;
            NavigateTo(new DirectoryInfo(dlg.SelectedPath));
            btnRegresar.Enabled = _currentDirectory?.Parent != null;
        }

        private void BtnCargarSubcarpetas_Click(object? sender, EventArgs e)
        {
            // Cargar las carpetas del directorio actual (si hay)
            if (_currentDirectory == null)
            {
                MessageBox.Show("Seleccione primero una carpeta con 'Seleccionar carpeta...'", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            listView1.Items.Clear();
            _iconIndexCache.Clear();
            _smallIcons.Images.Clear();

            try
            {
                foreach (var dir in _currentDirectory.GetDirectories())
                {
                    var item = new ListViewItem(dir.Name) { Tag = dir };
                    item.SubItems.Add("Carpeta");
                    item.SubItems.Add(""); // tamaño vacío para carpetas
                    item.SubItems.Add(dir.LastWriteTime.ToString("g"));
                    item.ImageIndex = EnsureIconIndexForPath(dir.FullName, true);
                    listView1.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No tiene permiso para listar algunas carpetas.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnRegresar_Click(object? sender, EventArgs e)
        {
            if (_currentDirectory == null)
                return;

            var parent = _currentDirectory.Parent;
            if (parent != null)
            {
                NavigateTo(parent);
                btnRegresar.Enabled = _currentDirectory.Parent != null;
            }
        }

        private void ListView1_DoubleClick(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;

            var sel = listView1.SelectedItems[0];
            if (sel.Tag is DirectoryInfo dir)
            {
                // navegar a la carpeta
                NavigateTo(dir);
                btnRegresar.Enabled = _currentDirectory.Parent != null;
            }
            else if (sel.Tag is FileInfo file)
            {
                try
                {
                    // Abrir con la aplicación por defecto
                    var psi = new ProcessStartInfo(file.FullName)
                    {
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo abrir el archivo:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ListView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                panelDetalles.Visible = false;
                return;
            }

            var sel = listView1.SelectedItems[0];
            panelDetalles.Visible = true;

            if (sel.Tag is DirectoryInfo dir)
            {
                MostrarDetallesCarpeta(dir);
            }
            else if (sel.Tag is FileInfo file)
            {
                MostrarDetallesArchivo(file);
            }
        }

        private void MostrarDetallesCarpeta(DirectoryInfo dir)
        {
            lblNombreCompleto.Text = $"Nombre: {dir.FullName}";
            lblTipo.Text = "Tipo: Carpeta";
            lblExtension.Text = "Extensión: N/A";

            try
            {
                var subdirs = dir.GetDirectories().Length;
                var files = dir.GetFiles().Length;
                lblCantidad.Text = $"Contenido: {subdirs} carpeta(s), {files} archivo(s)";

                // Calcular tamaño total (puede ser lento en carpetas grandes)
                long totalSize = 0;
                try
                {
                    totalSize = CalcularTamanioCarpeta(dir);
                    lblTamanio.Text = $"Tamaño: {FormatSize(totalSize)}";
                }
                catch
                {
                    lblTamanio.Text = "Tamaño: No disponible";
                }
            }
            catch (UnauthorizedAccessException)
            {
                lblCantidad.Text = "Contenido: Acceso denegado";
                lblTamanio.Text = "Tamaño: No disponible";
            }
        }

        private void MostrarDetallesArchivo(FileInfo file)
        {
            lblNombreCompleto.Text = $"Nombre: {file.FullName}";
            lblTipo.Text = $"Tipo: {GetTypeDescription(file.FullName) ?? "Archivo"}";
            lblExtension.Text = $"Extensión: {(string.IsNullOrEmpty(file.Extension) ? "Sin extensión" : file.Extension)}";
            lblTamanio.Text = $"Tamaño: {FormatSize(file.Length)}";
            lblCantidad.Text = ""; // No aplica para archivos
        }

        private long CalcularTamanioCarpeta(DirectoryInfo dir)
        {
            long size = 0;
            try
            {
                foreach (var file in dir.GetFiles())
                {
                    size += file.Length;
                }

                foreach (var subdir in dir.GetDirectories())
                {
                    size += CalcularTamanioCarpeta(subdir);
                }
            }
            catch
            {
                // Ignorar errores de acceso
            }
            return size;
        }

        private void NavigateTo(DirectoryInfo directory)
        {
            _currentDirectory = directory;
            txtRutaInicial.Text = directory.FullName;
            PopulateListView(directory);
        }

        private void PopulateListView(DirectoryInfo directory)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            _iconIndexCache.Clear();
            _smallIcons.Images.Clear();

            try
            {
                // Primero carpetas
                foreach (var dir in directory.GetDirectories())
                {
                    var lvi = new ListViewItem(dir.Name) { Tag = dir };
                    lvi.SubItems.Add("Carpeta");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add(dir.LastWriteTime.ToString("g"));
                    lvi.ImageIndex = EnsureIconIndexForPath(dir.FullName, true);
                    listView1.Items.Add(lvi);
                }

                // Luego archivos
                foreach (var file in directory.GetFiles())
                {
                    var lvi = new ListViewItem(file.Name) { Tag = file };
                    lvi.SubItems.Add(GetTypeDescription(file.FullName) ?? file.Extension);
                    lvi.SubItems.Add(FormatSize(file.Length));
                    lvi.SubItems.Add(file.LastWriteTime.ToString("g"));
                    lvi.ImageIndex = EnsureIconIndexForPath(file.FullName, false);
                    listView1.Items.Add(lvi);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Acceso denegado a alguna entrada dentro de la carpeta.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private int EnsureIconIndexForPath(string path, bool isDirectory)
        {
            var key = isDirectory ? "__FOLDER__" : Path.GetExtension(path).ToLowerInvariant();
            if (string.IsNullOrEmpty(key)) key = path.ToLowerInvariant();

            if (_iconIndexCache.TryGetValue(key, out var idx))
                return idx;

            var icon = GetSystemIcon(path, isDirectory, small: true) ?? SystemIcons.WinLogo;
            _smallIcons.Images.Add(icon);
            idx = _smallIcons.Images.Count - 1;
            _iconIndexCache[key] = idx;
            return idx;
        }

        private static string FormatSize(long bytes)
        {
            const long kilo = 1024;
            const long mega = kilo * 1024;
            const long giga = mega * 1024;

            if (bytes >= giga) return $"{bytes / (double)giga:0.##} GB";
            if (bytes >= mega) return $"{bytes / (double)mega:0.##} MB";
            if (bytes >= kilo) return $"{bytes / (double)kilo:0.##} KB";
            return $"{bytes} B";
        }

        #region Interop para iconos y tipo

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_SMALLICON = 0x000000001;
        private const uint SHGFI_TYPENAME = 0x000000400;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        private static Icon? GetSystemIcon(string path, bool isDirectory, bool small)
        {
            try
            {
                var shfi = new SHFILEINFO();
                var flags = SHGFI_ICON | (small ? SHGFI_SMALLICON : 0) | SHGFI_USEFILEATTRIBUTES;
                var attr = isDirectory ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;
                var res = SHGetFileInfo(path, attr, ref shfi, (uint)Marshal.SizeOf(shfi), flags);
                if (shfi.hIcon == IntPtr.Zero) return null;
                var icon = Icon.FromHandle(shfi.hIcon).Clone() as Icon;
                DestroyIcon(shfi.hIcon);
                return icon;
            }
            catch
            {
                return null;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        private static string? GetTypeDescription(string path)
        {
            try
            {
                var shfi = new SHFILEINFO();
                var flags = SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES;
                var attr = Path.HasExtension(path) ? FILE_ATTRIBUTE_NORMAL : FILE_ATTRIBUTE_DIRECTORY;
                SHGetFileInfo(path, attr, ref shfi, (uint)Marshal.SizeOf(shfi), flags);
                return string.IsNullOrWhiteSpace(shfi.szTypeName) ? null : shfi.szTypeName;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}