// <copyright file="OrderDtos.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Host.Dtos
{
    public record CreateOrderFromShoppingCartRequest(string ShoppingCartId, string CustomerId);
    public record CreateOrderFromShoppingCartResponseDto(string OrderId);

    public record ListUserInvoicesResponse(InvoiceDto[] Invoices);

    public record ListInvoicesResponse(InvoiceDto[] Invoices);

    public record InvoiceDto(string InvoiceId, string OrderId, string CustomerName, DateTime CreationDate, decimal TotalOrderPriceInEuro, bool IsPaid, InvoiceItemDto[] Items);

    public record InvoiceItemDto(string ItemId, string ProductId, decimal PriceInEuro, string ProductName, int Quantity);
}
