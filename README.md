# BetBlockerApi

<p align="center">
  <img src="https://github.com/user-attachments/assets/435ebf83-7c8d-45cb-afe0-bc62a0e8aaec" alt="screen-home" width="1200">
</p>

**BetBlockerApi** é uma API que fornece uma lista detalhada de hosts de plataformas de apostas, facilitando a integração com aplicativos e outras plataformas. O objetivo desse serviço é centralizar e organizar informações de domínios relacionados a apostas, tornando o processo de análise, consulta e integração mais prático e eficiente para desenvolvedores e administradores de sistemas.

## Recursos da API

## Iniciando uma resolução

### [POST] Start

Para iniciar uma resolução utilize  

```bash
curl --location "https://api.bet-blocker.com/v1/start"
```

Feito isso o sistema irá iniciar um job para gerar as resoluções para cada domínio informado.

Importante mencionar que após iniciado não será possível interromper ou começar outro, será possível iniciar uma resolução por dia.

Após gerado acesse o endpoint [GET] DNS.

### [GET] DOMAINS
A API utiliza o método **GET** para listar os dados dos dominios registrados na base de dados

```bash
curl --location "https://api.bet-blocker.com/v1/domains"
```

### [GET] DNS
A API utiliza o método **GET** para listar os dados dos hosts, fornecendo informações detalhadas sobre cada domínio, incluindo seu nome, DNS, protocolos suportados, IPs associados e dados de regulamentação da Anatel.

Sendo a data do dia: exemplo 09-11-2024.json

## Exemplo curl

```bash
curl --location "https://api.bet-blocker.com/v1/dns"
```

A parametro `date` corresponde a data que desejar obter o registro
```txt
https://api.bet-blocker.com/v1/dns?date=05-12-2024
```
### Estrutura de retorno previsto (Callback)

Abaixo, o contrato retornada pela API:

```bash
{
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
        public bool Https { get; set; }
        public bool Http { get; set; }
    }

    public class Dns
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Host { get; set; }
        public string? ReverseDns { get; set; }
        public string? TTl { get; set; }
        public DateTime ResolvedAt { get; set; }
    }
}
```

# Essa API é pública e gratuita
