# Mottu API — Gestão de Frotas (.NET 8)

API **RESTful** desenvolvida em **ASP.NET Core 8** para o gerenciamento de **Usuários**, **Motos** e **Manutenções**, com autenticação via **JWT**.

O projeto segue boas práticas de REST com **versionamento**, **Health Checks**, **paginação com HATEOAS**, **status codes** bem definidos, **testes unitários/integração** e **documentação interativa via Swagger/OpenAPI** (com exemplos de payload).

---

## Índice

* [Integrantes do Grupo](#integrantes-do-grupo)
* [Arquitetura e Decisões de Design](#arquitetura-e-decisões-de-design)

  * [Camadas](#camadas)
  * [Diagrama de Arquitetura (Modelo C4 - Nível 1)](#diagrama-de-arquitetura-modelo-c4---nível-1)
  * [Justificativa do Domínio](#justificativa-do-domínio)
* [Tecnologias e Pacotes Utilizados](#tecnologias-e-pacotes-utilizados)
* [Como Executar a API](#como-executar-a-api)
* [Como Testar a API](#como-testar-a-api)

  * [1. Testes Automatizados (xUnit)](#1-testes-automatizados-xunit)
  * [2. Teste Manual (Swagger e .http)](#2-teste-manual-swagger-e-http)
  * [3. Como Usar a Autenticação (Swagger)](#3-como-usar-a-autenticação-swagger)
* [Exemplos de Uso dos Endpoints](#exemplos-de-uso-dos-endpoints)

  * [Autenticação (Login)](#autenticação-login)
  * [Cadastrar uma nova moto](#cadastrar-uma-nova-moto)
  * [Prever Status da Manutenção (ML.NET)](#prever-status-da-manutenção-mlnet)
  * [Listar manutenções com paginação e HATEOAS](#listar-manutenções-com-paginação-e-hateoas)

---

## Integrantes do Grupo

**Gustavo Camargo de Andrade (RM555562)** — 2TDSPF
**Rodrigo Souza Mantovanello (RM555451)** — 2TDSPF
**Leonardo Cesar Rodrigues Nascimento (RM558373)** — 2TDSPF

---

## Arquitetura e Decisões de Design

A solução foi organizada para ser **simples**, **escalável** e de **fácil manutenção**, separando responsabilidades em camadas claras. Isso favorece **desacoplamento** e **testabilidade**.

### Camadas

* **Domain:** classes de negócio (`Usuario`, `Moto`, `Manutencao`). Núcleo do domínio, **sem dependências** de frameworks.
* **Data:** persistência com `AppDbContext` (**Entity Framework Core**) e **Migrations**. Isola o acesso ao banco de dados.
* **Services:** regras de negócio transversais (ex.: geração de tokens em `TokenService` e predição em `MLService`).
* **Contracts:** DTOs de entrada/saída (requests/responses), evitando expor o domínio diretamente.
* **Controllers:** expõem os endpoints HTTP, orquestrando serviços e persistência, retornando respostas padronizadas.
* **Examples:** exemplos de payload para **Swagger**, tornando a documentação mais clara e utilizável.
* **ML:** modelos de dados de entrada (`ManutencaoProblema`) e saída (`StatusPredicao`) para o **ML.NET**.

### Diagrama de Arquitetura (Modelo C4 - Nível 1)

O diagrama abaixo ilustra o **Contexto de Sistema** da Mottu API (Nível 1 do modelo C4).

```text
C4Context
  title Diagrama de Contexto de Sistema para a Mottu API

  Person(admin, "Usuário/Administrador", "Gerencia a frota de motos e agendamentos de manutenção.")

  System(mottu_api, "Mottu API", "Fornece endpoints RESTful para gerenciar usuários, motos e manutenções. Utiliza autenticação JWT.")

  SystemDb(database, "Banco de Dados SQLite", "Armazena todos os dados da aplicação, como informações de usuários, detalhes das motos e registros de manutenção.")

  Rel(admin, mottu_api, "Gerencia a frota utilizando", "JSON/HTTPS")
  Rel(mottu_api, database, "Lê e escreve dados", "Entity Framework Core")
```

### Justificativa do Domínio

* **Usuario:** base para identidade e controle de acesso aos recursos.
* **Moto:** ativo principal da frota (placa, ano, modelo).
* **Manutencao:** controle do histórico de serviços, essencial para gestão, segurança e valorização da frota.

---

## Tecnologias e Pacotes Utilizados

* **.NET 8 / ASP.NET Core Web API**
* **Entity Framework Core** com **SQLite** (persistência local)
* **Swagger / Swashbuckle** (documentação OpenAPI)
* `Swashbuckle.AspNetCore.Annotations` (metadados)
* `Swashbuckle.AspNetCore.Filters` (exemplos de payload)
* **Autenticação JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
* **BCrypt.Net-Next** (hashing seguro de senhas)
* **Asp.Versioning.Mvc** (versionamento de API)
* **Microsoft.ML** (endpoint de Machine Learning)
* **Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore** (Health Checks)
* **xUnit** e `Microsoft.AspNetCore.Mvc.Testing` (testes unitários e de integração)

---

## Como Executar a API

**Pré-requisito:** **.NET 8 SDK** instalado.

### Passos

1. **Clonar o repositório** e acessar a pasta do projeto:

```bash
git clone https://github.com/tcguus/Mottu-API
cd Mottu-API
```

2. **Restaurar as dependências** (incluindo as de teste):

```bash
dotnet restore
```

3. **Aplicar as migrations** para criar o banco:

```bash
dotnet ef database update
```

4. **Executar a API**:

```bash
dotnet run
```

5. **Acessar o Swagger** para interagir com a API:

```
http://localhost:5075/swagger
```

6. **Verificar a saúde da aplicação (Health Check):**

```
http://localhost:5075/health
```

> **Nota:** O banco `mottu.db` será criado na raiz do projeto. Um usuário **admin** é criado automaticamente para testes:
> **Email:** `admin@mottu.com` — **Senha:** `123456`

---

## Como Testar a API

A API pode ser testada de duas formas:

### 1. Testes Automatizados (xUnit)

O projeto inclui uma suíte de **testes unitários** (validando a lógica de negócio) e **testes de integração** (validando a configuração da API, como segurança e health checks).

**Como usar:**

* Certifique-se de que restaurou os pacotes (`dotnet restore`).
* Na pasta raiz do projeto, execute:

```bash
dotnet test
```

O resultado dos testes será exibido no console.

### 2. Teste Manual (Swagger e .http)

Você pode testar os endpoints manualmente usando a interface do **Swagger** ou o arquivo **`Mottu.Api.http`**.

* **Swagger:** `http://localhost:5075/swagger`
* **Arquivo .http:** abra `Mottu.Api.http` no **JetBrains Rider** ou no **VS Code** (com a extensão *REST Client*).

### 3. Como Usar a Autenticação (Swagger)

A maioria dos endpoints é protegida e retornará **401 Unauthorized** se um token JWT não for fornecido.

**Passo a passo:**

1. Execute a API (`dotnet run`) e abra o Swagger.
2. Vá até o endpoint **POST** `/api/v1/Auth/login`.
3. Clique em **Try it out** e execute o login com o usuário admin:

```json
{
  "email": "admin@mottu.com",
  "senha": "123456"
}
```

4. Copie o **token** da resposta.
5. No topo da página, clique no botão **Authorize**.
6. Na janela que abrir, digite `Bearer ` (com **B** maiúsculo e um espaço) e cole o seu token.
   **Exemplo:** `Bearer eyJhbGciOiJIUzI1NiIs...`
7. Clique em **Authorize** e **Close**.

Pronto! Os cadeados nos endpoints ficarão fechados e você poderá testar as rotas protegidas.

---

## Exemplos de Uso dos Endpoints

### Autenticação (Login)

**Request**

```
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "admin@mottu.com",
  "senha": "123456"
}
```

**Response — 200 OK**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "usuario": {
    "id": "GUID...",
    "nome": "Admin Demo",
    "email": "admin@mottu.com"
  }
}
```

---

### Cadastrar uma nova moto

**Request**

```
POST /api/v1/Motos
Content-Type: application/json
Authorization: Bearer {seu_token}

{
  "placa": "NEW-9876",
  "ano": 2024,
  "modelo": "Sport"
}
```

**Response — 201 Created**

```json
{
  "placa": "NEW-9876",
  "ano": 2024,
  "modelo": "Sport"
}
```

---

### Prever Status da Manutenção (ML.NET)

**Request**

```
POST /api/v1/ml/predict-status
Content-Type: application/json
Authorization: Bearer {seu_token}

{
  "problemas": "motor falhando"
}
```

**Response — 200 OK**

```json
{
  "status": "Aberta"
}
```

---

### Listar manutenções com paginação e HATEOAS

**Request**

```
GET /api/v1/Manutencoes?page=1&pageSize=5
Authorization: Bearer {seu_token}
```

**Response — 200 OK**

```json
{
  "items": [
    {
      "id": "0042",
      "placa": "ABC-1234",
      "problemas": "Troca de óleo",
      "status": "Aberta",
      "data": "2025-09-29T18:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 5,
  "total": 1,
  "links": [
    { "rel": "self", "href": "http://localhost:5075/api/v1/manutencoes?page=1&pageSize=5", "method": "GET" }
  ]
}
```
