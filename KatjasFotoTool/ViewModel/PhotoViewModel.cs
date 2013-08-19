using GalaSoft.MvvmLight;
using KatjasFotoTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KatjasFotoTool.ViewModel
{
    public class PhotoViewModel : ViewModelBase
    {
        public string PhotoUrl { get; private set; }
        public string Title { get; private set; }

        public PhotoViewModel(PhotoInfo photoInfo)
        {
            PhotoUrl = photoInfo.Datei;
            Title = photoInfo.Name;
        }
     }
}
