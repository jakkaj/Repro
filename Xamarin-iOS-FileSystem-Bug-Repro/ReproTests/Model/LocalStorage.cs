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
    public class LocalStorageHelper
    {

       

     
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
