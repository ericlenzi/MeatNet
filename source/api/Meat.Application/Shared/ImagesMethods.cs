using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Meat.Application.Shared.Settings;
using System;
using System.IO;
using System.Text;

namespace Meat.Application.Shared
{
    public class ImagesMethods
    {
        private readonly Directories Directories;

        public ImagesMethods(IOptions<Directories> directories)
        {
            Directories = directories.Value;
        }

        private string DirectoryImages
        {
            get
            {
                var path = Directories.Images;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
        }

        /// <summary>
        /// Guarda una imágen en Disco
        /// </summary>
        /// <param name="imageBase64"></param>
        /// <param name="name"></param>
        public void Saveimage(string imageBase64, string name)
        {
            name += ".jpg";
            var pathImage = Path.Combine(DirectoryImages, name);

            if (File.Exists(pathImage))
                File.Delete(pathImage);

            string base64 = imageBase64.Split(',')[1];
            byte[] bitmapData = new byte[base64.Length];
            bitmapData = Convert.FromBase64String(FixBase64ForFile(base64));
            File.WriteAllBytes(pathImage, bitmapData);
        }

        private string FixBase64ForFile(string image)
        {
            StringBuilder sbText = new StringBuilder(image, image.Length);
            sbText.Replace("\r\n", String.Empty);
            sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }
    }
}