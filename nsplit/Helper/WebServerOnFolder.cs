using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace nsplit.Helper
{
    internal class WebServerOnFolder : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, bool> m_Matches;
        private readonly string m_BaseFolder;
        private readonly Dictionary<string, FileType> m_FileTypes;

        public WebServerOnFolder(Func<HttpRequestMessage, bool> matches, string folderPath, params FileType[] allowedFileTypes)
        {
            m_Matches = matches;
            m_BaseFolder = folderPath;
            m_FileTypes = new Dictionary<string, FileType>();

            foreach (var fileType in allowedFileTypes)
            {
                m_FileTypes.Add(fileType.Extension, fileType);
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return 
                !m_Matches(request)
                    ? base.SendAsync(request, cancellationToken)
                    : Task<HttpResponseMessage>
                        .Factory
                        .StartNew(
                            () => Response(request),
                            cancellationToken);
        }

        private HttpResponseMessage Response(HttpRequestMessage request)
        {
            const string defaultPageName = "index.html";
            var path = request.RequestUri.AbsolutePath;
            var suffix = path == string.Empty ? defaultPageName : path.Substring(1);
            var fullPath = Path.Combine(m_BaseFolder, suffix);

            if (!File.Exists(fullPath))
            {
                return request.CreateErrorResponse(
                    HttpStatusCode.NotFound, 
                    string.Format("Sorry about that, but there is no file named '{0}' here.", suffix));
            }

            string extension = Path.GetExtension(path);
            FileType fileType;
            if (!m_FileTypes.TryGetValue(extension, out fileType))
            {
                return request.CreateErrorResponse(
                    HttpStatusCode.UnsupportedMediaType, 
                    string.Format("Sorry I can not process files like '{0}'.", extension));
            }

            var response = request.CreateResponse();
            var fileStream = new FileStream(fullPath, FileMode.Open);
            response.Content = new StreamContent(fileStream);
            response.Content.Headers.ContentType = fileType.ContentType;
            return response;
        }
    }
}