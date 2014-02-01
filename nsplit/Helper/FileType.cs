// This code is distributed under MIT license. 
// Copyright (c) 2014 George Mamaladze, Florian Greinacher
// See license.txt or http://opensource.org/licenses/mit-license.php

#region usings

using System.Net.Http.Headers;
using System.Net.Mime;

#endregion

namespace nsplit.Helper
{
    public class FileType
    {
        public static FileType Html = new FileType(".html", MediaTypeNames.Text.Html);
        public static FileType Javascript = new FileType(".js", "text/javascript");
        public static FileType Css = new FileType(".css", MediaTypeNames.Text.Plain);
        public static FileType Gif = new FileType(".gif", MediaTypeNames.Image.Gif);
        public static FileType Png = new FileType(".png", "image/png");
        public static FileType Json = new FileType(".json", MediaTypeNames.Text.Plain);


        private readonly string m_Extension;
        private readonly string m_MediaType;

        private FileType(string extension, string mediaType)
        {
            m_Extension = extension;
            m_MediaType = mediaType;
        }

        public string Extension
        {
            get { return m_Extension; }
        }

        public MediaTypeHeaderValue ContentType
        {
            get { return new MediaTypeHeaderValue(m_MediaType); }
        }
    }
}