using AppLogger.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    class Dumper
    {
        private static string DUMP_DIR = "./dumps";
        private string _filePathLogs;
        private string _filePathStatuses;

        public Dumper(string ServiceName)
        {
            Directory.CreateDirectory(DUMP_DIR);
            var filePath = DUMP_DIR + $"/{ServiceName.Replace(' ', '_')}";
            _filePathLogs = filePath + "_logs.dump";
            _filePathStatuses = filePath + "_statuses.dump";
        }

        public void DumpLogs(string SerializedLogs)
        {
            DumpStr(_filePathLogs, SerializedLogs);
        }

        public void DumpStatus(string SerializedStatuses)
        {
            DumpStr(_filePathStatuses, SerializedStatuses);
        }

        private void DumpStr(string path, string value)
        {
            FileLocker.Instance.LockAction(path, () => File.AppendAllText(path, value + Environment.NewLine));
        }

        public void retriveLogs(Action<int, string> processLog)
        {
            retriveStringsFromFile(_filePathLogs, processLog);
        }
    
        public void retriveStatus(Action<int, string> processStatus)
        {
            retriveStringsFromFile(_filePathStatuses, processStatus);
        }

        public void retriveStringsFromFile(string path, Action<int, string> processString)
        {
            FileLocker.Instance.LockAction(path, () => {
                if (!File.Exists(path)) return;
                var num = 1;
                foreach (var str in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(str)) continue;
                    try
                    {
                        processString(num++, str);
                    }
                    //Если в процессе восстановления данных из дампа сервер снова перестал отвечать, то тут уже сдаемся и просто теряем данные. ИСПРАИВТЬ!
                    //Ситуация воспроизводима с маленькой вероятностью
                    catch (Exception) { } //FIX ME
                }
                File.Delete(path); //тут возможен очень гадкий и хитрый баг, если есть права на чтение файла, но нет прав на запись. преполагаю, что он невоспроизводим в реальности
            });
        }

    }
}
