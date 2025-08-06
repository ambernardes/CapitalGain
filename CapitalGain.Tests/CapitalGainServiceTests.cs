using CapitalGain.Services;
using CapitalGain.Models;

namespace CapitalGain.Tests
{
    public class CapitalGainServiceTests
    {
        private readonly CapitalGainService _service;

        public CapitalGainServiceTests()
        {
            _service = new CapitalGainService();
        }

        [Fact]
        public void ParseOperations_ValidJson_ReturnsCorrectOperations()
        {
            // Arrange
            string inputJson = "[{\"operation\":\"buy\", \"unit-cost\":10.00, \"quantity\": 1000}]" + Environment.NewLine +
                              "[{\"operation\":\"sell\", \"unit-cost\":15.00, \"quantity\": 500}]";

            // Act
            var result = _service.ParseOperations(inputJson);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Single(result[0]);
            Assert.Single(result[1]);
            Assert.Equal("buy", result[0][0].Operation);
            Assert.Equal("sell", result[1][0].Operation);
            Assert.Equal(10.00m, result[0][0].UnitCost);
            Assert.Equal(15.00m, result[1][0].UnitCost);
        }

        [Fact]
        public void CalculateTaxes_BuyOperation_NoTax()
        {
            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry
                    {
                        Operation = "buy",
                        UnitCost = 10.00m,
                        Quantity = 1000
                    }
                }
            };

            _service.CalculateTaxes(operations);

            Assert.Equal(0, operations[0][0].TaxPaid);
        }

        [Fact]
        public void CalculateTaxes_SellWithLoss_NoTax()
        {
            // Arrange - fragmento de exemplo na especificação
            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = 20.00m, Quantity = 10000 },
                    new OperationEntry { Operation = "sell", UnitCost = 10.00m, Quantity = 5000 }
                }
            };

            _service.CalculateTaxes(operations);

            // Assert
            Assert.Equal(0, operations[0][0].TaxPaid);
            Assert.Equal(0, operations[0][1].TaxPaid);
        }

        [Fact]
        public void CalculateTaxes_SellBelowExemptionLimit_NoTax()
        {
            // Arrange - Venda de R$ 15.000 (abaixo do limite de R$ 20.000)
            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = 10.00m, Quantity = 1000 },
                    new OperationEntry { Operation = "sell", UnitCost = 15.00m, Quantity = 1000 }
                }
            };

            // Act
            _service.CalculateTaxes(operations);

            // Assert
            Assert.Equal(0, operations[0][0].TaxPaid);
            Assert.Equal(0, operations[0][1].TaxPaid);
        }

        [Fact]
        public void CalculateTaxes_SellAboveExemptionLimitWithProfit_PaysTax()
        {
            // Arrange - Venda de R$ 100.000 (acima do limite de R$ 20.000)
            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
                    new OperationEntry { Operation = "sell", UnitCost = 20.00m, Quantity = 5000 }
                }
            };

            // Act
            _service.CalculateTaxes(operations);

            // Assert
            Assert.Equal(0, operations[0][0].TaxPaid);
            Assert.Equal(10000.00m, operations[0][1].TaxPaid);
        }

        [Fact]
        public void CalculateTaxes_AccumulatedLoss_DeductsFromFutureGains()
        {
            // Arrange - Vou fazer separadamente para evitar problemas com o parse de múltiplas linhas
            var operations1 = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = 10.00m, Quantity = 10000 },
                    new OperationEntry { Operation = "sell", UnitCost = 5.00m, Quantity = 5000 },
                    new OperationEntry { Operation = "sell", UnitCost = 20.00m, Quantity = 3000 }
                }
            };

            // Act
            _service.CalculateTaxes(operations1);
            // Assert
            // Primeira operação: compra, sem imposto
            Assert.Equal(0, operations1[0][0].TaxPaid);
            // Segunda operação: Prejuízo de R$ 25000, sem imposto
            Assert.Equal(0, operations1[0][1].TaxPaid);
            // Terceira operação: Lucro de R$ 30000: Deve deduzir prejuízo de R$ 25000 e paga 20% de R$ 5000 em imposto (R$ 1000)
            Assert.Equal(1000.00m, operations1[0][2].TaxPaid); 
        }

        [Fact]
        public void CalculateTaxes_CustomTaxConfiguration_UsesCustomValues()
        {
            // Arrange
            var customConfig = new TaxConfiguration(0.15m, 30000m); // 15% tax, R$ 30.000 exemption
            var customService = new CapitalGainService(customConfig);

            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = 10.00m, Quantity = 1000 },
                    new OperationEntry { Operation = "sell", UnitCost = 25.00m, Quantity = 1000 }
                }
            };

            // Act
            customService.CalculateTaxes(operations);

            // Assert
            // Venda de R$ 25.000 está abaixo do limite de R$ 30.000 - sem imposto
            Assert.Equal(0, operations[0][1].TaxPaid);
        }

        [Theory]
        [InlineData(10.00, 1000, 15.00, 500, 0)] // Abaixo do limite de isenção
        [InlineData(10.00, 1000, 25.00, 1000, 3000)] // Acima do limite de isenção
        [InlineData(20.00, 1000, 15.00, 1000, 0)] // Prejuízo sem imposto
        public void CalculateTaxes_VariousScenarios_ProducesExpectedTax(decimal buyCost, int buyQuantity,
            decimal sellCost, int sellQuantity, decimal expectedTax)
        {
            // Arrange
            var operations = new List<List<OperationEntry>>
            {
                new()
                {
                    new OperationEntry { Operation = "buy", UnitCost = buyCost, Quantity = buyQuantity },
                    new OperationEntry { Operation = "sell", UnitCost = sellCost, Quantity = sellQuantity }
                }
            };

            // Act
            _service.CalculateTaxes(operations);

            // Assert
            Assert.Equal(expectedTax, operations[0][1].TaxPaid);
        }
    }
}
