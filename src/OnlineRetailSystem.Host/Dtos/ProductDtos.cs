// <copyright file="ProductDtos.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

namespace OnlineRetailSystem.Host.Dtos
{
    public record ViewProductResponseDto(ProductDataDto product);

    public record ProductDataDto(string ProductId, string ProductName, string ProductDescription, decimal PriceInEuro, int availableStock);

    public record ListProductsResponseDto(ProductListDto[] products);

    public record ProductListDto(string ProductId, string ProductName, decimal PriceInEuro, int availableStock);

    public record CreateProductDto(string ProductName, string ProductDescription, decimal PriceInEuro, int AvailableStock);

    public record UpdateProductDto(string ProductId, string ProductName, string ProductDescription, decimal PriceInEuro, int AvailableStock);

    public record DeleteProductDto(string ProductId);

    public record CreateProductResponseDto(string ProductId);


}
