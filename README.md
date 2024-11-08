# BetBlockerApi

<p align="center">
  <img src="https://github.com/user-attachments/assets/435ebf83-7c8d-45cb-afe0-bc62a0e8aaec" alt="screen-home" width="1200">
</p>

**BetBlockerApi** é uma API que fornece uma lista detalhada de hosts de plataformas de apostas, facilitando a integração com aplicativos e outras plataformas. O objetivo desse serviço é centralizar e organizar informações de domínios relacionados a apostas, tornando o processo de análise, consulta e integração mais prático e eficiente para desenvolvedores e administradores de sistemas.

## Recursos da API

A API utiliza o método **GET** para listar os dados dos hosts, fornecendo informações detalhadas sobre cada domínio, incluindo seu nome, DNS, protocolos suportados, IPs associados e dados de regulamentação da Anatel.

### Estrutura de Retorno (Callback)

Abaixo, a estrutura de dados retornada pela API:

```json
[
    {
        "name": "001game9",
        "host": "001game9.com",
        "dns": {
            "type": "CNAME",
            "name": "001game9",
            "reverse_dns": "",
            "canonical_name": "",
            "ttl": "",
            "resolved_at": ""
        },
        "protocols": {
            "https": true,
            "http": false
        },
        "ips": {
            "ip": "127.0.0.1",
            "resolved": ""
        },
        "anatel": [
            {
                "urlFull": "https://www.gov.br/anatel/pt-br/regulado/fiscalizacao/planilha_operacao_url20241011_09_10-1.pdf",
                "url": "https://www.gov.br/anatel/pt-br/regulado/fiscalizacao",
                "file": "planilha_operacao_url20241011_09_10-1.pdf",
                "date": "20241011",
                "hour": "09_10-1",
                "mime": "pdf"
            }
        ],
        "checked_at": "2024-11-08 00:00:00",
        "insert_at": "2024-11-08 00:00:00",
        "updated_at": "2024-11-08 00:00:00"
    },
    ...
]
