using System.Collections.Immutable;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests;

public class PricingTable
{
    private readonly decimal? _maxDailyPrice;
    public IReadOnlyCollection<PriceTier> Tiers;

    public PricingTable(IEnumerable<PriceTier> tiers, decimal? maxDailyPrice=null)
    {
        _maxDailyPrice = maxDailyPrice;
        Tiers = tiers.OrderBy(tire=>tire.HourLimit)?.ToImmutableArray() ?? throw new ArgumentNullException();
        if (!tiers.Any())
            throw new ArgumentException();

        if (Tiers.Last().HourLimit < 24) throw new ArgumentException();

        if (_maxDailyPrice.HasValue && _maxDailyPrice.Value > CalculateMaxDailyPriceFromTires())
            throw new ArgumentException();
    }

    public decimal GetMaxDailyPrice()
    {
        if (_maxDailyPrice.HasValue)
            return _maxDailyPrice.Value;
        return CalculateMaxDailyPriceFromTires();
    }

    private decimal CalculateMaxDailyPriceFromTires()
    {
        decimal total = 0;
        var hoursIncluded = 0;
        foreach (var tire in Tiers)
        {
            total += tire.Price * (tire.HourLimit - hoursIncluded);
            hoursIncluded = tire.HourLimit;
        }
        return total;
    }
}