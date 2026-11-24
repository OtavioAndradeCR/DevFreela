# DevFreela — API de Gestão de Projetos Freelance

API REST para gerenciamento de projetos entre clientes e freelancers, desenvolvida com C# e ASP.NET Core no .NET 8.

O projeto permite cadastrar usuários, consultar projetos e habilidades, acompanhar o ciclo de vida dos projetos e registrar comentários.

## Tecnologias

- C# e .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8 e SQL Server
- MediatR
- Swagger / OpenAPI

## Organização da solução

A solução é dividida em quatro projetos:

| Projeto | Responsabilidade |
|---|---|
| `DevFreela` | API REST, controllers, configuração e injeção de dependências |
| `DevFreela.Application` | Casos de uso, comandos, consultas, serviços e modelos de resposta |
| `DevFreela.Core` | Entidades, enums, regras de domínio e contratos de repositório |
| `DevFreela.Infrastructure` | Persistência com Entity Framework Core, mapeamentos e repositórios |

A camada Application depende de abstrações de persistência definidas em seu próprio projeto. A implementação concreta do contexto fica em Infrastructure, registrada no container de injeção de dependências.

## Funcionalidades e endpoints

### Projetos

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/projects` | Lista projetos; aceita o parâmetro opcional `query` para filtrar pelo título |
| GET | `/api/projects/{id}` | Consulta os detalhes de um projeto |
| POST | `/api/projects` | Cadastra um projeto |
| PUT | `/api/projects/{id}` | Atualiza título, descrição e valor |
| PUT | `/api/projects/{id}/start` | Inicia um projeto |
| PUT | `/api/projects/{id}/finish` | Finaliza um projeto |
| PUT | `/api/projects/{id}/cancel` | Cancela um projeto |
| POST | `/api/projects/{id}/comments` | Adiciona um comentário ao projeto |

O cancelamento altera o status do projeto; não remove fisicamente o registro do banco de dados. Operações incompatíveis com o estado atual retornam HTTP 409 (Conflict), enquanto identificadores inexistentes retornam HTTP 404 (Not Found).

### Usuários

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/users/{id}` | Consulta um usuário |
| POST | `/api/users` | Cadastra um usuário |
| PUT | `/api/users/{id}/login` | Endpoint reservado; autenticação ainda não implementada (retorna 501) |

### Habilidades

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/skills` | Lista as habilidades cadastradas |

## Persistência e padrões utilizados

- Entity Framework Core para mapeamento objeto-relacional, consultas e persistência no SQL Server.
- MediatR organiza operações de aplicação por meio de comandos e consultas.
- Repositórios abstraem o acesso a projetos, usuários e habilidades.
- Entidades encapsulam operações do domínio, como iniciar, finalizar, atualizar e cancelar projetos.

## Como executar

### Pré-requisitos

- .NET SDK 8
- SQL Server (local ou remoto)
- Banco de dados `DevFreela` criado e acessível

### Configuração

Ajuste a connection string `DevFreelaCs` em `DevFreela/appsettings.json` para apontar para sua instância do SQL Server.

Não utilize credenciais reais em arquivos versionados. Para ambientes compartilhados, prefira configurar a connection string por variáveis de ambiente ou Secret Manager.

### Executar

Na raiz do repositório:

```bash
dotnet restore
dotnet run --project DevFreela/DevFreela.API.csproj
```

Em ambiente de desenvolvimento, a documentação Swagger é disponibilizada pela própria aplicação. Consulte a URL exibida no terminal após a inicialização.

## Observações sobre o escopo

- O endpoint de login não implementa autenticação, emissão de tokens ou validação de credenciais.
- O cadastro de usuário não implementa armazenamento de senha.
- O projeto utiliza SQL Server como banco configurado por padrão.
- O comportamento de cancelamento preserva o registro e altera seu estado de domínio.

---

Projeto de portfólio para demonstrar desenvolvimento backend com .NET, organização em camadas, MediatR, Entity Framework Core e modelagem de regras de negócio.
