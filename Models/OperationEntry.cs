using System.Text.Json.Serialization;

public class OperationEntry
{
    [JsonPropertyName("operation")]
    public required string Operation { get; set; }

    [JsonPropertyName("unit-cost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    
    public decimal TaxPaid { get; set; }
}
