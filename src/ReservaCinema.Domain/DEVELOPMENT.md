# Domain — Guia de Desenvolvimento

## Responsabilidade da Camada

O Domain é o núcleo do sistema. Contém as entidades e as regras de negócio que existem independentemente de qualquer tecnologia — sem banco de dados, sem HTTP, sem Redis.

## Dependências Permitidas

**Nenhuma.** O projeto Domain não referencia nenhum outro projeto da solução e nenhum pacote NuGet de infraestrutura.

Se você sentiu necessidade de importar algo de `Application`, `Infrastructure` ou de uma biblioteca externa, o conceito está no lugar errado.

## Entidades

### Regras de Criação

- Entidades são criadas com propriedades que refletem o estado real do domínio
- Valores padrão de negócio (ex: `AvailableSeats = TotalSeats` ao criar uma sessão) são definidos na própria entidade, nunca no serviço
- Propriedades que representam dados estruturados armazenados como texto (ex: lista de assentos) expõem métodos de leitura e escrita (`GetSeats()`, `SetSeats()`) — o sufixo do campo reflete o formato real (`SeatsJson`)

### Entidades Atuais

#### `Session`
Representa uma sessão de cinema (filme + sala + horário).

Propriedades relevantes:
- `AvailableSeats` — decrementado a cada reserva confirmada; nunca vai abaixo de zero
- `IsActive` — soft delete; sessões inativas não aparecem em consultas
- `TicketPrice` — preço por assento, usado pelo `ReservationService` para calcular o total

#### `Reservation`
Representa uma reserva de assentos por um usuário em uma sessão.

Propriedades relevantes:
- `SeatsJson` — lista de assentos serializada; acesse via `GetSeats()` / `SetSeats()`
- `Status` — estado atual da reserva (ex: pendente, confirmada, cancelada)
- `ExpiresAt` — prazo para confirmação da reserva
- `TotalAmount` — calculado no momento da criação com base no preço e quantidade de assentos

## Exceções de Domínio

Exceções que representam violações de regras de negócio ficam em `Exceptions/`.

| Exceção | Quando usar |
|---|---|
| `ConflictException` | Tentativa de reservar assentos já ocupados ou estado inválido |

Exceções de domínio não carregam detalhes de infraestrutura (stack traces de banco, mensagens de Redis). Elas comunicam **o que violou a regra**, não como o sistema detectou.

## O Que Pertence ao Domain

| Pertence | Não Pertence |
|---|---|
| Entidades com estado e comportamento | Interfaces de repositório (ficam em Application) |
| Exceções de regra de negócio | Validação de input HTTP (fica em Application) |
| Cálculos baseados em dados da entidade | Acesso a banco ou cache |
| Invariantes (ex: assentos disponíveis ≥ 0) | DTOs e contratos de API |

## Adicionando uma Nova Entidade

1. Criar o arquivo em `Entities/{NomeEntidade}.cs`
2. Definir propriedades com os dados de negócio necessários
3. Incluir `CreatedAt` e `UpdatedAt` para rastreabilidade
4. Se houver soft delete, adicionar `IsActive`
5. Escrever os testes em `ReservaCinema.Domain.Tests/Entities/{NomeEntidade}Tests.cs` **antes** de implementar

## Adicionando uma Nova Exceção de Domínio

1. Criar em `Exceptions/{NomeExcecao}.cs`
2. Herdar de `Exception` ou de uma base de domínio existente
3. Garantir que a mensagem descreve a violação de negócio, não o erro técnico
