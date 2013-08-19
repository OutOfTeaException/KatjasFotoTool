using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KatjasFotoTool.Model
{
    public class PhotoInfo
    {
        public enum PhotoOrientation
        {
            Unknown = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4,
            LeftTop = 5,
            RightTop = 6,
            RightBottom = 7,
            LeftBottom = 8
        }

        public bool IsPhoto
        {
            get { return Extension == ".jpg" || Extension == ".jpeg"; }
        }

        public string Datei { get; set; }
        public DateTime DateTaken { get; set; }
        public PhotoOrientation Orientation { get; set; }
        public string OriginalName { get; set; }
        public Image Thumbnail { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
    }
}
