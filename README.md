# BetBlockerApi

<p align="center">
  <img src="https://github.com/user-attachments/assets/435ebf83-7c8d-45cb-afe0-bc62a0e8aaec" alt="screen-home" width="1200">
</p>

**BetBlockerApi** é uma API que fornece uma lista detalhada de hosts de plataformas de apostas, facilitando a integração com aplicativos e outras plataformas. O objetivo desse serviço é centralizar e organizar informações de domínios relacionados a apostas, tornando o processo de análise, consulta e integração mais prático e eficiente para desenvolvedores e administradores de sistemas.

## Recursos da API

A API utiliza o método **GET** para listar os dados dos hosts, fornecendo informações detalhadas sobre cada domínio, incluindo seu nome, DNS, protocolos suportados, IPs associados e dados de regulamentação da Anatel.

## Endpoint da API

Sendo a data do dia: exemplo 09-11-2024.json

## Exemplo curl

```bash
curl --location "https://bet-blocker.com/api/v1/dns"
```

A parametro `date` corresponde a data que desejar obter o registro
```txt
https://bet-blocker.com/api/v1/dns?date=11-11-2024
```
### Estrutura de retorno previsto (Callback)

Abaixo, a estrutura de dados retornada pela API:

```json
{
"Date": "09-11-2024",
"ResolvedHosts": [
    {
        "Name": "a5sbet.com",
        "Host": "a5sbet.com",
        "DNS": {
            "Type": "InterNetwork",
            "Name": "a5sbet.com",
            "Host": "a5sbet.com",
            "ReverseDns": "104.21.42.168",
            "CanonicalName": "a5sbet.com",
            "TTl": "3600",
            "ResolvedAt": "2024-11-09T17:48:37.149205Z"
        },
        "Protocols": {
            "Https": true,
            "Http": true
        },
        "Ips": {
            "Ip": "104.21.42.168",
            "ResolvedAt": "2024-11-09T17:48:37.149204Z"
        },
        "Anatel": {
            "AnatelInfo": {
                "UrlFull": null,
                "Url": null,
                "File": null,
                "Date": "11/9/2024",
                "Hour": "5:48PM",
                "Mime": "application/json"
            },
            "CheckedAt": "2024-11-09T17:48:13.770764Z",
            "InsertAt": "2024-11-09T17:48:13.770765Z",
            "UpdatedAt": "2024-11-09T17:48:13.770765Z"
        }
    },
    .......
    ]
}
```

# Essa API é pública e gratuita
