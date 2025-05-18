namespace WarehouseAnalysisApi.Service;

public interface Cube1Service
{
    Task<(List<Dictionary<string, object>> result, 
        List<Dictionary<string, object>> chart)> getRequirement1(string city, string state, string price);
    Task<List<Dictionary<string, object>>> getRequirement4(string city, string state);
    Task<List<Dictionary<string, object>>> getRequirement7(string city, string state);
}