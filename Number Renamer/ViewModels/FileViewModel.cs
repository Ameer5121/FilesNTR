using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
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
                SetPropertyValue(ref _beginningNumber, value);
            }
            
        }
        public string First
        {
            get => _first;
            set
            {
                if (IsRunning())
                    return;
                SetPropertyValue(ref _first, value);
            }
        }
        public string Last
        {
            get => _last;
            set
            {
                if (IsRunning())
                    return;
                SetPropertyValue(ref _last, value);
            }
        }
        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                SetPropertyValue(ref _visibility, value);
            }
        }
        public decimal Progress
        {
            get => _progress;
            set
            {
                SetPropertyValue(ref _progress, value);
            }
        }
        #endregion

        #region Commands
        public ICommand ChooseFiles => new RelayCommand(Choose, CanChoose);
        public ICommand Delete => new RelayCommand(RemoveFiles, CanRemoveFile);
        public ICommand Rename => new RelayCommand(SelectFolder, CanRenameFiles);
        #endregion

        #region Methods

        private bool CanChoose()
        {
            if (Visibility == Visibility.Visible)
                return false;
            return true;
        }

        private void Choose()
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
        private void RemoveFiles()
        {
            _files.Clear();
        }

        private bool CanRenameFiles()
        {
            if ( _files.Count <= 0)
                return false;

            return true;
        }

        private async Task SelectFolder()
        {
            var OpenDialog = new FolderBrowserDialog();
            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                await RenameFiles(OpenDialog.SelectedPath);
            }
        }

        private async Task RenameFiles(string FolderPath)
        {                      
            Visibility = Visibility.Visible;
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }           

            try
            {
                int i = BeginningNumber;
                await Task.Run(() =>
                {
                    Parallel.For(i, _files.Count + BeginningNumber, current =>
                    {
                        File.Copy($"{_files[current - BeginningNumber].FilePath}",
                        $"{FolderPath}\\{_first}{current}{_last}" +
                        $"{Path.GetExtension(_files[current - BeginningNumber].FilePath)}", true);
                        UpdateProgress((decimal)100 / _files.Count);
                    });
                });
            }
            catch (Exception e)
            {
                DisplayAlert?.Invoke(this, new MessageEventArgs { Message = e.Message });              
            }          
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
            DisplayAlert?.Invoke(this, new MessageEventArgs { Message = "Completed. Check the folder that you have specified." });
        }
        #endregion
    }
}
