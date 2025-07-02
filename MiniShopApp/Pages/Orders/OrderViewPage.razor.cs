using MiniShopApp.Infrastructures.Services.Interfaces;
using MiniShopApp.Models.Orders;
using Microsoft.JSInterop;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using Microsoft.AspNetCore.Components;

namespace MiniShopApp.Pages.Orders
{
    public partial class OrderViewPage(IOrderService orderService)
    {
        [Inject] IJSRuntime JS { get; set; } = default!;

        protected List<ViewTbOrders> orders = new List<ViewTbOrders>();
        bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            await GetOrderAsync();
            //await table.ReloadServerData();
            await base.OnInitializedAsync();
        }

        async Task GetOrderAsync()
        {
            try
            {
                isLoading = true;
                var results = await orderService.GetOrderSummaryAsync();
                if (results.IsSuccess)
                {
                    orders = results.Data!.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception ex)
            {
                SnackbarService.Add(ex.Message, MudBlazor.Severity.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        private async void ExportToExcel()
        {
            var filteredOrders = GetFilteredOrders();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Orders");
            worksheet.Cell(1, 1).Value = "Date";
            worksheet.Cell(1, 2).Value = "Customer Id";
            worksheet.Cell(1, 3).Value = "Customer";
            worksheet.Cell(1, 4).Value = "Table";
            worksheet.Cell(1, 5).Value = "Item";
            worksheet.Cell(1, 6).Value = "Price";
            worksheet.Cell(1, 7).Value = "Discount";
            worksheet.Cell(1, 8).Value = "Total";
            int row = 2;
            foreach (var order in filteredOrders)
            {
                worksheet.Cell(row, 1).Value = order.CreatedDT?.ToString("dd/MMM/yy");
                worksheet.Cell(row, 2).Value = order.CustomerId;
                worksheet.Cell(row, 3).Value = order.FirstName;
                worksheet.Cell(row, 4).Value = order.TableNumber;
                worksheet.Cell(row, 5).Value = order.ItemCount;
                worksheet.Cell(row, 6).Value = order.SubPrice;
                worksheet.Cell(row, 7).Value = order.DiscountPrice;
                worksheet.Cell(row, 8).Value = order.TotalPrice;
                row++;
            }
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            await JS.InvokeVoidAsync("DownloadReportFile", "Orders.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream.ToArray());
        }

        private async void ExportToPdf()
        {
            // Set QuestPDF license type to Community before using QuestPDF
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var filteredOrders = GetFilteredOrders();
            var stream = new MemoryStream();
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(40);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(60);
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).Text("Customer Id");
                                header.Cell().Element(CellStyle).Text("Customer");
                                header.Cell().Element(CellStyle).Text("Table");
                                header.Cell().Element(CellStyle).Text("Item");
                                header.Cell().Element(CellStyle).Text("Price");
                                header.Cell().Element(CellStyle).Text("Discount");
                                header.Cell().Element(CellStyle).Text("Total");
                            });
                            foreach (var order in filteredOrders)
                            {
                                table.Cell().Element(CellStyle).Text(order.CreatedDT?.ToString("dd/MMM/yy"));
                                table.Cell().Element(CellStyle).Text(order.CustomerId.ToString());
                                table.Cell().Element(CellStyle).Text(order.FirstName);
                                table.Cell().Element(CellStyle).Text(order.TableNumber);
                                table.Cell().Element(CellStyle).Text(order.ItemCount?.ToString());
                                table.Cell().Element(CellStyle).Text(order.SubPrice?.ToString("c2"));
                                table.Cell().Element(CellStyle).Text(order.DiscountPrice?.ToString("P0"));
                                table.Cell().Element(CellStyle).Text(order.TotalPrice?.ToString("c2"));
                            }
                        });
                });
            });
            document.GeneratePdf(stream);
            await JS.InvokeVoidAsync("DownloadReportFile", "Orders.pdf", "application/pdf", stream.ToArray());
        }

        private IEnumerable<ViewTbOrders> GetFilteredOrders()
        {
            return orders.Where(element =>
            {
                bool matches = true;
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    matches = element.CustomerId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                        || (element.TableNumber != null && element.TableNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }
                if (matches && startDate.HasValue)
                {
                    matches = element.CreatedDT.HasValue && element.CreatedDT.Value.Date >= startDate.Value.Date;
                }
                if (matches && endDate.HasValue)
                {
                    matches = element.CreatedDT.HasValue && element.CreatedDT.Value.Date <= endDate.Value.Date;
                }
                if (matches && selectedMonth.HasValue)
                {
                    matches = element.CreatedDT.HasValue && element.CreatedDT.Value.Month == selectedMonth.Value;
                }
                if (matches && selectedYear.HasValue)
                {
                    matches = element.CreatedDT.HasValue && element.CreatedDT.Value.Year == selectedYear.Value;
                }
                return matches;
            }).ToArray();
        }

        private IContainer CellStyle(IContainer container) => container.PaddingVertical(2).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
    }
}