using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube4Controller:ControllerBase
{
    
    private readonly Cube4Service _cube4Service;

    public Cube4Controller(Cube4Service cube4Service)
    {
        _cube4Service = cube4Service;
    }

    /*----------------------Requirement 5----------------------
    ---------http://localhost:5164/api/cube4/requirement5-----------*/
    
    
    [HttpGet("requirement5")]
    public async Task<IActionResult> GetRequirement5(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var result = await _cube4Service.getRequirement5();

            var pagedResult = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(new {
                success = true,
                message = "Requirement 5 has been retrieved successfully",
                total = result.Count,
                currentPage = pageNumber,
                pageSize,
                data = pagedResult
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
