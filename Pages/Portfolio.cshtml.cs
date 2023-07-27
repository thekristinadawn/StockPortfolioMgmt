using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StockPortfolioMgmt.Pages
{
    public class PortfolioPageModel : PageModel
    {
        private readonly IPortfolioRepository _portfolioRepository;

        public PortfolioPageModel(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public PortfolioModel Portfolio
        {
            get; private set;
        }

        public List<PortfolioStocksModel> Stocks
        {
            get; 
            private set; 
        }
        
        public List<TransactionsModel> Transactions
        {
            get; 
            private set; 
        }

        public async Task OnGet(int portfolioId)
        {
            Portfolio = await _portfolioRepository.GetPortfolioById(portfolioId);
            Stocks = (await _portfolioRepository.GetStocksByPortfolioId(portfolioId)).ToList();
            Transactions = (await _portfolioRepository.GetAllTransactions(portfolioId)).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string action, decimal amount, int stockId = 0)
        {
            int portfolioId = Portfolio.PortfolioId;
            switch (action)
            {
                case "Deposit":
                    await _portfolioRepository.DepositCash(portfolioId, amount);
                    break;
                case "Withdraw":
                    await _portfolioRepository.WithdrawCash(portfolioId, amount);
                    break;
                case "Transfer":
                    await _portfolioRepository.TransferCash(portfolioId, stockId, amount); 
                    break;
                case "Purchase":
                    await _portfolioRepository.PurchaseStock(portfolioId, stockId, (int)amount);
                    break;
                case "Sale":
                    await _portfolioRepository.SellStock(portfolioId, stockId, (int)amount);
                    break;
                default:
                    return BadRequest();
            }
            return RedirectToPage(".Portfolio", new
            { portfolioId });
        }

    }
}
