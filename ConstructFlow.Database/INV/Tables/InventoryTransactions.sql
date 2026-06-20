CREATE TABLE [INV].[InventoryTransactions] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [InventoryItemId] INT             NOT NULL,
    [TransactionType] INT             NOT NULL,
    [Quantity]        DECIMAL (18, 2) NOT NULL,
    [TransactionDate] DATETIME        NOT NULL,
    [Reference]       NVARCHAR (200)  NULL,
    [CreatedAt]       DATETIME        DEFAULT (getutcdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InvTransactions_Item] FOREIGN KEY ([InventoryItemId]) REFERENCES [INV].[InventoryItems] ([Id])
);

