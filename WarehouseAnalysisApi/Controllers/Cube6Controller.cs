using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class Cube6Controller:ControllerBase
{
    private readonly Cube6Service _cube6Service;

    public Cube6Controller(Cube6Service cube6Service)
    {
        _cube6Service = cube6Service;
    }

    /*----------------------Requirement 8----------------------
    ---------http://localhost:5164/api/cube6/requirement8-----------*/
    
    
    [HttpGet("requirement8")]
    public async Task<IActionResult> GetRequirement6(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var result = await _cube6Service.getRequirement8();

            var pagedResult = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        
            return Ok(new {
                success = true,
                message = "Requirement 8 has been retrieved successfully",
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