// <copyright file="OrdersController.cs" company="Konstantin Siegl">
// Copyright (c) 2024 - MIT License
// </copyright>
// <author>Konstantin Siegl, BSc.</author>
// <summary>This file is part of the DAA reference implementation.</summary>

using Microsoft.AspNetCore.Mvc;
using OnlineRetailSystem.Actors.ShoppingCartSubdomain.Contracts.ExternalWorkflows;
using OnlineRetailSystem.Host.Dtos;
using System.Net.Mime;

namespace OnlineRetailSystem.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController(ILogger<OrdersController> logger, IGrainFactory actorsSource) : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger = logger;
        private readonly IGrainFactory _actorsSource = actorsSource;

        [HttpGet]
        [Route("/list-invoices")]
        public async Task<ActionResult<ListInvoicesResponse>> ListInvoices()
        {
            var query = _actorsSource.GetGrain<IListInvoicesQueryActor>(0);

            var response = await query.Execute();

            return new ListInvoicesResponse(
                response.Invoices.Select(
                    i => new InvoiceDto(
                        i.InvoiceId,
                        i.OrderId,
                        i.CustomerName,
                        i.CreationDate,
                        i.TotalOrderPriceInEuro,
                        i.IsPaid,
                        i.Items.Select(
                            item => new InvoiceItemDto(
                                item.ItemdId,
                                item.ProductId,
                                item.PriceInEuro,
                                item.ProductName,
                                item.Quantity
                            )
                        ).ToArray()
                    )
                ).ToArray()
            );
        }


        [HttpGet]
        [Route("/generate-sales-report")]
        public async Task<ActionResult> GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            var query = _actorsSource.GetGrain<IGenerateSalesReportQueryActor>(0);

            var response = await query.Execute(startDate, endDate);

            return File(response.Data, MediaTypeNames.Application.Octet, "sales_report.csv");
        }

        [HttpPost]
        [Route("/create-order")]
        public async Task<ActionResult<CommandResponseResultDto<CreateOrderFromShoppingCartResponseDto>>> CreateOrderFromShoppingCart([FromBody] CreateOrderFromShoppingCartRequest request)
        {
            return await this.ExecuteCommand<CreateOrderFromShoppingCartResponseDto>(async () =>
            {
                var command = _actorsSource.GetGrain<ICreateOrderFromShoppingCartCommandActor>(0);

                var response = await command.Execute(request.ShoppingCartId, request.CustomerId);

                return new(response.IsSuccess, response.IsSuccess && response.Result is not null ? new(response.Result.OrderId) : null, response.Message);
            });
        }

        [HttpGet]
        [Route("/view-user-invoices/{userId}")]
        public async Task<ActionResult<ListUserInvoicesResponse>> ViewUserInvoices(string userId)
        {
            var query = _actorsSource.GetGrain<IListOwnInvoicesQueryActor>(0);

            var response = await query.Execute(userId);

            return new ListUserInvoicesResponse(
                response.Invoices.Select(
                    i => new InvoiceDto(
                        i.InvoiceId,
                        i.OrderId,
                        i.CustomerName,
                        i.CreationDate,
                        i.TotalOrderPriceInEuro,
                        i.IsPaid,
                        i.Items.Select(
                            item => new InvoiceItemDto(
                                item.ItemdId,
                                item.ProductId,
                                item.PriceInEuro,
                                item.ProductName,
                                item.Quantity
                            )
                        ).ToArray()
                    )
                ).ToArray()
            );
        }
    }
}
