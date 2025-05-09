using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AspNetCore.Mvc;

 /*http://localhost:5164/api/cube1/requirement1*/

namespace WarehouseAnalysisApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Cube1Controller : ControllerBase
    {
        private readonly string connectionString;

        private Dictionary<string, string> _fieldMappings = new Dictionary<string, string>
        {
            { "Store Id", "StoreId" },
            { "States", "State" },
            { "City Name", "City" },
            { "Description", "ProductDescription" },
            { "Weight", "Weight" },
            { "Size", "Size" },
            { "Price", "Price" },
            { "[Measures].[Quantity]", "Quantity" },
            { "[Measures].[Unit_sold]", "UnitSold" },
            { "[Measures].[Total_amount]", "TotalAmount" },
            { "Customer_name", "CustomerName" },
            { "Year", "Year" },
            { "Quarter", "Quarter" },
            { "Month", "Month" },
            { "Day", "Day" },
        };

        private List<Dictionary<string, object>> BeautifyResults(
            List<Dictionary<string, object>> rawData
        )
        {
            var cleaned = new List<Dictionary<string, object>>();

            foreach (var row in rawData)
            {
                var cleanedRow = new Dictionary<string, object>();

                foreach (var kv in row)
                {
                    // Skip MEMBER_UNIQUE_NAME fields
                    if (kv.Key.Contains("MEMBER_UNIQUE_NAME"))
                        continue;

                    string key = GetCleanFieldName(kv.Key);
                    cleanedRow[key] = kv.Value;
                }

                cleaned.Add(cleanedRow);
            }

            return cleaned;
        }

        private string GetCleanFieldName(string rawKey)
        {
            // Try direct mapping first
            if (_fieldMappings.ContainsKey(rawKey))
                return _fieldMappings[rawKey];

            // Try to map MEMBER_CAPTION keys dynamically
            if (rawKey.Contains("MEMBER_CAPTION"))
            {
                foreach (var map in _fieldMappings)
                {
                    if (rawKey.Contains(map.Key))
                        return map.Value;
                }

                // Default to last segment of hierarchy if unknown
                var parts = rawKey.Split('.');
                return parts[^2]; // e.g., "Store Id" from [Dim Store].[Store Id].[Store Id].[MEMBER_CAPTION]
            }

            // Leave Measures or unknown keys as-is
            return rawKey
                .Replace("[Measures].", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace(" ", "")
                .Replace("_", "");
        }

        public Cube1Controller(IConfiguration configuration)
        {
            connectionString =
                configuration["SSAS:ConnectionString"]
                ?? throw new InvalidOperationException("SSAS connection string is missing.");
        }

        private async Task<List<Dictionary<string, object>>> ExecuteMdxQuery(string mdxQuery)
        {
            return await Task.Run(() =>
            {
                var results = new List<Dictionary<string, object>>();
                using var connection = new AdomdConnection(connectionString);
                connection.Open();
                using var command = new AdomdCommand(mdxQuery, connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }
                return results;
            });
        }

        ///////////////////////////////////////////////////////////////////


        [HttpGet("requirement1")]
        public async Task<IActionResult> GetRequirement1()
        {
            string mdx =
                @"
                 SELECT NON EMPTY { [Measures].[Quantity] } ON COLUMNS, 
                     NON EMPTY { 
                     ([Dim Store].[Store Id].[Store Id].ALLMEMBERS * 
                     [Dim Store].[States].[States].ALLMEMBERS * 
                     [Dim Store].[City Name].[City Name].ALLMEMBERS * 
                     [Dim Product].[Description].[Description].ALLMEMBERS * 
                     [Dim Product].[Weight].[Weight].ALLMEMBERS * 
                     [Dim Product].[Size].[Size].ALLMEMBERS * 
                     [Dim Product].[Price].[Price].ALLMEMBERS ) 
                     } DIMENSION PROPERTIES MEMBER_CAPTION, 
                     MEMBER_UNIQUE_NAME ON ROWS FROM [Cube1] 
                                                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS               ";
            var rawResult = await ExecuteMdxQuery(mdx);
            var beautified = BeautifyResults(rawResult);
            return Ok(beautified);
        }
    }
}