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

            // Sobrescreve com variáveis de ambiente se existirem
            LoadFromEnvironmentVariables();
        }

        public TaxConfiguration(decimal? taxRate = null, decimal? taxExemptionLimit = null)
        {
            // Carrega valores padrão
            TaxRate = DEFAULT_TAX_RATE;
            TaxExemptionLimit = DEFAULT_TAX_EXEMPTION_LIMIT;

            // Sobrescreve com variáveis de ambiente se existirem
            LoadFromEnvironmentVariables();

            // Sobrescreve com parâmetros se fornecidos
            if (taxRate.HasValue)
                TaxRate = taxRate.Value;

            if (taxExemptionLimit.HasValue)
                TaxExemptionLimit = taxExemptionLimit.Value;
        }

        private void LoadFromEnvironmentVariables()
        {
            var taxRateEnv = Environment.GetEnvironmentVariable("CAPITAL_GAIN_TAX_RATE");
            if (!string.IsNullOrEmpty(taxRateEnv) && decimal.TryParse(taxRateEnv, out var taxRate))
            {
                TaxRate = taxRate;
            }

            var exemptionLimitEnv = Environment.GetEnvironmentVariable("CAPITAL_GAIN_EXEMPTION_LIMIT");
            if (!string.IsNullOrEmpty(exemptionLimitEnv) && decimal.TryParse(exemptionLimitEnv, out var exemptionLimit))
            {
                TaxExemptionLimit = exemptionLimit;
            }
        }

        public override string ToString()
        {
            return $"TaxRate: {TaxRate:P}, ExemptionLimit: {TaxExemptionLimit:C}";
        }
    }
}
