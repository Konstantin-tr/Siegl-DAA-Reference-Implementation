// <copyright file="DemoController.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.AspNetCore.Mvc;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.Demo;
using OnlineRetailSystem.Host.Dtos;

namespace OnlineRetailSystem.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController(IGrainFactory actorsSource) : ControllerBase
    {
        [HttpGet]
        [Route("/list-shopping-carts/")]
        public async Task<ActionResult<ShoppingCartInfoDto[]>> ListShoppingCarts()
        {
            var repo = actorsSource.GetGrain<IShoppingCartRepositoryActor>(0);

            var shoppingCarts = await repo.GetIds();

            return shoppingCarts.Select(s => new ShoppingCartInfoDto(s.Id, s.CustomerId)).ToArray();
        }

        [HttpPost]
        [Route("/add-shopping-cart/")]
        public async Task<ActionResult> AddShoppingCart([FromBody] ShoppingCartAddDto data)
        {
            var repo = actorsSource.GetGrain<IShoppingCartRepositoryActor>(0);

            await repo.Add(data.Id, data.CustomerId);

            return Ok();
        }
    }
}
