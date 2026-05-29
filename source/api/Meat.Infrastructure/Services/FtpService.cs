using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Meat.Infrastructure.Services
{
    public class FtpService : IFtpService
    {
        public string FtpServer { get; set; }
        public string FtpUser { get; set; }
        public string FtpPass { get; set; }
        public string FtpPath { get; set; }
        public string PathSyncMateriales { get; set; }

        public FtpService(IConfiguration configuration) 
        {
            FtpServer = configuration.GetValue<string>("FtpSAP:Server");
            FtpUser = configuration.GetValue<string>("FtpSAP:User");
            FtpPass = configuration.GetValue<string>("FtpSAP:Pass");
            FtpPath = configuration.GetValue<string>("FtpSAP:Path");
            PathSyncMateriales = configuration.GetValue<string>("FtpSAP:PathSyncMateriales");
        }

        public async Task<bool> Send(string filePath)
        {
            return await UploadFile(filePath);
        }

        public async Task<bool> SendSyncMateriales(string filePath)
        {
            return await UploadFile(filePath, this.PathSyncMateriales);
        }

        public async Task<bool> SendSyncVentas(string filePath)
        {
            return await UploadFile(filePath, this.FtpPath);
        }

        private async Task<bool> UploadFile(string filePath, string uploadPath = "")
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{FtpServer}{uploadPath}/{Path.GetFileName(filePath)}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(FtpUser, FtpPass);

            // Lee el contenido del archivo local
            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(filePath))
            {
                fileContents = System.Text.Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            // Establece el método de transferencia de datos
            request.ContentLength = fileContents.Length;

            // Obtiene el flujo de la solicitud para escribir los datos
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            // Obtiene la respuesta del servidor FTP
            using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            return response.StatusDescription.Substring(0, 3) == "226";//226 Transfer complete.
        }
    }
}
