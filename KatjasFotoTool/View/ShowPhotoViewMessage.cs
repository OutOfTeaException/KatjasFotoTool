using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KatjasFotoTool.Model;

namespace KatjasFotoTool.View
{
    public class ShowPhotoViewMessage
    {
        public PhotoInfo PhotoInfo { get; set; }

        public ShowPhotoViewMessage(PhotoInfo photoInfo)
        {
            PhotoInfo = photoInfo;
        }
    }
}
