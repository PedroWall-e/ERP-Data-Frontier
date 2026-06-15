using DataFrontier.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataFrontier.Infrastructure.Persistence.Configurations;

public class ConfiguracaoEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracaoEmpresa>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoEmpresa> builder)
    {
        builder.ToTable("configuracoes_empresa");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.TenantId).HasColumnName("tenant_id");
        builder.Property(c => c.CriadoEm).HasColumnName("criado_em");
        builder.Property(c => c.AtualizadoEm).HasColumnName("atualizado_em");

        // Identificação
        builder.Property(c => c.RazaoSocial).HasMaxLength(300).HasColumnName("razao_social");
        builder.Property(c => c.NomeFantasia).HasMaxLength(300).HasColumnName("nome_fantasia");
        builder.Property(c => c.Cnpj).HasMaxLength(18).HasColumnName("cnpj");
        builder.Property(c => c.InscricaoEstadual).HasMaxLength(20).HasColumnName("inscricao_estadual");
        builder.Property(c => c.InscricaoMunicipal).HasMaxLength(20).HasColumnName("inscricao_municipal");
        builder.Property(c => c.CRT).HasColumnName("crt");

        // Endereço
        builder.Property(c => c.Logradouro).HasMaxLength(300).HasColumnName("logradouro");
        builder.Property(c => c.Numero).HasMaxLength(20).HasColumnName("numero");
        builder.Property(c => c.Complemento).HasMaxLength(100).HasColumnName("complemento");
        builder.Property(c => c.Bairro).HasMaxLength(100).HasColumnName("bairro");
        builder.Property(c => c.CodigoIbge).HasMaxLength(7).HasColumnName("codigo_ibge");
        builder.Property(c => c.Municipio).HasMaxLength(100).HasColumnName("municipio");
        builder.Property(c => c.Uf).HasMaxLength(2).HasColumnName("uf");
        builder.Property(c => c.Cep).HasMaxLength(9).HasColumnName("cep");
        builder.Property(c => c.Telefone).HasMaxLength(20).HasColumnName("telefone");
        builder.Property(c => c.Email).HasMaxLength(200).HasColumnName("email");

        // Certificado Digital
        builder.Property(c => c.CertificadoPfx).HasColumnName("certificado_pfx");
        builder.Property(c => c.CertificadoSenha).HasMaxLength(500).HasColumnName("certificado_senha");
        builder.Property(c => c.CertificadoValidade).HasColumnName("certificado_validade");
        builder.Property(c => c.CertificadoNome).HasMaxLength(200).HasColumnName("certificado_nome");

        // Fiscal
        builder.Property(c => c.AmbienteFiscal).HasColumnName("ambiente_fiscal");
        builder.Property(c => c.SerieNfe).HasColumnName("serie_nfe");
        builder.Property(c => c.SerieNfse).HasColumnName("serie_nfse");

        // Banco Inter
        builder.Property(c => c.InterClientId).HasMaxLength(200).HasColumnName("inter_client_id");
        builder.Property(c => c.InterClientSecret).HasMaxLength(500).HasColumnName("inter_client_secret");
        builder.Property(c => c.InterCertificadoPfx).HasColumnName("inter_certificado_pfx");
        builder.Property(c => c.InterCertificadoSenha).HasMaxLength(500).HasColumnName("inter_certificado_senha");

        // 1 registro por tenant
        builder.HasIndex(c => c.TenantId).IsUnique().HasDatabaseName("ix_configuracoes_empresa_tenant");
    }
}
