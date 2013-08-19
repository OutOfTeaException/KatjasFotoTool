using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using KatjasFotoTool.Model;
using KatjasFotoTool.Service;
using KatjasFotoTool.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace KatjasFotoTool.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private const string TITLE = "Katjas Foto Tool";

        private List<PhotoInfo> allPhotos;
        private List<PhotoInfoViewModel> photos;
        
        private BackgroundWorker workerLoadPhotos;
        private BackgroundWorker workerSavePhotos;
        
        public ObservableCollection<PhotoInfoViewModel> Photos { get; private set; }
        public List<DateTime> Dates { get; private set; }
        
        public RelayCommand LoadPhotosCommand { get; private set; }
        public RelayCommand SetNameCommand { get; private set; }
        public RelayCommand SavePhotosCommand { get; private set; }
        public RelayCommand<PhotoInfoViewModel> ShowPhotoCommand { get; private set; }
        public RelayCommand<KeyEventArgs> NameTextBoxKeyDownCommand { get; private set; }

        private PhotosortiererService photoService;
        private bool canLoadPhotos = true;
        private bool canSavePhotos = false;
        private string directory;
        private int autoSelectedIndex = -1;
        
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    ShowPhotosAtCurrentDate();
                }
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    RaisePropertyChanged("IsBusy");
                }
            }
        }
        
        

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}


            photoService = new PhotosortiererService();

            LoadPhotosCommand = new RelayCommand(LoadPhotos, () => canLoadPhotos);
            SetNameCommand = new RelayCommand(SetName);
            SavePhotosCommand = new RelayCommand(SavePhotos, () => canSavePhotos);
            ShowPhotoCommand = new RelayCommand<PhotoInfoViewModel>(ShowPhoto);
            NameTextBoxKeyDownCommand = new RelayCommand<KeyEventArgs>(HandleNameTextBoxKeyDown);

            
            workerLoadPhotos = new BackgroundWorker();
            workerLoadPhotos.DoWork += workerLoadPhotos_DoWork;
            workerLoadPhotos.RunWorkerCompleted += workerLoadPhotos_RunWorkerCompleted;

            workerSavePhotos = new BackgroundWorker();
            workerSavePhotos.DoWork += workerSavePhotos_DoWork;
            workerSavePhotos.RunWorkerCompleted += workerSavePhotos_RunWorkerCompleted;
        }

        private void HandleNameTextBoxKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SetName();
            }
            else if (e.Key == Key.Down)
            {
                ExtendPhotoSelection();
            }
            else if (e.Key == Key.Up)
            {
                ReducePhotoSelection();
            }
        }

        private void ExtendPhotoSelection()
        {
            if (autoSelectedIndex == -1)
                return;

            int i = autoSelectedIndex + 1;

            while (i < photos.Count)
            {
                //TODO: RangeCheck
                var photoVm = photos[i];

                if (!photoVm.IsSelected)
                {
                    photoVm.IsSelected = true;
                    return;
                }

                i++;
            }
        }
        
        private void ReducePhotoSelection()
        {
            if (autoSelectedIndex == -1)
                return;

            int i = autoSelectedIndex + 2;

            while (i < photos.Count)
            {
                //TODO: RangeCheck
                var photoVm = photos[i];

                if (!photoVm.IsSelected)
                {
                    photos[i-1].IsSelected = false;
                    return;
                }

                i++;
            }

            photos[i-1].IsSelected = false;
        }

        private void SelectNextPhoto()
        {

            for (int i = autoSelectedIndex; i < photos.Count; i ++)
            {
                if (!photos[i].IsSelected)
                {
                    photos[i].IsSelected = true;
                    autoSelectedIndex = i;
                    return;
                }

                if (photos[i].IsSelected)
                    photos[i].IsSelected = false;
            }
        }

        void workerSavePhotos_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            canSavePhotos = true;

            if (e.Error != null)
            {
                Messenger.Default.Send(new ShowMessageBoxMessage("Huups, bei Speichern ist ein Fehler aufgetreten... :(   :" + e.Error.Message, true));
            }
            else
            {
                Messenger.Default.Send(new ShowMessageBoxMessage("Super! Die Fotos wurden sortiert und umbenannt! :-)"));
                Process.Start((string)e.Result);
            }
        }

        void workerSavePhotos_DoWork(object sender, DoWorkEventArgs e)
        {
            string dir = Path.Combine(directory, "NEU");
            photoService.SavePhotos(allPhotos, dir);

            e.Result = dir;
        }

        private void ShowPhoto(PhotoInfoViewModel photoInfo)
        {
            if (photoInfo == null)
                return;

            Messenger.Default.Send(new ShowPhotoViewMessage(photoInfo.PhotoInfo));
        }

        void workerLoadPhotos_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            canLoadPhotos = true;
            IsBusy = false;
            
            if (e.Error != null)
            {
                Messenger.Default.Send(new ShowMessageBoxMessage("Hoppalla, beim Laden der Fotos ist ein Fehler aufgetreten: " + e.Error.Message, true));
                return;
            }

            allPhotos = (List<PhotoInfo>)e.Result;

            if (allPhotos.Count == 0)
            {
                Messenger.Default.Send(new ShowMessageBoxMessage("Hmmm, ich habe keine Fotos in dem ausgeählten Verzeichnis gefunden...", true));
                return;
            }

            Dates = allPhotos.Select(p => p.DateTaken.Date).Distinct().OrderBy(d => d).ToList();

            RaisePropertyChanged("Dates");
            // Damit werden die Fotos automatisch angezeigt
            SelectedDate = Dates.First();
            RaisePropertyChanged("SelectedDate");
            canSavePhotos = true;
        }

        void workerLoadPhotos_DoWork(object sender, DoWorkEventArgs e)
        {
            var photos = photoService.LoadPhotos(directory).ToList();
            e.Result = photos;
        }
        
        private void SetName()
        {
            if (Photos == null)
                return;

            if (String.IsNullOrEmpty(Name))
                Name = "";

            if (Name.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
            {
                Messenger.Default.Send(new ShowMessageBoxMessage("Nanana! Folgende Zeichen darfst du nicht für den Namen benutzen:" + Environment.NewLine + "\" < > | : * ? \\ /", true));
                return;
            }

            foreach (var photo in Photos)
            {
                if (photo.IsSelected)
                {
                    photo.Name = Name;
                }
            }

            SelectNextPhoto();
        }

        public void LoadPhotos()
        {
            //directory = GetPhotosLoadFolder();
            directory = @"c:\Users\Timo\Pictures\Fotos\Rom\Ich\";

            if (directory != null)
            {
                IsBusy = true;
                canLoadPhotos = false;
                workerLoadPhotos.RunWorkerAsync();
            }
        }

        private string GetPhotosLoadFolder()
        {
            // Hm, wie mach ich das denn quasi synchron???
            Messenger.Default.Send(new ShowFolderDialogMessage("Jo, bitte den Ordner mit den Fotos auswählen!",
                s =>
                {
                    return s;
                }));
        }

        private void ShowPhotosAtCurrentDate()
        {
            photos = allPhotos.Where(p => p.DateTaken.Date == SelectedDate && p.IsPhoto).Select(p => new PhotoInfoViewModel(p)).ToList();
            Photos = new ObservableCollection<PhotoInfoViewModel>(photos);
            RaisePropertyChanged("Photos");

            if (photos.Any())
            {

                Photos.First().IsSelected = true;
                autoSelectedIndex = 0;
            }
            else
            {
                autoSelectedIndex = -1;
            }
        }

        private void SavePhotos()
        {
            canSavePhotos = false;
            IsBusy = true;
            workerSavePhotos.RunWorkerAsync();
        }

        public void RotatePhotos()
        {

        }
    }
}