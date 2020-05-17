using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Persistence
{
    public abstract class AbstractGamePersister<T>
    {
        private string persisterFolderName;
        private string fileExtension;
        private string fileName;

        private BinaryFormatter binaryFormatter;

        private string folderPath;

        protected AbstractGamePersister(string persisterFolderName, string fileExtension, string fileName)
        {
            this.binaryFormatter = new BinaryFormatter()
            {
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };

            this.persisterFolderName = persisterFolderName;
            this.fileExtension = fileExtension;
            this.fileName = fileName;
            folderPath = Path.Combine(Application.persistentDataPath, persisterFolderName);
        }


        public T Load()
        {
            var path = this.GetDataPath();
            return PersistanceManager.Get().Load<T>(folderPath, path, this.fileName, this.fileExtension);
        }

        public void SaveAsync(T dataToSave)
        {
            PersistanceManager.Get().OnPersistRequested(() => { this.SaveSync(dataToSave); });
        }

        public bool SaveSync(T dataToSave)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);


            using (FileStream fileStream = File.Open(GetDataPath(), FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, dataToSave);
                return true;
            }
        }

        private string GetDataPath()
        {
            return GetDataPath(this.folderPath, this.fileName, this.fileExtension);
        }

        public static string GetDataPath(string folderPath, string fileName, string fileExtension)
        {
            return Path.Combine(folderPath, fileName + fileExtension);
        }
    }
}