using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube6ServiceImpl: Cube6Service
{
    private Cube6Repository repository;

    public Cube6ServiceImpl(Cube6Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement8(int pageNumber = 1, int pageSize = 20)
    {
        var dataList = await repository.getRequirement8Data(pageSize, pageNumber);

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            {"city", item.city},
            {"state", item.state},
            {"customerId", item.customerId},
            {"customerName", item.customerName},
            {"totalAmount", item.totalAmount},
            {"unitSold", item.unitsold},
            {"day", item.day},
            {"month", item.month},
            {"quarter", item.quarter},
            {"year", item.year}
        }).ToList();
        
        return result;
    }
}