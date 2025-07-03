using Helpers.Responses;
using System.Diagnostics;
using System.Drawing.Printing;

namespace MiniShopApp.Shared.AdditionalServices
{
    public class PrintNodeService
    {

        public async Task<Result<string>> PrintPdfAutoAsync(byte[] pdfBytes)
        {
            try
            {
                //// Step 1: Get the default printer name from system
                //string printerName = GetDefaultPrinterName();

                //if (string.IsNullOrWhiteSpace(printerName))
                //    return Result.Failure<string>(new ErrorResponse("No default printer found."));

                // Step 2: Save PDF to temp path
                string filePath = Path.Combine(Path.GetTempPath(), "print.pdf");
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

                return Result.Success("Print job sent to: " );
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(new ErrorResponse($"Print failed: {ex.Message}"));
            }
        }

        //private string GetDefaultPrinterName()
        //{
        //    using (var printDoc = new PrintDocument())
        //    {
        //        return printDoc.PrinterSettings.PrinterName;
        //    }
        //}

    }
}
