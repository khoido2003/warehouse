namespace WarehouseAnalysisApi.Service;

public interface Cube2Service
{
    Task<List<Dictionary<string, object>>> getRequirement2(string name);
}