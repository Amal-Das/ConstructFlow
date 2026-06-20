CREATE TABLE [PR].[PurchaseRequests] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [ProjectId]     INT            NOT NULL,
    [RequestNumber] NVARCHAR (50)  NOT NULL,
    [Status]        INT            NOT NULL,
    [RequestedBy]   NVARCHAR (200) NOT NULL,
    [RequestDate]   DATETIME       NOT NULL,
    [Remarks]       NVARCHAR (500) NULL,
    [CreatedAt]     DATETIME       DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]     DATETIME       NULL,
    [IsDeleted]     BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PurchaseRequests_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [PRJ].[Projects] ([Id])
);

