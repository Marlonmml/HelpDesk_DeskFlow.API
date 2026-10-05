# DeskFlow.API

API REST para gestão de chamados (tickets) de suporte técnico de TI, desenvolvida em **ASP.NET Core** com **Entity Framework Core** e **SQL Server**.

Permite cadastrar categorias de incidentes (Hardware, Software, Redes, Gestão de acesso), abrir chamados, controlar o ciclo de vida do atendimento (Aberto → Em Andamento → Fechado), registrar interações de suporte e consultar chamados com filtros combinados.

---

## Índice

- [Pré-requisitos](#pré-requisitos)
- [Como executar do zero](#como-executar-do-zero)
- [Arquitetura em camadas](#arquitetura-em-camadas)
- [Ciclo de vida do chamado](#ciclo-de-vida-do-chamado)
- [Tratamento de erros](#tratamento-de-erros)
- [Endpoints](#endpoints)
- [Vídeo explicativo](#apresentação-do-projeto)

---

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (versão compatível com o `TargetFramework` do `.csproj`, ex: .NET 10)
- [Docker](https://www.docker.com/products/docker-desktop/) (para rodar o SQL Server em container)
- Ferramenta `dotnet-ef`:
  ```
  dotnet tool install --global dotnet-ef
  ```

---

## Como executar do zero

REALIZE O CLONE: ``` git clone https://github.com/Marlonmml/HelpDesk_DeskFlow.API.git ```


### 1. Subir o SQL Server via Docker

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=SuaSenha@123" \
  -p 1433:1433 --name sqlserver-deskflow \
  -v sqlserver-deskflow-data:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

A senha precisa ter no mínimo 8 caracteres, com maiúscula, minúscula, número e símbolo. Aguarde cerca de 20-30 segundos para o banco finalizar a inicialização.


### 2. Configurar a connection string

No arquivo `appsettings.json`, ajuste a senha para a mesma usada no passo anterior:

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DeskFlowDb;User Id=sa;Password=SuaSenha@123;TrustServerCertificate=True"
  }
}
```

### 3. Restaurar dependências e aplicar as migrations

Na raiz do projeto (onde está o `.csproj`):

```
dotnet restore
dotnet ef database update
```

Isso cria o banco `DeskFlowDb` e todas as tabelas (`Categorias`, `Chamados`, `Interacoes`) com os relacionamentos configurados, sem precisar de nenhum banco pré-populado.

### 4. Executar a aplicação

```
dotnet run
```

O terminal exibirá a porta em uso, por exemplo:
```
Now listening on: http://localhost/8080
```

### 5. Testar pelo Swagger

Acesse o localhost junto com `/swagger` no navegador. Lá é possível testar cada endpoint diretamente (**Try it out** → preencher os campos → **Execute**).

**Fluxo de teste sugerido:**
1. `POST /api/categorias` — cadastra uma categoria (ex: `"Hardware"`)
2. `POST /api/chamados` — abre um chamado usando o `categoriaId` retornado
3. `GET /api/chamados/{id}` — confirma os dados, já com categoria e interações
4. `POST /api/chamados/{id}/iniciar` — inicia o atendimento
5. `POST /api/chamados/{id}/interacoes` — adiciona um comentário de suporte
6. `POST /api/chamados/{id}/encerrar` — encerra o chamado com a solução

---

## Arquitetura em camadas

O projeto segue uma arquitetura em camadas, com responsabilidades bem separadas e fluxo de execução unidirecional:

```
Controller → Service → Repository → AppDbContext → SQL Server
```

```
DeskFlow.API/
├── Controllers/     → Exposição HTTP: rotas, verbos e status codes
├── Services/        → Regras de negócio: validações e coordenação das operações
├── Repositories/     → Acesso a dados: comandos EF Core (inserção, atualização, consulta, remoção)
├── Models/
│   └── Entities/    → Entidades do banco (Chamados, Categorias, Interacao)
├── Middlewares/     → Middleware global de tratamento de exceções
└── Data/
    ├── AppDbContext.cs
    └── Migrations/
```

| Camada | Responsabilidade |
|---|---|
| **Program** | Inicia a aplicação e monta a pipeline de configuração (DI, middlewares, Swagger) |
| **Controllers** | Recebem as requisições HTTP de acordo com a rota e devolvem o status code adequado |
| **Services** | Implementam as regras de negócio, validam dados e coordenam as operações antes de acessar o banco |
| **Repositories** | Comunicam-se exclusivamente com o SQL Server, executando os comandos de CRUD |
| **Models/Entities** | Representam as entidades do sistema (classes mapeadas para tabelas) |
| **AppDbContext** | Centraliza a configuração da conexão e o mapeamento das entidades via EF Core |

Essa separação reduz o acoplamento entre componentes: o Controller nunca acessa o banco diretamente, e o Repository nunca decide regra de negócio, apenas executa o que o Service solicita.

---

## Ciclo de vida do chamado

Um chamado percorre três estados (`StatusChamado`), controlados exclusivamente pelas rotas dedicadas do `ChamadoController`:

```
Aberto  --POST /{id}/iniciar-->  EmAndamento  --POST /{id}/encerrar-->  Fechado
```

- **Aberto**: estado inicial, atribuído automaticamente na criação (`POST /api/chamados`), junto da `DataAbertura`.
- **EmAndamento**: definido ao chamar `POST /api/chamados/{id}/iniciar`.
- **Fechado**: definido ao chamar `POST /api/chamados/{id}/encerrar`, que também grava a `Solucao` informada e a `DataFechamento`.

Interações de suporte (`POST /api/chamados/{id}/interacoes`) só podem ser adicionadas enquanto o chamado **não** estiver `Fechado`.

---

## Tratamento de erros

Um `ExceptionHandlingMiddleware` global captura qualquer exceção não tratada e converte para uma resposta JSON padronizada, sem expor stack trace:

```json
{ "erro": "mensagem descritiva" }
```

| Exceção lançada pelo Service | Status code |
|---|---|
| `KeyNotFoundException` | `404 Not Found` |
| `InvalidOperationException` | `400 Bad Request` |
| Qualquer outra exceção | `500 Internal Server Error` |

---

## Endpoints

### Categorias

| Verbo | Rota | Descrição |
|---|---|---|
| `POST` | `/api/categorias` | Cadastra uma categoria (nome deve ser um dos valores fixos permitidos) |
| `GET` | `/api/categorias` | Lista todas as categorias |
| `GET` | `/api/categorias/{id}` | Busca uma categoria por id |
| `PUT` | `/api/categorias/{id}` | Atualiza o nome de uma categoria |
| `DELETE` | `/api/categorias/{id}` | Remove uma categoria (bloqueado se houver chamados associados) |

### Chamados

| Verbo | Rota | Descrição |
|---|---|---|
| `POST` | `/api/chamados` | Abre um novo chamado (`Status = Aberto`) |
| `GET` | `/api/chamados` | Lista chamados, com filtros opcionais e combináveis via query string: `status`, `prioridade`, `categoriaId` |
| `GET` | `/api/chamados/{id}` | Retorna o chamado com a categoria e a lista de interações associadas |
| `POST` | `/api/chamados/{id}/iniciar` | Transiciona o status para `EmAndamento` |
| `POST` | `/api/chamados/{id}/encerrar` | Grava a solução, a data de fechamento e transiciona o status para `Fechado` |
| `POST` | `/api/chamados/{id}/interacoes` | Adiciona um comentário de suporte (bloqueado se o chamado já estiver `Fechado`) |

----

## Apresentação do projeto

[Vídeo Explicativo](https://drive.google.com/file/d/1YWLddgD4iW7FAbjNXTPvKBwaXpV7hXto/view?usp=drive_link)