using Microsoft.AspNetCore.Mvc;

namespace WarehouseAnalysisApi.DTO;

public class Cube2Request2
{
    public string customerId { get; set; }
    public string totalAmount { get; set; }
    public string customerName { get; set; }
    public string year { get; set; }
    public string quarter { get; set; }
    public string month { get; set; }
    public string day { get; set; }
}