using System.Security.Authentication;
using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube3Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube3Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube3Request3>> getRequirement3Data(string city, string state)
    {
        string mdxQuery = $@"
                SELECT NON EMPTY {{ [Measures].[Total Amount], [Measures].[Unit Sold] }} ON COLUMNS, 
                NON EMPTY {{ 
                    FILTER(
                        ([Dim Store 1].[Phone].[Phone].ALLMEMBERS *
                         [Dim Store 1].[City Name].[City Name].ALLMEMBERS * 
                         [Dim Store 1].[States].[States].ALLMEMBERS),
                        INSTR([Dim Store 1].[City Name].CURRENTMEMBER.MEMBER_CAPTION, ""{city}"") > 0
                        AND INSTR([Dim Store 1].[States].CURRENTMEMBER.MEMBER_CAPTION, ""{state}"") > 0
                    )
                }} 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube3] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube3Request3>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube3Request3>();
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
                var dto = new Cube3Request3
                {
                    phone = reader["[Dim Store 1].[Phone].[Phone].[MEMBER_CAPTION]"]?.ToString(),
                    city = reader["[Dim Store 1].[City Name].[City Name].[MEMBER_CAPTION]"]?.ToString(),
                    state = reader["[Dim Store 1].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                    unitSold = reader["[Measures].[Unit Sold]"]?.ToString(),
                    totalAmount =  reader["[Measures].[Unit Sold]"]?.ToString()
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