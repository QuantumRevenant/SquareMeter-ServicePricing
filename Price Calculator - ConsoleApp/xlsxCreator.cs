using ClosedXML.Excel;
using System;
using System.Globalization;
using System.IO;

public class xlsxCreator
{
    private XLWorkbook workbook;
    private IXLWorksheet worksheet;
    private int currentRow = 1;

    public void Create(string sheetName)
    {
        workbook = new XLWorkbook();
        worksheet = workbook.Worksheets.Add(sheetName);
    }

    public void AddRow(string csvColumn)
    {
        if (worksheet == null)
        {
            throw new InvalidOperationException("Debe crear una hoja antes de agregar filas.");
        }

        var values = csvColumn.Split(',');

        for (int i = 0; i < values.Length; i++)
        {
            string value = values[i].Trim();

            // Intentar parsear como fecha en formato dd/mm/yyyy
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
            {
                worksheet.Cell(currentRow, i + 1).Value = dateValue; // Asignar como fecha
                worksheet.Cell(currentRow, i + 1).Style.DateFormat.Format = "dd/MM/yyyy"; // Aplicar formato de fecha
            }
            // Intentar parsear como porcentaje (asumir que los porcentajes están dados como decimales)
            else if (value.EndsWith("%") && double.TryParse(value.TrimEnd('%'), out double percentageValue))
            {
                worksheet.Cell(currentRow, i + 1).Value = percentageValue / 100; // Guardar como decimal
                worksheet.Cell(currentRow, i + 1).Style.NumberFormat.Format = "0.00%"; // Aplicar formato de porcentaje
            }
            else if (double.TryParse(value, out double numericValue))
            {
                worksheet.Cell(currentRow, i + 1).Value = numericValue; // Si es numérico, lo asigna como número
            }
            else
            {
                worksheet.Cell(currentRow, i + 1).Value = value; // Si no es numérico, lo asigna como texto
            }
        }

        currentRow++;
    }

    public void Close(string filePath)
    {
        if (workbook == null)
        {
            throw new InvalidOperationException("No hay libro de Excel para guardar.");
        }

        workbook.SaveAs(filePath);
    }
}
