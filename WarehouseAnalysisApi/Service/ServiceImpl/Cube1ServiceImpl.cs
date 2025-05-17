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

        public async Task<List<Dictionary<string, object>>> getRequirement1(string city, string state, string price)
        {
            var dataList = new List<Cube1Request1>();
            if (price != "")
            {
                string[] prices = price.Split('-');
                int minPriceInt = Convert.ToInt32(prices[0]);
                int maxPriceInt = Convert.ToInt32(prices[1]);
                dataList = await repository.getRequirement1Data(city, state, minPriceInt, maxPriceInt);
            } else dataList = await repository.getRequirement1Data(city, state);

            var result = dataList.Select(item => new Dictionary<string, object>
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

            return result;
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