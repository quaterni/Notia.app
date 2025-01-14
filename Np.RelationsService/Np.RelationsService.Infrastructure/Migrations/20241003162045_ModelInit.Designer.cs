﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Np.RelationsService.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Np.RelationsService.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241003162045_ModelInit")]
    partial class ModelInit
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Np.RelationsService.Domain.Notes.Note", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_notes");

                    b.ToTable("notes", (string)null);
                });

            modelBuilder.Entity("Np.RelationsService.Domain.Relations.Relation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("IncomingId")
                        .HasColumnType("uuid")
                        .HasColumnName("incoming_id");

                    b.Property<Guid>("OutgoingId")
                        .HasColumnType("uuid")
                        .HasColumnName("outgoing_id");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_relations");

                    b.HasIndex("IncomingId")
                        .HasDatabaseName("ix_relations_incoming_id");

                    b.HasIndex("OutgoingId")
                        .HasDatabaseName("ix_relations_outgoing_id");

                    b.ToTable("relations", (string)null);
                });

            modelBuilder.Entity("Np.RelationsService.Domain.RootEntries.RootEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_root_entries");

                    b.ToTable("root_entries", (string)null);
                });

            modelBuilder.Entity("Np.RelationsService.Domain.Relations.Relation", b =>
                {
                    b.HasOne("Np.RelationsService.Domain.Notes.Note", "Incoming")
                        .WithMany()
                        .HasForeignKey("IncomingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_relations_notes_incoming_id");

                    b.HasOne("Np.RelationsService.Domain.Notes.Note", "Outgoing")
                        .WithMany()
                        .HasForeignKey("OutgoingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_relations_notes_outgoing_id");

                    b.Navigation("Incoming");

                    b.Navigation("Outgoing");
                });

            modelBuilder.Entity("Np.RelationsService.Domain.RootEntries.RootEntry", b =>
                {
                    b.HasOne("Np.RelationsService.Domain.Notes.Note", "Note")
                        .WithOne()
                        .HasForeignKey("Np.RelationsService.Domain.RootEntries.RootEntry", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_root_entries_notes_id");

                    b.Navigation("Note");
                });
#pragma warning restore 612, 618
        }
    }
}
