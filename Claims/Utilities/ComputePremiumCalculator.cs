using Claims.Models;

namespace Claims.Utilities
{
    public static class ComputePremiumCalculator
    {

        public static decimal compute(DateTime startDate, DateTime endDate, CoverType coverType)
        {

            // Set the base premium rate per day
            const decimal basePremiumPerDay = 1250m;

            // Assign multiplier based on the cover type
            decimal multiplier = coverType switch
            {
                CoverType.Yacht => 1.1m,       // Yacht is 10% more expensive
                CoverType.PassengerShip => 1.2m, // Passenger ship is 20% more expensive
                CoverType.Tanker => 1.5m,      // Tanker is 50% more expensive
                _ => 1.3m                      // All other types are 30% more expensive
            };

            decimal premiumPerDay = basePremiumPerDay * multiplier;

            // Calculate the total number of days the insurance covers
            int totalDays = (endDate - startDate).Days;

            decimal totalPremium = 0m;

            // Compute premium for each day in the period
            for (int day = 0; day < totalDays; day++)
            {
                decimal dailyRate = premiumPerDay;

                if (day >= 30 && day < 180) // Days 31-180 get a discount
                {
                    decimal discount = (coverType == CoverType.Yacht) ? 0.05m : 0.02m;
                    dailyRate -= premiumPerDay * discount;
                }
                else if (day >= 180) // Days 181 and beyond get a further discount
                {
                    decimal additionalDiscount = (coverType == CoverType.Yacht) ? 0.03m : 0.01m;
                    dailyRate -= premiumPerDay * additionalDiscount;
                }

                totalPremium += dailyRate;
            }

            return totalPremium;

        }

    }
}
