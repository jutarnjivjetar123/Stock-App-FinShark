using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces.IStockRepository;
using api.Models;
using api.Data;
using Microsoft.EntityFrameworkCore;
using api.Dtos.Stock.UpdateStockRequestDto;
using System.Runtime.CompilerServices;
using api.Helpers;
namespace api.Repository.StockRepository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stock.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stock.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModel == null) {
                return null;
             }

            _context.Stock.Remove(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;

        }

        public async Task<List<Stock>> GetAllAsync(QueryObject query)
        {
            var stocks = _context.Stock.Include(s => s.Comments).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.CompanyName)) {
                stocks = stocks.Where(s => s.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.Symbol)) { 
                stocks = stocks.Where(s => s.Symbol.Contains(query.Symbol));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy)) {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase)) {
                    stocks = query.IsDescending ? stocks.OrderByDescending(s => s.Symbol) : stocks.OrderBy(s => s.Symbol);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;


            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }


        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stock.FindAsync(id);
            
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateStockRequestDto)
        {
            var stockToUpdate = await _context.Stock.FindAsync(id);
            if (stockToUpdate == null) {
                return null;
            }

            stockToUpdate.Symbol = updateStockRequestDto.Symbol;
            stockToUpdate.CompanyName = updateStockRequestDto.CompanyName;
            stockToUpdate.Purchase = updateStockRequestDto.Purchase;
            stockToUpdate.LastDiv = updateStockRequestDto.LastDiv;
            stockToUpdate.MarketCap = updateStockRequestDto.MarketCap;
            stockToUpdate.Industry = updateStockRequestDto.Industry;

            await _context.SaveChangesAsync();
            return stockToUpdate;
        }

        public async Task<bool> StockExists(int id) {
            return await _context.Stock.AnyAsync(s => s.Id == id);
        }

        public async Task<Stock?> GetBySymbol(string symbol)
        {
            return await _context.Stock.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }
    }
}