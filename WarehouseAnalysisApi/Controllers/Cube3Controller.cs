using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube3Controller : ControllerBase
{
    private readonly Cube3Service _cube3Service;

    public Cube3Controller(Cube3Service cube3Service)
    {
        _cube3Service = cube3Service;
    }

    /*----------------------Requirement 3----------------------
    ---------http://localhost:5164/api/cube3/requirement3-----------*/


    [HttpGet("requirement3")]
    public async Task<IActionResult> GetRequirement3(
        int pageNumber = 1,
        int pageSize = 20,
        string city = "",
        string state = ""
    )
    {
        try
        {
            var result = await _cube3Service.getRequirement3(city, state);

            var pagedResult = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 3 has been retrieved successfully",
                    total = result.Count,
                    currentPage = pageNumber,
                    pageSize,
                    data = pagedResult,
                }
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

