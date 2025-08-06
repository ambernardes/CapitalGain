using CapitalGain.Services;
using CapitalGain.Models;

// Parse dos argumentos de linha de comando
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

// Validar se o arquivo de entrada foi fornecido
if (string.IsNullOrEmpty(inputFile))
{
    Console.Error.WriteLine("Erro: O parâmetro --input é obrigatório.");
    Console.Error.WriteLine("Uso: --input <caminho_do_arquivo> [--tax-rate <valor>] [--exemption-limit <valor>]");
    Environment.Exit(1);
}

// Cria a configuração com os parâmetros fornecidos
var taxConfig = new TaxConfiguration(taxRate, exemptionLimit);
var service = new CapitalGainService(taxConfig);

// Verificar se o arquivo de entrada existe, incluindo paths comuns no Docker
if (!File.Exists(inputFile))
{
    var alternativePaths = new[]
    {
        Path.Combine("inputs", Path.GetFileName(inputFile)),
        Path.Combine("/app/inputs", Path.GetFileName(inputFile)),
        Path.Combine("test-inputs", Path.GetFileName(inputFile)),
        Path.Combine("/app/test-inputs", Path.GetFileName(inputFile))
    };

    string? foundFile = null;
    foreach (var path in alternativePaths)
    {
        if (File.Exists(path))
        {
            foundFile = path;
            break;
        }
    }

    if (foundFile != null)
    {
        inputFile = foundFile;
        Console.WriteLine($"Arquivo encontrado em: {inputFile}");
    }
    else
    {
        Console.Error.WriteLine($"Erro: Arquivo '{inputFile}' não encontrado.");
        Console.Error.WriteLine("Verifique se o arquivo existe nos seguintes locais:");
        Console.Error.WriteLine($"- {inputFile}");
        foreach (var path in alternativePaths)
        {
            Console.Error.WriteLine($"- {path}");
        }
        Environment.Exit(1);
    }
}

string inputJson = File.ReadAllText(inputFile);

// Parse das operações
var allOperations = service.ParseOperations(inputJson);

// Calcula os impostos
service.CalculateTaxes(allOperations);

// Exibe os resultados
service.DisplayResults(allOperations);