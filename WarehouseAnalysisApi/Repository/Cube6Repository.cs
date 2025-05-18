using Microsoft.AnalysisServices.AdomdClient;
using WarehouseAnalysisApi.Config;
using WarehouseAnalysisApi.DTO;

namespace WarehouseAnalysisApi.Repository;

public class Cube6Repository
{
    private readonly AdomdConnectionFactory _connectionFactory;
    private readonly int _commandTimeout = 300; // Tăng timeout lên 5 phút

    public Cube6Repository(AdomdConnectionFactory factory)
    {
        _connectionFactory = factory;
    }

    public async Task<List<Cube6Request8>> getRequirement8Data(int pageSize = 20, int pageNumber = 1)
    {
        string mdxQuery;
        Console.WriteLine($"Repository: Processing page {pageNumber} with size {pageSize}");
        
        if (pageNumber == 1)
        {
            Console.WriteLine("Using FIRST PAGE query logic");
            // Trang đầu tiên - lấy top N thông qua TOPCOUNT
            mdxQuery = @"SELECT NON EMPTY { 
                [Measures].[Day], [Measures].[Month], [Measures].[Quarter], [Measures].[Year], 
                [Measures].[Dim Time Count], [Measures].[Quantity], [Measures].[Fact Inventory Count], 
                [Measures].[Unit Sold], [Measures].[Total Amount], [Measures].[Fact Sale Count] 
            } ON COLUMNS, 
            NON EMPTY 
            TOPCOUNT(
                { (
                    [Dim Customer 3].[City Id].[City Id].ALLMEMBERS *
                    [Dim Customer 3].[Customer Id].[Customer Id].ALLMEMBERS *
                    [Dim Customer 3].[Customer Name].[Customer Name].ALLMEMBERS *
                    [Dim Store 3].[City Name].[City Name].ALLMEMBERS *
                    [Dim Store 3].[States].[States].ALLMEMBERS
                ) }, 
                " + pageSize + @", 
                [Measures].[Total Amount]
            ) 
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS 
            FROM [Cube6] 
            CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        }
        else
        {
            // Chuyển sang lấy dựa trên từng phần trang
            // Cách tiếp cận: Lấy danh sách order by, sau đó lấy phần từ (n-1)*pageSize đến n*pageSize
            Console.WriteLine($"Using simplified pagination approach for page {pageNumber}");
            
            // Tính offset để lấy dữ liệu từ vị trí thích hợp
            int offset = (pageNumber - 1) * pageSize;
            
            // Sử dụng SUBSET và ORDER BY thay vì EXCEPT
            mdxQuery = @"SELECT NON EMPTY { 
                [Measures].[Day], [Measures].[Month], [Measures].[Quarter], [Measures].[Year], 
                [Measures].[Dim Time Count], [Measures].[Quantity], [Measures].[Fact Inventory Count], 
                [Measures].[Unit Sold], [Measures].[Total Amount], [Measures].[Fact Sale Count] 
            } ON COLUMNS, 
            NON EMPTY 
            SUBSET(
                ORDER(
                    { (
                        [Dim Customer 3].[City Id].[City Id].ALLMEMBERS *
                        [Dim Customer 3].[Customer Id].[Customer Id].ALLMEMBERS *
                        [Dim Customer 3].[Customer Name].[Customer Name].ALLMEMBERS *
                        [Dim Store 3].[City Name].[City Name].ALLMEMBERS *
                        [Dim Store 3].[States].[States].ALLMEMBERS
                    ) },
                    [Measures].[Total Amount], DESC
                ),
                " + offset + @", " + pageSize + @"
            )
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS 
            FROM [Cube6] 
            CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
        }

        Console.WriteLine("Executing MDX Query:");
        Console.WriteLine(mdxQuery);

        return await executeMdxQuery(mdxQuery);
    }

    // Phiên bản fallback sử dụng phương pháp đơn giản nhất
    public async Task<List<Cube6Request8>> getSimplifiedRequirement8Data(int pageSize = 20, int pageNumber = 1)
    {
        // Lấy danh sách customer ids
        int offset = (pageNumber - 1) * pageSize;
        
        string mdxQuery = @"SELECT NON EMPTY { 
                [Measures].[Day], [Measures].[Month], [Measures].[Quarter], [Measures].[Year], 
                [Measures].[Dim Time Count], [Measures].[Quantity], [Measures].[Fact Inventory Count], 
                [Measures].[Unit Sold], [Measures].[Total Amount], [Measures].[Fact Sale Count] 
            } ON COLUMNS, 
            NON EMPTY 
            SUBSET(
                ORDER(
                    { ([Dim Customer 3].[Customer Id].[Customer Id].ALLMEMBERS) },
                    [Measures].[Total Amount], DESC
                ),
                " + offset + @", " + pageSize + @"
            )
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS 
            FROM [Cube6] 
            CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";
            
        Console.WriteLine("Executing SIMPLIFIED MDX Query:");
        Console.WriteLine(mdxQuery);
        
        return await executeMdxQuery(mdxQuery);
    }

    private async Task<List<Cube6Request8>> executeMdxQuery(string mdxQuery)
    {
        var results = new List<Cube6Request8>();
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            
            using var command = new AdomdCommand(mdxQuery, connection);
            command.CommandTimeout = _commandTimeout;
            
            Console.WriteLine("Command created, executing reader...");
            DateTime startTime = DateTime.Now;
            using var reader = command.ExecuteReader();
            TimeSpan duration = DateTime.Now - startTime;
            Console.WriteLine($"Reader executed in {duration.TotalSeconds} seconds");
            
            // In ra tên các trường để debug
            var fieldNames = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                Console.WriteLine($"{i}: {fieldName}");
                fieldNames.Add(fieldName);
            }
            
            int counter = 0;
            DateTime readStartTime = DateTime.Now;
            
            while (reader.Read())
            {
                try
                {
                    var dto = new Cube6Request8();

                    // Chỉ thêm các trường nếu chúng tồn tại trong kết quả
                    if (fieldNames.Contains("[Dim Product 3].[Product Id].[Product Id].[MEMBER_CAPTION]"))
                    {
                        dto.productId = reader["[Dim Product 3].[Product Id].[Product Id].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Product 3].[Description].[Description].[MEMBER_CAPTION]"))
                    {
                        dto.description = reader["[Dim Product 3].[Description].[Description].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Store 3].[Store Id].[Store Id].[MEMBER_CAPTION]"))
                    {
                        dto.storeId = reader["[Dim Store 3].[Store Id].[Store Id].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Store 3].[City Name].[City Name].[MEMBER_CAPTION]"))
                    {
                        dto.city = reader["[Dim Store 3].[City Name].[City Name].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Store 3].[States].[States].[MEMBER_CAPTION]"))
                    {
                        dto.state = reader["[Dim Store 3].[States].[States].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Customer 3].[Customer Id].[Customer Id].[MEMBER_CAPTION]"))
                    {
                        dto.customerId = reader["[Dim Customer 3].[Customer Id].[Customer Id].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Customer 3].[Customer Name].[Customer Name].[MEMBER_CAPTION]"))
                    {
                        dto.customerName = reader["[Dim Customer 3].[Customer Name].[Customer Name].[MEMBER_CAPTION]"]?.ToString();
                    }

                    if (fieldNames.Contains("[Dim Customer 3].[City Id].[City Id].[MEMBER_CAPTION]"))
                    {
                        dto.cityId = reader["[Dim Customer 3].[City Id].[City Id].[MEMBER_CAPTION]"]?.ToString();
                    }

                    // Luôn lấy các measures quan trọng
                    if (fieldNames.Contains("[Measures].[Total Amount]"))
                    {
                        dto.totalAmount = reader["[Measures].[Total Amount]"]?.ToString();
                    }
                    
                    if (fieldNames.Contains("[Measures].[Unit Sold]"))
                    {
                        dto.unitsold = reader["[Measures].[Unit Sold]"]?.ToString();
                    }
                    
                    // Add time dimensions
                    if (fieldNames.Contains("[Measures].[Day]"))
                    {
                        dto.day = reader["[Measures].[Day]"]?.ToString();
                    }
                    
                    if (fieldNames.Contains("[Measures].[Month]"))
                    {
                        dto.month = reader["[Measures].[Month]"]?.ToString();
                    }
                    
                    if (fieldNames.Contains("[Measures].[Quarter]"))
                    {
                        dto.quarter = reader["[Measures].[Quarter]"]?.ToString();
                    }
                    
                    if (fieldNames.Contains("[Measures].[Year]"))
                    {
                        dto.year = reader["[Measures].[Year]"]?.ToString();
                    }
                    
                    results.Add(dto);
                    
                    counter++;
                    if (counter % 10 == 0)
                    {
                        Console.WriteLine($"Đã đọc {counter} dòng dữ liệu");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi đọc dòng dữ liệu: {ex.Message}");
                }
            }
            
            TimeSpan readDuration = DateTime.Now - readStartTime;
            Console.WriteLine($"Tổng số dòng dữ liệu đã đọc: {results.Count} trong {readDuration.TotalSeconds} giây");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Lỗi thực thi MDX Query: {e.Message}");
            if (e.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {e.InnerException.Message}");
            }
            
            // Nếu thất bại với cách phức tạp, thử lại với cách đơn giản hơn
            if (!mdxQuery.Contains("SUBSET"))
            {
                Console.WriteLine("Thử lại với phương pháp SUBSET...");
                return await getSimplifiedRequirement8Data(20, 1);
            }
            
            throw new Exception("Lỗi khi thực thi MDX query: " + e.Message, e);
        }
        
        return results;
    }
}