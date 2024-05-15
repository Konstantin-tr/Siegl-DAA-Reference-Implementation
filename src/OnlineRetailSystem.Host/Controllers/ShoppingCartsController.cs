// <copyright file="ShoppingCartsController.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.AspNetCore.Mvc;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using OnlineRetailSystem.Host.Dtos;

namespace OnlineRetailSystem.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingCartsController(ILogger<ShoppingCartsController> logger, IGrainFactory actorsSource) : ControllerBase
    {
        private readonly ILogger<ShoppingCartsController> _logger = logger;
        private readonly IGrainFactory _actorsSource = actorsSource;

        [HttpPost]
        [Route("/add-product")]
        public async Task<ActionResult<CommandResponseDto>> AddProductToShoppingCart([FromBody] AddProductToShoppingCartRequestDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<IAddProductToShoppingCartCommandActor>(0);

                var response = await command.Execute(request.ShoppingCartId, request.ProductId, request.Quantity);

                return new(response.IsSuccess, response.Message);
            });
        }

        [HttpPost]
        [Route("/update-product-quantity")]
        public async Task<ActionResult<CommandResponseDto>> UpdateShoppingCartProductQuantity([FromBody] ChangeProductQuantityRequestDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<IChangeShoppingCartProductQuantityCommandActor>(0);

                var response = await command.Execute(request.ShoppingCartId, request.ProductId, request.Quantity);

                return new(response.IsSuccess, response.Message);
            });
        }

        [HttpPost]
        [Route("/remove-product")]
        public async Task<ActionResult<CommandResponseDto>> RemoveProductFromShoppingCart([FromBody] RemoveProductRequestDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<IRemoveProductFromShoppingCartCommandActor>(0);

                var response = await command.Execute(request.ShoppingCartId, request.ProductId);

                return new(response.IsSuccess, response.Message);
            });
        }

        [HttpGet]
        [Route("/view-shopping-cart/{shoppingCartId}")]
        public async Task<ActionResult<ViewShoppingCartResponseDto>> ViewShoppingCart(string shoppingCartId)
        {
            var query = _actorsSource.GetGrain<IViewShoppingCartQueryActor>(0);

            var response = await query.Execute(shoppingCartId);

            var cart = response.ShoppingCart;

            if (cart is null)
            {
                return NotFound();
            }

            return new ViewShoppingCartResponseDto(
                new(
                    cart.TotalPriceInEuro,
                    cart.Items.Select(s =>
                        new ShoppingCartItemDto(
                            s.ItemdId,
                            s.ProductId,
                            s.ProductName,
                            s.Quantity,
                            s.PriceInEuro
                        )
                    )
                .ToArray()
                )
            );
        }
    }
}
