using System.Collections.Generic;
using StockPortfolioMgmt.Models;
using System.Threading.Tasks;


namespace StockPortfolioMgmt.Repositories
{
    public interface ITransactionsRepository
    {
        Task<TransactionsModel>GetTransactionById(int transactionId);
        Task<IEnumerable<TransactionsModel>> GetTransactionsByPortfolio(int portfolioId);

        Task CreateTransaction(TransactionsModel transaction);
        Task DeleteTransaction(int transactionId);
    }
}
