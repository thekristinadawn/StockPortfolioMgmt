using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockPortfolioMgmt.Context;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Repositories;

namespace StockPortfolioMgmt.ViewModels
{
    public class PortfolioViewModel : PageModel
    {

        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IBrokerRepository _brokerRepository;

        public PortfolioViewModel(IPortfolioRepository portfolioRepository, IBrokerRepository brokerRepository)
        {
            _portfolioRepository = portfolioRepository;
            _brokerRepository = brokerRepository;
        }

        public PortfolioModel Portfolio
        {
            get; set;
        }
        public List<PortfolioStocksModel> PortfolioStocks
        {
            get; set;
        }
        public List<TransactionsModel> Transactions
        {
            get; set;
        }
        public List<StockPricesModel> StockPrices
        {
            get; set;
        }

        public decimal GetCurrentStockPrice(int stockId)
        {
            var stockPrice = StockPrices.FirstOrDefault(s => s.StockId == stockId);
            return stockPrice?.OpenPrice ?? 0;
        }



        public decimal CalculateGainLossPercentage(int portfolioId, int stockId)
        {
            var stockData = PortfolioStocks.FirstOrDefault(s => s.StockId == stockId);
            if (stockData != null)
            {
                var stockPrice = StockPrices.FirstOrDefault(s => s.StockId == stockId);
                if (stockPrice != null)
                {
                    decimal highPrice = stockPrice.HighPrice;
                    decimal lowPrice = stockPrice.LowPrice;
                    decimal purchasePrice = stockData.PurchasePrice;
                    int quantity = stockData.Quantity;

                    decimal currentValue = (highPrice + lowPrice) / 2;
                    decimal totalInvestment = purchasePrice * quantity;
                    decimal gainLossAmount = (currentValue - purchasePrice) * quantity;
                    decimal gainLossPercentage = gainLossAmount / totalInvestment * 100;

                    return gainLossPercentage;
                }
            }

            return 0;
        }

        public async Task LoadDataAsync(int portfolioId)
        {
            Portfolio = await _portfolioRepository.GetPortfolioInfoById(portfolioId);
            PortfolioStocks = (await _portfolioRepository.GetStocksByPortfolioId(portfolioId)).ToList();
            Transactions = (await _portfolioRepository.GetAllTransactions(portfolioId)).ToList();
            StockPrices = (List<StockPricesModel>)await _brokerRepository.GetAllStockPrices();
        }

    }
}
