using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Number_Renamer.ViewModels;
using Number_Renamer.Events;

namespace Number_Renamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new FileViewModel();
            (DataContext as FileViewModel).DisplayAlert += Alert;
        }

        private void Alert(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) { this.DragMove(); }

        private void Exit(object sender, RoutedEventArgs e) { Application.Current.Shutdown(); }

    }
}
