using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests;

public class PricingTableSpecification
{
    [Fact]
    public void Should_fail_if_price_tiers_is_null()
    {
       var create=()=> new PricingTable(null);
       create.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Should_fail_if_has_no_price_tiers()
    {
        var create=()=> new PricingTable(Array.Empty<PriceTier>());
        create.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Should_have_one_tier_when_created_with_one()
    {
        var pricingTable = new PricingTable(new[] { CreatePriceTier() });
        pricingTable.Tiers.Should().HaveCount(1);
    }

    [Fact]
    public void Pricing_tiers_should_be_ordered_by_hour_Limit()
    {
        var pricingTable = new PricingTable (new []
        {
            CreatePriceTier(24),
            CreatePriceTier(4)
        });
        pricingTable.Tiers.Should().BeInAscendingOrder(tire=>tire.HourLimit);
    }

    [Theory]
    [InlineData(2,1,25)]
    [InlineData(3,2,49)]
    public void Maximum_daily_price_should_be_calculated_using_tiers_if_defined(decimal price1,decimal price2,decimal maxPrice)
    {
        var pricingTable = new PricingTable (new []
        { 
            CreatePriceTier(1,price1),
            CreatePriceTier(24,price2)
        },maxDailyPrice:null);

        pricingTable.GetMaxDailyPrice().Should().Be(maxPrice);

    }

    [Fact]
    public void Should_be_able_to_set_maximum_daily_price()
    {
        decimal? maxDailyPrice = 15;
        var pricingTable = new PricingTable (new []
        { 
            CreatePriceTier(24,1)
        },maxDailyPrice:maxDailyPrice);
        pricingTable.GetMaxDailyPrice().Should().Be(maxDailyPrice);
    }

    [Fact]
    public void Should_fail_if_tiers_do_not_cover_24h()
    {
        var create = () => new PricingTable(new[]
        {
            CreatePriceTier(20)
        });
        create.Should().Throw<ArgumentException>();
    }
    [Fact]
    public void Should_fail_if_max_daily_price_gt_tiers_price()
    {
        var create = () => new PricingTable(new[]
        {
            CreatePriceTier(24,1)
        },26);
        create.Should().Throw<ArgumentException>();
    }
    private static PriceTier CreatePriceTier(int hourLimit=24,decimal price=1) 
        => new PriceTier(hourLimit,price);
}