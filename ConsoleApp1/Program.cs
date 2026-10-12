using System;
using System.Text;
using System.Security.Cryptography;

Console.OutputEncoding = Encoding.UTF8;

const double Speed = 10_000_000_000.0;
const double UniverseYears = 13_800_000_000.0;

string[] popular = {
    "123456", "password", "12345678", "qwerty", "123456789",
    "12345", "1234", "111111", "1234567", "dragon",
    "123123", "baseball", "abc123", "football", "monkey",
    "letmein", "696969", "shadow", "master", "666666",
    "qwerty123", "1q2w3e4r", "admin", "welcome", "login"
};

string[] words = {
    "кот", "пёс", "дом", "лес", "река", "гора", "небо", "звезда", "луна", "солнце",
    "ветер", "дождь", "снег", "лёд", "огонь", "земля", "камень", "песок", "море", "океан",
    "птица", "рыба", "зверь", "цветок", "дерево", "трава", "лист", "корень", "плод", "семя",
    "хлеб", "молоко", "мясо", "сыр", "масло", "соль", "сахар", "мёд", "чай", "кофе",
    "стол", "стул", "окно", "дверь", "стена", "пол", "потолок", "крыша", "лестница", "ключ",
    "книга", "ручка", "тетрадь", "стекло", "бумага", "карта", "письмо", "слово", "буква", "цифра",
    "город", "село", "улица", "дорога", "мост", "поезд", "машина", "самолёт", "корабль", "лодка",
    "врач", "учитель", "инженер", "пилот", "шофёр", "повар", "актёр", "певец", "художник", "поэт",
    "утро", "день", "вечер", "ночь", "час", "минута", "секунда", "год", "месяц", "неделя",
    "красный", "синий", "зелёный", "жёлтый", "белый", "чёрный", "серый", "розовый", "голубой", "сиреневый"
};

bool exit = false;

while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("=== Пароли ===");
    Console.WriteLine("1. Проверить пароль");
    Console.WriteLine("2. Сгенерировать пароль");
    Console.WriteLine("3. Сгенерировать фразу");
    Console.WriteLine("0. Выход");
    Console.Write("Выберите пункт: ");

    string choice = (Console.ReadLine() ?? "").Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1": CheckPassword(); break;
        case "2": GeneratePassword(); break;
        case "3": GeneratePhrase(); break;
        case "0": exit = true; Console.WriteLine("До свидания!"); break;
        default: Console.WriteLine("Неверный пункт."); break;
    }
}

int AlphabetSize(string p)
{
    bool lower = false, upper = false, digit = false, other = false, nonAscii = false;

    foreach (char c in p)
    {
        if (c >= 'a' && c <= 'z') lower = true;
        else if (c >= 'A' && c <= 'Z') upper = true;
        else if (c >= '0' && c <= '9') digit = true;
        else if (c >= ' ' && c <= '~') other = true;
        else nonAscii = true;
    }

    int n = 0;
    if (lower) n += 26;
    if (upper) n += 26;
    if (digit) n += 10;
    if (other) n += 33;
    if (nonAscii) n += 66;
    return n;
}

double StrengthBits(string p)
{
    int n = AlphabetSize(p);
    if (n == 0 || p.Length == 0) return 0;
    return p.Length * Math.Log2(n);
}

string Rating(double bits)
{
    if (bits < 28) return "очень слабый";
    if (bits < 36) return "слабый";
    if (bits < 60) return "средний";
    if (bits < 128) return "сильный";
    return "очень сильный";
}

double CrackSeconds(double bits)
{
    double variants = Math.Pow(2, bits);
    return variants / 2.0 / Speed;
}

string HumanTime(double seconds)
{
    if (seconds < 1) return "меньше секунды";
    if (seconds < 60) return $"{seconds:F1} секунд";
    if (seconds < 3600) return $"{seconds / 60:F1} минут";
    if (seconds < 86400) return $"{seconds / 3600:F1} часов";
    if (seconds < 31_536_000) return $"{seconds / 86400:F1} дней";
    if (seconds < 31_536_000_000.0) return $"{seconds / 31_536_000:F1} лет";
    if (seconds < 31_536_000_000_000.0) return $"{seconds / 31_536_000_000.0:F1} тысяч лет";
    if (seconds < 31_536_000_000_000_000.0) return $"{seconds / 31_536_000_000_000.0:F1} миллионов лет";
    return $"{seconds / 31_536_000_000_000_000.0:F1} миллиардов лет";
}

bool InDictionary(string p)
{
    string lower = p.ToLower();
    foreach (string bad in popular)
        if (lower == bad) return true;
    return false;
}

bool HasPattern(string p)
{
    string lower = p.ToLower();

    bool same = true;
    for (int i = 1; i < p.Length; i++)
        if (p[i] != p[0]) { same = false; break; }
    if (same && p.Length >= 3) return true;

    for (int i = 0; i < lower.Length - 4; i++)
    {
        bool asc = true;
        for (int j = 1; j < 5; j++)
            if (lower[i + j] != lower[i] + j) { asc = false; break; }
        if (asc) return true;
    }

    foreach (string word in words)
    {
        string w = word.ToLower();
        if (lower.StartsWith(w) && lower.Length > w.Length)
        {
            bool restDigits = true;
            for (int i = w.Length; i < lower.Length; i++)
                if (!char.IsDigit(lower[i])) { restDigits = false; break; }
            if (restDigits) return true;
        }
    }

    return false;
}

void CheckPassword()
{
    Console.Write("Введите пароль: ");
    string p = Console.ReadLine() ?? "";

    if (p.Length == 0)
    {
        Console.WriteLine("Пустой пароль.");
        return;
    }

    Console.WriteLine($"Длина: {p.Length}");
    Console.WriteLine($"Алфавит: {AlphabetSize(p)}");

    if (InDictionary(p))
    {
        Console.WriteLine("Пароль в списке популярных — подбирается мгновенно.");
        return;
    }

    double bits = StrengthBits(p);
    if (HasPattern(p))
    {
        bits = Math.Min(bits, 25);
        Console.WriteLine("Пароль построен по шаблону — оценка понижена.");
    }

    Console.WriteLine($"Стойкость: {bits:F1} бит");
    Console.WriteLine($"Оценка: {Rating(bits)}");

    double sec = CrackSeconds(bits);
    Console.WriteLine($"Среднее время перебора: {HumanTime(sec)}");

    if (sec > UniverseYears * 31_536_000.0)
        Console.WriteLine("Это больше возраста Вселенной.");
}

void GeneratePassword()
{
    Console.Write("Длина пароля (8–64): ");
    if (!int.TryParse(Console.ReadLine(), out int len) || len < 8 || len > 64)
    {
        Console.WriteLine("Неверная длина.");
        return;
    }

    Console.Write("Строчные латинские? (д/н): ");
    bool lower = (Console.ReadLine() ?? "").Trim().ToLower() == "д";
    Console.Write("Заглавные латинские? (д/н): ");
    bool upper = (Console.ReadLine() ?? "").Trim().ToLower() == "д";
    Console.Write("Цифры? (д/н): ");
    bool digit = (Console.ReadLine() ?? "").Trim().ToLower() == "д";
    Console.Write("Спецсимволы? (д/н): ");
    bool other = (Console.ReadLine() ?? "").Trim().ToLower() == "д";

    if (!lower && !upper && !digit && !other)
    {
        Console.WriteLine("Нужно выбрать хотя бы одну группу.");
        return;
    }

    string pool = "";
    if (lower) pool += "abcdefghijklmnopqrstuvwxyz";
    if (upper) pool += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    if (digit) pool += "0123456789";
    if (other) pool += "!@#$%^&*()-_=+[]{};:,.<>?/";

    char[] pass = new char[len];
    int pos = 0;

    if (lower) pass[pos++] = "abcdefghijklmnopqrstuvwxyz"[RandomNumberGenerator.GetInt32(26)];
    if (upper) pass[pos++] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[RandomNumberGenerator.GetInt32(26)];
    if (digit) pass[pos++] = "0123456789"[RandomNumberGenerator.GetInt32(10)];
    if (other) pass[pos++] = "!@#$%^&*()-_=+[]{};:,.<>?/"[RandomNumberGenerator.GetInt32(26)];

    while (pos < len)
        pass[pos++] = pool[RandomNumberGenerator.GetInt32(pool.Length)];

    for (int i = pass.Length - 1; i > 0; i--)
    {
        int j = RandomNumberGenerator.GetInt32(i + 1);
        (pass[i], pass[j]) = (pass[j], pass[i]);
    }

    string result = new string(pass);
    Console.WriteLine($"Пароль: {result}");

    double bits = StrengthBits(result);
    Console.WriteLine($"Стойкость: {bits:F1} бит");
    Console.WriteLine($"Оценка: {Rating(bits)}");
    Console.WriteLine($"Время перебора: {HumanTime(CrackSeconds(bits))}");
}

void GeneratePhrase()
{
    Console.Write($"Сколько слов (2–10)? Всего в словаре {words.Length}. ");
    if (!int.TryParse(Console.ReadLine(), out int count) || count < 2 || count > 10)
    {
        Console.WriteLine("Неверное число слов.");
        return;
    }

    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < count; i++)
    {
        if (i > 0) sb.Append('-');
        sb.Append(words[RandomNumberGenerator.GetInt32(words.Length)]);
    }

    string phrase = sb.ToString();
    double bits = count * Math.Log2(words.Length);

    Console.WriteLine($"Фраза: {phrase}");
    Console.WriteLine($"Стойкость: {bits:F1} бит");
    Console.WriteLine($"Оценка: {Rating(bits)}");
    Console.WriteLine($"Время перебора: {HumanTime(CrackSeconds(bits))}");

    int needed = (int)Math.Ceiling(71.5 / Math.Log2(words.Length));
    Console.WriteLine($"Для 71,5 бит нужно слов из этого словаря: {needed}");
    Console.WriteLine($"По Diceware (7776 слов) для 71,5 бит нужно: {(int)Math.Ceiling(71.5 / Math.Log2(7776))}");
}