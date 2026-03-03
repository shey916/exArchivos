using System;
using System.IO;

namespace exArchivos.Services
{
    public class DirectoryNavigator
    {
        private DirectoryInfo? _currentDirectory;

        public DirectoryInfo? CurrentDirectory => _currentDirectory;

        public bool CanNavigateUp => _currentDirectory?.Parent != null;

        public event EventHandler<DirectoryChangedEventArgs>? DirectoryChanged;

        public void NavigateTo(DirectoryInfo directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));

            _currentDirectory = directory;
            OnDirectoryChanged(new DirectoryChangedEventArgs(directory));
        }

        public bool TryNavigateUp()
        {
            if (!CanNavigateUp)
                return false;

            NavigateTo(_currentDirectory!.Parent!);
            return true;
        }

        protected virtual void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }
    }

    public class DirectoryChangedEventArgs : EventArgs
    {
        public DirectoryInfo Directory { get; }

        public DirectoryChangedEventArgs(DirectoryInfo directory)
        {
            Directory = directory ?? throw new ArgumentNullException(nameof(directory));
        }
    }
}