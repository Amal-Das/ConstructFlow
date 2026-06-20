CREATE TABLE [VND].[VendorQuotes] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [PurchaseRequestId] INT             NOT NULL,
    [VendorId]          INT             NOT NULL,
    [QuoteDate]         DATETIME        NOT NULL,
    [TotalAmount]       DECIMAL (18, 2) NOT NULL,
    [IsAwarded]         BIT             DEFAULT ((0)) NOT NULL,
    [Remarks]           NVARCHAR (500)  NULL,
    [CreatedAt]         DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [IsDeleted]         BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VendorQuotes_PR] FOREIGN KEY ([PurchaseRequestId]) REFERENCES [PR].[PurchaseRequests] ([Id]),
    CONSTRAINT [FK_VendorQuotes_Vendor] FOREIGN KEY ([VendorId]) REFERENCES [VND].[Vendors] ([Id])
);

