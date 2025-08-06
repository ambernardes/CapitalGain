using CapitalGain.Models;
using CapitalGain.Services;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace CapitalGain.IntegrationTests.Scenarios
{
    public class FullFlowTests
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static IEnumerable<object[]> TestCases()
        {
            // Procura por pares de arquivos input/expected
            for (int i = 0; i <= 9; i++)
            {
                string inputPath = Path.Combine("Fixtures", $"input{i}.txt");
                string expectedPath = Path.Combine("Fixtures", $"output{i}.txt");
                
                if (File.Exists(inputPath) && File.Exists(expectedPath))
                {
                    yield return new object[] { inputPath, expectedPath, i };
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Scenario_Should_Match_Expected_Output(string inputPath, string expectedPath, int scenarioNumber)
        {
            // Arrange
            string inputJson = File.ReadAllText(inputPath);
            string expectedOutput = File.ReadAllText(expectedPath);
            var service = new CapitalGainService();
            
            // Act
            var operations = service.ParseOperations(inputJson);
            service.CalculateTaxes(operations);
            
            // Convert to expected output format (same as DisplayResults method)
            var actualOutputLines = new List<string>();
            foreach (var batch in operations)
            {
                var results = batch.Select(op => new { tax = Math.Round(op.TaxPaid, 1) }).ToArray();
                var json = JsonSerializer.Serialize(results, _jsonOptions);
                actualOutputLines.Add(json);
            }
            
            string actualOutput = string.Join(Environment.NewLine, actualOutputLines);
            
            var normalizedActualOutput = NormalizeOutput(actualOutput);
            var normalizedExpectedOutput = NormalizeOutput(expectedOutput);

            // Assert
            Assert.Equal(normalizedExpectedOutput, normalizedActualOutput);
        }

        /// <summary>
        /// Normaliza a saída removendo espaços em branco desnecessários e padronizando quebras de linha
        /// </summary>
        private static string NormalizeOutput(string output)
        {
            if (string.IsNullOrEmpty(output))
                return string.Empty;

            return Regex.Replace(output, @"[ \t]+", "")  // Remove espaços e tabs
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim();
        }

        /// <summary>
        /// Método auxiliar para validar se os resultados estão no formato correto
        /// </summary>
        private static void ValidateOutputFormat(string output)
        {
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                // Cada linha deve ser um array JSON válido
                Assert.True(line.StartsWith("[") && line.EndsWith("]"), 
                    $"Output line should be a JSON array: {line}");
                
                // Deve conseguir deserializar
                var results = JsonSerializer.Deserialize<object[]>(line, _jsonOptions);
                Assert.NotNull(results);
                Assert.True(results.Length > 0);
            }
        }
    }
}