using Claims.Models;
using Claims.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Xunit;
using System.Net;

public class ClaimsCoversControllersTests
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;

    public ClaimsCoversControllersTests()
    {
        var application = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        _client = application.CreateClient();

        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Fact]
    public async Task Get_ClaimsAndCoversPostClaimAndCover()
    {
        // Arrange
        var cover = await PostCoverAsync();
        var claim = await PostClaimAsync(cover.Id);

        // Act
        await AssertClaimAndCoverExistenceAsync(claim, cover);
        await DeleteClaimAndCoverAsync(claim.Id, cover.Id);
    }

    [Fact]
    public async Task Get_ClaimAndCoverByIdAndDeleteById()
    {
        // Arrange
        var cover = await PostCoverAsync();
        var claim = await PostClaimAsync(cover.Id);

        // Act
        await AssertClaimExistsAsync(claim);
        await DeleteClaimAndCoverAsync(claim.Id, cover.Id);
    }

    [Theory]
    [InlineData(CoverType.Yacht, 365, 483931.25)] // Test for a full year coverage for a Yacht   
    [InlineData(CoverType.Tanker, 30, 56250)] // Test for 1 month coverage for a Tanker  
    public void ComputePremium_ReturnsCorrectValue_ForDifferentCoverTypesAndDurations(CoverType coverType, int daysCovered, decimal expectedValue)
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddDays(daysCovered);

        // Act
        var computedPremium = ComputePremiumCalculator.compute(startDate, endDate, coverType);

        // Assert
        Assert.Equal(expectedValue, computedPremium);
    }


    [Fact]
    private async Task<Cover> PostCoverAsync()
    {
        // Arrange
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.AddMonths(1),
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            Id = "124",
            Premium = 10000,
            Type = CoverType.BulkCarrier
        };

        // Act
        var jsonCover = JsonSerializer.Serialize(cover);
        var contentCover = new StringContent(jsonCover, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Covers", contentCover);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Cover>(responseBody, _options);
    }

    [Fact]
    public async Task Get_AllCovers_ReturnsListOfCovers()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/Covers");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var covers = JsonSerializer.Deserialize<List<Cover>>(responseBody, _options);
        Assert.NotNull(covers);
        Assert.NotEmpty(covers);
    }

    [Fact]
    public async Task Get_CoverById_ReturnsSingleCover()
    {
        // Arrange
        var cover = await PostCoverAsync();

        // Act
        var response = await _client.GetAsync($"/Covers/{cover.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var retrievedCover = JsonSerializer.Deserialize<Cover>(responseBody, _options);
        Assert.NotNull(retrievedCover);
        Assert.Equal(cover.Id, retrievedCover.Id);
    }


    private async Task<Claim> PostClaimAsync(string coverId)
    {
        // Arrange
        var claim = new Claim
        {
            Id = "Example Claim",
            CoverId = coverId,
            Created = DateTime.UtcNow.AddMonths(2),
            Name = "redddd",
            Type = ClaimType.Fire,
            DamageCost = 10000
        };

        // Act
        var jsonClaim = JsonSerializer.Serialize(claim);
        var contentClaim = new StringContent(jsonClaim, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Claims", contentClaim);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Claim>(responseBody, _options);
    }

    
    private async Task AssertClaimAndCoverExistenceAsync(Claim claim, Cover cover)
    {
        // Arrange
        var claimsResponse = await _client.GetAsync("/Claims");
        claimsResponse.EnsureSuccessStatusCode();
        var claimsResponseBody = await claimsResponse.Content.ReadAsStringAsync();
        var claims = JsonSerializer.Deserialize<List<Claim>>(claimsResponseBody, _options);

        // Assert
        Assert.Contains(claims, c => c.DamageCost == claim.DamageCost);

        var coversResponse = await _client.GetAsync("/Covers");
        coversResponse.EnsureSuccessStatusCode();
    }

    private async Task DeleteClaimAndCoverAsync(string claimId, string coverId)
    {
        // Act
        var responseDeletedClaim = await _client.DeleteAsync($"/Claims/{claimId}");
        responseDeletedClaim.EnsureSuccessStatusCode();

        var responseDeletedCover = await _client.DeleteAsync($"/Covers/{coverId}");
        responseDeletedCover.EnsureSuccessStatusCode();
    }

    private async Task AssertClaimExistsAsync(Claim claim)
    {
        // Act
        var response = await _client.GetAsync($"/Claims/{claim.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    private decimal ComputeCoverPremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        // Arrange & Act
        return ComputePremiumCalculator.compute(startDate, endDate, coverType);
    }

}
