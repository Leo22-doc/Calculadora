// =============================================================================
//  CALCULADORA UNIVERSAL DE SISTEMAS NUMÉRICOS
// =============================================================================
//
//  RESUMEN:
//  Programa de consola que convierte números entre los sistemas Decimal,
//  Binario, Octal y Hexadecimal. Muestra un menú en bucle: el usuario elige
//  una conversión, escribe el número y obtiene el resultado. Repite hasta
//  que se elige "Salir".
//
//  IMPORTANTE (requisito del profesor):
//  NO usamos funciones "mágicas" como Convert.ToInt64 / Convert.ToString.
//  Todas las conversiones se hacen a mano, mostrando el proceso real:
//    - Decimal -> otra base : DIVISIONES SUCESIVAS guardando los residuos.
//    - Otra base -> Decimal : SUMA de cada dígito por su PESO (base^posición),
//                             implementada con el método de Horner.
//    - Entre bases (bin/oct/hex): se pasa primero a decimal y luego a la
//                             base destino, reutilizando los métodos anteriores.
//    - Suma binaria : suma bit por bit, de derecha a izquierda, con ACARREO.
//
//  Toda la lógica de conversión vive en la clase 'Conversor' (más abajo).
//  Se usa 'long' (entero de 64 bits) para admitir números grandes.
// =============================================================================

using System;

class CalculadoraBases
{
    static void Main()
    {
        Console.WriteLine("=== Calculadora Universal de Sistemas Numéricos ===");

        // Bucle principal: se repite hasta que el usuario elige "Salir".
        while (true)
        {
            // ---- Menú de opciones ----
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
            Console.WriteLine("10. Octal a Decimal");
            Console.WriteLine("11. Suma de números binarios");
            Console.WriteLine("12. Salir");

            // Lee la opción del menú. Si no es un número válido, se vuelve a pedir.
            int opcionSeleccionada;
            while (!int.TryParse(Console.ReadLine(), out opcionSeleccionada))
            {
                Console.Write("Entrada inválida. Escriba un número (1-12): ");
            }

            // Opción 12 = Salir: rompemos el bucle y termina el programa.
            if (opcionSeleccionada == 12)
            {
                Console.WriteLine("¡Gracias por usar la calculadora!");
                break;
            }

            // El try/catch atrapa entradas inválidas (p. ej. un "2" en un binario)
            // sin que el programa se cierre.
            try
            {
                // ---- OPCIÓN OPCIONAL: Suma de dos números binarios ----
                // Se trata aparte porque necesita leer DOS números, no uno.
                if (opcionSeleccionada == 11)
                {
                    Console.Write("Ingrese el primer número binario: ");
                    string primerBinario = (Console.ReadLine() ?? "").Replace(" ", "");
                    Console.Write("Ingrese el segundo número binario: ");
                    string segundoBinario = (Console.ReadLine() ?? "").Replace(" ", "");

                    string sumaEnBinario = Conversor.SumaBinaria(primerBinario, segundoBinario);

                    Console.WriteLine($"Suma (binario): {sumaEnBinario}");
                    Console.WriteLine($"Suma (decimal): {Conversor.BinarioADecimal(sumaEnBinario)}");

                    Console.WriteLine("\n---\n");
                    continue; // volvemos al inicio del bucle (saltamos el switch)
                }

                // ---- Conversiones normales (un solo número de entrada) ----
                Console.Write("Ingrese el número: ");
                string numeroIngresado = (Console.ReadLine() ?? "").Replace(" ", "");

                switch (opcionSeleccionada)
                {
                    case 1: // Binario a Decimal
                        Console.WriteLine($"Decimal: {Conversor.BinarioADecimal(numeroIngresado)}");
                        break;

                    case 2: // Binario a Octal
                        Console.WriteLine($"Octal: {Conversor.BinarioAOctal(numeroIngresado)}");
                        break;

                    case 3: // Binario a Hexadecimal
                        Console.WriteLine($"Hexadecimal: {Conversor.BinarioAHexadecimal(numeroIngresado)}");
                        break;

                    case 4: // Decimal a Binario
                        Console.WriteLine($"Binario: {Conversor.DecimalABinario(long.Parse(numeroIngresado))}");
                        break;

                    case 5: // Decimal a Octal
                        Console.WriteLine($"Octal: {Conversor.DecimalAOctal(long.Parse(numeroIngresado))}");
                        break;

                    case 6: // Decimal a Hexadecimal
                        Console.WriteLine($"Hexadecimal: {Conversor.DecimalAHexadecimal(long.Parse(numeroIngresado))}");
                        break;

                    case 7: // Hexadecimal a Binario
                        Console.WriteLine($"Binario: {Conversor.HexadecimalABinario(numeroIngresado)}");
                        break;

                    case 8: // Hexadecimal a Decimal
                        Console.WriteLine($"Decimal: {Conversor.HexadecimalADecimal(numeroIngresado)}");
                        break;

                    case 9: // Hexadecimal a Octal
                        Console.WriteLine($"Octal: {Conversor.HexadecimalAOctal(numeroIngresado)}");
                        break;

                    case 10: // Octal a Decimal
                        Console.WriteLine($"Decimal: {Conversor.OctalADecimal(numeroIngresado)}");
                        break;

                    default: // Cualquier número fuera de 1-12
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"Error en la conversión: {error.Message}");
            }

            Console.WriteLine("\n---\n"); // separador visual entre operaciones
        }
    }
}


// =============================================================================
//  CLASE Conversor
//  -----------------------------------------------------------------------------
//  Contiene un método por cada conversión. Cada método implementa el ALGORITMO
//  paso a paso (no usa Convert.ToInt64 ni Convert.ToString), para que el
//  proceso de conversión sea totalmente visible.
// =============================================================================
class Conversor
{
    // Dígitos que usa el sistema hexadecimal, en orden de valor 0..15.
    // El índice dentro de esta cadena ES el valor del dígito (posición 10 = 'A').
    private const string DigitosHexadecimales = "0123456789ABCDEF";

    // -------------------------------------------------------------------------
    //  DE DECIMAL HACIA OTRA BASE  ->  método de DIVISIONES SUCESIVAS
    //  Se divide el número entre la base una y otra vez; los RESIDUOS, leídos
    //  de abajo hacia arriba (del último al primero), forman el resultado.
    //  Por eso cada residuo se va agregando AL FRENTE de la cadena.
    // -------------------------------------------------------------------------

    public static string DecimalABinario(long numeroDecimal)
    {
        if (numeroDecimal < 0)
            throw new FormatException("Use solo números no negativos.");
        if (numeroDecimal == 0)
            return "0";

        string resultado = "";
        long valorRestante = numeroDecimal;

        while (valorRestante > 0)
        {
            long residuo = valorRestante % 2;        // dígito binario (0 o 1)
            resultado = residuo + resultado;         // se coloca al frente
            valorRestante = valorRestante / 2;       // se baja a la siguiente división
        }
        return resultado;
    }

    public static string DecimalAOctal(long numeroDecimal)
    {
        if (numeroDecimal < 0)
            throw new FormatException("Use solo números no negativos.");
        if (numeroDecimal == 0)
            return "0";

        string resultado = "";
        long valorRestante = numeroDecimal;

        while (valorRestante > 0)
        {
            long residuo = valorRestante % 8;        // dígito octal (0 a 7)
            resultado = residuo + resultado;
            valorRestante = valorRestante / 8;
        }
        return resultado;
    }

    public static string DecimalAHexadecimal(long numeroDecimal)
    {
        if (numeroDecimal < 0)
            throw new FormatException("Use solo números no negativos.");
        if (numeroDecimal == 0)
            return "0";

        string resultado = "";
        long valorRestante = numeroDecimal;

        while (valorRestante > 0)
        {
            long residuo = valorRestante % 16;       // valor de 0 a 15
            // Convertimos ese valor a su símbolo (10->A, 11->B, ... 15->F)
            // tomando el carácter en esa posición de la tabla.
            char simbolo = DigitosHexadecimales[(int)residuo];
            resultado = simbolo + resultado;
            valorRestante = valorRestante / 16;
        }
        return resultado;
    }

    // -------------------------------------------------------------------------
    //  DE OTRA BASE HACIA DECIMAL  ->  suma de cada dígito por su PESO
    //  Valor = d0*base^0 + d1*base^1 + ... usando el método de Horner:
    //  recorremos de izquierda a derecha y en cada paso hacemos
    //      acumulado = acumulado * base + dígitoActual
    //  que equivale a multiplicar todo lo anterior por la base y sumar el nuevo.
    // -------------------------------------------------------------------------

    public static long BinarioADecimal(string numeroBinario)
    {
        if (numeroBinario.Length == 0)
            throw new FormatException("No se ingresó ningún número binario.");

        long resultado = 0;
        foreach (char digito in numeroBinario)
        {
            if (digito != '0' && digito != '1')
                throw new FormatException($"'{digito}' no es un dígito binario válido.");

            int valorDigito = digito - '0';          // '0'->0, '1'->1
            resultado = resultado * 2 + valorDigito; // peso de base 2
        }
        return resultado;
    }

    public static long OctalADecimal(string numeroOctal)
    {
        if (numeroOctal.Length == 0)
            throw new FormatException("No se ingresó ningún número octal.");

        long resultado = 0;
        foreach (char digito in numeroOctal)
        {
            if (digito < '0' || digito > '7')
                throw new FormatException($"'{digito}' no es un dígito octal válido (0-7).");

            int valorDigito = digito - '0';          // '0'..'7' -> 0..7
            resultado = resultado * 8 + valorDigito; // peso de base 8
        }
        return resultado;
    }

    public static long HexadecimalADecimal(string numeroHexadecimal)
    {
        if (numeroHexadecimal.Length == 0)
            throw new FormatException("No se ingresó ningún número hexadecimal.");

        long resultado = 0;
        foreach (char caracter in numeroHexadecimal)
        {
            int valorDigito = ValorDeDigitoHexadecimal(caracter);
            resultado = resultado * 16 + valorDigito; // peso de base 16
        }
        return resultado;
    }

    // -------------------------------------------------------------------------
    //  CONVERSIONES ENTRE BASES (sin pasar por decimal en la cabeza del usuario,
    //  pero internamente usamos el decimal como "puente"). Reutilizamos los
    //  métodos de arriba, así que el proceso sigue siendo visible.
    // -------------------------------------------------------------------------

    public static string BinarioAOctal(string numeroBinario)
    {
        long valorDecimal = BinarioADecimal(numeroBinario); // paso 1: binario -> decimal
        return DecimalAOctal(valorDecimal);                 // paso 2: decimal -> octal
    }

    public static string BinarioAHexadecimal(string numeroBinario)
    {
        long valorDecimal = BinarioADecimal(numeroBinario);
        return DecimalAHexadecimal(valorDecimal);
    }

    public static string HexadecimalABinario(string numeroHexadecimal)
    {
        long valorDecimal = HexadecimalADecimal(numeroHexadecimal);
        return DecimalABinario(valorDecimal);
    }

    public static string HexadecimalAOctal(string numeroHexadecimal)
    {
        long valorDecimal = HexadecimalADecimal(numeroHexadecimal);
        return DecimalAOctal(valorDecimal);
    }

    // -------------------------------------------------------------------------
    //  SUMA DE NÚMEROS BINARIOS  ->  suma bit por bit con ACARREO
    //  Igual que sumamos a mano en decimal, pero la base es 2: cuando la suma
    //  de una columna llega a 2, se "lleva" 1 a la siguiente columna.
    // -------------------------------------------------------------------------

    public static string SumaBinaria(string primerBinario, string segundoBinario)
    {
        ValidarBinario(primerBinario);
        ValidarBinario(segundoBinario);

        string resultado = "";
        int acarreo = 0;

        // Índices que recorren cada número desde la DERECHA (bit menos significativo).
        int indiceA = primerBinario.Length - 1;
        int indiceB = segundoBinario.Length - 1;

        // Seguimos mientras queden bits en cualquiera de los dos o quede acarreo.
        while (indiceA >= 0 || indiceB >= 0 || acarreo > 0)
        {
            // Si un número ya se acabó, su bit cuenta como 0.
            int bitA = indiceA >= 0 ? primerBinario[indiceA] - '0' : 0;
            int bitB = indiceB >= 0 ? segundoBinario[indiceB] - '0' : 0;

            int sumaColumna = bitA + bitB + acarreo; // puede ser 0, 1, 2 o 3
            resultado = (sumaColumna % 2) + resultado; // el bit que se escribe
            acarreo = sumaColumna / 2;                 // lo que se lleva (0 o 1)

            indiceA--;
            indiceB--;
        }

        return resultado.Length == 0 ? "0" : resultado;
    }

    // -------------------------------------------------------------------------
    //  MÉTODOS AUXILIARES (privados)
    // -------------------------------------------------------------------------

    // Convierte un carácter hexadecimal (0-9, A-F, a-f) a su valor 0..15.
    private static int ValorDeDigitoHexadecimal(char caracter)
    {
        char enMayuscula = char.ToUpper(caracter);
        int valor = DigitosHexadecimales.IndexOf(enMayuscula); // -1 si no existe

        if (valor < 0)
            throw new FormatException($"'{caracter}' no es un dígito hexadecimal válido (0-9, A-F).");

        return valor;
    }

    // Verifica que una cadena contenga solo 0 y 1.
    private static void ValidarBinario(string numeroBinario)
    {
        if (numeroBinario.Length == 0)
            throw new FormatException("No se ingresó ningún número binario.");

        foreach (char digito in numeroBinario)
        {
            if (digito != '0' && digito != '1')
                throw new FormatException($"'{digito}' no es un dígito binario válido.");
        }
    }
}
