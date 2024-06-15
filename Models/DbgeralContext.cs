using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Models;

public partial class DbgeralContext : DbContext
{
    public DbgeralContext()
    {
    }

    public DbgeralContext(DbContextOptions<DbgeralContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consulta> Consultas { get; set; }

    public virtual DbSet<Endereco> Enderecos { get; set; }

    public virtual DbSet<Medico> Medicos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Telefone> Telefones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=EZT-NTB-636CNZ3\\sqlexpress;Database=DBGeral;User Id=sa;Password=sa123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Consulta__3214EC07F75809FE");

            entity.Property(e => e.Data).HasColumnType("datetime");
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.IdMedico)
                .HasConstraintName("FK__Consultas__IdMed__2F10007B");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK__Consultas__IdPac__2E1BDC42");
        });

        modelBuilder.Entity<Endereco>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Endereco__3214EC07498D3822");

            entity.ToTable("Endereco");

            entity.Property(e => e.Bairro).HasMaxLength(50);
            entity.Property(e => e.Cidade).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.Rua).HasMaxLength(100);

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Enderecos)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK__Endereco__IdPaci__29572725");
        });

        modelBuilder.Entity<Medico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Medico__3214EC071F913CE0");

            entity.ToTable("Medico");

            entity.Property(e => e.Crm)
                .HasMaxLength(20)
                .HasColumnName("CRM");
            entity.Property(e => e.Nome).HasMaxLength(100);
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Paciente__3214EC072ADA6C9E");

            entity.ToTable("Paciente");

            entity.Property(e => e.Altura).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Nome).HasMaxLength(100);
            entity.Property(e => e.Peso).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Telefone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Telefone__3214EC0706A0C76B");

            entity.ToTable("Telefone");

            entity.Property(e => e.Ddd)
                .HasMaxLength(3)
                .HasColumnName("DDD");
            entity.Property(e => e.Numero).HasMaxLength(10);

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Telefones)
                .HasForeignKey(d => d.IdPaciente)
                .HasConstraintName("FK__Telefone__IdPaci__267ABA7A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
