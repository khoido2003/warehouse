using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube2Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;

    public Cube2Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    /*
     public async Task<List<Cube2Request2>> getRequirement2Data()
    {
        string mdxQuery = @"SELECT NON EMPTY { [Measures].[Total Amount] } ON COLUMNS, 
                NON EMPTY { 
                    ([Dim Customer].[Customer ID].[Customer ID].ALLMEMBERS *
                     [Dim Customer].[Customer Name].[Customer Name].ALLMEMBERS * 
                     [Dim Time].[Year].[Year].ALLMEMBERS * 
                     [Dim Time].[Quarter].[Quarter].ALLMEMBERS * 
                     [Dim Time].[Month].[Month].ALLMEMBERS * 
                     [Dim Time].[Day].[Day].ALLMEMBERS) 
                } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube2] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }
     */
    
    
    public async Task<List<Cube2Request2>> getRequirement2Data(string customerName)
    {
        
        string mdxQuery = $@"
        SELECT NON EMPTY {{ [Measures].[Total Amount] }} ON COLUMNS, 
        NON EMPTY {{ 
            FILTER(
                (
                    [Dim Customer].[Customer ID].[Customer ID].ALLMEMBERS *
                    [Dim Customer].[Customer Name].[Customer Name].ALLMEMBERS * 
                    [Dim Time].[Year].[Year].ALLMEMBERS * 
                    [Dim Time].[Quarter].[Quarter].ALLMEMBERS * 
                    [Dim Time].[Month].[Month].ALLMEMBERS * 
                    [Dim Time].[Day].[Day].ALLMEMBERS
                ),
                INSTR([Dim Customer].[Customer Name].CURRENTMEMBER.MEMBER_CAPTION, ""{customerName}"") > 0
            )
        }} 
        DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
        ON ROWS 
        FROM [Cube2] 
        CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        return await executeMdxQuery(mdxQuery);
    }


    private async Task<List<Cube2Request2>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube2Request2>();
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
                var dto = new Cube2Request2
                {
                    
                    customerId = reader["[Dim Customer].[Customer Id].[Customer Id].[MEMBER_CAPTION]"]?.ToString(),
                    customerName = reader["[Dim Customer].[Customer Name].[Customer Name].[MEMBER_CAPTION]"]?.ToString(),
                    year = reader["[Dim Time].[Year].[Year].[MEMBER_CAPTION]"]?.ToString(),
                    quarter = reader["[Dim Time].[Quarter].[Quarter].[MEMBER_CAPTION]"]?.ToString(),
                    month = reader["[Dim Time].[Month].[Month].[MEMBER_CAPTION]"]?.ToString(),
                    day = reader["[Dim Time].[Day].[Day].[MEMBER_CAPTION]"]?.ToString(),
                    totalAmount = reader["[Measures].[Total Amount]"]?.ToString()
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