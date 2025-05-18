using System.Security.Authentication;
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
    
        public async Task<(List<Cube1Request1> Result, List<Cube1Request1> Chart)> getRequirement1Data(string city, string state, int? minPrice = null, int? maxPrice = null)
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
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS;";
            
            string mdxQuery2 = $@"
                SELECT 
                  NON EMPTY {{ [Measures].[Quantity] }} ON COLUMNS, 
                  NON EMPTY 
                    TopCount(
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
                      ),
                      10,
                      [Measures].[Quantity]
                    ) 
                  DIMENSION PROPERTIES 
                    MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                  ON ROWS 
                FROM [Cube1] 
                CELL PROPERTIES 
                  VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS;";
            
            Console.WriteLine(mdxQuery2);
            return await executeMdxQueryRequirement1(mdxQuery, mdxQuery2);
        }



        private async Task<(List<Cube1Request1> Result, List<Cube1Request1> Chart)> executeMdxQueryRequirement1(string mdxQuery, string mdxQuery2)
        {
            var result = new List<Cube1Request1>();
            var chart = new List<Cube1Request1>();
            try
            {
                var connection = _connectionFactory.CreateConnection();
                
                using (var command = new AdomdCommand(mdxQuery, connection))
                using (var reader = command.ExecuteReader())
                
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
                
                using (var command = new AdomdCommand(mdxQuery2, connection))
                using (var reader = command.ExecuteReader())
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
                    
                    chart.Add(dto);
                }
                
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
            }
            
            return (result, chart);
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
        
        /* -----------------------Request 7 -------------------------*/
    
    public async Task<List<Cube1Request7>> getRequirement7Data(string city, string state)
    {
        string mdxQuery = $@"
                SELECT 
	                NON EMPTY {{ [Measures].[Quantity] }} ON COLUMNS, 
	                NON EMPTY {{
		                FILTER(
			                (
				                [Dim Store].[Store Id].[Store Id].ALLMEMBERS * 
                                [Dim Store].[States].[States].ALLMEMBERS * 
                                [Dim Store].[City Name].[City Name].ALLMEMBERS * 
                                [Dim Product].[Product Id].[Product Id].ALLMEMBERS * 
                                [Dim Product].[Description].[Description].ALLMEMBERS
			                ),
			                INSTR([Dim Store].[City Name].CURRENTMEMBER.MEMBER_CAPTION, ""{city}"") > 0
			                AND INSTR([Dim Store].[States].CURRENTMEMBER.MEMBER_CAPTION, ""{state}"") > 0
		                )
	                }} 
	                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
	                ON ROWS 
	                FROM [Cube1] 
	                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS
";

        return await executeMdxQueryRequirement7(mdxQuery);
    }
    
    private async Task<List<Cube1Request7>> executeMdxQueryRequirement7(string mdxQuery)
    {
        var result = new List<Cube1Request7>();

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
                var dto = new Cube1Request7
                {
                    productId = reader["[Dim Product].[Product Id].[Product Id].[MEMBER_CAPTION]"]?.ToString(),
                    description = reader["[Dim Product].[Description].[Description].[MEMBER_CAPTION]"]?.ToString(),
                    storeId = reader["[Dim Store].[Store Id].[Store Id].[MEMBER_CAPTION]"]?.ToString(),
                    city = reader["[Dim Store].[City Name].[City Name].[MEMBER_CAPTION]"]?.ToString(),
                    state = reader["[Dim Store].[States].[States].[MEMBER_CAPTION]"]?.ToString(),
                    quantity = reader["[Measures].[Quantity]"]?.ToString()
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

