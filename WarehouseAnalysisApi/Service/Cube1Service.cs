namespace WarehouseAnalysisApi.Service;

public interface Cube1Service
{
    Task<List<Dictionary<string, object>>> getRequirement1(string city, string state, string price);
    Task<List<Dictionary<string, object>>> getRequirement4(string city, string state);
}