CREATE TABLE [PR].[PurchaseRequestItems] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [PurchaseRequestId] INT             NOT NULL,
    [ItemName]          NVARCHAR (200)  NOT NULL,
    [Unit]              NVARCHAR (50)   NOT NULL,
    [Quantity]          DECIMAL (18, 2) NOT NULL,
    [Specification]     NVARCHAR (500)  NULL,
    [CreatedAt]         DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [IsDeleted]         BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PRItems_PurchaseRequests] FOREIGN KEY ([PurchaseRequestId]) REFERENCES [PR].[PurchaseRequests] ([Id])
);

