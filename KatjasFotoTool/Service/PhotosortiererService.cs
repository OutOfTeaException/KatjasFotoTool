using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using KatjasFotoTool.Model;
using System.Drawing.Imaging;

namespace KatjasFotoTool.Service
{
    class PhotosortiererService
    {
        private static CultureInfo DE = CultureInfo.GetCultureInfo("de-DE");
        
        public IEnumerable<PhotoInfo> LoadPhotos(string dir)
        {
            foreach (string datei in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
            {
                var info = new FileInfo(datei);
                string ext = info.Extension.ToLowerInvariant();

                PhotoInfo photoInfo;
                

                if (ext == ".jpg" || ext == ".jpeg")
                {
                    photoInfo = GetPhotoInfo(datei);
                    if (photoInfo.DateTaken == null)
                        photoInfo.DateTaken = Extensions.Min(info.CreationTime, info.LastWriteTime);
                }
                else if (ext == ".mov" || ext == ".avi" || ext == ".mp4")
                {
                    photoInfo = new PhotoInfo();
                    photoInfo.DateTaken = Extensions.Min(info.CreationTime, info.LastWriteTime);
                }
                else
                    continue;

                photoInfo.OriginalName = info.Name;
                photoInfo.Name = Path.GetFileNameWithoutExtension(info.Name);
                photoInfo.Datei = datei;
                photoInfo.Extension = ext;

                yield return photoInfo;
            }
        }

        private static void CorrectOrientation(string file, PhotoInfo.PhotoOrientation orientation)
        {
            using (var image = Image.FromFile(file))
            {
                if (orientation == PhotoInfo.PhotoOrientation.RightTop)
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else if (orientation == PhotoInfo.PhotoOrientation.LeftBottom)
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);

                image.Save(file, ImageFormat.Jpeg);
            }
        }

        public static PhotoInfo GetPhotoInfo(string path)
        {
            const int thumbnailHeight = 200;
            const int idDateTaken = 36867;
            const int idOrientation = 274;
            Regex r = new Regex(":");
            DateTime? dateTaken = null;
            PhotoInfo.PhotoOrientation orientation = PhotoInfo.PhotoOrientation.Unknown;
            Image thumbnail;


            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                double format = (double)myImage.Size.Width / (double)myImage.Size.Height;
                int thumbnailWidth = (int)(thumbnailHeight * format);
                thumbnail = myImage.GetThumbnailImage(thumbnailWidth, thumbnailHeight, null, IntPtr.Zero);

                if (myImage.PropertyIdList.Contains(idDateTaken))
                {
                    PropertyItem propDateTaken = myImage.GetPropertyItem(idDateTaken);
                    string dateTakenText = r.Replace(Encoding.UTF8.GetString(propDateTaken.Value), "-", 2);
                    dateTaken = DateTime.Parse(dateTakenText);
                }

                if (myImage.PropertyIdList.Contains(idOrientation))
                {
                    var propOrientation = myImage.GetPropertyItem(idOrientation);
                    orientation = (PhotoInfo.PhotoOrientation)propOrientation.Value[0];
                }
            }

            return new PhotoInfo()
            {
                DateTaken = dateTaken.Value,
                Orientation = orientation,
                Thumbnail = thumbnail
            };
        }

        public void SavePhotos(IEnumerable<PhotoInfo> photos, string dir)
        {
            foreach (var photo in photos)
            {
                string destDir = Path.Combine(dir, photo.DateTaken.ToString("yyyy-MM-dd", DE));

                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                string newFilename = String.Format("{0:yyyy-MM-dd HHmmss} {1}{2}", photo.DateTaken, photo.Name, photo.Extension);

                string newFile = Path.Combine(destDir, newFilename);
                if (File.Exists(newFile))
                {
                    int c = 1;
                    do
                    {
                        newFilename = String.Format("{0:yyyy-MM-dd HHmmss}_{2:00} {1}{2}", photo.DateTaken, photo.Name, photo.Extension, c);
                        c++;
                    }
                    while (File.Exists(newFilename));

                    newFile = Path.Combine(destDir, newFilename);
                }



                //if (alteFotosLoeschen)
                //    File.Move(datei, newFile);
                //else
                File.Copy(photo.Datei, newFile, true);
            }
        }
    }
}
