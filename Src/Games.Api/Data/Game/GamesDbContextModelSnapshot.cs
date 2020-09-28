﻿// <auto-generated />
using System;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Games.Api.Data.Game
{
    [DbContext(typeof(GamesDbContext))]
    partial class GamesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Games.Domain.Entities.FriendAggregate.Friend", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Ver")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("Friends");
                });

            modelBuilder.Entity("Games.Domain.Entities.GameAggregate.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LaunchYear")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Platform")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)")
                        .HasMaxLength(30);

                    b.Property<byte[]>("Ver")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Games.Domain.Entities.GameAggregate.Game", b =>
                {
                    b.OwnsOne("Games.Domain.Entities.GameAggregate.GameLoan", "Loan", b1 =>
                        {
                            b1.Property<Guid>("GameId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<Guid>("FriendId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("LoanDate")
                                .HasColumnType("datetime2");

                            b1.HasKey("GameId");

                            b1.HasIndex("FriendId");

                            b1.ToTable("Games");

                            b1.HasOne("Games.Domain.Entities.FriendAggregate.Friend", "Friend")
                                .WithMany()
                                .HasForeignKey("FriendId")
                                .OnDelete(DeleteBehavior.Cascade)
                                .IsRequired();

                            b1.WithOwner()
                                .HasForeignKey("GameId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}