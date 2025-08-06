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
CapitalGain.sln                      --> Arquivo da solução
│
├── CapitalGain/                     --> Projeto principal
│   ├── Program.cs                   --> Ponto de entrada da aplicação
│   ├── input.txt                    --> Arquivo de entrada de exemplo
│   ├── Models/
│   │   ├── OperationEntry.cs        --> Classe para representar dados de entrada
│   │   └── TaxConfiguration.cs      --> Configuração de impostos
│   └── Services/
│       ├── CapitalGainService.cs    --> Lógica principal de processamento
│       ├── TaxCalculator.cs         --> Cálculos matemáticos (funções puras)
│       └── TaxCalculatorSimple.cs   --> Implementação simplificada de cálculos
│
├── CapitalGain.Tests/               --> Projeto de testes unitários
│   ├── CapitalGainServiceTests.cs   --> Testes do serviço principal
│   └── TaxCalculatorTests.cs        --> Testes das funções de cálculo
│
├── CapitalGain.IntegrationTests/    --> Projeto de testes de integração
│   ├── Scenarios/
│   │   └── FullFlowTests.cs         --> Testes de fluxo completo end-to-end
│   └── Fixtures/                    --> Arquivos de teste com cenários
│       ├── input0.txt ... input9.txt     --> Cenários de entrada
│       └── output0.txt ... output9.txt    --> Saídas de referência
│
└── README.md                        --> Documentação do projeto
```

## Como Usar

### Execução Local

1. Compile a solução:
   ```bash
   dotnet build
   ```

2. Execute o programa principal:
   ```bash
   # Execução básica (atualmente usa arquivo input.txt na pasta do projeto)
   dotnet run --project CapitalGain
   
   # Alternativamente, execute a partir da pasta do projeto
   cd CapitalGain
   dotnet run
   ```

3. Execute os testes:
   ```bash
   # Todos os testes (unitários e integração)
   dotnet test
   
   # Apenas testes unitários
   dotnet test CapitalGain.Tests
   
   # Apenas testes de integração
   dotnet test CapitalGain.IntegrationTests
   ```
## Configuração

O sistema utiliza configurações padrão que podem ser personalizadas através da classe `TaxConfiguration`:

### Valores Padrão
- Taxa de imposto: **20%** (0.20)
- Limite de isenção: **R$ 20.000**

### Configuração Programática
```csharp
// Usando configuração padrão
var service = new CapitalGainService();

// Usando configuração personalizada
var taxConfig = new TaxConfiguration(taxRate: 0.15m, exemptionLimit: 25000m);
var service = new CapitalGainService(taxConfig);
```

### Arquivo de Entrada
Atualmente, o sistema lê o arquivo `input.txt` localizado na pasta do projeto principal. O arquivo deve conter operações em formato JSON, uma linha por lote:

```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 10000}, {"operation":"sell", "unit-cost":10.00, "quantity": 5000}]
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

## Testes de Integração

### Estrutura dos Testes de Integração

O projeto inclui um conjunto abrangente de testes de integração que validam o fluxo completo do sistema usando cenários reais:

#### Arquivos de Fixtures
```
CapitalGain.IntegrationTests/
└── Fixtures/
    ├── input0.txt ... input9.txt      --> Cenários de entrada diversos
    ├── expected0.txt ... expected2.txt --> Resultados esperados validados
    └── output0.txt ... output9.txt     --> Arquivos de referência
```

#### Como Funcionam os Testes

1. **Leitura de Cenários**: Os testes leem pares de arquivos `inputX.txt` e `expectedX.txt`
2. **Processamento**: Executam o fluxo completo do `CapitalGainService`
3. **Validação**: Comparam a saída gerada com o resultado esperado
4. **Normalização**: Removem diferenças de formatação (espaços, quebras de linha)

#### Exemplo de Cenário
```json
// input0.txt
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 10000}, {"operation":"sell", "unit-cost":10.00, "quantity": 5000}]

// expected0.txt
[{"tax":0},{"tax":10000}]
[{"tax":0},{"tax":0}]
```

### Executando Testes de Integração

```bash
# Executar apenas testes de integração
dotnet test CapitalGain.IntegrationTests

# Executar com detalhes para debugging
dotnet test CapitalGain.IntegrationTests --verbosity normal

# Executar cenário específico
dotnet test --filter "input0"
```

## Testes

O projeto inclui testes unitários abrangentes para validar tanto a funcionalidade do serviço principal quanto as funções de cálculo.

### Categorias de Testes

#### 1. Testes Unitários (`CapitalGain.Tests`)
- **`TaxCalculatorTests`**: Validam funções puras de cálculo matemático
- **`CapitalGainServiceTests`**: Testam lógica de negócio e fluxo do serviço
- Validam que funções puras retornam sempre o mesmo resultado
- Testam cálculos matemáticos isoladamente
- Verificam diferentes cenários de entrada

#### 2. Testes de Integração (`CapitalGain.IntegrationTests`)
- **`FullFlowTests`**: Validam o fluxo completo end-to-end
- Testam cenários reais com arquivos de entrada e saída
- Verificam integração entre todos os componentes
- Comparam resultados atuais com resultados esperados

#### 3. Fixtures de Teste
- **Arquivos de Entrada**: `input0.txt` a `input9.txt` - Cenários diversos de operações
- **Resultados Esperados**: `expected0.txt` a `expected2.txt` - Saídas esperadas para validação
- **Arquivos de Referência**: `output0.txt` a `output9.txt` - Saídas de referência

### Executar Testes

```bash
# Executar todos os testes (unitários + integração)
dotnet test

# Executar testes com informações detalhadas
dotnet test --verbosity normal

# Executar apenas testes unitários
dotnet test CapitalGain.Tests

# Executar apenas testes de integração
dotnet test CapitalGain.IntegrationTests

# Executar testes específicos por nome
dotnet test --filter "TaxCalculatorTests"

# Executar testes específicos do serviço principal
dotnet test --filter "CapitalGainServiceTests"

# Executar testes de fluxo completo
dotnet test --filter "FullFlowTests"

# Executar com cobertura de código (se configurado)
dotnet test --collect:"XPlat Code Coverage"
```

## Benefícios da Arquitetura

### 🔧 **Manutenibilidade**
- Cálculos matemáticos isolados em funções puras
- Fácil identificação e correção de bugs em cálculos específicos
- Código mais legível e autodocumentado

### 🧪 **Estratégia de Testes em Camadas**
- **Testes Unitários**: Validação isolada de métodos e funções puras
- **Testes de Integração**: Validação end-to-end com cenários reais
- **Fixtures Organizadas**: Cenários de teste bem estruturados e reutilizáveis

### 📊 **Cobertura de Testes Abrangente**
- Cenários de compra e venda diversos
- Casos extremos (prejuízos, isenções, volumes altos)
- Validação de cálculos matemáticos complexos

## Arquitetura de Testes

### Estratégia em Três Camadas

O projeto implementa uma estratégia de testes em camadas para garantir qualidade e confiabilidade:

#### 1. **Testes Unitários** (`CapitalGain.Tests`)
- **Objetivo**: Validar componentes isoladamente
- **Foco**: Transparência referencial e lógica de negócio
- **Execução**: Rápida e determinística
- **Cobertura**: Métodos individuais e casos extremos

#### 2. **Testes de Integração** (`CapitalGain.IntegrationTests`)  
- **Objetivo**: Validar fluxo completo end-to-end
- **Foco**: Integração entre componentes e cenários reais
- **Execução**: Baseada em fixtures e comparação de resultados
- **Cobertura**: Cenários de negócio complexos

#### 3. **Fixtures Organizadas**
- **Objetivo**: Cenários reutilizáveis e bem documentados
- **Estrutura**: Pares `input`/`expected` para cada cenário
- **Benefícios**: Facilita debugging e adição de novos casos
- **Manutenção**: Versionamento de cenários de teste

### Benefícios da Arquitetura Atual

#### 🔧 **Manutenibilidade**
- Cálculos matemáticos isolados em funções puras
- Fácil identificação e correção de bugs em cálculos específicos
- Código mais legível e autodocumentado
- Separação clara entre lógica de negócio e apresentação

#### 🧪 **Testabilidade**
- Funções puras são facilmente testáveis
- Testes determinísticos (sempre produzem o mesmo resultado)
- Cobertura de testes mais granular
- Cenários de integração documentados e reproduzíveis

#### 🔄 **Reutilização**
- Métodos de cálculo podem ser reutilizados em outros contextos
- Separação clara entre lógica de negócio e cálculos matemáticos
- Fixtures reutilizáveis para diferentes tipos de teste

#### 🚀 **Performance**
- Funções puras podem ser otimizadas pelo compilador
- Possibilidade de memoização em cenários específicos
- Redução de efeitos colaterais indesejados
- Testes rápidos e paralelos

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

O sistema lê o arquivo `input.txt` localizado na pasta `CapitalGain/` do projeto. 

### Formato Esperado

O arquivo deve conter operações em formato JSON, uma linha por lote de operações:

```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 10000}, {"operation":"sell", "unit-cost":10.00, "quantity": 5000}]
```

### Campos Obrigatórios

- **`operation`**: Tipo da operação (`"buy"` ou `"sell"`)
- **`unit-cost`**: Preço unitário da ação (decimal)
- **`quantity`**: Quantidade de ações (inteiro)

### Exemplo Completo

```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 100}, {"operation":"sell", "unit-cost":15.00, "quantity": 50}, {"operation":"sell", "unit-cost":15.00, "quantity": 50}]
[{"operation":"buy", "unit-cost":10.00, "quantity": 10000}, {"operation":"sell", "unit-cost":20.00, "quantity": 5000}, {"operation":"sell", "unit-cost":5.00, "quantity": 5000}]
```

### Arquivo de Exemplo

O projeto inclui um arquivo `input.txt` de exemplo na pasta `CapitalGain/` com cenários de teste pré-configurados.

## Contribuição

### Diretrizes para Contribuição

1. **Mantenha a Transparência Referencial**
   - Novos métodos de cálculo devem ser implementados como funções puras
   - Evite efeitos colaterais em métodos da classe `TaxCalculator`

2. **Testes Obrigatórios**
   - Todo novo método deve ter testes correspondentes
   - Testes devem verificar transparência referencial (mesma entrada = mesma saída)

3. **Testes de Integração**
   - Adicione novos cenários em arquivos `inputX.txt` na pasta `Fixtures/`
   - Crie os arquivos `expectedX.txt` correspondentes com os resultados esperados
   - Execute os testes para validar os novos cenários

4. **Documentação**
   - Use comentários XML para documentar métodos públicos
   - Atualize o README quando adicionar nova funcionalidade

### Adicionando Novos Cenários de Teste

Para adicionar um novo cenário de teste de integração:

1. **Crie o arquivo de entrada**:
   ```bash
   # Exemplo: input10.txt
   [{"operation":"buy", "unit-cost":5.00, "quantity": 1000}]
   ```

2. **Execute o sistema para gerar a saída**:
   ```bash
   # Substitua temporariamente o input.txt pelo seu cenário
   dotnet run --project CapitalGain
   ```

3. **Crie o arquivo expected correspondente**:
   ```bash
   # expected10.txt com a saída correta
   [{"tax":0}]
   ```

4. **Execute os testes de integração**:
   ```bash
   dotnet test CapitalGain.IntegrationTests
   ```

### Estrutura de Commits
```
feat: adiciona cálculo de taxa de corretagem
test: adiciona testes unitários para CalculateBrokerageFee
test: adiciona cenários de integração para taxa de corretagem
docs: atualiza README com nova funcionalidade
refactor: melhora transparência referencial em TaxCalculator
fix: corrige cálculo de preço médio ponderado
```

### Exemplo de Pull Request
```markdown
## Descrição
Adiciona cálculo de taxa de corretagem como função pura

## Checklist
- [x] Método implementado como função pura
- [x] Testes unitários adicionados
- [x] Testes de integração com novos cenários
- [x] Documentação atualizada
- [x] Transparência referencial verificada
- [x] Arquivos expected criados e validados
```