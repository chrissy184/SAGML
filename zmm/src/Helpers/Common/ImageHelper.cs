using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ZMM.Helpers.Common
{
    public static class ImageHelper
    {
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}

