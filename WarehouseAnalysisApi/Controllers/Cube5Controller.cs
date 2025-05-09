using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube5Controller:ControllerBase
{

    private readonly Cube5Service _cube5Service;

    public Cube5Controller(Cube5Service cube5Service)
    {
        _cube5Service = cube5Service;
    }

    /*----------------------Requirement 6----------------------
    ---------http://localhost:5164/api/cube5/requirement6-----------*/
    
    
    [HttpGet("requirement6")]
    public async Task<IActionResult> GetRequirement6(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            var result = await _cube5Service.getRequirement6();

            var pagedResult = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        
            return Ok(new {
                success = true,
                message = "Requirement 6 has been retrieved successfully",
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