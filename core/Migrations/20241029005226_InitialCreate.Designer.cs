﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PermissionManagerCore;

#nullable disable

namespace PermissionManagerCore.Migrations
{
    [DbContext(typeof(PermissionContext))]
    [Migration("20241029005226_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.35");

            modelBuilder.Entity("PManager.PermGroup", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PermMask")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("BLOB");

                    b.HasKey("Name");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("PManager.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("PManager.PermUser", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PermMask")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("BLOB");

                    b.HasKey("Name");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("User_Groups", b =>
                {
                    b.Property<string>("User_Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Group_Name")
                        .HasColumnType("TEXT");

                    b.HasKey("User_Name", "Group_Name");

                    b.HasIndex("Group_Name");

                    b.ToTable("User_Groups");
                });

            modelBuilder.Entity("User_Groups", b =>
                {
                    b.HasOne("PManager.PermGroup", null)
                        .WithMany()
                        .HasForeignKey("Group_Name")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PManager.PermUser", null)
                        .WithMany()
                        .HasForeignKey("User_Name")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
