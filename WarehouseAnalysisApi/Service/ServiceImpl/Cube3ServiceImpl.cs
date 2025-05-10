using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl;

public class Cube3ServiceImpl: Cube3Service
{
    private Cube3Repository repository;

    public Cube3ServiceImpl(Cube3Repository repository)
    {
        this.repository = repository;
    }

    public async Task<List<Dictionary<string, object>>> getRequirement3(string city, string state)
    {
        
        Console.WriteLine(city + " " + state);
        var dataList = await repository.getRequirement3Data(city, state);

        var result = dataList.Select(item => new Dictionary<string, object>
        {
            {"phone", item.phone},
            { "city", item.city },
            { "state", item.state },
            { "unitSold", item.unitSold },
            { "totalAmount", item.totalAmount },
        }).ToList();
        
        return result;
    }
}