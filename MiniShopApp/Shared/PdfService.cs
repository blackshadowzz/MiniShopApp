namespace MiniShopApp.Shared
{
    using QuestPDF.Fluent;
    using MiniShopApp.Models.Orders;
    using QuestPDF.Infrastructure;
    using QuestPDF.Helpers;

    public class PdfService 
    {
        public byte[] CreateOrderReceiptPdf(ViewTbOrders order)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(80, PageSizes.A4.Height, Unit.Millimetre); // 80mm width, A4 height
                    page.Margin(5);
                    page.Header().Text($"Order Receipt\n").FontSize(13).Bold().AlignCenter();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Ordered #: {order.Id}").FontSize(10);
                        col.Item().Text($"Customer: {order.FirstName} {order.LastName}").FontSize(10);
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
        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }
        public byte[] CreateOrderReceiptPdf(List<ViewTbOrders> orders)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
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
