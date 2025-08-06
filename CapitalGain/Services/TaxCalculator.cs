namespace CapitalGain.Services
{
    public static class TaxCalculator
    {
        public static decimal CalculateWeightedAveragePrice(
            decimal currentWeightedPrice, 
            int currentQuantity, 
            decimal newPurchasePrice, 
            int newPurchaseQuantity)
        {
            if (currentQuantity + newPurchaseQuantity == 0)
                return 0m;

            var totalCostCurrent = currentWeightedPrice * currentQuantity;
            var totalCostNew = newPurchasePrice * newPurchaseQuantity;
            var totalQuantity = currentQuantity + newPurchaseQuantity;
            
            return Math.Round((totalCostCurrent + totalCostNew) / totalQuantity, 2);
        }

        public static decimal CalculateGain(
            decimal sellPrice, 
            int sellQuantity, 
            decimal weightedAveragePrice)
        {
            var soldPrice = Math.Round(sellPrice * sellQuantity, 2);
            var weightedAverageCostTotal = Math.Round(weightedAveragePrice * sellQuantity, 2);
            
            return Math.Round(soldPrice - weightedAverageCostTotal, 2);
        }
    }
}
