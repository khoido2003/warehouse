using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube4ServiceImpl: Cube4Service
{
    private Cube4Repository repository;

    public Cube4ServiceImpl(Cube4Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement5()
    {
        var dataList = await repository.getRequirement5Data();

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            
            { "storeId", item.storeId },
            { "customerId", item.customerId },
            { "productId", item.productId },
            { "description", item.description },
            { "unitSold", item.unitSold },
            { "totalAmount", item.totalAmount },
        }).ToList();
        
        return result;
    }
}