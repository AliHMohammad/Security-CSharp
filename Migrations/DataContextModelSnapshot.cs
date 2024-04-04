﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Security_CSharp.Data;

#nullable disable

namespace Security_CSharp.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<string>("user_username")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("role_name")
                        .HasColumnType("varchar(255)");

                    b.HasKey("user_username", "role_name");

                    b.HasIndex("role_name");

                    b.ToTable("RoleUser");

                    b.HasData(
                        new
                        {
                            user_username = "Admin",
                            role_name = "ADMIN"
                        });
                });

            modelBuilder.Entity("Security_CSharp.Security.Entitites.Role", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("name");

                    b.HasKey("Name");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Name = "ADMIN"
                        },
                        new
                        {
                            Name = "USER"
                        });
                });

            modelBuilder.Entity("Security_CSharp.Security.Entitites.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("username");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longblob")
                        .HasColumnName("password_hash");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("longblob")
                        .HasColumnName("password_salt");

                    b.HasKey("Username");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Username = "Admin",
                            CreatedAt = new DateTime(2024, 4, 4, 20, 12, 4, 154, DateTimeKind.Local).AddTicks(8898),
                            Email = "admin@kea.dk",
                            PasswordHash = new byte[] { 141, 72, 190, 234, 166, 32, 160, 162, 174, 119, 49, 201, 81, 197, 20, 63, 102, 205, 196, 125, 172, 65, 169, 211, 39, 37, 36, 178, 172, 73, 197, 58 },
                            PasswordSalt = new byte[] { 34, 74, 239, 73, 39, 105, 55, 216, 133, 152, 158, 211, 45, 102, 13, 232, 39, 218, 143, 94, 193, 131, 13, 175, 98, 172, 33, 130, 209, 43, 155, 114, 20, 68, 183, 253, 48, 121, 98, 47, 120, 38, 159, 188, 251, 98, 195, 73, 180, 150, 94, 135, 136, 23, 26, 45, 168, 75, 52, 138, 174, 142, 188, 5 }
                        });
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("Security_CSharp.Security.Entitites.Role", null)
                        .WithMany()
                        .HasForeignKey("role_name")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Security_CSharp.Security.Entitites.User", null)
                        .WithMany()
                        .HasForeignKey("user_username")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
