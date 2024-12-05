using System.Net;   
using System.Text.Json;
using Application.Business.Interfaces;
using Application.Services.Interfaces;
using static Application.DTOs.ResponseHostDto;
using DNSDto = Application.DTOs.ResponseHostDto.Dns;
using System.Net.Sockets;

namespace Application.Business
{
    public class BetBusiness : IBetBusiness
    {
        private readonly ICaller _caller;
        private readonly string _blockList;
        private double _progress;
        private bool _isResolutionInProgress;

        public BetBusiness(ICaller caller, IConfiguration configuration)
        {
            _caller = caller;
            _blockList = configuration.GetSection("blockList").Value;
            _progress = 0;
            _isResolutionInProgress = false;
        }

        public double GetStatus()
        {
            return _progress; 
        }

        public bool IsResolutionInProgress => _isResolutionInProgress;

        public async Task StartResolutionProcess(CancellationToken cancellationToken)
        {
            if (_isResolutionInProgress)
            {
                throw new Exception($"Uma resolução já está em andamento. Progresso atual: {_progress:F2}%");
            }

            _isResolutionInProgress = true;

            try
            {
                var blocklist = await GetBlocklistGithub();
                var totalDomains = blocklist.Count;
                var date = DateTime.UtcNow.ToString("dd-MM-yyyy");
                var filePath = $"Json/{date}.json";

                if (!File.Exists(filePath))
                {
                    var initialData = new List<ResponseHostsDTO>(); 
                    var initialJson = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync(filePath, initialJson, cancellationToken);
                }

                int processedCount = 0;

                foreach (var item in blocklist)
                {
                    try
                    {
                        var responseHost = await ResolveDnsForHost(item);
                        if (responseHost != null)
                        {
                            var existingData = JsonSerializer.Deserialize<List<ResponseHostsDTO>>(await File.ReadAllTextAsync(filePath));
                            existingData.Add(responseHost);
                            var updatedJson = JsonSerializer.Serialize(existingData, new JsonSerializerOptions { WriteIndented = true });

                            await File.WriteAllTextAsync(filePath, updatedJson, cancellationToken);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"Erro ao resolver DNS reverso para o IP {item}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro inesperado ao resolver DNS para o IP {item}: {ex.Message}");
                    }

                    processedCount++;
                    _progress = (double)processedCount / totalDomains * 100;
                    Console.WriteLine($"Progresso: {_progress:F2}% concluído.");

                    cancellationToken.ThrowIfCancellationRequested();
                }

                Console.WriteLine($"Arquivo {filePath} atualizado com sucesso.");
            }
            finally
            {
                _isResolutionInProgress = false;
            }
        }

        public async Task<List<string>> GetBlocklistGithub()
        {
            var response = await _caller.Call(_blockList, HttpMethod.Get);

            return response
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(domain => domain.Trim())
                .ToList();
        }

        private static async Task<ResponseHostsDTO> ResolveDnsForHost(string domain)
        {
            var responseHost = new ResponseHostsDTO
            {
                Name = domain,
                Host = domain,
                DNS = new DNSDto
                {
                    Name = domain,
                    ResolvedAt = DateTime.UtcNow
                },
                Protocols = new Protocols
                {
                    Https = false,
                    Http = false
                },
                Ips = new Ips
                {
                    ResolvedAt = DateTime.UtcNow
                }
            };

            try
            {
                var uri = new Uri(domain.StartsWith("http") ? domain : $"http://{domain}");
                var cleanDomain = uri.Host;

                var isIp = IsValidIp(cleanDomain);
                IPAddress ip = null;

                if (!isIp)
                {
                    var hostEntry = await System.Net.Dns.GetHostEntryAsync(domain);
                    ip = hostEntry.AddressList.FirstOrDefault();
                }
                else
                {
                    ip = IPAddress.Parse(cleanDomain);
                }

                if (ip != null)
                {
                    responseHost.Ips.Ip = ip.ToString();
                }
                else
                {
                    responseHost.Ips.Ip = "0.0.0.0";
                }

                if (ip != null && ip.ToString() != "0.0.0.0")
                {
                    responseHost.DNS.ReverseDns = System.Net.Dns.GetHostEntryAsync(ip).Result.HostName;
                }
                else
                {
                    responseHost.DNS.ReverseDns = "N/A";
                }

                responseHost.DNS.Type = "A";
                responseHost.DNS.Host = isIp ? ip.ToString() : cleanDomain;

                if (string.IsNullOrEmpty(responseHost.Ips.Ip) || string.IsNullOrEmpty(responseHost.DNS.Host))
                {
                    Console.WriteLine($"Informações incompletas para o domínio: {domain}");
                    responseHost.Ips.Ip = "N/A";
                    responseHost.DNS.Host = "N/A";
                }

                return responseHost;
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Erro ao resolver DNS para {domain}: {ex.Message}");
                responseHost.Ips.Ip = "N/A";
                responseHost.DNS.Host = "N/A";
                responseHost.DNS.ReverseDns = "N/A";
                return responseHost; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro desconhecido ao resolver DNS para {domain}: {ex.Message}");
                responseHost.Ips.Ip = "N/A";
                responseHost.DNS.Host = "N/A";
                responseHost.DNS.ReverseDns = "N/A";
                return responseHost; 
            }
        }

        private static bool IsValidIp(string ipString)
        {
            return IPAddress.TryParse(ipString, out _);
        }
    }
}