using System.Collections.Generic;
using System.Threading.Tasks;
using StockPortfolioMgmt.Models;

namespace StockPortfolioMgmt.Repositories
{
    public interface IPortfolioRepository
    {
        Task<IEnumerable<PortfolioModel>> GetAllPortfolios();
        Task<PortfolioModel> GetPortfolioInfoById(int id);
        Task CreatePortfolio(PortfolioModel portfolio);
        Task UpdatePortfolio(PortfolioModel portfolio);
        Task DeletePortfolio(int portfolioId);
        Task DepositCash(int portfolioId, decimal amount);
        Task WithdrawCash(int portfolioId, decimal amount);
        Task TransferCash(int fromPortfolioId, int toPortfolioId, decimal amount);
        Task PurchaseStock(int portfolioId, int stockId, int quantity);
        Task SellStock(int portfolioId, int stockId, int quantity);
        Task<decimal> GetCurrentCashBalance(int portfolioId);
        Task<IEnumerable<PortfolioStocksModel>> GetStocksByPortfolioId(int portfolioId);
        Task<decimal> GetCurrentStockPrice(int stockId);
        Task<decimal> CalculateGainLossPercentage(int portfolioId);
        Task<IEnumerable<TransactionsModel>>GetAllTransactions(int portfolioId);
        
    }
}
