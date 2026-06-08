using System;

class CalculadoraBases
{
    static void Main()
    {
        Console.WriteLine("=== Calculadora Universal de Sistemas Numéricos ===");

        while (true)
        {
            Console.WriteLine("\nSeleccione la conversión:");
            Console.WriteLine("1. Binario a Decimal");
            Console.WriteLine("2. Binario a Octal");
            Console.WriteLine("3. Binario a Hexadecimal");
            Console.WriteLine("4. Decimal a Binario");
            Console.WriteLine("5. Decimal a Octal");
            Console.WriteLine("6. Decimal a Hexadecimal");
            Console.WriteLine("7. Hexadecimal a Binario");
            Console.WriteLine("8. Hexadecimal a Decimal");
            Console.WriteLine("9. Hexadecimal a Octal");
            Console.WriteLine("10. Salir");

            int opcion;
            while (!int.TryParse(Console.ReadLine(), out opcion))
            {
                Console.Write("Entrada inválida. Escriba un número (1-10): ");
            }

            if (opcion == 10)
            {
                Console.WriteLine("¡Gracias por usar la calculadora!");
                break;
            }

            Console.Write("Ingrese el número: ");
            string entrada = Console.ReadLine();

            try
            {
                switch (opcion)
                {
                    case 1: // Binario a Decimal
                            entrada = entrada.Replace(" ", ""); // quitar espacios
                            long decimalResult = Convert.ToInt64(entrada, 2);
                            Console.WriteLine($"Decimal: {decimalResult}");
                            break;

                    case 2: // Binario a Octal
                           entrada = entrada.Replace(" ", ""); // quitar espacios
                           long binToOct = Convert.ToInt64(entrada, 2); // usar long
                           Console.WriteLine($"Octal: {Convert.ToString(binToOct, 8)}");
                           break;

                   case 3: // Binario a Hexadecimal
                          entrada = entrada.Replace(" ", ""); // quitar espacios
                          long binToHex = Convert.ToInt64(entrada, 2); // usar long
                          Console.WriteLine($"Hexadecimal: {Convert.ToString(binToHex, 16).ToUpper()}");
                          break; 

                    case 4: // Decimal a Binario
                        int decToBin = int.Parse(entrada);
                        Console.WriteLine($"Binario: {Convert.ToString(decToBin, 2)}");
                        break;

                    case 5: // Decimal a Octal
                        int decToOct = int.Parse(entrada);
                        Console.WriteLine($"Octal: {Convert.ToString(decToOct, 8)}");
                        break;

                    case 6: // Decimal a Hexadecimal
                        int decToHex = int.Parse(entrada);
                        Console.WriteLine($"Hexadecimal: {Convert.ToString(decToHex, 16).ToUpper()}");
                        break;

                    case 7: // Hexadecimal a Binario
                        int hexToBin = Convert.ToInt32(entrada, 16);
                        Console.WriteLine($"Binario: {Convert.ToString(hexToBin, 2)}");
                        break;

                    case 8: // Hexadecimal a Decimal
                        Console.WriteLine($"Decimal: {Convert.ToInt32(entrada, 16)}");
                        break;

                    case 9: // Hexadecimal a Octal
                        int hexToOct = Convert.ToInt32(entrada, 16);
                        Console.WriteLine($"Octal: {Convert.ToString(hexToOct, 8)}");
                        break;

                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la conversión: {ex.Message}");
            }

            Console.WriteLine("\n---\n");
        }
    }
}