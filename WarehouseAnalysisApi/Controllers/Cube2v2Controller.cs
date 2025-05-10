using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;
[ApiController]
[Route("api/v2/[controller]")]
public class Cube2v2Controller:ControllerBase
{
    private readonly Cube2Service _cube2Service;

    public Cube2v2Controller(Cube2Service cube2Service)
    {
        _cube2Service = cube2Service;
    }

    /*----------------------Requirement 2----------------------
    ---------http://localhost:5164/api/v2/cube2v2/requirement2-----------*/
    
    [HttpGet("requirement2")]
    public async Task<IActionResult> GetRequirement2(int pageNumber = 1, int pageSize = 20, string time = null)
    {
        try
        {
            var result = await _cube2Service.getRequirement2(time);

            var pagedResult = result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            return Ok(new {
                success = true,
                message = "Requirement 2 has been retrieved successfully",
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