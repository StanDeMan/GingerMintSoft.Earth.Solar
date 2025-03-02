using Microsoft.AspNetCore.Mvc;

namespace GingerMintSoft.Earth.Location.Service.Controllers
{
    [Route("Solar")]
    public class SolarController(ILogger<SolarController> logger) : Controller
    {
        private readonly ILogger<SolarController> _logger = logger;

        //[HttpGet(Name = "Powerplant")]
        //public IEnumerable<Solar> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new Solar
        //        {
        //            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //            TemperatureC = Random.Shared.Next(-20, 55),
        //            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //        })
        //        .ToArray();
        //}

        //[HttpPost]
        //[Route("Users/{userId}/Addresses")]
        //public async Task<ActionResult> Post(Guid userId, [FromBody] AddressDto addressData)
        //{
        //    try
        //    {
        //        addressData.UserId = userId;
        //        await new User(userId).Addresses.Address.Create(addressData);
        //        return new CreatedResult($"/HelpingHands/Users/{userId}/Addresses", addressData);
        //    }
        //    catch (Exception e)
        //    {
        //        return Conflict($"Cannot create address: {e}");
        //    }
        //}
    }
}
