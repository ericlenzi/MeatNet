using System;
using System.IO;
using System.Threading.Tasks;

namespace Meat.Infrastructure.Logger
{
    public class LoggerService
    {
        private readonly string LogFilePath;

        public LoggerService(string apiName)
        {
            var fileName = $"{apiName}_Log.txt";
            LogFilePath = Path.Combine(Path.GetTempPath(), fileName);

            if (!File.Exists(LogFilePath))
            {
                using StreamWriter writer = new StreamWriter(LogFilePath, true);
                writer.WriteLine($"Starting logging at {DateTime.Now}");
                writer.Close();
            }

            if (File.Exists(LogFilePath))
            {
                long lenght = new FileInfo(LogFilePath).Length;

                //Si el archivo supera 1MB
                if (lenght >= 1048576)
                {
                    //Se borran los archivos de logs anteriores
                    DeleteOldFiles();
                    //Se renombra el archivo de log lleno
                    File.Move(LogFilePath, LogFilePath + ".old");
                    //Se crea un nuevo archivo de log para escribir
                    using StreamWriter writer = new StreamWriter(LogFilePath, true);
                    writer.WriteLine("Starting logging at " + DateTime.Now.ToString());
                }
            }
        }

        private void DeleteOldFiles()
        {
            string DeleteThis = ".old";
            string[] Files = Directory.GetFiles(LogFilePath);

            foreach (string file in Files)
            {
                if (file.ToUpper().Contains(DeleteThis.ToUpper()))
                {
                    File.Delete(file);
                }
            }
        }

        public void Log(string message)
        {
            Retry(() => PerformFileProcessing(message));
        }

        private void PerformFileProcessing(string message)
        {
            if (File.Exists(LogFilePath))
            {
                using StreamWriter writer = File.AppendText(LogFilePath);
                writer.WriteLine(DateTime.Now.ToString() + " : " + message);
                writer.Close();
            }
        }

        private void Retry(Action action)
        {
            int retries = 0;
            while (retries < 5)
            {
                try
                {
                    action();
                    return;
                }
                catch (IOException)
                {
                    retries++;
                    Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}