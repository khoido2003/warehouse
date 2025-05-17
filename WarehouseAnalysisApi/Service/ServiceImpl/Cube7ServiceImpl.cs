using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube7ServiceImpl: Cube7Service
{
    private Cube7Repository repository;

    public Cube7ServiceImpl(Cube7Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement9()
    {
        var dataList = await repository.getRequirement9Data();

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            {"customerId", item.customerId},
            {"customerName", item.customerName},
            {"post", item.post},
            {"travel", item.travel}
        }).ToList();
        
        return result;
    }
}