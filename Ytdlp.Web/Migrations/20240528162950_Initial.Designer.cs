﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ytdlp.Web.Data;

#nullable disable

namespace Ytdlp.Web.Migrations
{
    [DbContext(typeof(YtdlpContext))]
    [Migration("20240528162950_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("Ytdlp.Web.Data.Content", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("AssetGuid")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("ChannelName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAdultContent")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Length")
                        .HasColumnType("REAL");

                    b.Property<string>("RequestedUri")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SourceMediaId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailAssetGuid")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("UploadDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Content");
                });
#pragma warning restore 612, 618
        }
    }
}
