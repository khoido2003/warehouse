using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube2ServiceImpl: Cube2Service
{
    private Cube2Repository repository;

    public Cube2ServiceImpl(Cube2Repository repository)
    {
        this.repository = repository;
    }
    
    public async Task<List<Dictionary<string, object>>> getRequirement2(string time)
    {
        var dataList = await repository.getRequirement2Data(time);

        var result = dataList.Select(item =>
        {
            var dict = new Dictionary<string, object>
            {
                { "customerId", item.customerId },
                { "customerName", item.customerName },
                { "totalAmount", item.totalAmount },
            };
            
            if(item.day != null) dict.Add("day", item.day);
            if(item.month != null) dict.Add("month", item.month);
            if(item.quarter != null) dict.Add("quarter", item.quarter);
            if(item.year != null) dict.Add("year", item.year);
            return dict;
        }).ToList();
        
        return result;
    }
}