using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Repositories;
using StockPortfolioMgmt.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StockPortfolioMgmt.Pages
{
    public class PortfolioPageModel : PageModel
    {
        public class PortfolioModel : PageModel
        {
            private readonly IPortfolioRepository _portfolioRepository;
            private readonly PortfolioViewModel _viewModel;

            public PortfolioModel(IPortfolioRepository portfolioRepository, PortfolioViewModel viewModel)
            {
                _portfolioRepository = portfolioRepository;
                _viewModel = viewModel;
            }

            public async Task<IActionResult> OnGet(int portfolioId)
            {
                await _viewModel.LoadDataAsync(portfolioId);
                return Page();
            }

            public int PortfolioId
            {
                get; set;
            }

            public PortfolioViewModel ViewModel
            {
                get; set;
            }

            public async Task<IActionResult> OnPostAsync(string action, decimal amount, int stockId = 0)
            {
                int portfolioId = PortfolioId;
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
                {
                    portfolioId
                });
            }
        }

    }
}
