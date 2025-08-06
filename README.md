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
   # Configuração padrão
   dotnet run
   
   # Com parâmetros personalizados
   dotnet run -- --tax-rate 0.15 --exemption-limit 25000
   
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
- `-t, --tax-rate <valor>`: Taxa de imposto (ex: 0.15 para 15%)
- `-e, --exemption-limit <valor>`: Limite de isenção em reais
- `-i, --input <arquivo>`: Arquivo de entrada (padrão: input.txt)

### Exemplos de Uso

```bash
# Configuração padrão (20%, R$ 20.000)
dotnet run

# Taxa personalizada de 15%
dotnet run -- --tax-rate 0.15

# Limite de isenção de R$ 30.000
dotnet run -- --exemption-limit 30000

# Configuração completa personalizada
dotnet run -- --tax-rate 0.12 --exemption-limit 50000 --show-config

# Usando arquivo de entrada diferente
dotnet run -- --input input2.txt

# Combinando variáveis de ambiente e parâmetros
$env:CAPITAL_GAIN_TAX_RATE="0.25"
dotnet run -- --exemption-limit 35000 --show-config
```

## Regras de Negócio

- **Compra**: Não há imposto sobre operações de compra
- **Venda**: 
  - Isento se o valor total da venda for ≤ R$ 20.000
  - 20% sobre o lucro se valor > R$ 20.000
  - Prejuízos são acumulados para dedução de lucros futuros
  - Utiliza preço médio ponderado para cálculo do custo base

## Formato do Arquivo de Entrada

O arquivo `input.txt` deve conter operações em formato JSON, uma linha por lote:

```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 10000}, {"operation":"sell", "unit-cost":10.00, "quantity": 5000}]
```