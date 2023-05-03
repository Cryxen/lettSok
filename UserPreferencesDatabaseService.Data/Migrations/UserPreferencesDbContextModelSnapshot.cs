﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserPreferencesDatabaseService.Data;

#nullable disable

namespace UserPreferencesDatabaseService.Data.Migrations
{
    [DbContext(typeof(UserPreferencesDbContext))]
    partial class UserPreferencesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.InterestedAdvertisement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AdvertisementUuid")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("interestedAdvertisements");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Municipality")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("locations");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.LoggedOnUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("LoggedOnUser", (string)null);
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.SearchLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.HasIndex("UserId");

                    b.ToTable("searchLocations");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.UninterestedAdvertisement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AdvertisementUuid")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("uninterestedAdvertisements");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.InterestedAdvertisement", b =>
                {
                    b.HasOne("UserPreferencesDatabaseService.Data.User", "User")
                        .WithMany("interestedAdvertisements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.LoggedOnUser", b =>
                {
                    b.HasOne("UserPreferencesDatabaseService.Data.User", "user")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.SearchLocation", b =>
                {
                    b.HasOne("UserPreferencesDatabaseService.Data.Location", "location")
                        .WithMany("searchLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserPreferencesDatabaseService.Data.User", "user")
                        .WithMany("searchLocations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("location");

                    b.Navigation("user");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.UninterestedAdvertisement", b =>
                {
                    b.HasOne("UserPreferencesDatabaseService.Data.User", "User")
                        .WithMany("UninterestedAdvertisements")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.Location", b =>
                {
                    b.Navigation("searchLocations");
                });

            modelBuilder.Entity("UserPreferencesDatabaseService.Data.User", b =>
                {
                    b.Navigation("UninterestedAdvertisements");

                    b.Navigation("interestedAdvertisements");

                    b.Navigation("searchLocations");
                });
#pragma warning restore 612, 618
        }
    }
}
