using WarehouseAnalysisApi.DTO;
using WarehouseAnalysisApi.Repository;

namespace WarehouseAnalysisApi.Service.ServiceImpl
{
    public class Cube1ServiceImpl : Cube1Service
    {
        private readonly Cube1Repository repository;

        public Cube1ServiceImpl(Cube1Repository cube1Repository)
        {
            this.repository = cube1Repository;
        }

        public async Task<(List<Dictionary<string, object>> result, 
            List<Dictionary<string, object>> chart)> getRequirement1(string city, string state, string price)
        {
            List<Cube1Request1> dataList, chartList;

            if (!string.IsNullOrWhiteSpace(price))
            {
                var prices = price.Split('-');
                if (prices.Length == 2 &&
                    int.TryParse(prices[0], out int minPrice) &&
                    int.TryParse(prices[1], out int maxPrice))
                {
                    (dataList, chartList) = await repository.getRequirement1Data(city, state, minPrice, maxPrice);
                }
                else
                {
                    throw new ArgumentException("Invalid price range format. Expected format: 'min-max'");
                }
            }
            else
            {
                (dataList, chartList) = await repository.getRequirement1Data(city, state);
            }

            List<Dictionary<string, object>> ConvertToDict(List<Cube1Request1> list) =>
                list.Select(item => new Dictionary<string, object>
                {
                    { "storeId", item.storeId },
                    { "state", item.state },
                    { "city", item.city },
                    { "productDescription", item.productDescription },
                    { "weight", item.weight },
                    { "size", item.size },
                    { "price", item.price },
                    { "quantity", item.quantity }
                }).ToList();

            return (ConvertToDict(dataList), ConvertToDict(chartList));
        }

        public async Task<List<Dictionary<string, object>>> getRequirement4(string city, string state)
        {
            var dataList = await repository.getRequirement4Data(city, state);

            var result = dataList.Select(item => new Dictionary<string, object>
            {
                { "state", item.state },
                { "city", item.city },
                { "officeAddress", item.officeAddress },
                { "inventoryQuantity", item.quantity }
            }).ToList();

            return result;
        }
        
        public async Task<List<Dictionary<string, object>>> getRequirement7(string city, string state)
        {
            var dataList = await repository.getRequirement7Data(city, state);

            var result = dataList.Select(item => new Dictionary<string, object>
            {
                {"productId", item.productId },
                {"description", item.description },
                {"storeId", item.storeId },
                { "city", item.city },
                { "state", item.state },
                { "inventoryQuantity", item.quantity }
            }).ToList();

            return result;
        }
    }
}
