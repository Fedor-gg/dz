using System;
using System.Globalization;
using System.Text;

class Program
{
    static void Main()
    {
        Console.WriteLine("FCorp Machine Calculator\n");
        Console.WriteLine("Как тебя зовут?");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
            name = "незнакомец";

        Console.WriteLine($"Привет, {name}!");
        Console.WriteLine($"Сегодня: {DateTime.Now:dd.MM.yyyy}");
        Console.WriteLine($"В твоём имени {name.Length} символов.");

        
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== МЕНЮ ===");
            Console.WriteLine("1 — Байты в КБ/МБ/ГБ");
            Console.WriteLine("2 — Объём флешки");
            Console.WriteLine("3 — Время скачивания");
            Console.WriteLine("4 — Байты в тексте");
            Console.WriteLine("5 — Видео на накопитель");
            Console.WriteLine("0 — Выход");
            Console.Write("Выберите пункт: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": TaskBytes(); break;
                case "2": TaskFlash(); break;
                case "3": TaskDownload(); break;
                case "4": TaskText(); break;
                case "5": TaskVideo(); break;
                case "0":
                    Console.WriteLine($"До встречи, {name}!");
                    return;
                default:
                    Console.WriteLine("Нет такого пункта. Попробуйте снова.");
                    break;
            }
        }
    }

    
    static void TaskBytes()
    {
        Console.WriteLine("Введите размер в байтах:");
        if (!long.TryParse(Console.ReadLine(), out long bytes) || bytes < 0)
        {
            Console.WriteLine("Ошибка: нужно целое неотрицательное число.");
            return;
        }

        double kb1000 = bytes / 1000.0;
        double mb1000 = bytes / 1000.0 / 1000.0;
        double gb1000 = bytes / 1000.0 / 1000.0 / 1000.0;

        double kib1024 = bytes / 1024.0;
        double mib1024 = bytes / 1024.0 / 1024.0;
        double gib1024 = bytes / 1024.0 / 1024.0 / 1024.0;

        Console.WriteLine();
        Console.WriteLine($"{"Единица",-12}{"По 1000",15}{"По 1024",15}");
        Console.WriteLine(new string('-', 42));
        Console.WriteLine($"{"Килобайты",-12}{kb1000,15:F2}{kib1024,15:F2}");
        Console.WriteLine($"{"Мегабайты",-12}{mb1000,15:F2}{mib1024,15:F2}");
        Console.WriteLine($"{"Гигабайты",-12}{gb1000,15:F2}{gib1024,15:F2}");
    }

    
    static void TaskFlash()
    {
        Console.WriteLine("Введите объём накопителя в ГБ:");
        string input = Console.ReadLine().Replace(',', '.');
        if (!double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double gbBox) || gbBox <= 0)
        {
            Console.WriteLine("Ошибка: нужно положительное число.");
            return;
        }

        double bytesBox = gbBox * 1000.0 * 1000.0 * 1000.0;
        double gbWindows = bytesBox / 1024.0 / 1024.0 / 1024.0;
        double lostGb = gbBox - gbWindows;
        double lostPercent = lostGb / gbBox * 100.0;

        Console.WriteLine();
        Console.WriteLine($"Заявлено:        {gbBox:F2} ГБ");
        Console.WriteLine($"Windows покажет: {gbWindows:F2} ГБ");
        Console.WriteLine($"«Потеря»:        {lostGb:F2} ГБ ({lostPercent:F2} %)");
        Console.WriteLine("Ничего не пропало: производитель считает по 1000, Windows — по 1024.");

        long photoBytes = 4L * 1024 * 1024;
        long totalBytes = (long)bytesBox;
        long photos = totalBytes / photoBytes;
        long remainder = totalBytes % photoBytes;
        double remainderMb = remainder / 1024.0 / 1024.0;

        Console.WriteLine($"Фото по 4 МБ поместится: {photos}");
        Console.WriteLine($"Останется места:        {remainderMb:F2} МБ");
    }

    
    static void TaskDownload()
    {
        Console.WriteLine("Размер файла в ГБ:");
        string s1 = Console.ReadLine().Replace(',', '.');
        if (!double.TryParse(s1, NumberStyles.Any, CultureInfo.InvariantCulture, out double gb) || gb <= 0)
        {
            Console.WriteLine("Ошибка: нужно положительное число.");
            return;
        }

        Console.WriteLine("Скорость в Мбит/с:");
        string s2 = Console.ReadLine().Replace(',', '.');
        if (!double.TryParse(s2, NumberStyles.Any, CultureInfo.InvariantCulture, out double mbit) || mbit <= 0)
        {
            Console.WriteLine("Ошибка: нужно положительное число.");
            return;
        }

        double mbPerSec = mbit / 8.0;
        double fileBytes = gb * 1024.0 * 1024.0 * 1024.0;
        double secondsExact = fileBytes / (mbPerSec * 1024.0 * 1024.0);
        long totalSeconds = (long)Math.Ceiling(secondsExact);

        long hours = totalSeconds / 3600;
        long minutes = (totalSeconds % 3600) / 60;
        long seconds = totalSeconds % 60;

        Console.WriteLine();
        Console.WriteLine($"Скорость: {mbPerSec:F2} МБ/с");
        Console.WriteLine($"Время: {hours} ч {minutes} мин {seconds} с");
    }

    
    static void TaskText()
    {
        Console.WriteLine("Введите строку:");
        string text = Console.ReadLine() ?? "";

        int cyrillic = 0, latin = 0, digits = 0, spaces = 0, punct = 0, other = 0;
        int expected = 0;

        foreach (char c in text)
        {
            bool isCyr = (c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё';
            bool isLat = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

            if (isCyr) { cyrillic++; expected += 2; }
            else if (isLat) { latin++; expected += 1; }
            else if (char.IsDigit(c)) { digits++; expected += 1; }
            else if (char.IsWhiteSpace(c)) { spaces++; expected += 1; }
            else if (char.IsPunctuation(c)) { punct++; expected += 1; }
            else { other++; expected += 1; }
        }

        int real = Encoding.UTF8.GetByteCount(text);

        Console.WriteLine();
        Console.WriteLine($"Символов (char):        {text.Length}");
        Console.WriteLine($"Байт UTF-8 (реально):   {real}");
        Console.WriteLine($"Байт по правилу:        {expected}");
        Console.WriteLine($"Русских: {cyrillic}, латинских: {latin}, цифр: {digits}, пробелов: {spaces}, знаков: {punct}, прочих: {other}");

        if (real == expected)
            Console.WriteLine("Правило подтвердилось.");
        else
            Console.WriteLine($"Расхождение: {real - expected} байт (вероятно, эмодзи или символы вне BMP).");
    }

    
    static void TaskVideo()
    {
        Console.WriteLine("Объём накопителя в ГБ:");
        string s1 = Console.ReadLine().Replace(',', '.');
        if (!double.TryParse(s1, NumberStyles.Any, CultureInfo.InvariantCulture, out double gb) || gb <= 0)
        {
            Console.WriteLine("Ошибка: нужно положительное число.");
            return;
        }

        Console.WriteLine("Битрейт видео в Мбит/с (например, 8 для 1080p, 25 для 4K):");
        string s2 = Console.ReadLine().Replace(',', '.');
        if (!double.TryParse(s2, NumberStyles.Any, CultureInfo.InvariantCulture, out double mbit) || mbit <= 0)
        {
            Console.WriteLine("Ошибка: нужно положительное число.");
            return;
        }

        double totalBytes = gb * 1024.0 * 1024.0 * 1024.0;
        double bitsPerSecond = mbit * 1000.0 * 1000.0;
        double seconds = totalBytes * 8.0 / bitsPerSecond;
        double minutes = seconds / 60.0;

        Console.WriteLine();
        Console.WriteLine($"Поместится примерно {minutes:F1} минут видео ({minutes / 60.0:F2} часов).");
        Console.WriteLine("Источник цифр: типичные битрейты OBS — 1080p ~8 Мбит/с, 4K ~25 Мбит/с.");
    }
}