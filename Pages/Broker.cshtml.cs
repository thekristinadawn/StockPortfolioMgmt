using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Repositories;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using StockPortfolioMgmt.Pages.ViewModels;

namespace StockPortfolioMgmt.Pages
{
    public class BrokerModel : PageModel
    {
        private readonly IBrokerRepository _brokerRepository;
        private const string AdminId = "admin123";

        public BrokerModel(IBrokerRepository brokerRepository)
        {
            _brokerRepository = brokerRepository;
        }

        public BrokerViewModel ViewModel
        {
            get; private set;
        }

        public async Task<IActionResult> OnGet(string adminId)
        {
            if (adminId != AdminId)
            {
                Unauthorized(); // Return unauthorized status if the admin ID is incorrect
            }

            ViewModel = new BrokerViewModel
            {
                PortfolioWithBalances = await _brokerRepository.GetAllCustomersWithBalances(),
                StockPrices = await _brokerRepository.GetAllStockPrices(),
                AdminId = adminId
            };

            return Page();
        }
    }
}
