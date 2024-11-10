using System.Collections.Concurrent;
using bet_blocker.Business.Interfaces;
using Infrastructure.Services.Interfaces;
using static bet_blocker.DTOs.ResponseHostDto;
using System.Text.Json;

namespace bet_blocker.Business
{
    public class BetBusiness : IBetBusiness
    {
        private readonly ICaller _caller;
        private readonly string _endpoint;
        private readonly string _storagePath;
        private static readonly object LockObject = new object();

        public BetBusiness(ICaller caller, IConfiguration configuration)
        {
            _caller = caller;
            _endpoint = configuration.GetSection("blockList").Value;
            _storagePath = configuration.GetSection("StoragePath").Value;

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public void StartResolutionProcess(CancellationToken cancellationToken)
        {
            lock (LockObject)
            {
                var currentDate = DateTime.UtcNow.ToString("dd-MM-yyyy");
                var filePath = Path.Combine(_storagePath, $"{currentDate}.json");

                if (File.Exists(filePath))
                {
                    throw new InvalidOperationException("A resolução de DNS já foi gerada para hoje. Tente novamente amanhã.");
                }
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    var resolvedHosts = await GetList(cancellationToken);
                    var currentDate = DateTime.UtcNow.ToString("dd-MM-yyyy");

                    var filePath = Path.Combine(_storagePath, $"{currentDate}.json");

                    var json = JsonSerializer.Serialize(new
                    {
                        Date = currentDate,
                        ResolvedHosts = resolvedHosts
                    }, new JsonSerializerOptions { WriteIndented = true });

                    File.WriteAllText(filePath, json);

                    if (!Directory.Exists(_storagePath))
                    {
                        Directory.CreateDirectory(_storagePath);
                    }

                    File.WriteAllText(filePath, json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro durante a resolução de DNS: {ex.Message}");
                }
            }, cancellationToken);
        }

        public async Task<List<ResponseHostsDTO>> GetList(CancellationToken cancellationToken)
        {
            var response = await _caller.Call(_endpoint, HttpMethod.Get);

            var blockList = response
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(domain => domain.Trim())
                .ToList();

            var resolvedHosts = new ConcurrentBag<ResponseHostsDTO>();

            var tasks = blockList.Select(async domain =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                var hostDto = await ResolveDomainInfo(domain);
                resolvedHosts.Add(hostDto);
            });

            await Task.WhenAll(tasks);

            return resolvedHosts.ToList();
        }

        private static async Task<ResponseHostsDTO> ResolveDomainInfo(string domain)
        {
            var responseHost = new ResponseHostsDTO
            {
                Name = domain,
                Host = domain,
                Protocols = new Protocols { Http = true, Https = true },
                Anatel = new Anatel
                {
                    AnatelInfo = new AnatelInfo
                    {
                        Date = DateTime.UtcNow.ToShortDateString(),
                        Hour = DateTime.UtcNow.ToShortTimeString(),
                        Mime = "application/json"
                    },
                    CheckedAt = DateTime.UtcNow,
                    InsertAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            try
            {
                var hostEntry = await System.Net.Dns.GetHostEntryAsync(domain);
                var ipAddress = hostEntry.AddressList.FirstOrDefault()?.ToString();

                responseHost.Ips = new Ips
                {
                    Ip = ipAddress,
                    ResolvedAt = DateTime.UtcNow
                };

                responseHost.DNS = new Dns
                {
                    Type = hostEntry.AddressList.FirstOrDefault()?.AddressFamily.ToString(),
                    Name = hostEntry.HostName,
                    Host = domain,
                    CanonicalName = hostEntry.HostName,
                    ReverseDns = ipAddress,
                    TTl = "3600",
                    ResolvedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao resolver {domain}: {ex.Message}");
            }

            return responseHost;
        }

        public object GetResolutionStatus()
        {
            var currentDate = DateTime.UtcNow.ToString("dd-MM-yyyy");
            var filePath = Path.Combine(_storagePath, $"{currentDate}.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var result = JsonSerializer.Deserialize<object>(json); 
                return result;
            }

            return "Nenhum processo iniciado ou resolução não encontrada para hoje.";
        }
    }
}