namespace WarehouseAnalysisApi.Service;

public interface Cube6Service
{
    Task<List<Dictionary<string, object>>> getRequirement8(int pageNumber = 1, int pageSize = 20);
}