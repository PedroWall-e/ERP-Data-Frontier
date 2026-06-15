# DataFrontier ERP

ERP moderno focado no mercado brasileiro, com arquitetura **Multi-Tenant Híbrida** e preparado para a **Reforma Tributária** (IBS/CBS, 2026-2033).

## Stack Tecnológico

| Camada | Tecnologia |
|--------|------------|
| **Back-end** | C# / ASP.NET Core 8 Web API |
| **Banco de Dados** | PostgreSQL + Entity Framework Core |
| **Front-end** | React + TypeScript + Vite + MUI _(futuro)_ |
| **Fiscal** | ACBrLib (NF-e, NFC-e, NFS-e) _(futuro)_ |
| **Financeiro** | API Banco Inter (PIX, Webhooks) _(futuro)_ |

## Arquitetura

```
Clean Architecture (4 camadas)
├── Domain          → Entidades, interfaces, regras de negócio
├── Application     → Casos de uso, DTOs, validações
├── Infrastructure  → EF Core, serviços externos, multi-tenancy
└── API             → Controllers, middleware, pipeline HTTP
```

### Multi-Tenancy Híbrida

- **SaaS:** Banco compartilhado com isolamento por Global Query Filters (`WHERE TenantId = @tid`)
- **Enterprise:** Banco dedicado com troca dinâmica de Connection String

### Abstração Tributária

Sem colunas fixas para tributos. Tabela relacional `impostos_do_item` com `TipoImposto` dinâmico (ICMS, ISS, IBS, CBS, IS), permitindo coexistência de regimes durante a transição.

## Pré-requisitos

- .NET 8 SDK
- PostgreSQL 15+
- (Opcional) Docker

## Quick Start

```bash
# Restaurar dependências
dotnet restore

# Executar em modo desenvolvimento
cd src/DataFrontier.API
dotnet run

# Acessar Swagger
# http://localhost:5000
```

## Estrutura do JWT

O token JWT deve conter as seguintes claims:

```json
{
  "sub": "user-id",
  "tenant_id": "guid-do-tenant",
  "tenant_tier": "SaaS",
  "...": "outras claims"
}
```

## Licença

Proprietary — © DataFrontier
