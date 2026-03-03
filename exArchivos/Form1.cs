using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using exArchivos.Services;

namespace exArchivos
{
    public partial class Form1 : Form
    {
        private readonly SystemIconProvider _iconProvider;
        private readonly DirectoryNavigator _navigator;

        private const string SelectFolderMessage = "Seleccione primero una carpeta con 'Seleccionar carpeta...'";
        private const string AccessDeniedMessage = "No tiene permiso para listar algunas carpetas.";
        private const string CannotOpenFileMessage = "No se pudo abrir el archivo:\r\n{0}";
        private const string SelectFolderTitle = "Seleccione una carpeta raíz";
        private const string ErrorTitle = "Error";
        private const string WarningTitle = "Aviso";

        private const int ColumnNameWidth = 350;
        private const int ColumnTypeWidth = 150;
        private const int ColumnSizeWidth = 100;
        private const int ColumnDateWidth = 160;

        public Form1()
        {
            InitializeComponent();

            _iconProvider = new SystemIconProvider(new ImageList());
            _navigator = new DirectoryNavigator();

            InitializeListView();
            SubscribeToEvents();
            InitializeControls();
        }

        private void InitializeListView()
        {
            listView1.SmallImageList = _iconProvider.ImageList;
            listView1.View = View.Details;
            listView1.FullRowSelect = true;

            ConfigureListViewColumns();
        }

        private void ConfigureListViewColumns()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add("Nombre", ColumnNameWidth);
            listView1.Columns.Add("Tipo", ColumnTypeWidth);
            listView1.Columns.Add("Tamaño", ColumnSizeWidth);
            listView1.Columns.Add("Última modificación", ColumnDateWidth);
        }

        private void SubscribeToEvents()
        {
            listView1.DoubleClick += OnListViewDoubleClick;
            listView1.SelectedIndexChanged += OnListViewSelectionChanged;
            btnSeleccionarCarpeta.Click += OnSelectFolderClick;
            btnRegresar.Click += OnNavigateBackClick;
            btnCargarSubcarpetas.Click += OnLoadSubfoldersClick;
            _navigator.DirectoryChanged += OnDirectoryChanged;
            FormClosing += OnFormClosing;
        }

        private void InitializeControls()
        {
            btnRegresar.Enabled = false;
            panelDetalles.Visible = false;
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs e)
        {
            _iconProvider?.Dispose();
        }

        private void OnSelectFolderClick(object? sender, EventArgs e)
        {
            var selectedPath = ShowFolderSelectionDialog();

            if (string.IsNullOrEmpty(selectedPath))
                return;

            NavigateToPath(selectedPath);
        }

        private string? ShowFolderSelectionDialog()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = SelectFolderTitle,
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            return dialog.ShowDialog() == DialogResult.OK
                ? dialog.SelectedPath
                : null;
        }

        private void NavigateToPath(string path)
        {
            txtRutaInicial.Text = path;
            _navigator.NavigateTo(new DirectoryInfo(path));
        }

        private void OnLoadSubfoldersClick(object? sender, EventArgs e)
        {
            if (!ValidateCurrentDirectory())
                return;

            LoadSubdirectories(_navigator.CurrentDirectory!);
        }

        private bool ValidateCurrentDirectory()
        {
            if (_navigator.CurrentDirectory != null)
                return true;

            MessageBox.Show(SelectFolderMessage, WarningTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void LoadSubdirectories(DirectoryInfo directory)
        {
            ClearListView();

            try
            {
                foreach (var subdirectory in directory.GetDirectories())
                {
                    AddDirectoryItem(subdirectory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                ShowAccessDeniedWarning();
            }
        }

        private void OnNavigateBackClick(object? sender, EventArgs e)
        {
            _navigator.TryNavigateUp();
        }

        private void OnListViewDoubleClick(object? sender, EventArgs e)
        {
            if (!TryGetSelectedItem(out var selectedItem))
                return;

            HandleItemDoubleClick(selectedItem);
        }

        private bool TryGetSelectedItem(out ListViewItem? item)
        {
            item = listView1.SelectedItems.Count > 0
                ? listView1.SelectedItems[0]
                : null;

            return item != null;
        }

        private void HandleItemDoubleClick(ListViewItem item)
        {
            switch (item.Tag)
            {
                case DirectoryInfo directory:
                    _navigator.NavigateTo(directory);
                    break;
                case FileInfo file:
                    OpenFile(file);
                    break;
            }
        }

        private void OpenFile(FileInfo file)
        {
            try
            {
                var startInfo = new ProcessStartInfo(file.FullName)
                {
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                ShowFileOpenError(ex.Message);
            }
        }

        private void OnListViewSelectionChanged(object? sender, EventArgs e)
        {
            if (!TryGetSelectedItem(out var selectedItem))
            {
                HideDetailsPanel();
                return;
            }

            ShowItemDetails(selectedItem!);
        }

        private void ShowItemDetails(ListViewItem item)
        {
            panelDetalles.Visible = true;

            switch (item.Tag)
            {
                case DirectoryInfo directory:
                    DisplayDirectoryDetails(directory);
                    break;
                case FileInfo file:
                    DisplayFileDetails(file);
                    break;
            }
        }

        private void DisplayDirectoryDetails(DirectoryInfo directory)
        {
            lblNombreCompleto.Text = $"Nombre: {directory.FullName}";
            lblTipo.Text = "Tipo: Carpeta";
            lblExtension.Text = "Extensión: N/A";

            try
            {
                var (subdirectoryCount, fileCount) = CountDirectoryContents(directory);
                lblCantidad.Text = $"Contenido: {subdirectoryCount} carpeta(s), {fileCount} archivo(s)";

                var totalSize = DirectorySizeCalculator.Calculate(directory);
                lblTamanio.Text = $"Tamaño: {FileSizeFormatter.Format(totalSize)}";
            }
            catch (UnauthorizedAccessException)
            {
                DisplayAccessDeniedDetails();
            }
        }

        private (int subdirectories, int files) CountDirectoryContents(DirectoryInfo directory)
        {
            var subdirectoryCount = directory.GetDirectories().Length;
            var fileCount = directory.GetFiles().Length;
            return (subdirectoryCount, fileCount);
        }

        private void DisplayFileDetails(FileInfo file)
        {
            lblNombreCompleto.Text = $"Nombre: {file.FullName}";
            lblTipo.Text = $"Tipo: {FileTypeProvider.GetTypeDescription(file.FullName)}";
            lblExtension.Text = $"Extensión: {GetFileExtensionDisplay(file)}";
            lblTamanio.Text = $"Tamaño: {FileSizeFormatter.Format(file.Length)}";
            lblCantidad.Text = string.Empty;
        }

        private static string GetFileExtensionDisplay(FileInfo file)
        {
            return string.IsNullOrEmpty(file.Extension)
                ? "Sin extensión"
                : file.Extension;
        }

        private void DisplayAccessDeniedDetails()
        {
            lblCantidad.Text = "Contenido: Acceso denegado";
            lblTamanio.Text = "Tamaño: No disponible";
        }

        private void OnDirectoryChanged(object? sender, DirectoryChangedEventArgs e)
        {
            txtRutaInicial.Text = e.Directory.FullName;
            PopulateListView(e.Directory);
            UpdateNavigationButtons();
        }

        private void UpdateNavigationButtons()
        {
            btnRegresar.Enabled = _navigator.CanNavigateUp;
        }

        private void PopulateListView(DirectoryInfo directory)
        {
            listView1.BeginUpdate();
            ClearListView();

            try
            {
                LoadDirectoriesIntoListView(directory);
                LoadFilesIntoListView(directory);
            }
            catch (UnauthorizedAccessException)
            {
                ShowAccessDeniedWarning();
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void LoadDirectoriesIntoListView(DirectoryInfo directory)
        {
            foreach (var subdirectory in directory.GetDirectories())
            {
                AddDirectoryItem(subdirectory);
            }
        }

        private void LoadFilesIntoListView(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                AddFileItem(file);
            }
        }

        private void AddDirectoryItem(DirectoryInfo directory)
        {
            var item = CreateListViewItem(directory.Name, directory);
            item.SubItems.Add("Carpeta");
            item.SubItems.Add(string.Empty);
            item.SubItems.Add(directory.LastWriteTime.ToString("g"));
            item.ImageIndex = _iconProvider.GetIconIndex(directory.FullName, isDirectory: true);

            listView1.Items.Add(item);
        }

        private void AddFileItem(FileInfo file)
        {
            var item = CreateListViewItem(file.Name, file);
            item.SubItems.Add(FileTypeProvider.GetTypeDescription(file.FullName));
            item.SubItems.Add(FileSizeFormatter.Format(file.Length));
            item.SubItems.Add(file.LastWriteTime.ToString("g"));
            item.ImageIndex = _iconProvider.GetIconIndex(file.FullName, isDirectory: false);

            listView1.Items.Add(item);
        }

        private static ListViewItem CreateListViewItem(string text, object tag)
        {
            return new ListViewItem(text) { Tag = tag };
        }

        private void ClearListView()
        {
            listView1.Items.Clear();
            _iconProvider.ClearCache();
        }

        private void HideDetailsPanel()
        {
            panelDetalles.Visible = false;
        }

        private void ShowAccessDeniedWarning()
        {
            MessageBox.Show(AccessDeniedMessage, WarningTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowFileOpenError(string errorMessage)
        {
            MessageBox.Show(string.Format(CannotOpenFileMessage, errorMessage),
                ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}