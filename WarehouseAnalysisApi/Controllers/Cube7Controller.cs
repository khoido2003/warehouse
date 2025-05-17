using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube7Controller : ControllerBase
{
    private readonly Cube7Service _cube7Service;

    public Cube7Controller(Cube7Service cube7Service)
    {
        _cube7Service = cube7Service;
    }

    /*----------------------Requirement 9----------------------
    ---------http://localhost:5164/api/cube7/requirement9-----------*/


    [HttpGet("requirement9")]
    public async Task<IActionResult> GetRequirement9(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var result = await _cube7Service.getRequirement9();

            var pagedResult = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 8 has been retrieved successfully",
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

