using CapitalGain.Services;
using CapitalGain.Models;

decimal? taxRate = null;
decimal? exemptionLimit = null;
string? inputFile = null;

for (int i = 0; i < args.Length; i++)
{
   switch (args[i].ToLower())
   {
       case "--tax-rate":
       case "-t":
           if (i + 1 < args.Length && decimal.TryParse(args[i + 1], out var rate))
           {
               taxRate = rate;
               i++; // Skip next argument
           }
           break;
       case "--exemption-limit":
       case "-e":
           if (i + 1 < args.Length && decimal.TryParse(args[i + 1], out var limit))
           {
               exemptionLimit = limit;
               i++; // Skip next argument
           }
           break;
       case "--input":
       case "-i":
           if (i + 1 < args.Length)
           {
               inputFile = args[i + 1];
               i++; // Skip next argument
           }
           break;
   }
}

if (string.IsNullOrEmpty(inputFile))
{
   Console.Error.WriteLine("Erro: O parâmetro --input é obrigatório.");
   Console.Error.WriteLine("Uso: --input <caminho_do_arquivo> [--tax-rate <valor>] [--exemption-limit <valor>]");
   Environment.Exit(1);
}

var taxConfig = new TaxConfiguration(taxRate, exemptionLimit);
var service = new CapitalGainService(taxConfig);

//string inputJson = File.ReadAllText("input.txt");
string inputJson = File.ReadAllText(inputFile);

// Read operations
var allOperations = service.ParseOperations(inputJson);

// Process operations
service.CalculateTaxes(allOperations);

// Show results
service.DisplayResults(allOperations);