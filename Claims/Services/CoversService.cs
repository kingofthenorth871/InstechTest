using Claims.Data;
using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Claims.Services
{
    public class CoversService : ICoversService
    {
        private readonly CoversContext _context;

        public CoversService(CoversContext context)
        {
            _context = context;
        }
        public async Task AddItemAsync(Cover item)
        {
          
            if (item.EndDate > item.StartDate.Date.AddYears(1))
            { 
                throw new ArgumentOutOfRangeException(nameof(item.StartDate), "End date cannot exceed the start date by more than one year."); 
            }
           
            if (item.StartDate < DateTime.Today)
            {
                throw new ArgumentOutOfRangeException(nameof(item.StartDate), "Start date cannot be in the past.");
            }

            await _context.AddItemAsync(item);
                         
        }

        public async Task DeleteItemAsync(string id)
        {
            await _context.DeleteCoverItemAsync(id);
        }

        public async Task<Cover> GetCoverAsync(string id)
        {
            return await _context.GetCoverAsync(id);
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await _context.GetCoversAsync();
        }

        
    }
}
