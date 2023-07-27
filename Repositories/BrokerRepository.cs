using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Threading.Tasks;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Context;
using StockPortfolioMgmt.Repositories;
using Microsoft.VisualBasic.FileIO;

namespace StockPortfolioMgmt.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly DapperContext _dapperContext;

        public BrokerRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<PortfolioModel>> GetAllCustomersWithBalances()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM PortfolioInfo WHERE Balance > 0";
                var portfolios = await connection.QueryAsync<PortfolioModel>(query);
                return portfolios;
                
            }
        }
        public async Task ImportStockPriceFromCSV(string csvFilePath)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"INSERT INTO StockPrices (Symbol, StockDate, OpenPrice, HighPrice, LowPrice, ClosePrice, Volume, Exchange)
                                VALUES (@Symbol, @StockDate, @OpenPrice, @HighPrice, @LowPrice, @ClosePrice, @Volume, @Exchange)";

                using (TextFieldParser parser = new TextFieldParser(csvFilePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    if (!parser.EndOfData)
                        parser.ReadLine();

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        var stockPrice = new StockPricesModel
                        {
                            Symbol = fields[0],
                            StockDate = DateOnly.Parse(fields[1]),
                            OpenPrice = decimal.Parse(fields[2]),
                            HighPrice = decimal.Parse(fields[3]),
                            LowPrice = decimal.Parse(fields[4]),
                            ClosePrice = decimal.Parse(fields[5]),
                            Volume = decimal.Parse(fields[6]),
                            Exchange = ParseExchangeFromFileName(csvFilePath)
                        };

                        await connection.ExecuteAsync(query, stockPrice);
                    }
                }
            }
        }

        public async Task<PortfolioModel> GetPortfolioById(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM PortfolioInfo WHERE PortfolioId = @PortfolioId";
                var portfolio = await connection.QuerySingleOrDefaultAsync<PortfolioModel>(query, new
                {
                    PortfolioId = portfolioId
                });

                if (portfolio != null)
                {
                    portfolio.Stocks = await GetPortfolioStocks(connection, portfolio.PortfolioId);
                }

                return portfolio;
            }
        }

        public async Task<IEnumerable<StockPricesModel>> GetAllStockPrices()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM StockPrices";
                return await connection.QueryAsync<StockPricesModel>(query);
            }
        }


        //helpers

        private async Task<List<PortfolioStocksModel>> GetPortfolioStocks(IDbConnection connection, int portfolioId)
        {
            string query = "SELECT * FROM PortfolioStocks WHERE PortfolioId=@PortfolioId";
            return (await connection.QueryAsync<PortfolioStocksModel>(query, new
            {
                PortfolioId = portfolioId
            })).ToList();
        }

        private string ParseExchangeFromFileName(string filePath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath); //built in function
            string[] fileNameDissected = fileName.Split('_');
            return fileNameDissected[0];
        }

    }
}
