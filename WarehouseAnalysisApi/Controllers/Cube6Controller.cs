using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube6Controller : ControllerBase
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
            Console.WriteLine($"Requesting page {pageNumber} with size {pageSize}");
            
            // Always pass the pageNumber parameter to ensure the correct branch is taken in the repository
            var result = await _cube6Service.getRequirement8(pageNumber, pageSize);

            if (result == null || result.Count == 0)
            {
                return Ok(
                    new
                    {
                        success = true,
                        message = "No data found for Requirement 8",
                        total = 0,
                        currentPage = pageNumber,
                        pageSize,
                        data = new List<Dictionary<string, object>>(),
                    }
                );
            }

            // Only get total count when needed (first page or when result count is less than page size)
            int totalCount;
            if (pageNumber == 1 || result.Count < pageSize)
            {
                var allData = await _cube6Service.getRequirement8(1, 1000);
                totalCount = allData.Count;
            }
            else
            {
                // Approximate total based on current page data
                totalCount = (pageNumber * pageSize) + pageSize; // Estimate at least one more page
            }

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 8 has been retrieved successfully",
                    total = totalCount,
                    currentPage = pageNumber,
                    pageSize,
                    data = result,
                }
            );
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in Cube6Controller.GetRequirement6: {e.Message}");
            if (e.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
            }
            
            return StatusCode(500, new
            {
                success = false,
                message = "Error retrieving data for Requirement 8",
                error = e.Message,
                stackTrace = e.StackTrace
            });
        }
    }
}

