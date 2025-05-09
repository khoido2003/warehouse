using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository
{
    public class Cube1Repository
    {
        private readonly AdomdConnectionFactory _connectionFactory;

        public Cube1Repository(AdomdConnectionFactory factory)
        {
            _connectionFactory = factory;
        }

        // Request 1
    /*    public async Task<List<Cube1Request1>> getRequirement1Data()
        {
            string mdxQuery = @"SELECT NON EMPTY { [Measures].[Quantity] } ON COLUMNS, 
                NON EMPTY { 
                    ([Dim Store].[Store Id].[Store Id].ALLMEMBERS * 
                    [Dim Store].[States].[States].ALLMEMBERS * 
                    [Dim Store].[City Name].[City Name].ALLMEMBERS * 
                    [Dim Product].[Description].[Description].ALLMEMBERS * 
                    [Dim Product].[Weight].[Weight].ALLMEMBERS * 
                    [Dim Product].[Size].[Size].ALLMEMBERS * 
                    [Dim Product].[Price].[Price].ALLMEMBERS ) 
                } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS FROM [Cube1] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

            return await executeMdxQueryRequirement1(mdxQuery);
        }
        
        */
    
        public async Task<List<Cube1Request1>> getRequirement1Data(string city, string state, int? minPrice = null, int? maxPrice = null)
        {
            string priceFilter = "";
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                decimal min = Convert.ToDecimal(minPrice.Value);
                decimal max = Convert.ToDecimal(maxPrice.Value);

                priceFilter = $@"
                AND Val([Dim Product].[Price].CURRENTMEMBER.MEMBER_CAPTION) >= {min}
                AND Val([Dim Product].[Price].CURRENTMEMBER.MEMBER_CAPTION) <= {max}";
            }

            string mdxQuery = $@"
                SELECT NON EMPTY {{ [Measures].[Quantity] }} ON COLUMNS, 
                NON EMPTY {{
                    FILTER(
                        (
                            [Dim Store].[Store Id].[Store Id].ALLMEMBERS * 
                            [Dim Store].[States].[States].ALLMEMBERS * 
                            [Dim Store].[City Name].[City Name].ALLMEMBERS * 
                            [Dim Product].[Description].[Description].ALLMEMBERS * 
                            [Dim Product].[Weight].[Weight].ALLMEMBERS * 
                            [Dim Product].[Size].[Size].ALLMEMBERS * 
                            [Dim Product].[Price].[Price].ALLMEMBERS
                        ),
                        INSTR([Dim Store].[City Name].CURRENTMEMBER.MEMBER_CAPTION, ""{city}"") > 0
                        AND INSTR([Dim Store].[States].CURRENTMEMBER.MEMBER_CAPTION, ""{state}"") > 0
                        {priceFilter}
                    )
                }}
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube1] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

            return await executeMdxQueryRequirement1(mdxQuery);
        }



        private async Task<List<Cube1Request1>> executeMdxQueryRequirement1(string mdxQuery)
        {
            var result = new List<Cube1Request1>();

            try
            {
                var connection = _connectionFactory.CreateConnection();
                using var command = new AdomdCommand(mdxQuery, connection);
                using var reader = command.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"{i}: {reader.GetName(i)}");
                }
                
                while (reader.Read())
                {
                    var dto = new Cube1Request1
                    {
                        storeId = reader["[Dim Store].[Store Id].[Store Id].[MEMBER_CAPTION]"]?.ToString(),
                        state = reader["[Dim Store].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                        city = reader["[Dim Store].[City Name].[City Name].[MEMBER_CAPTION]"]?.ToString(),
                        productDescription = reader["[Dim Product].[Description].[Description].[MEMBER_CAPTION]"]?.ToString(),
                        weight = reader["[Dim Product].[Weight].[Weight].[MEMBER_CAPTION]"]?.ToString(),
                        size = reader["[Dim Product].[Size].[Size].[MEMBER_CAPTION]"]?.ToString(),
                        price = reader["[Dim Product].[Price].[Price].[MEMBER_CAPTION]"]?.ToString(),
                        quantity = reader.IsDBNull(reader.GetOrdinal("[Measures].[Quantity]")) 
                        ? 0 
                        : Convert.ToInt32(reader["[Measures].[Quantity]"])
                    };
                    
                    result.Add(dto);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
            }
            
            return result;
        }
        
        // Request 4
        public async Task<List<Cube1Request4>> getRequirement4Data(string city, string state)
        {

            string mdxQuery = $@"
                SELECT 
                    NON EMPTY {{ [Measures].[Quantity] }} ON COLUMNS, 
                    NON EMPTY {{
                        FILTER(
                            (
                                [Dim Store].[Office Addr].[Office Addr].ALLMEMBERS * 
                                [Dim Store].[City Name].[City Name].ALLMEMBERS * 
                                [Dim Store].[States].[States].ALLMEMBERS
                            ),
                            INSTR([Dim Store].[City Name].CURRENTMEMBER.MEMBER_CAPTION, ""{city}"") > 0
                            AND INSTR([Dim Store].[States].CURRENTMEMBER.MEMBER_CAPTION, ""{state}"") > 0
                        )
                    }} 
                    DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                    ON ROWS 
                FROM [Cube1] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

            return await executeMdxQueryRequirement4(mdxQuery);
        }


        private async Task<List<Cube1Request4>> executeMdxQueryRequirement4(string mdxQuery)
        {
            var result = new List<Cube1Request4>();

            try
            {
                var connection = _connectionFactory.CreateConnection();
                using var command = new AdomdCommand(mdxQuery, connection);
                using var reader = command.ExecuteReader();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.WriteLine($"{i}: {reader.GetName(i)}");
                }
                
                while (reader.Read())
                {
                    var dto = new Cube1Request4
                    {
                        
                        state = reader["[Dim Store].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                        city = reader["[Dim Store].[City Name].[City Name].[MEMBER_CAPTION]"]?.ToString(),
                        officeAddress = reader["[Dim Store].[Office Addr].[Office Addr].[MEMBER_CAPTION]"]?.ToString(),
                        quantity = reader.IsDBNull(reader.GetOrdinal("[Measures].[Quantity]")) 
                        ? 0 
                        : Convert.ToInt32(reader["[Measures].[Quantity]"])
                    };
                    
                    result.Add(dto);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
            }
            
            return result;
        }
    }
}
