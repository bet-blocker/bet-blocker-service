namespace Api.DTOs
{
    public class ResponseHostsDTO
    {
        public string? Name { get; set; }
        public string? Host { get; set; }
        public DNS? DNS { get; set; }
        public Protocols Protocols { get; set; }
        public Ips Ips { get; set; }
        public Anatel Anatel{ get; set; }
    }
    
    public class Anatel
    {
        public AnatelDataSet AnatelInfo { get; set; }
        public DateTime CheckedAt { get; set; }
        public DateTime InsertAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class AnatelDataSet
    {
        public string? UrlFull { get; set; }
        public string? Url { get; set; }
        public string? File { get; set; }
        public string? Date { get; set; }
        public string? Hour { get; set; }
        public string? Mime { get; set; }
    }

    public class Ips
    {
        public string? Ip { get; set; }
        public DateTime ResolvedAt { get; set; }
    }

    public class Protocols { 
        public bool Https {  get; set; }
        public bool Http { get; set; }
    }
    
    public class DNS
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? Host { get; set; }
        public string? ReverseDns { get; set; }
        public string? CanonicalName { get; set; }
        public string? TTl { get; set; }
        public DateTime ResolvedAt { get; set; }
    }

}
