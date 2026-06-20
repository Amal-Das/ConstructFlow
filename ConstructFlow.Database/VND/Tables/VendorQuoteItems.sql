CREATE TABLE [VND].[VendorQuoteItems] (
    [Id]                    INT             IDENTITY (1, 1) NOT NULL,
    [VendorQuoteId]         INT             NOT NULL,
    [PurchaseRequestItemId] INT             NOT NULL,
    [UnitPrice]             DECIMAL (18, 2) NOT NULL,
    [TotalPrice]            DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VQItems_PRItem] FOREIGN KEY ([PurchaseRequestItemId]) REFERENCES [PR].[PurchaseRequestItems] ([Id]),
    CONSTRAINT [FK_VQItems_VendorQuote] FOREIGN KEY ([VendorQuoteId]) REFERENCES [VND].[VendorQuotes] ([Id])
);

