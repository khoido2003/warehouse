using Microsoft.AspNetCore.Mvc;
using WarehouseAnalysisApi.Service;

namespace WarehouseAnalysisApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Cube1Controller : ControllerBase
{
    private readonly Cube1Service _cube1Service;

    public Cube1Controller(Cube1Service cube1Service)
    {
        _cube1Service = cube1Service;
    }

    /*----------------------Requirement 1----------------------
    ---------http://localhost:5164/api/cube1v2/requirement1-----------*/


    [HttpGet("requirement1")]
    public async Task<IActionResult> GetRequirement1(
        int pageNumber = 1,
        int pageSize = 20,
        string city = "",
        string state = "",
        string price = ""
    )
    {
        try
        {
            var result = await _cube1Service.getRequirement1(city, state, price);

            var pagedResult = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 1 has been retrieved successfully",
                    total = result.Count,
                    currentPage = pageNumber,
                    pageSize,
                    data = pagedResult,
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", error = ex.Message }
            );
        }
    }

    /*----------------------Requirement 4----------------------
    ---------http://localhost:5164/api/cube1v2/requirement4-----------*/

    [HttpGet("requirement4")]
    public async Task<IActionResult> GetRequirement4(
        int pageNumber = 1,
        int pageSize = 20,
        string city = "",
        string state = ""
    )
    {
        try
        {
            var result = await _cube1Service.getRequirement4(city, state);

            var pagedResult = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 4 has been retrieved successfully",
                    total = result.Count,
                    currentPage = pageNumber,
                    pageSize,
                    data = pagedResult,
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", error = ex.Message }
            );
        }
    }

    /*-------------------------Requirement 7-----------------------------
     ---------http://localhost:5164/api/cube1v2/requirement7-----------*/

    [HttpGet("requirement7")]
    public async Task<IActionResult> GetRequirement7(
        int pageNumber = 1,
        int pageSize = 20,
        string city = "",
        string state = ""
    )
    {
        try
        {
            var result = await _cube1Service.getRequirement7(city, state);

            var pagedResult = result.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return Ok(
                new
                {
                    success = true,
                    message = "Requirement 4 has been retrieved successfully",
                    total = result.Count,
                    currentPage = pageNumber,
                    pageSize,
                    data = pagedResult,
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { message = "Đã xảy ra lỗi khi xử lý yêu cầu.", error = ex.Message }
            );
        }
    }
}
