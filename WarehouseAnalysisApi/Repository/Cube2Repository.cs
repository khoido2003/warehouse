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

    
     public async Task<List<Cube2Request2>> getRequirement2Data(string time)
     {

         string timefilter = "";
         Console.WriteLine(time);
         switch (time)
         {
             case "day":
                 timefilter = "[Dim Time].[Hierarchy].[day].ALLMEMBERS";
                 break;
             case "month":
                 timefilter = "[Dim Time].[Hierarchy].[month].ALLMEMBERS";
                 break;
             case "quarter":
                 timefilter = "[Dim Time].[Hierarchy].[quarter].ALLMEMBERS";
                 break;
             case "year":
                 timefilter = "[Dim Time].[Hierarchy].[year].ALLMEMBERS";
                 break;
             default:
                 timefilter = "[Dim Time].[Year].[Year].ALLMEMBERS * " +
                              "[Dim Time].[Quarter].[Quarter].ALLMEMBERS * " +
                              "[Dim Time].[Month].[Month].ALLMEMBERS * " +
                              "[Dim Time].[Day].[Day].ALLMEMBERS";
                 break;
         }
         
         
        string mdxQuery = $@"SELECT NON EMPTY {{ [Measures].[Total Amount] }} ON COLUMNS, 
                NON EMPTY {{
                    ([Dim Customer].[Customer ID].[Customer ID].ALLMEMBERS *
                     [Dim Customer].[Customer Name].[Customer Name].ALLMEMBERS * 
                     {timefilter}) 
                }}
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube2] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        
        Console.WriteLine(mdxQuery);
        return await executeMdxQuery(mdxQuery);
    }
     
    
    /*
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
*/

    private async Task<List<Cube2Request2>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube2Request2>();
        try
        {
            var connection = _connectionFactory.CreateConnection();
            using var command = new AdomdCommand(mdxQuery, connection);
            var reader = command.ExecuteReader();

            var availableFields = new HashSet<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                availableFields.Add(fieldName);
                Console.WriteLine($"{i}: {fieldName}");
            }

            while (reader.Read())
            {
                var dto = new Cube2Request2
                {
                    customerId = reader["[Dim Customer].[Customer Id].[Customer Id].[MEMBER_CAPTION]"]?.ToString(),
                    customerName = reader["[Dim Customer].[Customer Name].[Customer Name].[MEMBER_CAPTION]"]?.ToString(),
                    totalAmount = reader["[Measures].[Total Amount]"]?.ToString()
                };

                if (availableFields.Contains("[Dim Time].[Hierarchy].[Year].[MEMBER_CAPTION]"))
                    dto.year = reader["[Dim Time].[Hierarchy].[Year].[MEMBER_CAPTION]"]?.ToString();
                
                if (availableFields.Contains("[Dim Time].[Year].[Year].[MEMBER_CAPTION]"))
                    dto.year = reader["[Dim Time].[Year].[Year].[MEMBER_CAPTION]"]?.ToString();
                
                if (availableFields.Contains("[Dim Time].[Quarter].[Quarter].[MEMBER_CAPTION]"))
                    dto.quarter = reader["[Dim Time].[Quarter].[Quarter].[MEMBER_CAPTION]"]?.ToString();
                
                if (availableFields.Contains("[Dim Time].[Hierarchy].[Quarter].[MEMBER_CAPTION]"))
                    dto.quarter = reader["[Dim Time].[Hierarchy].[Quarter].[MEMBER_CAPTION]"]?.ToString();

                if (availableFields.Contains("[Dim Time].[Month].[Month].[MEMBER_CAPTION]"))
                    dto.month = reader["[Dim Time].[Month].[Month].[MEMBER_CAPTION]"]?.ToString();
                
                if (availableFields.Contains("[Dim Time].[Hierarchy].[Month].[MEMBER_CAPTION]"))
                    dto.month = reader["[Dim Time].[Hierarchy].[Month].[MEMBER_CAPTION]"]?.ToString();

                if (availableFields.Contains("[Dim Time].[Day].[Day].[MEMBER_CAPTION]"))
                    dto.day = reader["[Dim Time].[Day].[Day].[MEMBER_CAPTION]"]?.ToString();
                
                if (availableFields.Contains("[Dim Time].[Hierarchy].[Day].[MEMBER_CAPTION]"))
                    dto.day = reader["[Dim Time].[Hierarchy].[Day].[MEMBER_CAPTION]"]?.ToString();

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