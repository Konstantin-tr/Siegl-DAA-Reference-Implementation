// <copyright file="ShoppingCartDtos.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Host.Dtos
{
    public record AddProductToShoppingCartRequestDto(string ShoppingCartId, string ProductId, int Quantity);

    public record ChangeProductQuantityRequestDto(string ShoppingCartId, string ProductId, int Quantity);

    public record RemoveProductRequestDto(string ShoppingCartId, string ProductId);

    public record ViewShoppingCartResponseDto(ShoppingCartDto? ShoppingCart);

    public record ShoppingCartDto(decimal TotalPriceInEuro, ShoppingCartItemDto[] ShoppingCartItems);

    public record ShoppingCartItemDto(string ItemdId, string ProductId, string ProductName, int Quantity, decimal PriceInEuro);



    public record ShoppingCartInfoDto(string Id, string CustomerId);

    public record ShoppingCartAddDto(string Id, string CustomerId);
}
