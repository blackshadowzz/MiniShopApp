namespace MiniShopApp.Shared.AdditionalServices
{
    using Helpers.Responses;
    using MiniShopApp.Models.Orders;
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;
    using System.Diagnostics;

    public class PdfService 
    {
        public async Task<Result<string>> PrintPdfAutoAsync(byte[] pdfBytes)
        {
            try
            {
                // Step 2: Save PDF to temp path
                var appTemp = Path.Combine(AppContext.BaseDirectory, "PrintCache");
                Directory.CreateDirectory(appTemp);

                string filePath = Path.Combine(appTemp, $"print_{Guid.NewGuid()}.pdf");
                await File.WriteAllBytesAsync(filePath, pdfBytes);

                // Step 3: Set up process to print via default printer
                var psi = new ProcessStartInfo
                {
                    FileName = filePath,
                    Verb = "print",
                    CreateNoWindow = true,
                    UseShellExecute = true
                };

                Process.Start(psi);

                return Result.Success("Print job sent to: ");
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new ErrorResponse($"Print failed: {ex.Message}"));
            }
        }
        
        public byte[] CreateOrderReceiptPdf(ViewTbOrders order)
        {
            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.ContinuousSize(80, Unit.Millimetre); // 80mm width, auto height
                        page.MarginTop(5);                                   
                        page.Margin(5);

                        page.Header().Column(row =>
                        {
                            
                            row.Item().Text($"\nMINI APP ORDER").FontSize(13).Bold().AlignCenter();
                            row.Item().Text($"Sample order receipt\n").FontSize(8).AlignCenter();
                            //var path=
                            row.Item().AlignCenter().Width(30).Height(30).Image("wwwroot/images/MiniLogo.jpg").FitWidth();
                            row.Item().Text($"Order Receipt\n").FontSize(12).Bold().AlignCenter();
                        });
                        page.Content().Column(col =>
                        {
                            string? customer = "";
                            if (string.IsNullOrEmpty(order.FirstName + order.LastName))
                                customer = order.CustomerId.ToString();
                            else
                                customer = order.FirstName + " " + order.LastName;
                            col.Item().Text($"Ordered #: OR{order.OrderCode??order.Id.ToString()}").FontSize(10);
                            col.Item().Text($"Customer: {customer} ").FontSize(10);
                            col.Item().Text($"Date: {order.CreatedDT?.ToString("dd/MMM/yy hh:mm tt")}").FontSize(10);

                            col.Item().Text("\n");
                            col.Item().Text($"Table: {order.TableNumber}").FontSize(10).AlignCenter();

                            col.Item().LineHorizontal(1);

                            col.Item().EnsureSpace();
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(10);

                                    columns.RelativeColumn();
                                    columns.ConstantColumn(25);
                                    columns.ConstantColumn(35);
                                    columns.ConstantColumn(35);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("#").Bold().FontSize(10);
                                    header.Cell().Element(CellStyle).Text("Item").Bold().FontSize(10).DecorationSolid();
                                    header.Cell().Element(CellStyle).Text("Qty").Bold().FontSize(10).DecorationSolid();
                                    header.Cell().Element(CellStyle).Text("Price").Bold().FontSize(10).DecorationSolid();
                                    header.Cell().Element(CellStyle).Text("Total").Bold().FontSize(10).DecorationSolid();
                                });

                                if (order.TbOrderDetails != null)
                                {
                                    foreach (var detail in order.TbOrderDetails)
                                    {
                                        table.Cell().Element(CellStyle).Text($"{order.TbOrderDetails.IndexOf(detail) + 1}").FontSize(9);
                                        table.Cell().Element(CellStyle).Text(detail.ItemName).FontSize(9);
                                        table.Cell().Element(CellStyle).Text(detail.Quantity?.ToString() ?? "").FontSize(9).AlignRight();
                                        table.Cell().Element(CellStyle).Text(detail.Price?.ToString("c2") ?? "").FontSize(9).AlignRight();
                                        table.Cell().Element(CellStyle).Text(detail.TotalPrice?.ToString("c2") ?? "").FontSize(9).AlignRight();
                                    }
                                }
                                table.Footer(footer =>
                                {
                                    footer.Cell().Text("");
                                    footer.Cell().Text("Sub-Price:").FontSize(10);
                                    footer.Cell().Text("");
                                    footer.Cell().Text("");

                                    footer.Cell().AlignRight().Text($" {order.TotalPrice?.ToString("c2")}").FontSize(10).Bold().FontColor(Color.FromRGB(255, 10, 10));
                                    footer.Cell().Text("");
                                    footer.Cell().Text("Discount%:").FontSize(10);
                                    footer.Cell().Text("");
                                    footer.Cell().Text("");

                                    footer.Cell().AlignRight().Text($" {order.DiscountPrice?.ToString()}").FontSize(10).Bold().FontColor(Color.FromRGB(255, 10, 10));
                                    footer.Cell().Text("");
                                    footer.Cell().Text("Total:").FontSize(10);
                                    footer.Cell().Text("");
                                    footer.Cell().Text("");

                                    footer.Cell().AlignRight().Text($" {order.TotalPrice?.ToString("c2")}").FontSize(10).Bold().FontColor(Color.FromRGB(255, 10, 10));
                                }
                                );
                            });

                            col.Item().Text("\n");
                            col.Item().AlignCenter().Text("Thank you for orderd!").FontSize(9);
                            col.Item().AlignCenter().Text("See you next time!").FontSize(9); col.Item().Text("\n");

                        });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
        public byte[] CreateOrderReceiptPdf(List<ViewTbOrders> orders)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(80, PageSizes.A4.Height, Unit.Millimetre); // 80mm width, A4 height
                    page.Margin(5);
                    page.Content().Column(col =>
                    {
                        foreach (var order in orders)
                        {
                            col.Item().Text($"Order Receipt #{order.Id}").FontSize(14).Bold();
                            col.Item().Text($"Customer: {order.FirstName} {order.LastName}");
                            col.Item().Text($"Table: {order.TableNumber}");
                            col.Item().Text($"Date: {order.CreatedDT?.ToString("dd/MMM/yy hh:mm tt")}");
                            col.Item().LineHorizontal(1);
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(25);
                                    columns.ConstantColumn(35);
                                    columns.ConstantColumn(35);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Item").Bold();
                                    header.Cell().Text("Qty").Bold();
                                    header.Cell().Text("Price").Bold();
                                    header.Cell().Text("Total").Bold();
                                });

                                if (order.TbOrderDetails != null)
                                {
                                    foreach (var detail in order.TbOrderDetails)
                                    {
                                        table.Cell().Text(detail.ItemName);
                                        table.Cell().Text(detail.Quantity?.ToString() ?? "");
                                        table.Cell().Text(detail.Price?.ToString("c2") ?? "");
                                        table.Cell().Text(detail.TotalPrice?.ToString("c2") ?? "");
                                    }
                                }
                            });
                            col.Item().LineHorizontal(1);
                            col.Item().AlignRight().Text($"Total: {order.TotalPrice?.ToString("c2")}").FontSize(12).Bold();
                            col.Item().Text(""); // Spacer between orders
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
