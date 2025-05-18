using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube4Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube4Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube4Request5>> getRequirement5Data()
    {
        // Simplified query without time measures
        string mdxQuery = @"SELECT NON EMPTY { 
                [Measures].[Customer Id], 
                [Measures].[Unit Sold], 
                [Measures].[Total Amount]
            } ON COLUMNS, 
            NON EMPTY { 
                (
                    [Dim Product 2].[Product Id].[Product Id].ALLMEMBERS *
                    [Dim Product 2].[Description].[Description].ALLMEMBERS * 
                    [Dim Store 2].[Store Id].[Store Id].ALLMEMBERS
                )
            } 
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS 
            FROM [Cube4] 
            CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube4Request5>> executeMdxQuery(string mdxQuery)
    {
        var tempResults = new List<Cube4Request5>();
        try
        {
            var connection = _connectionFactory.CreateConnection();
            using var command = new AdomdCommand(mdxQuery, connection);
            var reader = command.ExecuteReader();
            
            // Get field names to check which ones are available
            var availableFields = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                Console.WriteLine($"{i}: {fieldName}");
                availableFields.Add(fieldName);
            }

            while (reader.Read())
            {
                var dto = new Cube4Request5
                {
                    storeId = reader["[Dim Store 2].[Store Id].[Store Id].[MEMBER_CAPTION]"]?.ToString(),
                    customerId = reader["[Measures].[Customer Id]"]?.ToString(),
                    productId = reader["[Dim Product 2].[Product Id].[Product Id].[MEMBER_CAPTION]"]?.ToString(),
                    description = reader["[Dim Product 2].[Description].[Description].[MEMBER_CAPTION]"]?.ToString(),
                    totalAmount = reader["[Measures].[Total Amount]"]?.ToString(),
                    unitSold = reader["[Measures].[Unit Sold]"]?.ToString(),
                };
                
                tempResults.Add(dto);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
        }
        
        // Group by composite key to eliminate duplicates
        var results = tempResults
            .GroupBy(item => new { 
                item.storeId, 
                item.productId, 
                item.customerId
            })
            .Select(group => group.First())
            .ToList();
            
        return results;
    }
}