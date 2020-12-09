using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
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
            _visibility = Visibility.Collapsed;           
        }
        #endregion

        #region Fields
        private string _first;
        private string _last;           
        private int _beginningNumber;
        private ObservableCollection<FileModel> _files;
        private Visibility _visibility;
        private decimal _progress;
        #endregion

        #region Properties
        public event EventHandler<MessageEventArgs> DisplayAlert;
        public ObservableCollection<FileModel> Files => _files;
        public int BeginningNumber
        {
            get => _beginningNumber;
            set
            {
                if (IsRunning())
                    return;               
                _beginningNumber = value;
                OnPropertyChanged();
            }
            
        }
        public string First
        {
            get => _first;
            set
            {
                if (IsRunning())
                    return;
                _first = value;
                OnPropertyChanged();
            }
        }
        public string Last
        {
            get => _last;
            set
            {
                if (IsRunning())
                    return;
                _last = value;
                OnPropertyChanged();
            }
        }
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }
        public decimal Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand ChooseFiles => new RelayCommand(Choose, CanChoose);
        public ICommand Delete => new RelayCommand(RemoveFiles, CanRemoveFile);
        public ICommand Rename => new RelayCommand(RenameFiles, CanRenameFiles);
        #endregion

        #region Methods

        private bool CanChoose()
        {
            if (Visibility == Visibility.Visible)
                return false;
            return true;
        }

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
            if (_files.Count <= 0 || Visibility == Visibility.Visible)
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
            Visibility = Visibility.Visible;
            if (!Directory.Exists(desktopfolder))
            {
                Directory.CreateDirectory(desktopfolder);
            }
            await Task.Run(() =>
            {
                for (int i = BeginningNumber; i < _files.Count + BeginningNumber; i++)
                {
                    UpdateProgress((decimal)100 / _files.Count);
                    File.Copy($"{_files[i - BeginningNumber].FilePath}", 
                    $"{desktopfolder}\\{_first}{i}{_last}" +
                    $"{Path.GetExtension(_files[i - BeginningNumber].FilePath)}", true);
                }
            });          
            OnFinish();
        }

        private bool IsRunning() 
        {
            return Visibility == Visibility.Visible ? true : false;
        }

        private void UpdateProgress(decimal ValueToAdd) => Progress += ValueToAdd;

        private void OnFinish()
        {
            Progress = 0;
            Visibility = Visibility.Collapsed;
            _files.Clear();
            DisplayAlert?.Invoke(this, new MessageEventArgs { Message = "Completed. Check your desktop for a folder." });
        }
        #endregion
    }
}
