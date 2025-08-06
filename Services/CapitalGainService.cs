using System.Text.Json;
using CapitalGain.Models;

namespace CapitalGain.Services
{
    public class CapitalGainService
    {
        private readonly TaxConfiguration _taxConfig;

        public CapitalGainService(TaxConfiguration? taxConfig = null)
        {
            _taxConfig = taxConfig ?? new TaxConfiguration();
        }
        public List<List<OperationEntry>> ParseOperations(string inputContent)
        {
            var allOperations = new List<List<OperationEntry>>();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var lines = inputContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                try
                {
                    var operations = JsonSerializer.Deserialize<List<OperationEntry>>(line, options);
                    if (operations != null)
                    {
                        allOperations.Add(operations);
                    }
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"Erro ao fazer parse da linha: {ex.Message}");
                    Environment.Exit(1);
                }
            }

            return allOperations;
        }

        public void CalculateTaxes(List<List<OperationEntry>> allOperations)
        {
            foreach (var batch in allOperations)
            {
                var weightedAveragePrice = 0m;
                var totalQuantity = 0;
                var accumulatedLoss = 0m;
                
                foreach (var op in batch)
                {
                    if (op.Operation.Equals("buy"))
                    {
                        op.TaxPaid = 0;
                        
                        weightedAveragePrice = Math.Round(((weightedAveragePrice * totalQuantity) + (op.Quantity * op.UnitCost)) / (totalQuantity + op.Quantity), 2);
                        totalQuantity += op.Quantity;
                    }
                    else if (op.Operation.Equals("sell"))
                    {
                        var soldPrice = Math.Round(op.UnitCost * op.Quantity, 2);
                        var weightedAverageCostTotal = Math.Round(weightedAveragePrice * op.Quantity, 2);
                        var gain = Math.Round(soldPrice - weightedAverageCostTotal, 2);
                        
                        totalQuantity -= op.Quantity;
                        
                        if (gain > 0) // Lucro
                        {
                            // Não paga imposto se o valor total da operação for <= limite de isenção
                            if (soldPrice <= _taxConfig.TaxExemptionLimit)
                            {
                                op.TaxPaid = 0;
                            }
                            else
                            {
                                // Deduz prejuízos acumulados do lucro antes de calcular o imposto
                                var taxableGain = Math.Round(Math.Max(0, gain - accumulatedLoss), 2);
                                var taxPaid = Math.Round(taxableGain * _taxConfig.TaxRate, 2);
                                
                                // Atualiza prejuízos acumulados
                                accumulatedLoss = Math.Round(Math.Max(0, accumulatedLoss - gain), 2);
                                
                                op.TaxPaid = taxPaid;
                            }
                        }
                        else
                        {
                            op.TaxPaid = 0;
                            accumulatedLoss = Math.Round(accumulatedLoss + Math.Abs(gain), 2);
                        }
                    }
                }
            }
        }

        public void DisplayResults(List<List<OperationEntry>> allOperations)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            foreach (var batch in allOperations)
            {
                var results = batch.Select(op => new { tax = Math.Round(op.TaxPaid, 1) }).ToArray();
                var json = JsonSerializer.Serialize(results, options);
                Console.WriteLine(json);
            }
        }
    }
}
