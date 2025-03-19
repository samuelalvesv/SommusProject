# SommusProject - Sistema de Monitoramento de Dengue

## Descrição

Esse projeto foi idealizado pela empresa Sommus Sistemas. O sistema se integra com a API AlertaDengue, armazena dados dos últimos 6 meses e disponibiliza essas informações através de endpoints RESTful, facilitando o acompanhamento epidemiológico de Belo Horizonte - MG.

## Tecnologias Utilizadas

- **Backend**: .NET 9 com C# (versão mais recente da plataforma)
- **Banco de Dados**: MySQL 8.0+
- **ORM**: Dapper (micro-ORM para acesso a dados eficiente)
- **Testes**: xUnit, Moq
- **Documentação API**: Swagger
- **Integração**: HttpClient para comunicação com APIs externas

## Requisitos de Sistema
- .NET SDK 9.0 ou superior
- MySQL 8.0 ou superior
- Acesso à internet (para consulta à API AlertaDengue)

## Estrutura do Projeto

O projeto segue uma arquitetura em camadas para separação de responsabilidades:

### Controllers
- Responsáveis pelo tratamento das requisições HTTP
- Implementação dos endpoints REST
- Mapeamento de rotas e validação de parâmetros
- Tratamento consistente de erros e exceções

### Data
- Configuração do contexto de dados
- Classes para acesso ao banco de dados

### Models
- Entidades de domínio como `AlertDengue`
- DTOs (Data Transfer Objects)
- Mapeamento de propriedades com atributos `JsonPropertyName`

### Options
- Record `AlertDengueOptions` para configurações personalizáveis da API AlertaDengue
- Valores padrão para URL base, código geográfico (Belo Horizonte: 3106200), doença e período

### Repositories
- Implementação do padrão Repository
- Acesso a dados encapsulado
- Consultas ao banco de dados via Dapper
- Utilização de queries SQL otimizadas
  
### Services
- Lógica de negócios
- Integração com API externa (AlertaDengue)
- Processamento e transformação de dados
- Validação de regras de negócio

### Tests
- Testes unitários
- Mock de dependências para isolamento de testes

### Integração Contínua

O projeto utiliza GitHub Actions para execução automatizada de testes, garantindo que o código mantenha sua qualidade a cada modificação:

- **Testes Automatizados**: Execução dos testes unitários em cada push e pull request
- **Validação de Código**: Verificação da integridade do projeto a cada alteração
- **Fluxo de Trabalho**: Configurado no arquivo `.github/workflows/dotnet.yml`

Esta abordagem permite identificar problemas rapidamente e manter a qualidade do código de forma contínua.

## Configuração do Banco de Dados

1. Instale o MySQL Server 8.0+
2. Execute os scripts SQL disponíveis na pasta `Scripts` para criar:
   - Banco de dados SOMMUS
   - Tabela ALERTA_DENGUE
   - Stored procedure para consulta
3. Atualize a string de conexão no arquivo `appsettings.json`:

```json
"ConnectionStrings": {
  "MySqlConnection": "Server=localhost;Database=SOMMUS;User=seu_usuario;Password=sua_senha;"
}
```

## Configuração da API AlertaDengue

O sistema está pré-configurado para acessar a API AlertaDengue com os seguintes parâmetros:

- URL Base: https://info.dengue.mat.br/api/alertcity
- Código Geográfico: 3106200 (Belo Horizonte)
- Doença: dengue
- Formato: json
- Período: últimos 6 meses

Para personalizar estas configurações, modifique os valores no record `SommusProject/Options/AlertDengueOptions.cs`.

## Executando o Projeto

1. Clone o repositório:
```
git clone https://github.com/seu-usuario/SommusProject.git
```

2. Navegue até a pasta do projeto:
```
cd SommusProject
```

3. Restaure as dependências:
```
dotnet restore
```

4. Execute a aplicação:
```
dotnet run --project SommusProject
```

5. Acesse a documentação Swagger:
```
https://localhost:5001/swagger
```

## Endpoints da API

### 1. GET /AlertDengue/GetByWeek
Consulta dados salvos no banco de dados por semana epidemiológica específica. Inclui validações para garantir que a semana e o ano informados sejam válidos.

**Parâmetros:**
- `ew`: Semana epidemiológica (inteiro entre 1 e número máximo de semanas do ano)
- `ey`: Ano epidemiológico (inteiro com 4 dígitos)

**Exemplo de Requisição:**
```
GET /AlertDengue/GetByWeek?ew=10&ey=2025
```

**Exemplo de Resposta:**
```json
{
  "dataInicioSemanaEpidemiologicDaTimestamp": 1696204800000,
  "semanaEpidemiologica": 202340,
  "casosEstimados": 45,
  "casosEstimadosMinimo": 36,
  "casosEstimadosMaximo": 58,
  "casosNotificados": 38,
  "probabilidadeTransmissao": 0.87,
  "incidenciaPor100kHabitantes": 1.79,
  "codigoLocalidade": 0,
  "nivelRisco": 2,
  "identificador": 3106200202340,
  "versaoModelo": "2023-10-05",
  "taxaTransmissao": 1.23,
  "populacao": 2521564,
  "temperaturaMinima": 18.7,
  "umidadeMaxima": 94.2,
  "areaReceptiva": 1,
  "areaTransmissao": 1,
  "nivelIncidencia": 1,
  "umidadeMedia": 65.3,
  "umidadeMinima": 45.8,
  "temperaturaMedia": 23.4,
  "temperaturaMaxima": 28.9,
  "casosProvaveis": 38,
  "notificacoesAcumuladasAno": 25789
}
```

### 2. GET /AlertDengue/Service
Retorna os dados de alertas de dengue dos últimos 6 meses para Belo Horizonte.

**Exemplo de Requisição:**
```
GET /AlertDengue/Service
```

**Exemplo de Resposta:**
```json
[
  {
    "dataInicioSemanaEpidemiologicDaTimestamp": 1696204800000,
    "semanaEpidemiologica": 202340,
    "casosEstimados": 45,
    "casosNotificados": 38,
    "nivelRisco": 2
    // outros campos...
  },
  {
    "dataInicioSemanaEpidemiologicDaTimestamp": 1696809600000,
    "semanaEpidemiologica": 202341,
    "casosEstimados": 52,
    "casosNotificados": 43,
    "nivelRisco": 2
    // outros campos...
  }
  // mais registros...
]
```

### 3. POST /AlertDengue/Service
Importa novos alertas de dengue da API AlertaDengue para o banco de dados local. Valida e armazena apenas dados que ainda não existem no banco.

**Exemplo de Requisição:**
```
POST /AlertDengue/Service
```

**Exemplo de Resposta:**
```json
[
  {
    "dataInicioSemanaEpidemiologicDaTimestamp": 1712534400000,
    "semanaEpidemiologica": 202414,
    "casosEstimados": 120,
    "casosNotificados": 98,
    "nivelRisco": 3
    // outros campos...
  },
  // mais registros importados...
]
```

## Tratamento de Erros

O sistema implementa um tratamento de erros consistente em todos os endpoints:

- **Erros 400**: Para parâmetros inválidos (ex: semana ou ano inválidos)
- **Erros 404**: Quando não são encontrados dados
- **Erros 500**: Para falhas internas ou problemas de comunicação com a API externa

Todas as exceções são devidamente registradas no log do sistema para facilitar o diagnóstico de problemas.

## Observações

- O projeto SommusProject contempla apenas as etapas 1 e 2 do desafio
- A etapa 3 (visualização dos dados) está implementada em outro projeto chamado SommusView

## Considerações Finais

Este projeto segue as boas práticas de desenvolvimento como SOLID e Clean Code. A arquitetura em camadas facilita a manutenção e extensão do sistema. A documentação completa da API está disponível via Swagger.
