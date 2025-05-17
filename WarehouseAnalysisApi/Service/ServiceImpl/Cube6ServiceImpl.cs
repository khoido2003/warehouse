using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube6ServiceImpl: Cube6Service
{
    private Cube6Repository repository;

    public Cube6ServiceImpl(Cube6Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement8()
    {
        var dataList = await repository.getRequirement8Data();

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            {"customerId", item.customerId},
            {"customerName", item.customerName},
            {"city", item.city},
            {"state", item.state},
        }).ToList();
        
        return result;
    }
}