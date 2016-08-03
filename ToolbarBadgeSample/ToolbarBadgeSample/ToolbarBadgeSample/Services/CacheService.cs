using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using PCLStorage;
using ToolbarBadgeSample.Services;
using ToolbarBadgeSample.Services.Contracts;
using Xamarin.Forms;

[assembly: Dependency(typeof(CacheService))]

namespace ToolbarBadgeSample.Services
{
    public class CacheService : ICacheService
    {
        #region Private fields

        private readonly IDeviceService _deviceService;
        private static readonly AsyncReaderWriterLock LockersDictionaryLocker = new AsyncReaderWriterLock();
        private readonly Dictionary<string, AsyncReaderWriterLock> _lockers;
        private readonly Dictionary<string, CancellationTokenSource> _tokens;

        #endregion

        #region Constructors

        public CacheService(IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _lockers = new Dictionary<string, AsyncReaderWriterLock>();
            _tokens = new Dictionary<string, CancellationTokenSource>();
        }

        #endregion

        #region ICacheService implementation

        public async Task<byte[]> GetCachedItem(IFolder folder, string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId))
                return null;
            AsyncReaderWriterLock locker;
            CancellationTokenSource token;
            using(var writerLock = await LockersDictionaryLocker.WriterLockAsync().ConfigureAwait(false))
            {
                if (_lockers.ContainsKey(cacheId))
                {
                    locker = _lockers[cacheId];
                    token = _tokens[cacheId];
                }
                else
                {
                    _lockers.Add(cacheId, locker = new AsyncReaderWriterLock());
                    _tokens.Add(cacheId, token = new CancellationTokenSource());
                }
            }

            try
            {
                var item = await GetCachedItemInternal(folder, cacheId, locker, token).ConfigureAwait(false);
                return item;
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't load cached item for id {0}. {1}", cacheId, ex);
            }
            return null;
        }

        public async Task StoreItem(IFolder folder, byte[] item, string cacheId)
        {
            if (string.IsNullOrEmpty(cacheId) || item == null || folder == null)
                return;
            AsyncReaderWriterLock locker;
            CancellationTokenSource token;
            lock (LockersDictionaryLocker)
            {
                if (_lockers.ContainsKey(cacheId))
                {
                    locker = _lockers[cacheId];
                    token = _tokens[cacheId];
                    token.Cancel();
                }
                else
                {
                    _lockers.Add(cacheId, locker = new AsyncReaderWriterLock());
                }
                _tokens[cacheId] = token = new CancellationTokenSource();
            }

            try
            {
                using (var writerLock = await locker.WriterLockAsync(token.Token).ConfigureAwait(false))
                {
                    var file = await folder.CreateFileAsync(cacheId, CreationCollisionOption.ReplaceExisting, token.Token).ConfigureAwait(false);
                    using (var writer = await file.OpenAsync(FileAccess.ReadAndWrite, token.Token).ConfigureAwait(false))
                    {
                        await writer.WriteAsync(item, 0, item.Length, token.Token).ConfigureAwait(false);
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't load cached item for id {0}. {1}", cacheId, ex);
                return;
            }
        }

        #endregion

        #region Utility methods

        private static async Task<byte[]> GetCachedItemInternal(IFolder folder, string cacheId, AsyncReaderWriterLock locker, CancellationTokenSource token)
        {
            using (var readerLock = await locker.ReaderLockAsync(token.Token).ConfigureAwait(false))
            {
                var file = await folder.GetFileAsync(cacheId, token.Token).ConfigureAwait(false);
                using (var reader = await file.OpenAsync(FileAccess.Read, token.Token).ConfigureAwait(false))
                {
                    var buffer = new byte[reader.Length];
                    using (var writer = new MemoryStream())
                    {
                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            await writer.WriteAsync(buffer, 0, read).ConfigureAwait(false);
                        }
                        return writer.ToArray();
                    }
                }
            }
        }

        #endregion

    }
}
