// <copyright file="ProductsController.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.AspNetCore.Mvc;
using OnlineRetailSystem.Actors.ProductSubdomain.Contracts.ExternalWorkflows;
using OnlineRetailSystem.Host.Dtos;

namespace OnlineRetailSystem.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController(ILogger<ProductsController> logger, IGrainFactory actorsSource) : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger = logger;
        private readonly IGrainFactory _actorsSource = actorsSource;

        [HttpGet]
        [Route("/view-product/{id}")]
        public async Task<ActionResult<ViewProductResponseDto>> ViewProduct(string id)
        {
            var query = _actorsSource.GetGrain<IViewProductQueryActor>(0);

            var data = await query.Execute(id);

            var product = data.Product;

            if (product is null)
            {
                return NotFound();
            }

            return new ViewProductResponseDto(new ProductDataDto(product.ProductId, product.ProductName, product.Description, product.PriceInEuro, product.AvailableStock));
        }


        [HttpGet]
        [Route("/list-all-products")]
        public async Task<ActionResult<ListProductsResponseDto>> ListProducts(string? nameFilter, string? orderBy, bool? orderDescending)
        {
            var query = _actorsSource.GetGrain<IListProductsQueryActor>(0);

            var data = await query.Execute(nameFilter, orderBy, orderDescending);

            var products = data.Products.Select(p => new ProductListDto(p.ProductId, p.ProductName, p.PriceInEuro, p.AvailableStock)).ToArray();

            return new ListProductsResponseDto(products);
        }

        [HttpPost]
        [Route("/create-product")]
        public async Task<ActionResult<CommandResponseResultDto<CreateProductResponseDto>>> CreateProduct([FromBody] CreateProductDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<ICreateProductCommandActor>(0);

                var response = await command.Execute(request.ProductName, request.ProductDescription, request.PriceInEuro, request.AvailableStock);

                return new CommandResponseResultDto<CreateProductResponseDto>(
                    response.IsSuccess,
                    response.IsSuccess && response.Result is not null
                        ? new(response.Result.ProductId)
                        : null,
                    response.Message
                    );
            });
        }

        [HttpPost]
        [Route("/update-product")]
        public async Task<ActionResult<CommandResponseDto>> UpdateProduct([FromBody] UpdateProductDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<IUpdateProductCommandActor>(0);

                var response = await command.Execute(request.ProductId, request.ProductName, request.ProductDescription, request.PriceInEuro, request.AvailableStock);

                return new CommandResponseDto(response.IsSuccess, response.Message);
            });
        }

        [HttpPost]
        [Route("/delete-product")]
        public async Task<ActionResult<CommandResponseDto>> DeleteProduct([FromBody] DeleteProductDto request)
        {
            return await this.ExecuteCommand(async () =>
            {
                var command = _actorsSource.GetGrain<IDeleteProductCommandActor>(0);

                var response = await command.Execute(request.ProductId);

                return new CommandResponseDto(response.IsSuccess, response.Message);
            });
        }
    }
}
