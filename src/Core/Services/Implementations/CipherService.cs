﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models.Table;
using Bit.Core.Repositories;
using Core.Models.Data;

namespace Bit.Core.Services
{
    public class CipherService : ICipherService
    {
        private readonly ICipherRepository _cipherRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPushService _pushService;

        public CipherService(
            ICipherRepository cipherRepository,
            IFolderRepository folderRepository,
            IUserRepository userRepository,
            IPushService pushService)
        {
            _cipherRepository = cipherRepository;
            _folderRepository = folderRepository;
            _userRepository = userRepository;
            _pushService = pushService;
        }

        public async Task SaveAsync(CipherDetails cipher)
        {
            if(cipher.Id == default(Guid))
            {
                await _cipherRepository.CreateAsync(cipher);

                // push
                await _pushService.PushSyncCipherCreateAsync(cipher);
            }
            else
            {
                cipher.RevisionDate = DateTime.UtcNow;
                await _cipherRepository.ReplaceAsync(cipher);

                // push
                await _pushService.PushSyncCipherUpdateAsync(cipher);
            }
        }

        public async Task DeleteAsync(Cipher cipher)
        {
            await _cipherRepository.DeleteAsync(cipher);

            // push
            await _pushService.PushSyncCipherDeleteAsync(cipher);
        }

        public async Task SaveFolderAsync(Folder folder)
        {
            if(folder.Id == default(Guid))
            {
                await _folderRepository.CreateAsync(folder);

                // push
                //await _pushService.PushSyncCipherCreateAsync(cipher);
            }
            else
            {
                folder.RevisionDate = DateTime.UtcNow;
                await _folderRepository.UpsertAsync(folder);

                // push
                //await _pushService.PushSyncCipherUpdateAsync(cipher);
            }
        }

        public async Task DeleteFolderAsync(Folder folder)
        {
            await _folderRepository.DeleteAsync(folder);

            // push
            //await _pushService.PushSyncCipherDeleteAsync(cipher);
        }

        public async Task ImportCiphersAsync(
            List<Folder> folders,
            List<CipherDetails> ciphers,
            IEnumerable<KeyValuePair<int, int>> folderRelationships)
        {
            // create all the folders
            var folderTasks = new List<Task>();
            foreach(var folder in folders)
            {
                folderTasks.Add(_folderRepository.CreateAsync(folder));
            }
            await Task.WhenAll(folderTasks);

            // associate the newly created folders to the ciphers
            foreach(var relationship in folderRelationships)
            {
                var cipher = ciphers.ElementAtOrDefault(relationship.Key);
                var folder = folders.ElementAtOrDefault(relationship.Value);

                if(cipher == null || folder == null)
                {
                    continue;
                }

                //cipher.FolderId = folder.Id;
            }

            // create all the ciphers
            await _cipherRepository.CreateAsync(ciphers);

            // push
            var userId = folders.FirstOrDefault()?.UserId ?? ciphers.FirstOrDefault()?.UserId;
            if(userId.HasValue)
            {
                await _pushService.PushSyncCiphersAsync(userId.Value);
            }
        }
    }
}
