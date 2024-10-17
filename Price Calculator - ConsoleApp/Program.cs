using QR.QRMenu;
internal class Program
{
    private static void Main(string[] args)
    {
        SqMtPrice.LoadValues();
        do
        {
            string[] options = { "Calcular un valor único", "Generar Excel con la tendencia completa" };
            int option = QRMenu.OptionMenu(string.Empty, "¿Qué deseas hacer?", options);

            double numero = -1;

            if (option is 1 or 2)
                numero = QRMenu.AskNumberMenu("Escribe los metros cuadrados:", (0, double.PositiveInfinity), false);

            Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.

            switch (option)
            {
                case 1:
                    double valorm2 = SqMtPrice.GetPricePerSquareMeter(numero);
                    double valortotal = SqMtPrice.GetTotalPrice(numero);
                    int m2Evaluados = (int)SqMtPrice.GetActualPost(numero);
                    Console.WriteLine($"El costo por el área evaluada ({numero} m2) es: S/. {Math.Round(valortotal, 2)} (S/. {Math.Round(valorm2, 4)}/m2) - se usó el área de {m2Evaluados} m2");
                    Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
                    break;

                case 2:
                    string fileName = QRMenu.AskMenu("Introduce el nombre del archivo Excel (sin extensión):");
                    string filePath = $"./{fileName}.xlsx";

                    // Advertir sobre la sobreescritura si el archivo ya existe
                    if (File.Exists(filePath))
                    {
                        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
                        if (!QRMenu.ConfirmationMenu($"Advertencia: El archivo '{fileName}.xlsx' ya existe. ¿Deseas sobreescribirlo?"))
                        {
                            Console.WriteLine("Operación cancelada. No se generó el archivo Excel.");
                            break;
                        }
                    }

                    // Generar el Excel
                    double ValorObjetivo = numero;
                    var excel = new xlsxCreator();

                    excel.Create("Hoja1");
                    excel.AddRow("Metros,Precio Total,Precio/m2,Descuento Respecto al anterior,Descuento Total");

                    Print(createPrint(10), excel);

                    if (ValorObjetivo >= 25)
                        Print(createPrint(25), excel);

                    for (int i = 0; i * 50 < ValorObjetivo; i++)
                    {
                        Print(createPrint((i + 1) * 50), excel);
                    }

                    excel.Close(filePath);
                    Console.WriteLine($"¡Excel generado exitosamente como '{fileName}.xlsx'!");
                    break; // Salir del bucle después de generar el Excel

                case 0:
                    QRMenu.Exit();
                    break;

                default:
                    QRMenu.ErrorMenu("No deberías de estar aquí. Este es un mensaje de error.\nContacta al administrador.\n", "Código de Error: #OPTMEN421", 421);
                    QRMenu.Exit();
                    break;
            }
        }
        while (QRMenu.ConfirmationMenu($"¿Deseas continuar?"));

        QRMenu.Exit();
    }

    public static void Print(Printable print, xlsxCreator xcl)
    {
        xcl.AddRow($"{print.metros},{print.precioTotal},{print.precioM2},{print.descuento / 100}%,{(15 - print.precioM2) / 15 * 100}%");
    }

    public static Printable createPrint(double metrosCuadrados)
    {
        return new Printable
        {
            metros = SqMtPrice.GetActualPost(metrosCuadrados),
            precioTotal = SqMtPrice.GetTotalPrice(metrosCuadrados),
            precioM2 = SqMtPrice.GetPricePerSquareMeter(metrosCuadrados),
            descuento = SqMtPrice.GetDiscount(metrosCuadrados) * 100
        };
    }
}

struct Printable
{
    public double metros;
    public double precioTotal;
    public double precioM2;
    public double descuento;
}
