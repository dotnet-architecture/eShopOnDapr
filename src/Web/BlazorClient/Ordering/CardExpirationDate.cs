namespace Microsoft.eShopOnDapr.BlazorClient.Ordering;

public static class CardExpirationDate
{
    private static readonly Regex Pattern = new(@"(?<month>\d{2})\/(?<year>\d{2})");

    public static DateTime Parse(string value)
    {
        if (TryParse(value, out DateTime result))
        {
            return result;
        }

        return DateTime.MinValue;
    }

    public static ValidationResult Validate(string value, ValidationContext _)
    {
        if (Pattern.IsMatch(value))
        {
            return ValidationResult.Success!;
        }
        return new ValidationResult("Expiration date must be in MM/YY format.");
    }

    private static bool TryParse(string value, out DateTime result)
    {
        var match = Pattern.Match(value);
        if (match.Success)
        {
            var year = int.Parse($"20{match.Groups["year"].Value}");
            var month = int.Parse(match.Groups["month"].Value);

            if (month > 0 && month <= 12)
            {
                var day = DateTime.DaysInMonth(year, month);

                result = new DateTime(year, month, day);
                return true;
            }
        }

        result = DateTime.MinValue;
        return false;
    }
}
