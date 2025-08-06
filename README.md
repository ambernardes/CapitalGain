# CapitalGain

## Descrição
Sistema para cálculo de impostos sobre ganhos de capital em operações de compra e venda de ações, conforme regras fiscais brasileiras. A aplicação processa dados JSON e retorna os valores de imposto devidos por operação.

## Requisitos
- .NET 8.0 SDK
- Terminal/Prompt de comando

## Estrutura do Projeto

```
CapitalGain.sln
│
├── CapitalGain/                     --> Projeto principal
│   ├── Program.cs                   --> Ponto de entrada
│   ├── Models/
│   │   ├── OperationEntry.cs        --> Dados de entrada
│   │   └── TaxConfiguration.cs      --> Configuração de impostos
│   └── Services/
│       ├── CapitalGainService.cs    --> Lógica principal
│       └── TaxCalculator.cs         --> Cálculos matemáticos (funções puras)
│
├── CapitalGain.Tests/               --> Testes unitários
└── CapitalGain.IntegrationTests/    --> Testes de integração
    ├── Scenarios/FullFlowTests.cs
    └── Fixtures/                    --> Cenários de teste
        ├── input0.txt ... input9.txt
        └── expected0.txt ... expected2.txt
```

## Execução

### 1. Prepare o arquivo de entrada

```bash
# Crie um arquivo com suas operações
cat > operacoes.txt << 'EOF'
[{"operation":"buy", "unit-cost":10.00, "quantity": 1000}, {"operation":"sell", "unit-cost":15.00, "quantity": 500}]
[{"operation":"buy", "unit-cost":20.00, "quantity": 500}, {"operation":"sell", "unit-cost":18.00, "quantity": 200}]
EOF
```

### 2. Execute o programa

```bash
# Navegue para a pasta e compile
cd CapitalGain
dotnet build

# Execute com arquivo de entrada (OBRIGATÓRIO)
dotnet run --project CapitalGain -- --input operacoes.txt

# Com configurações personalizadas (opcionais - caso não informadas, usa valores padrão)
dotnet run --project CapitalGain -- --input operacoes.txt --tax-rate 0.15 --exemption-limit 25000
```

> **Nota**: O parâmetro `--input` é obrigatório. Os demais parâmetros são opcionais e, se não informados, utilizam os valores padrão conforme especificação: taxa de 20% e limite de isenção de R$ 20.000.
Essa abordagem torna o código extensível e flexível, permitindo testar diferentes cenários e configurações diretamente via linha de comando, sem a necessidade de alterar o código-fonte.

### 3. Execute os testes

```bash
# Todos os testes
dotnet test

# Apenas unitários
dotnet test CapitalGain.Tests

# Apenas integração
dotnet test CapitalGain.IntegrationTests
```

## Parâmetros

- **`--input` / `-i`**: Arquivo de entrada (OBRIGATÓRIO)
- **`--tax-rate` / `-t`**: Taxa de imposto (padrão: 0.20 = 20%)
- **`--exemption-limit` / `-e`**: Limite de isenção (padrão: 20000 = R$ 20.000)

## Formato do Arquivo

- Uma linha por lote de operações
- JSON válido com: `operation`, `unit-cost`, `quantity`
- Operações: `"buy"` (compra) ou `"sell"` (venda)

### Exemplo:
```json
[{"operation":"buy", "unit-cost":10.00, "quantity": 100}]
[{"operation":"sell", "unit-cost":15.00, "quantity": 50}]
```

## Regras de Negócio

Regras implementadas conforme [especificação](docs/spec-ptbr.pdf). 

## Arquitetura

### Transparência Referencial
O projeto usa **funções puras** na classe `TaxCalculator`:
- Sempre retornam o mesmo resultado para as mesmas entradas
- Sem efeitos colaterais
- Facilita testes e manutenção

```csharp
// Exemplo: função pura
var preco1 = TaxCalculator.CalculateWeightedAveragePrice(10.00m, 100, 20.00m, 100);
var preco2 = TaxCalculator.CalculateWeightedAveragePrice(10.00m, 100, 20.00m, 100);
// preco1 == preco2 sempre será true
```

### Separação de Responsabilidades
- **`CapitalGainService`**: Controle de fluxo e regras de negócio
- **`TaxCalculator`**: Cálculos matemáticos puros
- **`TaxConfiguration`**: Configurações do sistema
- **`OperationEntry`**: Representação de dados

## Testes

### Testes Unitários
- **`TaxCalculatorTests`**: Valida funções matemáticas puras
- **`CapitalGainServiceTests`**: Valida regras de negócio

**Principais cenários testados:**
- Cálculos de preço médio ponderado
- Operações de compra e venda
- Limites de isenção
- Prejuízos acumulados
- Configurações personalizadas

### Testes de Integração
- **`FullFlowTests`**: Valida fluxo completo end-to-end
- Usa arquivos de fixtures com cenários reais
- Compara resultados com saídas esperadas

### Comandos Úteis
```bash
# Testes específicos
dotnet test --filter "TaxCalculatorTests"
dotnet test --filter "CalculateWeightedAveragePrice"

# Com detalhes
dotnet test --verbosity normal

# Por categoria
dotnet test --filter "ExemptionLimit|AccumulatedLoss"
```

## Desenvolvimento

### Adicionando Novos Cálculos
```csharp
public static class TaxCalculator
{
    public static decimal NovoCalculo(decimal entrada)
    {
        // Manter função pura: sem efeitos colaterais
        return Math.Round(entrada * 0.1m, 2);
    }
}
```

### Testando
```csharp
[Fact]
public void NovoCalculo_ValidInput_ReturnsExpected()
{
    // Arrange, Act, Assert
    var result = TaxCalculator.NovoCalculo(100m);
    Assert.Equal(10m, result);
}
```

## Contribuição

1. **Mantenha funções puras** em novos métodos de cálculo
2. **Teste isoladamente** cada nova funcionalidade
3. **Use nomes descritivos** seguindo padrão `Method_Scenario_Expected`
4. **Adicione cenários de integração** para fluxos complexos