using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube7Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube7Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube7Request9>> getRequirement9Data()
    {

        string mdxQuery = @"SELECT NON EMPTY {[Measures].[Dim Customer Count]} ON COLUMNS, 
                NON EMPTY {{
                    (
                     [Dim Customer 4].[Customer Id].[Customer Id].ALLMEMBERS*
					 [Dim Customer 4].[Customer Name].[Customer Name].ALLMEMBERS *
					 [Dim Customer 4].[Post].[Post].ALLMEMBERS *
                     [Dim Customer 4].[Travel].[Travel].ALLMEMBERS
					 )
                }} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube7] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube7Request9>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube7Request9>();
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
                var dto = new Cube7Request9
                {
                    customerId = reader["[Dim Customer 4].[Customer Id].[Customer Id].[MEMBER_CAPTION]"]?.ToString(),
                    customerName = reader["[Dim Customer 4].[Customer Name].[Customer Name].[MEMBER_CAPTION]"]?.ToString(),
                    post = reader["[Dim Customer 4].[Post].[Post].[MEMBER_CAPTION]"]?.ToString(),
                    travel = reader["[Dim Customer 4].[Travel].[Travel].[MEMBER_CAPTION]"]?.ToString(),
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