using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube2ServiceImpl: Cube2Service
{
    private Cube2Repository repository;

    public Cube2ServiceImpl(Cube2Repository repository)
    {
        this.repository = repository;
    }
    
    public async Task<List<Dictionary<string, object>>> getRequirement2(string name)
    {
        var dataList = await repository.getRequirement2Data(name);

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            {"customerId", item.customerId},
            { "customerName", item.customerName },
            { "day", item.day },
            { "month", item.month },
            { "quarter", item.quarter },
            { "year", item.year },
            { "totalAmount", item.totalAmount },
        }).ToList();
        
        return result;
    }
}