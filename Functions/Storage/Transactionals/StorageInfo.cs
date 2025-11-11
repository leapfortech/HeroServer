using System;

namespace HeroServer
{
    public class StorageInfo
    {
        public String ContainerName { get; set; }
        public String FileName { get; set; }
        public String FileExt { get; set; }
        public String Content { get; set; }
        public int Backup { get; set; }

        public StorageInfo()
        {
        }

        public StorageInfo(String containerName, String fileName, String fileExt, String content, int backup)
        {
            ContainerName = containerName;
            FileName = fileName;
            FileExt = fileExt;
            Content = content;
            Backup = backup;
        }
    }
}
