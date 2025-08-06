namespace CapitalGain.Models
{
    public class TaxConfiguration
    {
        // Constantes padrão
        private const decimal DEFAULT_TAX_RATE = 0.20m;
        private const decimal DEFAULT_TAX_EXEMPTION_LIMIT = 20000m;

        public decimal TaxRate { get; set; }
        public decimal TaxExemptionLimit { get; set; }

        public TaxConfiguration()
        {
            // Carrega valores padrão
            TaxRate = DEFAULT_TAX_RATE;
            TaxExemptionLimit = DEFAULT_TAX_EXEMPTION_LIMIT;
        }

        public TaxConfiguration(decimal? taxRate = null, decimal? taxExemptionLimit = null)
        {
            // Carrega valores padrão
            TaxRate = DEFAULT_TAX_RATE;
            TaxExemptionLimit = DEFAULT_TAX_EXEMPTION_LIMIT;

            // Sobrescreve com parâmetros se fornecidos
            if (taxRate.HasValue)
                TaxRate = taxRate.Value;

            if (taxExemptionLimit.HasValue)
                TaxExemptionLimit = taxExemptionLimit.Value;
        }        
    }
}
