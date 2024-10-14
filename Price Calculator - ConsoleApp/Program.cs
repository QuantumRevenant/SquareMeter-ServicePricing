internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        bool ask = true;

        SqMtPrice.TemporalStart();

        // Preguntar si desean generar Excel o calcular un único valor.
        Console.WriteLine("¿Qué deseas hacer?");
        Console.WriteLine("1. Calcular un valor único");
        Console.WriteLine("2. Generar Excel con la tendencia completa");
        string? opcion = Console.ReadLine();
        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.

        if (opcion == "2")
        {
            ask = false; // Cambiar a modo de generación de Excel
        }

        string? continuar;
        do
        {
            Console.WriteLine("Escribe los metros cuadrados:");
            string? input = Console.ReadLine();
            if (double.TryParse(input, out double numero))
            {
                if (ask)
                {
                    double valorm2 = SqMtPrice.GetPricePerSquareMeter(numero);
                    double valortotal = SqMtPrice.GetTotalPrice(numero);
                    int m2Evaluados = (int)SqMtPrice.GetActualPost(numero);
                    Console.WriteLine($"El costo por el área evaluada ({numero} m2) es: S/. {Math.Round(valortotal, 2)} (S/. {Math.Round(valorm2, 4)}/m2) - se usó el área de {m2Evaluados} m2");
                }
                else
                {
                    // Solicitar nombre del archivo de Excel
                    Console.WriteLine("Introduce el nombre del archivo Excel (sin extensión):");
                    string? fileName = Console.ReadLine();
                    string filePath = $"./{fileName}.xlsx";

                    // Advertir sobre la sobreescritura si el archivo ya existe
                    if (System.IO.File.Exists(filePath))
                    {
                        Console.WriteLine($"Advertencia: El archivo '{fileName}.xlsx' ya existe. ¿Deseas sobreescribirlo? (Y/S para sí, cualquier otra tecla para cancelar)");
                        char sobreescribir = Console.ReadKey().KeyChar;
                        Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
                        if (!(sobreescribir == 'Y' || sobreescribir == 'y' || sobreescribir == 'S' || sobreescribir == 's'))
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
                }
            }
            else
            {
                Console.WriteLine("Debe ser un valor numérico :(");
            }

            // Preguntar si desea continuar
            Console.WriteLine("¿Deseas continuar? (Y/S para sí, cualquier otra tecla para salir)");
            continuar = Console.ReadLine();
            Console.WriteLine(); // Para saltar a la siguiente línea después de la entrada.
        }
        while (continuar == "Y" || continuar == "y" || continuar == "S" || continuar == "s");

        Console.WriteLine("Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }


    public static void Print(Printable print, xlsxCreator xcl)
    {
        xcl.AddRow($"{print.metros},{print.precioTotal},{print.precioM2},{print.descuento / 100}%,{(15 - print.precioM2) / 15 * 100}%");
    }

    public static Printable createPrint(double metrosCuadrados)
    {
        return new Printable { metros = SqMtPrice.GetActualPost(metrosCuadrados), precioTotal = SqMtPrice.GetTotalPrice(metrosCuadrados), precioM2 = SqMtPrice.GetPricePerSquareMeter(metrosCuadrados), descuento = SqMtPrice.GetDiscount(metrosCuadrados) * 100 };
    }
}

struct Printable
{
    public double metros;
    public double precioTotal;
    public double precioM2;
    public double descuento;
}
