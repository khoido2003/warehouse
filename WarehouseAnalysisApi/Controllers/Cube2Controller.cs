using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseAnalysisApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Cube2Controller : ControllerBase
    {
        private readonly string connectionString;

        private readonly Dictionary<string, string> _fieldMappings = new()
        {
            { "Customer Name", "CustomerName" },
            { "Year", "Year" },
            { "Quarter", "Quarter" },
            { "Month", "Month" },
            { "Day", "Day" },
            { "[Measures].[Total Amount]", "TotalAmount" },
        };

        public Cube2Controller(IConfiguration configuration)
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

        private List<Dictionary<string, object>> BeautifyResults(
            List<Dictionary<string, object>> rawData
        )
        {
            var beautified = new List<Dictionary<string, object>>();

            foreach (var row in rawData)
            {
                var cleanRow = new Dictionary<string, object>();

                foreach (var kv in row)
                {
                    // Skip MEMBER_UNIQUE_NAME fields
                    if (kv.Key.Contains("MEMBER_UNIQUE_NAME"))
                        continue;

                    string cleanKey = GetCleanFieldName(kv.Key);
                    cleanRow[cleanKey] = kv.Value;
                }

                beautified.Add(cleanRow);
            }

            return beautified;
        }

        private string GetCleanFieldName(string rawKey)
        {
            // Direct match
            if (_fieldMappings.ContainsKey(rawKey))
                return _fieldMappings[rawKey];

            // Try MEMBER_CAPTION dynamic match
            if (rawKey.Contains("MEMBER_CAPTION"))
            {
                foreach (var pair in _fieldMappings)
                {
                    if (rawKey.Contains(pair.Key))
                        return pair.Value;
                }

                // Fallback: last segment
                var parts = rawKey.Split('.');
                return parts[^2];
            }

            // For Measures or unexpected fields
            return rawKey
                .Replace("[Measures].", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace(" ", "")
                .Replace("_", "");
        }

        [HttpGet("requirement2")]
        public async Task<IActionResult> GetRequirement2()
        {
            string mdx =
                @"
                SELECT NON EMPTY { [Measures].[Total Amount] } ON COLUMNS, 
                NON EMPTY { 
                    ([Dim Customer].[Customer Name].[Customer Name].ALLMEMBERS * 
                     [Dim Time].[Year].[Year].ALLMEMBERS * 
                     [Dim Time].[Quarter].[Quarter].ALLMEMBERS * 
                     [Dim Time].[Month].[Month].ALLMEMBERS * 
                     [Dim Time].[Day].[Day].ALLMEMBERS) 
                } 
                DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
                ON ROWS 
                FROM [Cube2] 
                CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

            var rawResult = await ExecuteMdxQuery(mdx);
            var beautified = BeautifyResults(rawResult);
            return Ok(beautified);
        }
    }
}
