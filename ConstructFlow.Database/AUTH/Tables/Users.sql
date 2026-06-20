CREATE TABLE [AUTH].[Users] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [FullName]     NVARCHAR (200) NOT NULL,
    [Email]        NVARCHAR (200) NOT NULL,
    [PasswordHash] NVARCHAR (500) NOT NULL,
    [Role]         NVARCHAR (50)  DEFAULT ('User') NOT NULL,
    [IsActive]     BIT            DEFAULT ((1)) NOT NULL,
    [CreatedAt]    DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]    DATETIME       NULL,
    [IsDeleted]    BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

