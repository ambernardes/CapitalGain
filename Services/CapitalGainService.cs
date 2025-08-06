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
                        
                        weightedAveragePrice = ((weightedAveragePrice * totalQuantity) + (op.Quantity * op.UnitCost)) / (totalQuantity + op.Quantity);
                        totalQuantity += op.Quantity;
                    }
                    else if (op.Operation.Equals("sell"))
                    {
                        var soldPrice = op.UnitCost * op.Quantity;
                        var weightedAverageCostTotal = weightedAveragePrice * op.Quantity;
                        var gain = soldPrice - weightedAverageCostTotal;
                        
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
                                var taxableGain = Math.Max(0, gain - accumulatedLoss);
                                var taxPaid = taxableGain * _taxConfig.TaxRate;
                                
                                // Atualiza prejuízos acumulados
                                accumulatedLoss = Math.Max(0, accumulatedLoss - gain);
                                
                                op.TaxPaid = taxPaid;
                            }
                        }
                        else
                        {
                            op.TaxPaid = 0;
                            accumulatedLoss += Math.Abs(gain);
                        }
                    }
                }
            }
        }

        public void DisplayResults(List<List<OperationEntry>> allOperations)
        {
            foreach (var batch in allOperations)
            {
                var results = batch.Select(op => new { tax = op.TaxPaid }).ToArray();
                var json = JsonSerializer.Serialize(results);
                Console.WriteLine(json);
            }
        }
    }
}
