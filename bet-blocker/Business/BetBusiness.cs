using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using bet_blocker.Business.Interfaces;
using Infrastructure.Services.Interfaces;
using static bet_blocker.DTOs.ResponseHostDto;

namespace bet_blocker.Business
{
    public class BetBusiness : IBetBusiness
    {
        private readonly ICaller _caller;
        private readonly string _endpoint;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "DNSResolutionStatus";
        private static readonly object LockObject = new object();

        public BetBusiness(ICaller caller, IConfiguration configuration, IMemoryCache cache)
        {
            _caller = caller;
            _endpoint = configuration.GetSection("blockList").Value;
            _cache = cache;
        }

        public void StartResolutionProcess(CancellationToken cancellationToken)
        {
            lock (LockObject)
            {
                if (_cache.TryGetValue(CacheKey, out var status) && status.ToString() == "Processing")
                {
                    throw new InvalidOperationException("A resolução de DNS já está em andamento. Tente novamente mais tarde.");
                }

                _cache.Set(CacheKey, "Processing", TimeSpan.FromDays(1));
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    var resolvedHosts = await GetList(cancellationToken);
                    _cache.Set(CacheKey, resolvedHosts);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro durante a resolução de DNS: {ex.Message}");
                    _cache.Set(CacheKey, "Falha ao resolver DNS");
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
            if (_cache.TryGetValue(CacheKey, out var status))
            {
                return status;
            }

            return "Nenhum processo iniciado";
        }
    }
}
