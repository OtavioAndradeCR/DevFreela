# DevFreela – API de Gestão de Projetos Freelance

## 📌 Visão Geral

O **DevFreela** é uma **API REST desenvolvida em C# com .NET**, voltada para o gerenciamento de projetos entre **clientes e freelancers**. O sistema permite o cadastro, consulta e atualização de projetos, seguindo boas práticas de desenvolvimento backend e organização em camadas.

Este projeto foi desenvolvido com foco em **portfólio profissional**, destacando arquitetura limpa, separação de responsabilidades e construção de APIs escaláveis com .NET.

---

## 🎯 Objetivo do Projeto

* Desenvolver uma API REST utilizando .NET
* Demonstrar organização em camadas (Application, Core e Infrastructure)
* Aplicar boas práticas de arquitetura e código limpo
* Servir como projeto demonstrativo para recrutadores

---

## 🛠️ Tecnologias Utilizadas

* **C#**
* **.NET / ASP.NET Core**
* **Web API REST**
* **Entity Framework Core**
* **Arquitetura em Camadas**
* **Injeção de Dependências**

---

## 🧱 Arquitetura

O projeto segue uma **arquitetura em camadas**, organizada da seguinte forma:

* **DevFreela.Application** – Casos de uso e regras de aplicação
* **DevFreela.Core** – Entidades e regras de domínio
* **DevFreela.Infrastructure** – Persistência de dados e infraestrutura
* **DevFreela.API** – Camada de apresentação (API REST)

Essa estrutura facilita a manutenção, testes e evolução da aplicação.

---

## ▶️ Como Executar o Projeto

### Pré-requisitos

* .NET SDK 6.0 ou superior
* Visual Studio ou VS Code

### Passos

```bash
# Clonar o repositório
git clone https://github.com/OtavioAndradeCR/DevFreela.git

# Acessar a pasta do projeto
cd DevFreela

# Restaurar dependências
dotnet restore

# Executar a aplicação
dotnet run --project DevFreela.API
```

A aplicação estará disponível conforme a configuração padrão do ASP.NET Core.

---

## 📡 Endpoints Principais

```
GET    /api/projects
GET    /api/projects/{id}
POST   /api/projects
PUT    /api/projects/{id}
DELETE /api/projects/{id}
```

---

## 🧪 Exemplos de Uso (curl)

```bash
curl http://localhost:5000/api/projects
```

```bash
curl http://localhost:5000/api/projects/1
```

---

## 🧩 Diferenciais Técnicos

* Separação clara entre domínio, aplicação e infraestrutura
* Estrutura de projeto profissional e escalável
* Base sólida para implementação de novas funcionalidades
* Código organizado e de fácil entendimento

---

## 📈 Possíveis Evoluções

* Autenticação e autorização (JWT / Identity)
* Implementação de testes automatizados
* Paginação e filtros
* Documentação com Swagger / OpenAPI
* Deploy em nuvem (Azure ou AWS)

📌 *Projeto desenvolvido para demonstrar conhecimentos práticos em backend .NET, com foco em APIs REST e arquitetura em camadas, ideal para portfólio e processos seletivos.*
