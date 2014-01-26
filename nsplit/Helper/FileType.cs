using System.Net.Http.Headers;
using System.Net.Mime;

namespace nsplit.Helper
{
    public class FileType
    {
        public static FileType Html = new FileType(".html", MediaTypeNames.Text.Html);
        public static FileType Javascript = new FileType(".js", "text/javascript");
        public static FileType Css = new FileType(".css", MediaTypeNames.Text.Plain);

        
        private readonly string m_Extension;
        private readonly string m_MediaType;

        private FileType(string extension, string mediaType)
        {
            m_Extension = extension;
            m_MediaType = mediaType;
        }

        public string Extension
        {
            get
            {
                return m_Extension;
            }
        }

        public MediaTypeHeaderValue ContentType
        {
            get
            {
                return new MediaTypeHeaderValue(m_MediaType);
            }
        }
    }
}