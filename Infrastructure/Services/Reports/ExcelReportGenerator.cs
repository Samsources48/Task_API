using ClosedXML.Excel;

namespace Infrastructure.Services.Notification.Reports
{
    public class ExcelReportGenerator : IExcelReportGenerator
    {
        //public byte[] GenerateTrackingReport(TrackingExecutionStatsDto stats,List<TrackingResultDto> resultados)
        //{
        //    using var workbook = new XLWorkbook();

        //    BuildResumenSheet(workbook, stats);
        //    BuildDetalleSheet(workbook, resultados);

        //    using var stream = new MemoryStream();
        //    workbook.SaveAs(stream);
        //    return stream.ToArray();
        //}

        ///// Hoja 1: Resumen con estadísticas por naviera.
        //private static void BuildResumenSheet(IXLWorkbook workbook, TrackingExecutionStatsDto stats)
        //{
        //    var sheet = workbook.Worksheets.Add("Resumen");

        //    // Título principal
        //    sheet.Cell("A1").Value = "REPORTE DE TRACKING RPA";
        //    sheet.Cell("A1").Style.Font.Bold = true;
        //    sheet.Cell("A1").Style.Font.FontSize = 16;
        //    sheet.Range("A1:E1").Merge();

        //    // Información del proceso
        //    sheet.Cell("A3").Value = "Inicio de proceso:";
        //    sheet.Cell("A3").Style.Font.Bold = true;
        //    sheet.Cell("B3").Value = stats.FechaInicio.ToString("yyyy-MM-dd HH:mm:ss");

        //    sheet.Cell("A4").Value = "Fin de proceso:";
        //    sheet.Cell("A4").Style.Font.Bold = true;
        //    sheet.Cell("B4").Value = stats.FechaFin.ToString("yyyy-MM-dd HH:mm:ss");

        //    // Encabezados de tabla de estadísticas
        //    int row = 6;
        //    var headers = new[] { "Naviera", "Registros Iniciales", "Registros Procesados", "Registros Fallidos", "% Procesados" };

        //    for (int i = 0; i < headers.Length; i++)
        //        sheet.Cell(row, i + 1).Value = headers[i];

        //    var headerRange = sheet.Range(row, 1, row, headers.Length);
        //    headerRange.Style.Font.Bold = true;
        //    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2c3e50");
        //    headerRange.Style.Font.FontColor = XLColor.White;
        //    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    // Datos por naviera
        //    foreach (var naviera in stats.EstadisticasPorNaviera)
        //    {
        //        row++;
        //        sheet.Cell(row, 1).Value = naviera.Naviera;
        //        sheet.Cell(row, 2).Value = naviera.RegistrosIniciales;
        //        sheet.Cell(row, 3).Value = naviera.RegistrosProcesados;
        //        sheet.Cell(row, 4).Value = naviera.RegistrosFallidos;
        //        sheet.Cell(row, 5).Value = naviera.PorcentajeProcesados / 100.0;
        //        sheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";

        //        // Centrar columnas numéricas
        //        for (int col = 2; col <= 5; col++)
        //            sheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //        // Color de fondo alternado
        //        if (row % 2 == 0)
        //        {
        //            sheet.Range(row, 1, row, headers.Length)
        //                .Style.Fill.BackgroundColor = XLColor.FromHtml("#ecf0f1");
        //        }
        //    }

        //    // Fila de totales
        //    row++;
        //    sheet.Cell(row, 1).Value = "TOTAL";
        //    sheet.Cell(row, 2).Value = stats.TotalContenedores;
        //    sheet.Cell(row, 3).Value = stats.TotalExitosos;
        //    sheet.Cell(row, 4).Value = stats.TotalFallidos;

        //    double porcentajeTotal = stats.TotalContenedores > 0
        //        ? (double)stats.TotalExitosos / stats.TotalContenedores
        //        : 0;
        //    sheet.Cell(row, 5).Value = porcentajeTotal;
        //    sheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";

        //    var totalRange = sheet.Range(row, 1, row, headers.Length);
        //    totalRange.Style.Font.Bold = true;
        //    totalRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#bdc3c7");
        //    for (int col = 2; col <= 5; col++)
        //        sheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    // Bordes en toda la tabla
        //    var tableRange = sheet.Range(6, 1, row, headers.Length);
        //    tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        //    sheet.Columns().AdjustToContents();
        //}

        ///// Hoja 2: Detalle de cada contenedor con toda su información.
        ///// Contenedores fallidos se marcan en rojo claro.
        //private static void BuildDetalleSheet(IXLWorkbook workbook, List<TrackingResultDto> resultados)
        //{
        //    var sheet = workbook.Worksheets.Add("Detalle");

        //    // Encabezados
        //    var headers = new[]
        //    {
        //        "Contenedor", "Naviera", "Estado", "Exitoso",
        //        "Puerto Origen", "Puerto Destino",
        //        "ETA Final", "Buque", "Viaje",
        //        "Ultimo Evento", "Fecha Ultimo Evento", "Error"
        //    };

        //    for (int i = 0; i < headers.Length; i++)
        //        sheet.Cell(1, i + 1).Value = headers[i];

        //    var headerRange = sheet.Range(1, 1, 1, headers.Length);
        //    headerRange.Style.Font.Bold = true;
        //    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2c3e50");
        //    headerRange.Style.Font.FontColor = XLColor.White;
        //    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //    // Datos de cada contenedor
        //    int row = 2;
        //    foreach (var r in resultados)
        //    {
        //        sheet.Cell(row, 1).Value = r.Contenedor ?? "";
        //        sheet.Cell(row, 2).Value = r.Naviera ?? "";
        //        sheet.Cell(row, 3).Value = r.Estado ?? "";
        //        sheet.Cell(row, 4).Value = r.Exitoso ? "SI" : "NO";
        //        sheet.Cell(row, 5).Value = r.PuertoOrigen ?? "";
        //        sheet.Cell(row, 6).Value = r.PuertoDestino ?? "";
        //        sheet.Cell(row, 7).Value = r.EtaFinal ?? "";
        //        sheet.Cell(row, 8).Value = r.VesselActual ?? "";
        //        sheet.Cell(row, 9).Value = r.Voyage ?? "";
        //        sheet.Cell(row, 10).Value = r.UltimoEvento ?? "";
        //        sheet.Cell(row, 11).Value = r.FechaUltimoEvento ?? "";
        //        sheet.Cell(row, 12).Value = r.Error ?? "";

        //        // Filas fallidas en rojo claro
        //        if (!r.Exitoso)
        //        {
        //            sheet.Range(row, 1, row, headers.Length)
        //                .Style.Fill.BackgroundColor = XLColor.FromHtml("#f8d7da");
        //        }

        //        row++;
        //    }

        //    // Bordes en toda la tabla
        //    if (resultados.Count > 0)
        //    {
        //        var tableRange = sheet.Range(1, 1, row - 1, headers.Length);
        //        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //    }

        //    // Auto-filtro en la primera fila
        //    sheet.RangeUsed()?.SetAutoFilter();

        //    sheet.Columns().AdjustToContents();
        //}
    }
}
