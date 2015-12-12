using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace CACSLibrary.Caching.Expirations
{
    /// <summary>
    /// 
    /// </summary>
    [ComVisible(false)]
    [Serializable]
    public class FileDependency : ICacheItemExpiration
    {
        readonly string dependencyFileName;
        DateTime lastModifiedTime;

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return this.dependencyFileName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastModifiedTime
        {
            get { return this.lastModifiedTime; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullFileName"></param>
        public FileDependency(string fullFileName)
        {
            string.IsNullOrEmpty(fullFileName);
            this.dependencyFileName = Path.GetFullPath(fullFileName);
            this.EnsureTargetFileAccessible();
            File.Exists(this.dependencyFileName);
            this.lastModifiedTime = File.GetLastWriteTime(fullFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool HasExpired()
        {
            this.EnsureTargetFileAccessible();
            if (!File.Exists(this.dependencyFileName))
            {
                return true;
            }
            DateTime lastWriteTime = File.GetLastWriteTime(this.dependencyFileName);
            return DateTime.Compare(this.lastModifiedTime, lastWriteTime) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Notify()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owningCacheItem"></param>
        public void Initialize(CacheItem owningCacheItem)
        {
        }

        private void EnsureTargetFileAccessible()
        {
            string path = this.dependencyFileName;
            FileIOPermission fileIOPermission = new FileIOPermission(FileIOPermissionAccess.Read, path);
            fileIOPermission.Demand();
        }
    }
}
