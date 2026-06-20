CREATE TABLE [PRJ].[Projects] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (200)  NOT NULL,
    [Code]      NVARCHAR (50)   NOT NULL,
    [Location]  NVARCHAR (300)  NOT NULL,
    [Status]    INT             NOT NULL,
    [StartDate] DATETIME        NOT NULL,
    [EndDate]   DATETIME        NULL,
    [Budget]    DECIMAL (18, 2) NOT NULL,
    [CreatedAt] DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt] DATETIME        NULL,
    [IsDeleted] BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_Projects_Code]
    ON [PRJ].[Projects]([Code] ASC) WHERE ([IsDeleted]=(0));

