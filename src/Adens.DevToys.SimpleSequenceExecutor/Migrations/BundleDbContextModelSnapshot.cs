﻿// <auto-generated />
using System;
using Adens.DevToys.SimpleSequenceExecutor.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Adens.DevToys.SimpleSequenceExecutor.Migrations
{
    [DbContext(typeof(BundleDbContext))]
    partial class BundleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.Bundle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Bundles");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStep", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BundleId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BundleId");

                    b.ToTable("BundleSteps");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStepParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("StepId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.ToTable("BundleStepParameters");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStep", b =>
                {
                    b.HasOne("Adens.DevToys.SimpleSequenceExecutor.Entities.Bundle", "Bundle")
                        .WithMany("Steps")
                        .HasForeignKey("BundleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bundle");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStepParameter", b =>
                {
                    b.HasOne("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStep", "Step")
                        .WithMany("Parameters")
                        .HasForeignKey("StepId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Step");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.Bundle", b =>
                {
                    b.Navigation("Steps");
                });

            modelBuilder.Entity("Adens.DevToys.SimpleSequenceExecutor.Entities.BundleStep", b =>
                {
                    b.Navigation("Parameters");
                });
#pragma warning restore 612, 618
        }
    }
}
