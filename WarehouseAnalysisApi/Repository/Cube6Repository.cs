using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube6Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube6Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube6Request8>> getRequirement8Data()
    {

        string mdxQuery = @"SELECT NON EMPTY {{ [Measures].[Total Amount], [Measures].[Unit Sold] }} ON COLUMNS, 
                NON EMPTY {{
                    (
                     [Dim Product 3].[Product Id].[Product Id].ALLMEMBERS*
					 [Dim Product 3].[Description].[Description].ALLMEMBERS *
					 [Dim Store 3].[Store Id].[Store Id].ALLMEMBERS *
                     [Dim Store 3].[City Name].[City Name].ALLMEMBERS *
                     [Dim Store 3].[States].[States].ALLMEMBERS*
                     [Dim Customer 3].[Customer Id].[Customer Id].ALLMEMBERS*
                     [Dim Customer 3].[Customer Name].[Customer Name].ALLMEMBERS
					 )
                }} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube6] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube6Request8>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube6Request8>();
        try
        {
            var connection = _connectionFactory.CreateConnection();
            using var command = new AdomdCommand(mdxQuery, connection);
            var reader = command.ExecuteReader();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine($"{i}: {reader.GetName(i)}");
            }
        

            while (reader.Read())
            {
                var dto = new Cube6Request8
                {
                    customerId = reader["[Dim Customer 2].[Customer Id].[Customer Id].[MEMBER_CAPTION]"]?.ToString(),
                    customerName = reader["[Dim Customer 2].[Customer Name].[Customer Name].[MEMBER_CAPTION]"]?.ToString(),
                    city = reader["[Dim City].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                    state = reader["[Dim City].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                };
                results.Add(dto);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
        }
        
        return results;
    }
}