CREATE TABLE [INV].[InventoryItems] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [ProjectId]         INT             NOT NULL,
    [ItemName]          NVARCHAR (200)  NOT NULL,
    [Unit]              NVARCHAR (50)   NOT NULL,
    [QuantityInStock]   DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [MinimumStockLevel] DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    [UnitCost]          DECIMAL (18, 2) NOT NULL,
    [CreatedAt]         DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]         DATETIME        NULL,
    [IsDeleted]         BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InventoryItems_Projects] FOREIGN KEY ([ProjectId]) REFERENCES [PRJ].[Projects] ([Id])
);

