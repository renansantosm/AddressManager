# Address Manager 📍
[![NPM](https://img.shields.io/npm/l/react)](https://github.com/renansantosm/AddressManager/blob/master/LICENSE) 

API RESTful desenvolvida em .NET para o cadastro e gerenciamento de endereços, com um diferencial : a automação, resiliência e validação de dados através da integração com a API externa ViaCEP.

## ✨ Funcionalidades Principais
* 🔎 **Cadastro Automatizado**: Cria endereços completos a partir do CEP, garantindo dados precisos através da integração com a ViaCEP.
* ⚡ **Cache Inteligente**: Otimiza o desempenho com cache (IMemoryCache) das respostas da ViaCEP, evitando chamadas repetitivas
* 🛡️ **Tratamento Global de Exceções**: Um Middleware centralizado para tratamento de erros, garantindo respostas de API consistentes.
* 🔎 **Logging Estruturado**: Rastreamento completo das operações com logs estruturados (Serilog) em pontos-chave do fluxo.
* 🔄 **Resiliência em Chamadas Externas**: Políticas de resiliência (Retry & Circuit Breaker) para proteger a comunicação com APIs externas.
* 📖 **Documentação Swagger**: API documentada com Swagger/OpenAPI e detalhada através de comentários XML no código.

## 🛠️ Tecnologias e Arquitetura
* **.NET 9** - Framework para a construção da API
* **Entity Framework Core** - ORM para acesso a dados
* **SQL Server** - Banco de dados relacional
* **Serilog** - Provedor de logging estruturado
* **FluentValidation** - Biblioteca para validações de entrada
* **IMemoryCache** - Cache em memória para otimização de performance
* **Microsoft.Extensions.Http.Resilience** - Políticas de resiliência para chamadas HTTP
* **Docker** - Containerização da aplicação e do banco de dados

## 🏗️ Arquitetura e Padrões de Design

### Princípios Arquiteturais
- **Clean Architecture:** Separação clara de responsabilidades em camadas (Domain, Application, Infra, API).
- **Domain-Driven Design (DDD):** Foco em um domínio rico com Value Objects, Entidades e validações de negócio.

### Padrões Implementados
- **Repository & Unit of Work:** Abstração da persistência e garantia de consistência transacional.
- **Factory Pattern:** Criação controlada e encapsulada de objetos de domínio.

## 🔗 Endpoint Principal
```
POST /api/v1/addresses - Cadastra endereço a partir do CEP
```

## 🚀 Como Executar

### 🐳 Execução com Docker (Recomendado)
**Pré-requisitos:** Docker e Git

```bash
# Clone o repositório
git clone [url-do-repositorio]
cd cep-api

# Execute com docker-compose (inclui SQL Server)
docker-compose up -d

# A aplicação estará disponível em:
# http://localhost:8080
# Swagger: http://localhost:8080/swagger
```

### 🔧 Execução Local (Desenvolvimento)
**Pré-requisitos:** .NET 9 SDK e SQL Server

```bash
# Restaure as dependências
dotnet restore

# Execute migrations
dotnet ef database update

# Execute a aplicação
dotnet run --project src/CepApi.API

# Acesse: https://localhost:7001/swagger
```
