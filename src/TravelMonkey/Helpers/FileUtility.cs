using System;
using System.IO;
using Xamarin.Forms;

namespace TravelMonkey.Helpers
{
    public class FileUtility
    {
        public string SaveFile(string folder, string fileName, byte[] fileBytes)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folder);

            // Check if the folder exist or not
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, fileName);

            // Try to write the file bytes to the specified location.
            try
            {
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string TemporarilySaveFile(string fileName, byte[] fileBytes)
        {
            var tmpdir = Path.GetTempPath();
            string folderPath = tmpdir;

            // Check if the folder exist or not
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = Path.Combine(folderPath, fileName);

            // Try to write the file bytes to the specified location.
            try
            {
                File.WriteAllBytes(filePath, fileBytes);
                return filePath;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public byte[] ReturnBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public void DeleteDirectory(string folder)
        {
            string imageFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folder);
            if (Directory.Exists(imageFolderPath))
            {
                Directory.Delete(imageFolderPath, true);
            }
        }

        public FileImageSource Image_Source(string fileName)
        {
            return new FileImageSource { File = new FileImageSource { File = fileName } };
        }

        public static FileUtility Instance { get; } = new FileUtility();
    }
}
