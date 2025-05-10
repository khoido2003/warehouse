namespace WarehouseAnalysisApi.Service;

public interface Cube3Service
{
    Task<List<Dictionary<string, object>>> getRequirement3(string city, string state);
}