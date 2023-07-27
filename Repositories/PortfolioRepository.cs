using Dapper;
using System.Collections.Generic;
using System.Data;
using StockPortfolioMgmt.Models;
using System.Threading.Tasks;
using StockPortfolioMgmt.Context;

namespace StockPortfolioMgmt.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly DapperContext _dapperContext;

        public PortfolioRepository(DapperContext _dapperContext)
        {
            this._dapperContext = _dapperContext;
        }

        public async Task<IEnumerable<PortfolioModel>>GetAllPortfolios()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM Portfolios";
                var portfolios = await connection.QueryAsync<PortfolioModel>(query);

                foreach (var portfolio in portfolios)
                {
                    portfolio.Stocks = await GetPortfolioStocks(connection, portfolio.PortfolioId);
                }
                return portfolios;
            }
        }


        //NEED TO UPDATE THIS TO GET JUST PORTFOLIO INFO
        public async Task<PortfolioModel> GetPortfolioInfoById(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM PortfolioInfo WHERE PortfolioId = @PortfolioId";
                var portfolioInfo = await connection.QuerySingleOrDefaultAsync<PortfolioModel>(query, new
                {
                    PortfolioId = portfolioId
                });

                return portfolioInfo;
            }
        }


        public async Task CreatePortfolio(PortfolioModel portfolio)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"INSERT INTO PortfolioInfo(UserId,PortfolioName,Balance) VALUES (@UserId, @PortfolioName, @Balance); SELECT CAST(SCOPE_IDENTITY() as int)";

                portfolio.PortfolioId = await connection.ExecuteScalarAsync<int>(query, portfolio);

                await InsertPortfolioStocks(connection, portfolio);
            }
        }

        public async Task UpdatePortfolio(PortfolioModel portfolio)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE PortfolioInfo SET UserId = @UserId, PortfolioName = @PortfolioName, Balance = @Balance WHERE PortfolioId = @PortfolioId";

                await connection.ExecuteAsync(query, portfolio);

                await UpdatePortfolioStocks(connection, portfolio);


            }
        }

        public async Task DeletePortfolio(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "DELETE FROM PortfolioInfo WHERE PortfolioId = @PortfolioId";

                await connection.ExecuteAsync(query, new
                {
                    PortfolioId = portfolioId
                });
            }
        }

        public async Task DepositCash(int portfolioId, decimal amount)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "UPDATE PortfolioInfo SET Balance = Balance +@Amount WHERE PortfolioId = @PortfolioId";

                await connection.ExecuteAsync(query, new
                {
                    PortfolioId = portfolioId,
                    Amount = amount
                });
            }
        }

        public async Task WithdrawCash(int portfolioId, decimal amount)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "UPDATE PorfolioInfo SET Balance = Balance - @Amount WHERE PortfolioId = @PortfolioId";
                await connection.ExecuteAsync(query, new
                {
                    PortfolioId = portfolioId,
                    Amount = amount
                });
            }
        }

        public async Task TransferCash(int fromPortfolioId, int toPortfolioId, decimal amount)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string queryDebit = "UPDATE PortfolioInfo SET Balance = Balance - @Amount WHERE PortfolioId = @FromPortfolio";
                        string queryCredit = "UPDATE PortfolioInfo SET Balance = Balance + @Amount WHERE PortfolioId = @ToPortfolio";

                        await connection.ExecuteAsync(queryDebit, new
                        {
                            FromPortolioId = fromPortfolioId,
                            Amount = amount
                        }, transaction);

                        await connection.ExecuteAsync(queryCredit, new
                        {
                            ToPorfolioId = toPortfolioId,
                            Amount = amount
                        }, transaction);

                        transaction.Commit();
                    }
                    catch
                    {

                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task PurchaseStock(int portfolioId, int stockId, int quantity)
        {
            using (var connection = _dapperContext.CreateConnection())
            {

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var stock = await GetStockById(connection, stockId);
                        if (stock == null)
                        {
                            throw new Exception("Stock not found.");
                        }

                        decimal totalAmount = stock.OpenPrice * quantity;
                        if (totalAmount > 0)
                        {
                            string queryDebit = "UPDATE PorfolioInfo SET Balance - @TotalAmount WHERE PortfolioId = @PortfolioId";

                            await connection.ExecuteAsync(queryDebit, new
                            { PortfolioId = portfolioId, TotalAmount = totalAmount }, transaction);

                            string queryInsertStock = @"INSERT INTO PortfolioStocks(PortfolioId, StockId, PurchasePrice, Quantity) VALUES(@PortfolioId, @StockId, @PurchasePrice, @Quantity)";

                            await connection.ExecuteAsync(queryInsertStock, new
                            { portfolioId = portfolioId, StockId = stockId, PurchasePrice = stock.OpenPrice, Quantity = quantity }, transaction);

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
            }

        }

        public async Task SellStock(int portfolioId, int stockId, int quantity)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var stock = await GetStockById(connection, stockId);
                        if (stockId == null)
                        {
                            throw new InvalidOperationException("Stock not found.");
                        }

                        decimal totalAmount = stock.OpenPrice * quantity;
                        if (totalAmount > 0)
                        {
                            string queryCredit = "UPDATE PortfolioInfo SET Balance = Balance + @TotalAmount WHERE PortfolioId = @PortfolioId";

                            await connection.ExecuteAsync(queryCredit, new
                            {
                                PortfolioId = portfolioId,
                                TotalAmount = totalAmount
                            }, transaction);

                            string queryUpdateStock = @"UPDATE PortfolioStocks SET Quantity = Quantity - @Quantity WHERE PortfolioId = @PortfolioId AND StockId = @StockId";

                            await connection.ExecuteAsync(queryUpdateStock, new
                            { PortfolioId = portfolioId,stockId = stockId, Quantity = quantity}, transaction);

                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IEnumerable<TransactionsModel>> GetAllTransactions(int portfolioId)
        {

            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM Transactions WHERE PortfolioId=@PortfolioId";
                return await connection.QueryAsync<TransactionsModel>(query, new
                { PortfolioId = portfolioId });
            }
        }

        public async Task<decimal> GetCurrentCashBalance(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT Balance FROM PortfolioInfo WHERE PortfolioId = @PortfolioId";
                return await connection.QuerySingleOrDefaultAsync<decimal>(query, new
                {
                    PortfolioId = portfolioId
                });
            }
        }

        public async Task<IEnumerable<PortfolioStocksModel>> GetStocksByPortfolioId(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM PortfolioStocks WHERE PortfolioId = @PortfolioId";
                return await connection.QueryAsync<PortfolioStocksModel>(query, new
                {
                    PortfolioId = portfolioId
                });
            }
        }

        //Using Open Price as Current Price
        public async Task<decimal> GetCurrentStockPrice(int stockId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT OpenPrice FROM StockPrices WHERE StockId = @StockId";
                return await connection.QuerySingleOrDefaultAsync<decimal>(query, new
                {
                    StockId = stockId
                });
            }
        }

        
        public async Task<decimal> CalculateGainLossPercentage(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"SELECT TOP 1 sp.HighPrice, sp.LowPrice, ps.PurchasePrice, ps.Quantity
                                 FROM PortfolioStocks ps
                                 INNER JOIN StockPrices sp ON ps.StockId = sp.StockId
                                 WHERE ps.PortfolioId = @PortfolioId AND ps.StockId = @StockId
                                 ORDER BY ps.PurchaseDate DESC";

                var stockData = await connection.QuerySingleOrDefaultAsync(query, new
                {
                    PortfolioId = portfolioId
                });

                if (stockData != null)
                {
                    decimal highPrice = stockData.HighPrice;
                    decimal lowPrice = stockData.LowPrice;
                    decimal purchasePrice = stockData.PurchasePrice;
                    int quantity = stockData.Quantity;

                    decimal currentValue = (highPrice + lowPrice) / 2; // Using the average of high and low prices as a random price
                    decimal totalInvestment = purchasePrice * quantity;
                    decimal gainLossAmount = (currentValue - purchasePrice) * quantity;
                    decimal gainLossPercentage = (gainLossAmount / totalInvestment) * 100;

                    return gainLossPercentage;
                }

                // If stockData is null (no data found), return 0 for gain/loss percentage
                return 0;
            }
        }


        //helpers

        private async Task<List<PortfolioStocksModel>> GetPortfolioStocks(IDbConnection connection, int portfolioId)
        {
            string query = "SELECT * FROM PortfolioStocks WHERE PortfolioId=@PortfolioId";
            return (await connection.QueryAsync<PortfolioStocksModel>(query, new
            { PortfolioId = portfolioId })).ToList();
        }

        private async Task<StockPricesModel> GetStockById(IDbConnection connection, int stockId)
        {
            string query = "SELECT * FROM StockPrices WHERE StockId = @StockId";
            return await connection.QuerySingleOrDefaultAsync<StockPricesModel>(query, new
            { StockId = stockId });
        }

        private async Task InsertPortfolioStocks(IDbConnection connection, PortfolioModel portfolio)
        {
            if(portfolio.Stocks != null && portfolio.Stocks.Any())
            {
                string query = @"INSERT INTO PortfolioStocks (PortfolioId, StockId, PurchasePrice, Quantity) VALUES (@PortfolioId, @StockId, @PurchasePrice, @Quantity)";

                await connection.ExecuteAsync(query, portfolio.Stocks.Select(s => new
                {
                    PortfolioId = portfolio.PortfolioId,
                    StockId = s.StockId,
                    PurchasePrice = s.PurchasePrice,
                    Quantity = s.Quantity
                }));
            }
        }

        private async Task UpdatePortfolioStocks(IDbConnection connection, PortfolioModel portfolio)
        {
            string deleteQuery = "DELETE FROM PortfolioStocks WHERE PortfolioId = @PortfolioId";

            await connection.ExecuteAsync(deleteQuery, new
            {
                PortfolioId = portfolio.PortfolioId,
            });

            await InsertPortfolioStocks(connection, portfolio);
        
        }

        private decimal GetRandomPrice(decimal highPrice, decimal lowPrice)
        {
            return (highPrice + lowPrice) / 2;
        }


        private decimal CalculateGainLossPercentage(decimal currentPrice, decimal purchasePrice)
        {
            return ((currentPrice - purchasePrice) / purchasePrice) * 100;
        }

    }
}
