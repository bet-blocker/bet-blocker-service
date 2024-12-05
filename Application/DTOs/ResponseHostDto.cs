namespace Application.DTOs
{
    public class ResponseHostDto
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
}

