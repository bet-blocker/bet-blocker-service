# Bet Blocker Api

**BetBlockerApi** é uma API pública e gratuita que fornece uma lista detalhada de hosts de plataformas de apostas e seus registros DNS. Ideal para integração em aplicativos, proxies, firewalls ou clientes DNS, permitindo bloquear automaticamente domínios de apostas.

**Repositório oficial de blocklist:** [blocklist.txt](https://github.com/bet-blocker/bet-blocker/blob/main/blocklist.txt)

**Coleção Postman:** [Acessar documentação](https://documenter.getpostman.com/view/15935769/2sB2qdgzjM)

---

## Índice

1. [Recursos da API](#recursos-da-api)
2. [Iniciando Resolução](#iniciando-resolução)
3. [Listando Domínios](#listando-domínios)
4. [Consultando Registros DNS](#consultando-registros-dns)
5. [Exemplos de `curl`](#exemplos-de-curl)
6. [Modelo de Resposta (DTO)](#modelo-de-resposta-dto)
7. [Contribuindo](#contribuindo)
8. [Licença](#licença)

---

## Recursos da API

| Método   | Endpoint                          | Descrição                                        |
|----------|-----------------------------------|--------------------------------------------------|
| POST     | `/v1/start`                       | Inicia um job diário de resolução DNS (00:00).   |
| GET      | `/v1/domains`                     | Retorna a lista de domínios do blocklist.        |
| GET      | `/v1/dns`                         | Retorna registros DNS da última resolução.       |
| GET      | `/v1/dns?date=DD-MM-YYYY`         | Retorna registros DNS de data específica.        |

---

## Iniciando Resolução

Dispara o processo de resolução DNS para todos os domínios do blocklist. Só pode ser executado uma vez por dia.

```bash
curl -X POST "https://api.bet-blocker.com/v1/start"
```

**Resposta:** `202 Accepted`

---

## Listando Domínios

Retorna a lista completa de domínios cadastrados no blocklist.

```bash
curl "https://api.bet-blocker.com/v1/domains"
```

**Resposta (200 OK)**

```json
[
  "example-bet1.com",
  "example-bet2.net",
  ...
]
```

---

## Consultando Registros DNS

### Resolução Atual

Retorna os registros DNS gerados na última execução.

```bash
curl "https://api.bet-blocker.com/v1/dns"
```

### Resolução por Data

Retorna registros DNS de uma data específica (formato `DD-MM-YYYY`).

```bash
curl "https://api.bet-blocker.com/v1/dns?date=05-12-2024"
```

**Parâmetro de consulta**:
- `date` (string, obrigatório) — data no formato `DD-MM-YYYY`.

---

## Exemplos de `curl`

- Disparar nova resolução:  
  ```bash
  curl -X POST "https://api.bet-blocker.com/v1/start"
  ```
- Listar domínios:  
  ```bash
  curl "https://api.bet-blocker.com/v1/domains"
  ```
- Obter resolução atual:  
  ```bash
  curl "https://api.bet-blocker.com/v1/dns"
  ```
- Obter resolução de 12 de maio de 2024:  
  ```bash
  curl "https://api.bet-blocker.com/v1/dns?date=12-05-2024"
  ```

---

## Modelo de Resposta (DTO)

```csharp
public class ResponseHostsDTO
{
    public string? Name { get; set; }
    public string? Host { get; set; }
    public Dns? DNS { get; set; }
    public Protocols Protocols { get; set; }
    public Ips Ips { get; set; }
}

public class Ips
{
    public string? Ip { get; set; }
    public DateTime ResolvedAt { get; set; }
}

public class Protocols
{
    public bool Http { get; set; }
    public bool Https { get; set; }
}

public class Dns
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Host { get; set; }
    public string? ReverseDns { get; set; }
    public string? Ttl { get; set; }
    public DateTime ResolvedAt { get; set; }
}
```

---

## Contribuindo

1. Faça um fork do repositório.  
2. Crie uma branch: `feature/nova-resolucao`.  
3. Abra um pull request descrevendo suas mudanças.

---

## Licença

Distribuído sob a [MIT License](https://opensource.org/licenses/MIT).

---

## Sobre o Criador

A **BetBlockerApi** foi idealizada e desenvolvida por **Bruno Hashimoto**. 
