using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using StockPortfolioMgmt.Models;
using StockPortfolioMgmt.Context;
using Dapper;
using StockPortfolioMgmt.Repositories;
using System.Collections.Generic;

namespace StockPortfolioMgmt.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly DapperContext _dapperContext;

        public TransactionsRepository(DapperContext dapperContext)
        {
            this._dapperContext = dapperContext;
        }

        public async Task<TransactionsModel>GetTransactionById(int transactionId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM Transactions WHERE TransactionId=@TransactionId";
                return await connection.QuerySingleOrDefaultAsync<TransactionsModel>(query, new
                {
                    TransactionId = transactionId
                });
            }
        }

        public async Task<IEnumerable<TransactionsModel>> GetTransactionsByPortfolio(int portfolioId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "SELECT * FROM Transactions WHERE PortfolioId = @PortfolioId";
                return await connection.QueryAsync<TransactionsModel>(query, new
                { PortfolioId = portfolioId});
            }
        }

        public async Task CreateTransaction(TransactionsModel transaction)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"INSERT INTO Transactions (PortfolioId, StockId, TransactionId, TransactionType, TransactionDate, TransactionAmount, Quantity, Fee)
                                VALUES (@PortfolioId, @StockId, @TransactionId, @TransactionType, @TransactionDate, @TransactionAmount, @Quantity, @Fee)";
                await connection.ExecuteAsync(query, transaction);
            }
        }

        public async Task DeleteTransaction(int transactionId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "DELETE FROM Transactions WHERE TransactionId = @TransactionId";
                await connection.ExecuteAsync(query, new
                {
                    TransactionId = transactionId
                });
            }
        }
    }
}
