# CapitalGain

## Descrição
Sistema para cálculo de impostos sobre ganho de capital em operações de compra e venda de ações.

## Requisitos Técnicos
- .NET 8.0
- Sistema de testes: xUnit
- Formato de entrada: JSON

## Arquitetura e Princípios

### Transparência Referencial
O projeto implementa **transparência referencial** através da classe `TaxCalculator`, que contém métodos estáticos puros:

- **Funções Puras**: Sempre retornam o mesmo resultado para as mesmas entradas
- **Sem Efeitos Colaterais**: Não modificam estado externo nem dependem de variáveis globais
- **Facilidade de Teste**: Cada função pode ser testada isoladamente
- **Reutilização**: Métodos podem ser usados em diferentes contextos

#### Exemplo de Transparência Referencial:
```csharp
// Método puro - sempre retorna o mesmo resultado para as mesmas entradas
var preco1 = TaxCalculator.CalculateWeightedAveragePrice(10.00m, 100, 20.00m, 100);
var preco2 = TaxCalculator.CalculateWeightedAveragePrice(10.00m, 100, 20.00m, 100);
// preco1 == preco2 sempre será true
```

### Separação de Responsabilidades
- **`CapitalGainService`**: Controle de fluxo e regras de negócio
- **`TaxCalculator`**: Cálculos matemáticos puros
- **`TaxConfiguration`**: Gerenciamento de configurações
- **`OperationEntry`**: Representação de dados

## Estrutura do Projeto
```
CapitalGain.sln                     --> Arquivo da solução
│
├── CapitalGain/                    --> Projeto principal
│   ├── Program.cs                  --> Ponto de entrada da aplicação
│   ├── Models/
│   │   ├── OperationEntry.cs       --> Classe para representar dados de entrada
│   │   └── TaxConfiguration.cs     --> Configuração de impostos
│   └── Services/
│       ├── CapitalGainService.cs   --> Lógica principal de processamento
│       └── TaxCalculator.cs        --> Cálculos matemáticos (funções puras)
│
├── CapitalGain.Tests/              --> Projeto de testes
│   ├── CapitalGainServiceTests.cs  --> Testes do serviço principal
│   └── TaxCalculatorSimpleTests.cs --> Testes das funções de cálculo
│
└── README.md                       --> Documentação do projeto
```

## Como Usar

### Execução Local

1. Compile a solução:
   ```bash
   dotnet build
   ```

2. Execute o programa principal:
   ```bash
   # Execução básica (arquivo de entrada obrigatório)
   dotnet run --project CapitalGain -- --input input.txt
   
   # Com parâmetros personalizados
   dotnet run --project CapitalGain -- --input input.txt --tax-rate 0.15 --exemption-limit 25000
   ```

3. Execute os testes:
   ```bash
   dotnet test
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
dotnet run --project CapitalGain -- --input input.txt

# Taxa personalizada de 15%
dotnet run --project CapitalGain -- --input input.txt --tax-rate 0.15

# Limite de isenção de R$ 30.000
dotnet run --project CapitalGain -- --input input.txt --exemption-limit 30000

# Configuração completa personalizada
dotnet run --project CapitalGain -- --input input.txt --tax-rate 0.12 --exemption-limit 50000

# Usando arquivo de entrada diferente
dotnet run --project CapitalGain -- --input operacoes.txt

# Combinando variáveis de ambiente e parâmetros
$env:CAPITAL_GAIN_TAX_RATE="0.25"
dotnet run --project CapitalGain -- --input input.txt --exemption-limit 35000

# Executar testes
dotnet test
```

## Desenvolvimento e Extensibilidade

### Adicionando Novos Cálculos
Para adicionar novos tipos de cálculos, siga o padrão de transparência referencial:

```csharp
// Exemplo: Novo método no TaxCalculator
public static class TaxCalculator
{
    // Método puro para calcular taxa de corretagem
    public static decimal CalculateBrokerageFee(decimal operationValue, decimal feeRate)
    {
        return Math.Round(operationValue * feeRate, 2);
    }
}
```

### Testando Novos Métodos
```csharp
[Fact]
public void CalculateBrokerageFee_ValidInputs_ReturnsCorrectFee()
{
    // Arrange
    var operationValue = 1000.00m;
    var feeRate = 0.005m; // 0.5%

    // Act
    var result = TaxCalculator.CalculateBrokerageFee(operationValue, feeRate);

    // Assert
    Assert.Equal(5.00m, result);
}
```

### Princípios para Extensão
1. **Mantenha Funções Puras**: Novos métodos devem ser determinísticos
2. **Teste Isoladamente**: Cada função deve ter seus próprios testes
3. **Documente Comportamento**: Use comentários XML para documentar
4. **Valide Entradas**: Trate casos extremos e entradas inválidas

## Testes

O projeto inclui testes unitários abrangentes para validar tanto a funcionalidade do serviço principal quanto as funções de cálculo.

### Categorias de Testes

#### 1. Testes de Transparência Referencial (`TaxCalculatorSimpleTests`)
- Validam que funções puras retornam sempre o mesmo resultado
- Testam cálculos matemáticos isoladamente
- Verificam diferentes cenários de entrada

#### 2. Testes de Integração (`CapitalGainServiceTests`)
- Validam o fluxo completo de processamento
- Testam regras de negócio complexas
- Verificam integração entre componentes

### Executar Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes com verbose
dotnet test --verbosity normal

# Executar apenas testes de cálculo
dotnet test --filter "TaxCalculatorSimpleTests"

# Executar apenas testes do serviço principal
dotnet test --filter "CapitalGainServiceTests"

# Executar testes específicos do projeto
dotnet test CapitalGain.Tests
```

## Benefícios da Arquitetura

### 🔧 **Manutenibilidade**
- Cálculos matemáticos isolados em funções puras
- Fácil identificação e correção de bugs em cálculos específicos
- Código mais legível e autodocumentado

### 🧪 **Testabilidade**
- Funções puras são facilmente testáveis
- Testes determinísticos (sempre produzem o mesmo resultado)
- Cobertura de testes mais granular

### 🔄 **Reutilização**
- Métodos de cálculo podem ser reutilizados em outros contextos
- Separação clara entre lógica de negócio e cálculos matemáticos

### 🚀 **Performance**
- Funções puras podem ser otimizadas pelo compilador
- Possibilidade de memoização em cenários específicos
- Redução de efeitos colaterais indesejados

## Regras de Negócio

- **Compra**: Não há imposto sobre operações de compra
- **Venda**: 
  - Isento se o valor total da venda for ≤ R$ 20.000
  - 20% sobre o lucro se valor > R$ 20.000
  - Prejuízos são acumulados para dedução de lucros futuros
  - Utiliza preço médio ponderado para cálculo do custo base

### Implementação dos Cálculos

Os cálculos matemáticos são implementados através de **funções puras** na classe `TaxCalculator`:

- **`CalculateWeightedAveragePrice`**: Calcula o preço médio ponderado das ações
- **`CalculateGain`**: Determina o ganho ou perda em uma operação de venda

Estes métodos garantem:
- ✅ **Consistência**: Sempre produzem o mesmo resultado para as mesmas entradas
- ✅ **Auditabilidade**: Cálculos podem ser verificados independentemente
- ✅ **Confiabilidade**: Sem efeitos colaterais ou dependências externas

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

## Contribuição

### Diretrizes para Contribuição

1. **Mantenha a Transparência Referencial**
   - Novos métodos de cálculo devem ser implementados como funções puras
   - Evite efeitos colaterais em métodos da classe `TaxCalculator`

2. **Testes Obrigatórios**
   - Todo novo método deve ter testes correspondentes
   - Testes devem verificar transparência referencial (mesma entrada = mesma saída)

3. **Documentação**
   - Use comentários XML para documentar métodos públicos
   - Atualize o README quando adicionar nova funcionalidade

### Estrutura de Commits
```
feat: adiciona cálculo de taxa de corretagem
test: adiciona testes para CalculateBrokerageFee
docs: atualiza README com nova funcionalidade
refactor: melhora transparência referencial em TaxCalculator
```

### Exemplo de Pull Request
```markdown
## Descrição
Adiciona cálculo de taxa de corretagem como função pura

## Checklist
- [x] Método implementado como função pura
- [x] Testes unitários adicionados
- [x] Documentação atualizada
- [x] Transparência referencial verificada
```