# CapitalGain

## Descrição
Sistema para cálculo de impostos sobre ganho de capital em operações de compra e venda de ações.

## Estrutura do Projeto
```
CapitalGain/
│
├── Program.cs                    --> Ponto de entrada da aplicação
├── Models/
│   ├── OperationEntry.cs        --> Classe para representar dados de entrada
│   └── TaxConfiguration.cs      --> Configuração de impostos
├── Services/
│   └── CapitalGainService.cs    --> Lógica de cálculo de impostos
└── README.md                    --> Documentação do projeto
```

## Como Usar

### Execução Local

1. Compile o projeto:
   ```bash
   dotnet build
   ```

2. Execute o programa:
   ```bash
   # Execução básica (arquivo de entrada obrigatório)
   dotnet run -- --input input.txt
   
   # Com parâmetros personalizados
   dotnet run -- --input input.txt --tax-rate 0.15 --exemption-limit 25000
   
   ```
## Configuração

O sistema oferece **3 níveis de configuração** com precedência hierárquica:

### 1. Valores Padrão (menor prioridade)
- Taxa de imposto: **20%** (0.20)
- Limite de isenção: **R$ 20.000**

### 2. Variáveis de Ambiente (prioridade média)
```bash
# PowerShell
$env:CAPITAL_GAIN_TAX_RATE="0.15"
$env:CAPITAL_GAIN_EXEMPTION_LIMIT="25000"

# Linux/Mac
export CAPITAL_GAIN_TAX_RATE=0.15
export CAPITAL_GAIN_EXEMPTION_LIMIT=25000
```

### 3. Parâmetros de Linha de Comando (maior prioridade)
```bash
dotnet run -- --tax-rate 0.15 --exemption-limit 25000
```

### Opções Disponíveis
- `-i, --input <arquivo>`: **OBRIGATÓRIO** - Arquivo de entrada com as operações
- `-t, --tax-rate <valor>`: **OPCIONAL** - Taxa de imposto (ex: 0.15 para 15%)
- `-e, --exemption-limit <valor>`: **OPCIONAL** - Limite de isenção em reais

### Exemplos de Uso

```bash
# Execução básica com arquivo padrão
dotnet run -- --input input.txt

# Taxa personalizada de 15%
dotnet run -- --input input.txt --tax-rate 0.15

# Limite de isenção de R$ 30.000
dotnet run -- --input input.txt --exemption-limit 30000

# Configuração completa personalizada
dotnet run -- --input input.txt --tax-rate 0.12 --exemption-limit 50000

# Usando arquivo de entrada diferente
dotnet run -- --input operacoes.txt

# Combinando variáveis de ambiente e parâmetros
$env:CAPITAL_GAIN_TAX_RATE="0.25"
dotnet run -- --input input.txt --exemption-limit 35000
```

## Regras de Negócio

- **Compra**: Não há imposto sobre operações de compra
- **Venda**: 
  - Isento se o valor total da venda for ≤ R$ 20.000
  - 20% sobre o lucro se valor > R$ 20.000
  - Prejuízos são acumulados para dedução de lucros futuros
  - Utiliza preço médio ponderado para cálculo do custo base

## Formato do Arquivo de Entrada

**IMPORTANTE**: O arquivo de entrada é **OBRIGATÓRIO** e deve ser especificado através do parâmetro `--input`.

O programa procurará o arquivo nos seguintes locais:
1. Caminho especificado diretamente
2. Pasta `inputs/` no diretório atual
3. Pasta `/app/inputs/` (para execução em Docker)
4. Pasta `test-inputs/` no diretório atual
5. Pasta `/app/test-inputs/` (para execução em Docker)

Se o arquivo não for encontrado em nenhum desses locais, o programa exibirá uma mensagem de erro e terminará.

O arquivo deve conter operações em formato JSON, uma linha por lote:

```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 10000}, {"operation":"sell", "unit-cost":10.00, "quantity": 5000}]
```

### Mensagens de Erro

Se o parâmetro `--input` não for fornecido:
```
Erro: O parâmetro --input é obrigatório.
Uso: --input <caminho_do_arquivo> [--tax-rate <valor>] [--exemption-limit <valor>]
```

Se o arquivo especificado não for encontrado:
```
Erro: Arquivo 'arquivo.txt' não encontrado.
Verifique se o arquivo existe nos seguintes locais:
- arquivo.txt
- inputs/arquivo.txt
- /app/inputs/arquivo.txt
- test-inputs/arquivo.txt
- /app/test-inputs/arquivo.txt
```