using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube5Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube5Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube5Request6>> getRequirement6Data()
    {

        string mdxQuery = @"SELECT NON EMPTY { [Measures].[Dim Customer Count]} ON COLUMNS, 
                NON EMPTY {{
                    (
                     [Dim City].[City Name].[City Name].ALLMEMBERS *
                     [Dim City].[States].[States].ALLMEMBERS * 
                     [Dim Customer 2].[Customer Name].[Customer Name].ALLMEMBERS*
					 [Dim Customer 2].[Customer Id].[Customer Id].ALLMEMBERS
					 )
                }} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube5] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube5Request6>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube5Request6>();
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
                var dto = new Cube5Request6
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