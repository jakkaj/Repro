using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using XamlingCore.Portable.Util.Lock;

namespace ReproTests.Model
{
    public interface ILocalStorage
    {
        Task<bool> FileExists(string fileName);
        Task<bool> Copy(string source, string destinationFolder, string newName, bool replace = true);

        Task<byte[]> Load(string fileName);
        Task<Stream> LoadStream(string fileName);
        Task<string> LoadString(string fileName);

        Task Save(string fileName, byte[] data);
        Task SaveStream(string fileName, Stream stream);
        Task<bool> SaveString(string fileName, string data);

        Task<bool> DeleteFile(string fileName);
        Task<bool> EnsureFolderExists(string folderPath);
        Task<List<string>> GetAllFilesInFolder(string folderPath);
        Task<bool> IsZero(string fileName);
        System.Threading.Tasks.Task<string> LoadStringUTF(string fileName);
        Task<bool> SaveStringUTF(string fileName, string data);
        bool SaveStringUTFNoAsync(string fileName, string data);
        string LoadStringUTFNoAsync(string fileName);
    }

    namespace XamlingCore.iOS.Implementations
    {
        public class LocalStorage : ILocalStorage
        {
            public async System.Threading.Tasks.Task<bool> Copy(string source, string destinationFolder, string newName, bool replace = true)
            {
                var _lock = NamedLock.Get(destinationFolder + "\\" + newName);
                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        store.CopyFile(source, Path.Combine(destinationFolder, newName), replace);
                        return true;
                    }
                }
            }

            public async System.Threading.Tasks.Task<bool> DeleteFile(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (store.FileExists(fileName))
                        {
                            store.DeleteFile(fileName);
                            return true;
                        }
                        return false;
                    }
                }
            }

            public async System.Threading.Tasks.Task<bool> EnsureFolderExists(string folderPath)
            {
                var _lock = NamedLock.Get(folderPath);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.DirectoryExists(folderPath))
                        {
                            store.CreateDirectory(folderPath);
                        }
                        return true;
                    }
                }

            }

            public async System.Threading.Tasks.Task<bool> FileExists(string fileName)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    return store.FileExists(fileName);
                }
            }

            public async System.Threading.Tasks.Task<List<string>> GetAllFilesInFolder(string folderPath)
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string[] subDirectories =
                        store.GetFileNames(Path.Combine(folderPath, "*"));
                    return subDirectories.ToList();
                }
            }


            public async System.Threading.Tasks.Task<bool> IsZero(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(fileName))
                        {
                            return true;
                        }

                        using (var f = store.OpenFile(fileName, FileMode.Open))
                        {
                            return f.Length == 0;
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task<byte[]> Load(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(fileName))
                        {
                            return null;
                        }

                        using (var f = store.OpenFile(fileName, FileMode.Open))
                        {
                            var b = new byte[f.Length];
                            await f.ReadAsync(b, 0, b.Length);
                            return b;
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task<System.IO.Stream> LoadStream(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(fileName))
                        {
                            return null;
                        }

                        var f = store.OpenFile(fileName, FileMode.Open);

                        return f;
                    }
                }
            }

            public async System.Threading.Tasks.Task<string> LoadString(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(fileName))
                        {
                            return null;
                        }

                        using (var f = store.OpenFile(fileName, FileMode.Open))
                        {
                            using (var sr = new StreamReader(f))
                            {
                                return await sr.ReadToEndAsync();
                            }
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task<string> LoadStringUTF(string fileName)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!store.FileExists(fileName))
                        {
                            return null;
                        }

                        using (var f = store.OpenFile(fileName, FileMode.Open))
                        {
                            var b = new byte[f.Length];
                            await f.ReadAsync(b, 0, b.Length);
                            var s = Encoding.UTF8.GetString(b);
                            return s;
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task Save(string fileName, byte[] data)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var s = store.OpenFile(fileName, FileMode.OpenOrCreate))
                        {
                            await s.WriteAsync(data, 0, data.Length);
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task SaveStream(string fileName, System.IO.Stream stream)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var s = store.OpenFile(fileName, FileMode.OpenOrCreate))
                        {
                            await stream.CopyToAsync(s);
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task<bool> SaveString(string fileName, string data)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var sw = new StreamWriter(store.OpenFile(fileName, FileMode.OpenOrCreate)))
                        {
                            await sw.WriteAsync(data);
                            return true;
                        }
                    }
                }
            }

            public async System.Threading.Tasks.Task<bool> SaveStringUTF(string fileName, string data)
            {
                var _lock = NamedLock.Get(fileName);

                using (var releaser = await _lock.LockAsync())
                {
                    using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var s = store.OpenFile(fileName, FileMode.OpenOrCreate))
                        {
                            var b = Encoding.UTF8.GetBytes(data);
                            await s.WriteAsync(b, 0, b.Length);
                            await s.FlushAsync();
                            return true;
                        }
                    }
                }
            }

            public bool SaveStringUTFNoAsync(string fileName, string data)
            {

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var s = store.OpenFile(fileName, FileMode.OpenOrCreate))
                    {
                        var b = Encoding.UTF8.GetBytes(data);
                        s.Write(b, 0, b.Length);
                        s.Flush();
                        return true;
                    }
                }
            }
            public string LoadStringUTFNoAsync(string fileName)
            {

                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists(fileName))
                    {
                        return null;
                    }

                    using (var f = store.OpenFile(fileName, FileMode.Open))
                    {
                        var b = new byte[f.Length];
                        f.Read(b, 0, b.Length);
                        var s = Encoding.UTF8.GetString(b);
                        return s;
                    }
                }

            }



        }
    }
}