using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using Number_Renamer.Models;
using Number_Renamer.Command;
using Number_Renamer.Events;

namespace Number_Renamer.ViewModels
{
    class FileViewModel : ViewModelBase
    {
        #region Constructor
        public FileViewModel()
        {
            _files = new ObservableCollection<FileModel>();
        }
        #endregion

        #region Fields
        private string _first;
        private string _last;           
        private int _beginningNumber;
        private ObservableCollection<FileModel> _files;
        #endregion

        #region Properties
        public event EventHandler<MessageEventArgs> DisplayAlert;
        public ObservableCollection<FileModel> Files => _files;
        public int BeginningNumber
        {
            get => _beginningNumber;
            set
            {
                _beginningNumber = value;
                OnPropertyChanged();
            }
            
        }
        public string First
        {
            get => _first;
            set
            {
                _first = value;
                OnPropertyChanged();
            }
        }
        public string Last
        {
            get => _last;
            set
            {
                _last = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand ChooseFiles => new RelayCommand(Choose);
        public ICommand Delete => new RelayCommand(RemoveFiles, CanRemoveFile);
        public ICommand Rename => new RelayCommand(RenameFiles, CanRenameFiles);
        #endregion

        #region Methods
        private void Choose(object extra)
        {
            var OpenDialog = new OpenFileDialog();
            OpenDialog.Multiselect = true;
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                if (_files.Any(x => x.FilePath == OpenDialog.FileName))
                {
                    DisplayAlert?.Invoke(this, new MessageEventArgs { Message = "A File of the same type already exists!" });
                    return;
                }    
                foreach(var FileSelected in OpenDialog.FileNames)
                {
                    _files.Add(new FileModel { Name = Path.GetFileName(FileSelected), FilePath = FileSelected });
                }
               
            }
        }
        private bool CanRemoveFile()
        {
            if (_files.Count <= 0)
                return false;

            return true;
        }
        private void RemoveFiles(object FileList)
        {
            _files.Clear();
        }

        private bool CanRenameFiles()
        {
            if ( _files.Count <= 0)
                return false;

            return true;
        }

        private async void RenameFiles(object extra)
        {
            
            var desktopfolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\ReNamedFiles";
            if (!Directory.Exists(desktopfolder))
            {
                Directory.CreateDirectory(desktopfolder);
            }
            await Task.Run(() =>
            {
                for (int i = BeginningNumber; i < _files.Count + BeginningNumber; i++)
                {
                    File.Copy($"{_files[i - BeginningNumber].FilePath}", 
                    $"{desktopfolder}\\{_first}{i}{_last}" +
                    $"{Path.GetExtension(_files[i - BeginningNumber].FilePath)}", true);
                }
            });
            DisplayAlert?.Invoke(this, new MessageEventArgs { Message = "Completed. Check your desktop for a folder." });
            _files.Clear();

        }
        #endregion
    }
}
