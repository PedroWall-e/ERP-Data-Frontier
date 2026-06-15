using DataFrontier.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataFrontier.Infrastructure.Services;

public class XmlStorageService : IXmlStorageService
{
    private readonly string _basePath;
    private readonly ILogger<XmlStorageService> _logger;

    public XmlStorageService(IConfiguration configuration, ILogger<XmlStorageService> logger)
    {
        _basePath = configuration["XmlStorage:BasePath"] ?? Path.Combine(AppContext.BaseDirectory, "XML");
        _logger = logger;
    }

    public async Task<string> SalvarAsync(Guid tenantId, string chaveAcesso, string tipo, string xmlContent)
    {
        var now = DateTime.UtcNow;
        var dir = Path.Combine(_basePath, tenantId.ToString(), now.Year.ToString(), now.Month.ToString("D2"));
        Directory.CreateDirectory(dir);

        var fileName = $"{chaveAcesso}-{tipo}.xml";
        var fullPath = Path.Combine(dir, fileName);

        await File.WriteAllTextAsync(fullPath, xmlContent);
        _logger.LogInformation("XML {Tipo} salvo: {Path}", tipo, fullPath);

        return fullPath;
    }

    public async Task<string?> ObterAsync(Guid tenantId, string chaveAcesso, string tipo)
    {
        var pattern = $"{chaveAcesso}-{tipo}.xml";
        var tenantDir = Path.Combine(_basePath, tenantId.ToString());

        if (!Directory.Exists(tenantDir)) return null;

        var files = Directory.GetFiles(tenantDir, pattern, SearchOption.AllDirectories);
        if (files.Length == 0) return null;

        return await File.ReadAllTextAsync(files[0]);
    }

    public async Task<byte[]?> ObterBytesAsync(Guid tenantId, string chaveAcesso, string tipo)
    {
        var xml = await ObterAsync(tenantId, chaveAcesso, tipo);
        return xml is null ? null : System.Text.Encoding.UTF8.GetBytes(xml);
    }
}
