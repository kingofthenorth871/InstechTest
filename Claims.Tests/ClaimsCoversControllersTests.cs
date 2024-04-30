using Claims.Models;
using Claims.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Xunit;

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
        var cover = await PostCoverAsync();
        var claim = await PostClaimAsync(cover.Id);

        await AssertClaimAndCoverExistenceAsync(claim, cover);
        await DeleteClaimAndCoverAsync(claim.Id, cover.Id);
    }

    [Fact]
    public async Task Get_ClaimAndCoverByIdAndDeleteById()
    {
        var cover = await PostCoverAsync();
        var claim = await PostClaimAsync(cover.Id);

        await AssertClaimExistsAsync(claim);
        await DeleteClaimAndCoverAsync(claim.Id, cover.Id);
    }

    [Fact]
    public void Get_ComputeCovers()
    {
        var computedPremium = ComputeCoverPremium(DateTime.Now, DateTime.Now.AddYears(1), CoverType.Yacht);
        const decimal expectedValue = 483931.250M;
        Assert.Equal(expectedValue, computedPremium);
    }

    private async Task<Cover> PostCoverAsync()
    {
        var cover = new Cover
        {
            StartDate = DateTime.UtcNow.AddMonths(1),
            EndDate = DateTime.UtcNow.Date.AddMonths(6),
            Id = "124",
            Premium = 10000,
            Type = CoverType.BulkCarrier
        };

        var jsonCover = JsonSerializer.Serialize(cover);
        var contentCover = new StringContent(jsonCover, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Covers", contentCover);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Cover>(responseBody, _options);
    }

    private async Task<Claim> PostClaimAsync(string coverId)
    {
        var claim = new Claim
        {
            Id = "Example Claim",
            CoverId = coverId,
            Created = DateTime.UtcNow.AddMonths(2),
            Name = "redddd",
            Type = ClaimType.Fire,
            DamageCost = 10000
        };

        var jsonClaim = JsonSerializer.Serialize(claim);
        var contentClaim = new StringContent(jsonClaim, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Claims", contentClaim);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Claim>(responseBody, _options);
    }

    private async Task AssertClaimAndCoverExistenceAsync(Claim claim, Cover cover)
    {
        var claimsResponse = await _client.GetAsync("/Claims");
        claimsResponse.EnsureSuccessStatusCode();
        var claimsResponseBody = await claimsResponse.Content.ReadAsStringAsync();
        var claims = JsonSerializer.Deserialize<List<Claim>>(claimsResponseBody, _options);
        Assert.Contains(claims, c => c.DamageCost == claim.DamageCost);

        var coversResponse = await _client.GetAsync("/Covers");
        coversResponse.EnsureSuccessStatusCode();
    }

    private async Task DeleteClaimAndCoverAsync(string claimId, string coverId)
    {
        var responseDeletedClaim = await _client.DeleteAsync($"/Claims/{claimId}");
        responseDeletedClaim.EnsureSuccessStatusCode();

        var responseDeletedCover = await _client.DeleteAsync($"/Covers/{coverId}");
        responseDeletedCover.EnsureSuccessStatusCode();
    }

    private async Task AssertClaimExistsAsync(Claim claim)
    {
        var response = await _client.GetAsync($"/Claims/{claim.Id}");
        response.EnsureSuccessStatusCode();
    }

    private decimal ComputeCoverPremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return ComputePremiumCalculator.compute(startDate, endDate, coverType);
    }
}
