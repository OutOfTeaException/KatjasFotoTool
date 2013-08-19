using GalaSoft.MvvmLight.Messaging;
using KatjasFotoTool.View;
using KatjasFotoTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;

namespace KatjasFotoTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            Messenger.Default.Register<ShowPhotoViewMessage>(this, ShowPhotoView);
            Messenger.Default.Register<ShowMessageBoxMessage>(this, ShowMessageBox);
            Messenger.Default.Register<ShowFolderDialogMessage>(this, ShowFolderBrowser);
        }

        private void ShowPhotoView(ShowPhotoViewMessage msg)
        {
            PhotoView view = new PhotoView();
            view.DataContext = new PhotoViewModel(msg.PhotoInfo);
            view.ShowDialog();
        }

        private void ShowMessageBox(ShowMessageBoxMessage msg)
        {
            MessageBox.Show(this, msg.Text, "Katjas Foto Tool", MessageBoxButton.OK, msg.IsError ? MessageBoxImage.Error : MessageBoxImage.Information);
        }

        private void ShowFolderBrowser(ShowFolderDialogMessage msg)
        {
            var dlg = new Forms.FolderBrowserDialog();
            dlg.Description = msg.Text;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                msg.ResultCallback(dlg.SelectedPath);
            else
                msg.ResultCallback(null);
        }
    }
}
