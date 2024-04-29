using Claims.Data;
using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Claims.Services
{
    public class ClaimsService : IClaimsService
    {
        private readonly ClaimsContext _context;
        private readonly ICoversService _coversService;

        public ClaimsService(ClaimsContext context, ICoversService coversService)
        {
            _context = context;
            _coversService = coversService;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync() 
        { 
            return await _context.GetClaimsAsync();      
        }

        public async Task<Claim> GetClaimAsync(string id)
        {
            return await _context.GetClaimAsync(id);
        }

        public async Task AddItemAsync(Claim item) 
        {
            if (item.DamageCost > 100000)
            {
                throw new ValidationException("Damage cost cannot exceed 100,000.");
            }

            bool isValid = await ValidateClaimDateAsync(item);

            if (isValid)
            {
                await _context.AddItemAsync(item);
            }
            else
            {
                throw new ArgumentException("Invalid claim date.");
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            await _context.DeleteItemAsync(id);
        }

        public async Task<bool> ValidateClaimDateAsync(Claim claim)
        {
            var response = await _coversService.GetCoversAsync();
            var cover = response.SingleOrDefault(cover => cover.Id == claim.CoverId);

            //var cover = await _context.Covers.FirstOrDefaultAsync(c => c.Id == claim.CoverId);
            if (cover == null)
                throw new ArgumentException("Cover not found.");

            return claim.Created.Date >= cover.StartDate.Date && claim.Created.Date <= cover.EndDate.Date;
        }

    }
}