using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Repositories;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;

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

        public IEnumerable<PortfolioModel> PortfolioInfo
        {
            get; private set;
        }
        public IEnumerable<StockPricesModel> StockPrices
        {
            get; private set;
        }

        public async Task<IActionResult> OnGet(int? id, string adminId)
        {
            if (adminId != AdminId)
            {
                return Unauthorized(); // Return unauthorized status if the admin ID is incorrect
            }

            if (id.HasValue)
            {
                PortfolioInfo = new List<PortfolioModel> { await _brokerRepository.GetPortfolioById(id.Value) };
                StockPrices = await _brokerRepository.GetAllStockPrices();
            }
            else
            {
                PortfolioInfo = await _brokerRepository.GetAllCustomersWithBalances();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostImportStockPrices(string csvFilePath, string adminId)
        {
            if (adminId != AdminId)
            {
                return Unauthorized(); // Return unauthorized status if the admin ID is incorrect
            }

            await _brokerRepository.ImportStockPriceFromCSV(csvFilePath);
            return RedirectToPage("Broker");
        }
    }
}



    //private const int AdminUserId = 999;

    //public async Task OnGet(int? userId)
    //{
    //    if (userId != AdminUserId)
    //    {
    //        // You can create a new Razor Page for AccessDenied if you haven't already
    //        return RedirectToPage("AccessDenied");
    //    }

    //    Portfolios = (List<PortfolioModel>)await _brokerRepository.GetAllCustomersWithBalances();
    //}

    //public async Task<IActionResult> OnPostImportStockPrices()
    //{
    //    var file = Request.Form.Files["file"];

    //    if (file == null || file.Length == 0)
    //    {
    //        return RedirectToPage("Please select a file.");
    //    }

    //    var filePath = Path.GetTempFileName();
    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await file.CopyToAsync(stream);
    //    }

    //    await _brokerRepository.ImportStockPriceFromCSV(filePath);

    //    File.Delete(filePath);

    //    return RedirectToPage("/Broker");


    //}


