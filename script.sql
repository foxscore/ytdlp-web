CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Content" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Content" PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Length" REAL NULL,
    "Size" INTEGER NOT NULL,
    "Type" INTEGER NOT NULL,
    "UploadDate" TEXT NULL,
    "ChannelName" TEXT NOT NULL,
    "Source" TEXT NOT NULL,
    "SourceMediaId" TEXT NOT NULL,
    "RequestedUri" TEXT NOT NULL,
    "IsAdultContent" INTEGER NOT NULL,
    "AssetGuid" TEXT NOT NULL,
    "ThumbnailAssetGuid" TEXT NULL
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240528162950_Initial', '8.0.5');

COMMIT;

BEGIN TRANSACTION;

ALTER TABLE "Content" ADD "DownloadDate" TEXT NOT NULL DEFAULT '0001-01-01 00:00:00';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240528191533_Added Download Date', '8.0.5');

COMMIT;

