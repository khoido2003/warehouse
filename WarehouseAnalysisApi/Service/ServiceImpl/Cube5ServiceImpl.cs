using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube5ServiceImpl: Cube5Service
{
    private Cube5Repository repository;

    public Cube5ServiceImpl(Cube5Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement6()
    {
        var dataList = await repository.getRequirement6Data();

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