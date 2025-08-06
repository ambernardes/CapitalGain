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
   
   # Exibindo configuração atual
   dotnet run -- --show-config
   
   # Ver ajuda completa
   dotnet run -- --help
   ```

### Execução com Docker

#### Usando Docker Compose (Recomendado)

1. **Build da imagem:**
   ```bash
   # Linux/Mac/WSL
   ./docker-run.sh build
   
   # Windows PowerShell
   .\docker-run.ps1 -Action build
   
   # Ou diretamente
   docker-compose build
   ```

2. **Execução básica:**
   ```bash
   # Configuração padrão
   docker-compose run --rm capitalgain
   
   # Com parâmetros personalizados
   docker-compose run --rm capitalgain --tax-rate 0.15 --exemption-limit 25000
   
   # Exibir configuração
   docker-compose run --rm capitalgain --show-config
   ```

3. **Ambientes pré-configurados:**
   ```bash
   # Ambiente de desenvolvimento (taxa 15%, limite R$ 25.000)
   docker-compose run --rm capitalgain-dev
   
   # Ambiente de teste (taxa 10%, limite R$ 30.000)
   docker-compose run --rm capitalgain-test
   ```

4. **Scripts auxiliares:**
   ```bash
   # Linux/Mac/WSL
   ./docker-run.sh run                    # Execução padrão
   ./docker-run.sh dev                    # Modo desenvolvimento
   ./docker-run.sh test                   # Modo teste
   ./docker-run.sh shell                  # Shell interativo
   ./docker-run.sh clean                  # Limpeza
   
   # Windows PowerShell
   .\docker-run.ps1 -Action run           # Execução padrão
   .\docker-run.ps1 -Action dev           # Modo desenvolvimento
   .\docker-run.ps1 -Action test          # Modo teste
   .\docker-run.ps1 -Action shell         # Shell interativo
   .\docker-run.ps1 -Action clean         # Limpeza
   ```

#### Usando Docker Diretamente

```bash
# Build
docker build -t capitalgain .

# Execução básica
docker run --rm capitalgain

# Com parâmetros
docker run --rm capitalgain --tax-rate 0.15 --show-config

# Com arquivo personalizado (mapeando volume)
docker run --rm -v $(pwd)/inputs:/app/inputs capitalgain --input inputs/my-file.txt
```

### Estrutura de Arquivos para Docker

```
CapitalGain/
├── inputs/              --> Arquivos de entrada personalizados
│   └── example-input.txt
├── outputs/             --> Saída de resultados (opcional)
├── test-inputs/         --> Arquivos para testes
│   └── test-input.txt
├── test-outputs/        --> Saída de testes
├── Dockerfile           --> Configuração do container
├── docker-compose.yml   --> Orquestração de serviços
├── docker-run.sh        --> Script auxiliar (Linux/Mac)
└── docker-run.ps1       --> Script auxiliar (Windows)
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
- `-s, --show-config`: Exibe a configuração atual
- `-c, --config`: Exibe informações sobre configuração
- `-h, --help`: Exibe ajuda completa

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