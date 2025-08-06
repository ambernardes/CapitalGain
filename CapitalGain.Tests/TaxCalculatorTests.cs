using CapitalGain.Services;

namespace CapitalGain.Tests
{
    public class TaxCalculatorTests
    {
        [Fact]
        public void CalculateWeightedAveragePrice_NewPurchase_ReturnsCorrectAverage()
        {
            // Arrange
            var currentPrice = 10.00m;
            var currentQuantity = 100;
            var newPrice = 20.00m;
            var newQuantity = 100;

            // Act
            var result = TaxCalculator.CalculateWeightedAveragePrice(currentPrice, currentQuantity, newPrice, newQuantity);

            // Assert
            Assert.Equal(15.00m, result); // (10*100 + 20*100) / 200 = 15
        }

        [Fact]
        public void CalculateWeightedAveragePrice_ZeroQuantity_ReturnsZero()
        {
            // Act
            var result = TaxCalculator.CalculateWeightedAveragePrice(0m, 0, 10.00m, 0);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculateWeightedAveragePrice_SameInputs_ReturnsSameOutput()
        {
            // Arrange - Transparência referencial: mesmas entradas devem gerar mesmas saídas
            var currentPrice = 10.00m;
            var currentQuantity = 100;
            var newPrice = 20.00m;
            var newQuantity = 100;

            // Act
            var result1 = TaxCalculator.CalculateWeightedAveragePrice(currentPrice, currentQuantity, newPrice, newQuantity);
            var result2 = TaxCalculator.CalculateWeightedAveragePrice(currentPrice, currentQuantity, newPrice, newQuantity);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void CalculateGain_ProfitOperation_ReturnsPositiveGain()
        {
            // Arrange
            var sellPrice = 20.00m;
            var sellQuantity = 100;
            var weightedAveragePrice = 10.00m;

            // Act
            var result = TaxCalculator.CalculateGain(sellPrice, sellQuantity, weightedAveragePrice);

            // Assert
            Assert.Equal(1000.00m, result); // (20*100) - (10*100) = 1000
        }

        [Fact]
        public void CalculateGain_LossOperation_ReturnsNegativeGain()
        {
            // Arrange
            var sellPrice = 5.00m;
            var sellQuantity = 100;
            var weightedAveragePrice = 10.00m;

            // Act
            var result = TaxCalculator.CalculateGain(sellPrice, sellQuantity, weightedAveragePrice);

            // Assert
            Assert.Equal(-500.00m, result); // (5*100) - (10*100) = -500
        }

        [Theory]
        [InlineData(10.00, 100, 20.00, 100, 15.00)]
        [InlineData(5.00, 200, 15.00, 100, 8.33)]
        [InlineData(0, 0, 10.00, 100, 10.00)]
        public void CalculateWeightedAveragePrice_VariousInputs_ReturnsExpectedResults(
            decimal currentPrice, int currentQuantity, 
            decimal newPrice, int newQuantity, 
            decimal expected)
        {
            // Act
            var result = TaxCalculator.CalculateWeightedAveragePrice(currentPrice, currentQuantity, newPrice, newQuantity);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
