# Address Manager 📍
[![NPM](https://img.shields.io/npm/l/react)](https://github.com/renansantosm/AddressManager/blob/master/LICENSE) 

API RESTful desenvolvida em .NET para o cadastro e gerenciamento de endereços, com um diferencial : a automação, resiliência e validação de dados através da integração com a API externa ViaCEP.

## ✨ Funcionalidades Principais
* 🔎 **Cadastro Automatizado**: Cria endereços completos a partir do CEP via integração com a ViaCEP.
* ⚡ **Cache Inteligente**: Cacheia respostas da ViaCEP permitindo criar múltiplos endereços do mesmo CEP
* 🛡️ **Tratamento Global de Exceções**: Um Middleware centralizado para tratamento de erros, garantindo respostas de API consistentes.
* 🔎 **Logging Estruturado**: Rastreamento completo das operações com logs estruturados (Serilog) em pontos-chave do fluxo.
* 🔄 **Resiliência em Chamadas Externas**: Políticas de resiliência (Retry & Circuit Breaker) para proteger a comunicação com APIs externas.
* 📖 **Documentação Interativa**: API documentada com Swagger/OpenAPI e detalhada através de comentários XML no código.

## 🛠️ Tecnologias Utilizadas
* **.NET 9** - Framework principal
* **Entity Framework Core** - ORM
* **SQL Server** - Banco de dados
* **Serilog** - Logging estruturado
* **FluentValidation** - Validações de entrada
* **IMemoryCache** - Sistema de cache
* **Microsoft.Extensions.Http.Resilience** - Resiliência HTTP
* **Docker & Docker Compose** - Containerização

## 🏗️ Arquitetura e Padrões de Design

### Princípios Arquiteturais
- **Clean Architecture:** Separação clara de responsabilidades em camadas (Domain, Application, Infra, API).
- **Domain-Driven Design (DDD):** Value Objects, encapsulamento de estado, validações e métodos de domínio.

### Padrões Implementados
- **Repository Pattern** - Abstração da camada de persistência
- **Unit of Work** - Controle transacional e coordenação de repositórios
- **Factory Pattern:** Criação controlada e encapsulada de objetos de domínio.

## 🔗 Endpoint Principal
```
POST /api/v1/addresses - Cadastra endereço a partir do CEP
```
**Request:**
```json
{
  "zipCode": "01001000",
  "number": "123",
  "complement": "Apto 45",
  "reference": "Próximo ao metrô"
}
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "zipCode": "01001000",
  "street": "Praça da Sé",
  "number": "123",
  "complement": "Apto 45",
  "reference": "Próximo ao metrô",
  "neighborhood": "Sé",
  "city": "São Paulo",
  "state": "SP",
  "region": "Sudeste"
}
```

## 🚀 Como Executar

### 🐳 Execução com Docker (Recomendado)
**Pré-requisitos:** Docker e Git

```bash
# Clone o repositório
git clone https://github.com/renansantosm/AddressManager
cd addressmanager

# Execute com docker-compose (inclui SQL Server)
docker-compose up -d

# A aplicação estará disponível em:
# http://localhost:8081
# Swagger: http://localhost:8081/swagger

# Parar a aplicação
docker-compose down

# Parar a aplicação e remover dados do banco
docker-compose down -v
```

### 🔧 Execução Local (Desenvolvimento)
**Pré-requisitos:** .NET 9 SDK, SQL Server e Git

```bash
# Clone o repositório
git clone https://github.com/renansantosm/AddressManager
cd addressmanager

# Restaure as dependências
dotnet restore

# Execute a aplicação
dotnet run --project src/AddressManager.API

# Acesse a documentação Swagger
# # http://localhost:5194/swagger
# # https://localhost:7140/swagger
```

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.
