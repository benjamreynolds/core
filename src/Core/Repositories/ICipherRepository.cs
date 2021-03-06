﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bit.Core.Models.Table;
using Core.Models.Data;

namespace Bit.Core.Repositories
{
    public interface ICipherRepository : IRepository<Cipher, Guid>
    {
        Task<CipherDetails> GetByIdAsync(Guid id, Guid userId);
        Task<ICollection<CipherDetails>> GetManyByUserIdAsync(Guid userId);
        Task<ICollection<CipherDetails>> GetManyByTypeAndUserIdAsync(Enums.CipherType type, Guid userId);
        Task<Tuple<ICollection<CipherDetails>, ICollection<Guid>>> GetManySinceRevisionDateAndUserIdWithDeleteHistoryAsync(
            DateTime sinceRevisionDate, Guid userId);
        Task CreateAsync(CipherDetails cipher);
        Task ReplaceAsync(CipherDetails cipher);
        Task UpsertAsync(CipherDetails cipher);
        Task UpdateUserEmailPasswordAndCiphersAsync(User user, IEnumerable<Cipher> ciphers);
        Task CreateAsync(IEnumerable<Cipher> ciphers);
    }
}
