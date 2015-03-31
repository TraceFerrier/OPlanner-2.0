using System.IO;
using System.Windows.Media.Imaging;

namespace PlannerNameSpace
{
    public class FileUtils
    {
        //------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a full path to a temp file suitable for serializing and unserializing
        /// schedule definition data.
        /// </summary>
        //------------------------------------------------------------------------------------
        public static string GetFullPathToTempFile(string fileName)
        {
            string fullpath = Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            DirectoryInfo di = Directory.CreateDirectory(fullpath);
            return Path.Combine(fullpath, fileName);
        }

        public static BitmapSource GetBitmapSourceFromStream(Stream stream)
        {
            JpegBitmapDecoder decoder = new JpegBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            BitmapSource source = decoder.Frames[0];
            source.Freeze();
            return source;

        }

    }
}
