using MultichoiceCollection.Common.Entities;
using MultichoiceCollection.Models.Repositories.Context;
using MultichoiceCollection.Models.Repositories.Generic;
using MultichoiceCollection.Models.Repositories.Interfaces;

namespace MultichoiceCollection.Models.Repositories.Implementations
{
    public class TransactionRepository:GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context):base(context)
        {

        }
    }
}