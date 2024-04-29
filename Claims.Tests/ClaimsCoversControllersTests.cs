using Claims.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Moq;
using Claims.Services;
using Claims.Models;
using Claims.Data;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Claims.Utilities;

namespace Claims.Tests
{
    public class ClaimsCoversControllersTests
    {
        private HttpClient _client;
        private JsonSerializerOptions _options;

        public class CoverRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public CoverType Type { get; set; }
        }

        public ClaimsCoversControllersTests()
            {
            // Arrange
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                { });

            _client = application.CreateClient();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            _options = options;

        }

        [Fact]
        public async Task Get_ClaimsAndCoversPostClaimAndCover()
        {           
            
            //Arrange           

            //creating a new cover
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                Id = "123",
                Premium = 10000,
                Type = CoverType.BulkCarrier
            };

            //Posting Cover into database
            var jsonCover = JsonSerializer.Serialize(cover);
            var contentCover = new StringContent(jsonCover, Encoding.UTF8, "application/json");
            var responseClaimPostingCover = await _client.PostAsync("/Covers", contentCover);

            //Check that the cover posting was successfull
            Assert.Equal(HttpStatusCode.OK, responseClaimPostingCover.StatusCode);

            var responseBodyCover = await responseClaimPostingCover.Content.ReadAsStringAsync();
            var coverCreated = JsonSerializer.Deserialize<Cover>(responseBodyCover, _options);

            //Create a new claim and using the cover.id from the created cover
            var claim = new Claim
            {
                // Set the properties as needed for your test
                Id = "Example Claim",
                CoverId = coverCreated.Id,
                Created = DateTime.UtcNow,
                Name = "redddd",
                Type = ClaimType.Fire,
                DamageCost = 10000
            };
         
            //Posting Claim into database
            var json = JsonSerializer.Serialize(claim);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseClaimPosting = await _client.PostAsync("/Claims", content);
            
            //Check that the posting was successfull
            Assert.Equal(HttpStatusCode.OK, responseClaimPosting.StatusCode);
                               
            //Get all claims
            var response = await _client.GetAsync("/Claims");

            // Assert
            response.EnsureSuccessStatusCode();

            //Get all claims
            var responseAllCovers = await _client.GetAsync("/Covers");

            // Assert
            response.EnsureSuccessStatusCode();

            // Assert
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(responseBody), "Response body should not be empty");

            //See if we can find the claim we created by looking at the damageCost
            var claims = JsonSerializer.Deserialize<List<Claim>>(responseBody, _options);
            var claimExists = claims.Any(c => c.DamageCost == claim.DamageCost);
            Assert.True(claimExists, "Claim was not found in the response.");

            // Assert that the response content type is JSON
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
          
        }

        [Fact]
        public async Task Get_ClaimAndCoverByIdAndDeleteById()
        {
                       
            //creating a new cover
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.Date.AddYears(1),
                Id = "123",
                Premium = 10000,
                Type = CoverType.BulkCarrier
            };

            //Posting Cover into database
            var jsonCover = JsonSerializer.Serialize(cover);
            var contentCover = new StringContent(jsonCover, Encoding.UTF8, "application/json");
            var responsePostingCover = await _client.PostAsync("/Covers", contentCover);

            //Check that the cover posting was successfull
            Assert.Equal(HttpStatusCode.OK, responsePostingCover.StatusCode);

            var responseBodyCover = await responsePostingCover.Content.ReadAsStringAsync();
            var coverCreated = JsonSerializer.Deserialize<Cover>(responseBodyCover, _options);

            //Create a new claim and using the cover.id from the created cover
            var claim = new Claim
            {
                // Set the properties as needed for your test
                Id = "Example Claim",
                CoverId = coverCreated.Id,
                Created = DateTime.UtcNow,
                Name = "redddd",
                Type = ClaimType.Fire,
                DamageCost = 10000
            };

            //Posting Claim into database
            var json = JsonSerializer.Serialize(claim);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var responseClaimPosting = await _client.PostAsync("/Claims", content);

            var responseBodyClaim = await responseClaimPosting.Content.ReadAsStringAsync();
            var claimCreated = JsonSerializer.Deserialize<Claim>(responseBodyClaim, _options);

            var jsonClaim = JsonSerializer.Serialize(claimCreated.Id);
            var contentClaim = new StringContent(jsonClaim, Encoding.UTF8, "application/json");


            //Get the same claim in return
            var response = await _client.GetAsync("/Claims/" +"{"+contentClaim+"}");
            // Assert
            response.EnsureSuccessStatusCode();

            //Delete the claim
            var responseDeleted = await _client.DeleteAsync("/Claims/"+"{"+contentClaim+"}");
            responseDeleted.EnsureSuccessStatusCode();

            var responseShouldBeDeleted = await _client.GetAsync("/Claims/" + "{" + contentClaim + "}");
            // Assert
            responseShouldBeDeleted.EnsureSuccessStatusCode();

            var responseBodyShouldBeDeleted = await responseShouldBeDeleted.Content.ReadAsStringAsync();

            // Check if the response is empty, or deserialize and check for null/defaults
            if (string.IsNullOrWhiteSpace(responseBodyShouldBeDeleted))
            {
                Assert.True(true, "");
            }
            else
            {
                // Deserialize the response to check if it's an empty or null object
                var claimShouldBeDeleted = JsonSerializer.Deserialize<Claim>(responseBodyShouldBeDeleted, _options);
                Assert.Null(claimShouldBeDeleted); 
            }

            var responseBodyCoverTest = await responsePostingCover.Content.ReadAsStringAsync();
            var coverCreatedtest = JsonSerializer.Deserialize<Cover>(responseBodyCoverTest, _options);

            var jsonCoverTest = JsonSerializer.Serialize(coverCreatedtest.Id);
            var coverClaim = new StringContent(jsonCoverTest, Encoding.UTF8, "application/json");


            //Get the same cover in return
            var responseCoverTest = await _client.GetAsync("/Covers/" + "{" + coverClaim + "}");
            // Assert
            responseCoverTest.EnsureSuccessStatusCode();

            //Delete the cover
            var responseDeletedCoverTest = await _client.DeleteAsync("/Covers/" + "{" + coverClaim + "}");
            responseDeletedCoverTest.EnsureSuccessStatusCode();

        }

        [Fact]
        public async Task Get_ComputeCovers()
        {
            // Arrange
            DateTime dateTimeNow = DateTime.Now;
            DateTime dateTimeNowPlusYear = DateTime.Now.AddYears(1);

            // Act
            decimal returned = ComputePremiumCalculator.compute(dateTimeNow, dateTimeNowPlusYear, CoverType.Yacht);

            // Assert
            decimal expectedValue = 483931.250M;            

            Assert.Equal(expectedValue, returned);

        }
    }


}