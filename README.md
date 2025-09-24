# Mottu API — Gestão de Frotas (.NET 8)

API RESTful desenvolvida em ASP.NET Core 8 para o gerenciamento de **Usuários**, **Motos** e **Manutenções**, com autenticação via **JWT**.

O projeto foi estruturado com foco em boas práticas REST, implementando funcionalidades como **paginação**, **HATEOAS**, uso correto de status codes HTTP e uma documentação interativa completa com **Swagger/OpenAPI**, incluindo exemplos de payload.

---

## Integrantes do Grupo

- **Gustavo Camargo de Andrade** (RM555562) - 2TDSPF
- **Rodrigo Souza Mantovanello** (RM555451) - 2TDSPF
- **Leonardo Cesar Rodrigues Nascimento** (RM558373) - 2TDSPF

---

## Arquitetura e Decisões de Design

A solução foi organizada em uma estrutura de pastas simples e funcional para separar responsabilidades:

-   **Domain**: Contém as classes de negócio (`Usuario`, `Moto`, `Manutencao`). São os objetos centrais do domínio, sem dependências externas.
-   **Data**: Responsável pela persistência de dados, contém o `AppDbContext` do Entity Framework Core e as `Migrations`.
-   **Services**: Camada de serviço que abstrai a lógica de negócio, como a geração de tokens (`TokenService`).
-   **Contracts**: Define os DTOs (Data Transfer Objects), que são os contratos de entrada e saída da API, garantindo que o domínio não seja exposto diretamente.
-   **Controllers**: Camada de apresentação (endpoints). Recebe as requisições HTTP, utiliza os serviços e DTOs e retorna as respostas.
-   **Examples**: Contém classes que fornecem exemplos de payloads (request/response) para o Swagger, melhorando a documentação.

### Justificativa do Domínio
O escopo do projeto foi pensado para um cenário real de gestão de frotas, como uma locadora de motos ou uma empresa de entregas:
-   **Usuario**: Garante a identidade e o acesso seguro aos recursos da API.
-   **Moto**: É o ativo principal, com informações essenciais como placa, ano e modelo.
-   **Manutencao**: Permite o rastreamento do histórico de serviços realizados, um requisito vital para a gestão e segurança da frota.

---

## Tecnologias e Pacotes Utilizados

-   **.NET 8** / ASP.NET Core Web API
-   **Entity Framework Core** com **SQLite** para persistência de dados local.
-   **Swagger / Swashbuckle** para documentação da API.
    -   `Swashbuckle.AspNetCore.Annotations` para enriquecer a documentação.
    -   `Swashbuckle.AspNetCore.Filters` para habilitar exemplos de payloads.
-   **Autenticação JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`).
-   **BCrypt.Net-Next** para hashing de senhas.

---

## Como Executar a API

**Pré-requisitos:**
-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

**Passos:**

1.  Clone o repositório:
    ```bash
    git clone https://github.com/tcguus/Mottu-API
    cd Mottu.Api
    ```

2.  Restaure as dependências do projeto:
    ```bash
    dotnet restore
    ```

3.  Aplique as migrations para criar o banco de dados SQLite:
    ```bash
    dotnet ef database update
    ```

4.  Execute a API:
    ```bash
    dotnet run
    ```
5.  Acesse a documentação interativa do Swagger no seu navegador, geralmente em: `http://localhost:5000/swagger`.

> **Nota:** O banco de dados (`mottu.db`) será criado na raiz do projeto na primeira execução, e um usuário administrador e duas motos serão adicionados automaticamente (seeding).

---

## Autenticação com JWT

A API utiliza tokens JWT para proteger os endpoints.

1.  **Registre um usuário:**
    -   Use o endpoint `POST /api/v1/Auth/register` para criar um novo usuário.
2.  **Faça o login:**
    -   Use o endpoint `POST /api/v1/Auth/login` com as credenciais criadas para obter um token de acesso.
3.  **Autorize suas requisições:**
    -   No Swagger, clique no botão **"Authorize"** e cole o token no formato `Bearer {seu_token}`.
    -   Em outras ferramentas, adicione o header `Authorization: Bearer {seu_token}` às suas requisições.

---

## Endpoints da API

URL Base: `/api/v1`

### Auth
-   `POST /Auth/register` - Registra um novo usuário.
-   `POST /Auth/login` - Autentica um usuário e retorna um token JWT.

### Usuarios
-   `GET /Usuarios` - Lista todos os usuários.
-   `GET /Usuarios/{id}` - Busca um usuário por ID.
-   `PUT /Usuarios/{id}` - Atualiza um usuário.
-   `DELETE /Usuarios/{id}` - Exclui um usuário.

### Motos
-   `GET /Motos` - Lista todas as motos com **paginação** e **HATEOAS**.
    -   Parâmetros: `page` (número da página) e `pageSize` (itens por página).
-   `GET /Motos/{placa}` - Busca uma moto pela placa.
-   `POST /Motos` - Cadastra uma nova moto.
-   `DELETE /Motos/{placa}` - Exclui uma moto.

### Manutencoes
-   `GET /Manutencoes` - Lista todas as manutenções com **paginação** e **HATEOAS**.
-   `GET /Manutencoes/{id}` - Busca uma manutenção por ID.
-   `POST /Manutencoes` - Abre uma nova manutenção para uma moto.
-   `PUT /Manutencoes/{id}` - Atualiza uma manutenção.
-   `DELETE /Manutencoes/{id}` - Exclui uma manutenção.

---

## Testes

O projeto ainda não possui uma suíte de testes automatizados. Para executar os testes quando forem implementados, o comando será:

```bash
dotnet test
