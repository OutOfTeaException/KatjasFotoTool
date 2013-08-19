using GalaSoft.MvvmLight;
using KatjasFotoTool.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace KatjasFotoTool.ViewModel
{
    public class PhotoInfoViewModel : ViewModelBase
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected;  }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        public PhotoInfo PhotoInfo { get; private set; }
        
        public string Name
        {
            get { return PhotoInfo.Name;  }
            set
            {
                if (PhotoInfo.Name != value)
                {
                    PhotoInfo.Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public PhotoInfoViewModel(PhotoInfo photoInfo)
        {
            this.PhotoInfo = photoInfo;
        }
    }
}
