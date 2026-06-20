CREATE TABLE [VND].[Vendors] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [ContactPerson] NVARCHAR (200) NOT NULL,
    [Email]         NVARCHAR (200) NOT NULL,
    [Phone]         NVARCHAR (20)  NOT NULL,
    [Address]       NVARCHAR (300) NULL,
    [IsActive]      BIT            DEFAULT ((1)) NOT NULL,
    [CreatedAt]     DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]     DATETIME       NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

