namespace Pricing.Core.Domain;

public class PriceTier
{
    public int HourLimit { get;}
    public decimal Price { get; set; }

    public PriceTier(int hourLimit, decimal price)
    {
        HourLimit = hourLimit;
        Price = price;
    }
}