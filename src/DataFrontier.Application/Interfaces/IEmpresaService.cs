using DataFrontier.Application.DTOs.Empresa;

namespace DataFrontier.Application.Interfaces;

public interface IEmpresaService
{
    Task<ConfiguracaoEmpresaResponse> GetAsync();
    Task<ConfiguracaoEmpresaResponse> SaveAsync(ConfiguracaoEmpresaRequest request);
    Task<ConfiguracaoEmpresaResponse> UploadCertificadoAsync(Stream arquivoStream, string fileName, string senha);
    Task RemoverCertificadoAsync();
}
