using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using ValidationException = WebAPI.Exceptions.ValidationException;

namespace WebAPI.Services.Client;

public static class ClientValidationService
{
    private static readonly string PlPhoneNumberPattern = @"^(\+48|48)[0-9]{9}$";

    private static readonly int[] PeselChecksumDigitCoefficients = new[] { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };

    private static readonly int PeselLength = 11;

    public static void ValidateEmail(string email)
    {
        CheckEmptiness(email, "Email number can't be null or empty!");

        try
        {
            new MailAddress(email);
        }
        catch
        {
            throw new ValidationException("Please provide email in correct format");
        }
    }

    public static void ValidatePhoneNumber(string phoneNumber)
    {
        CheckEmptiness(phoneNumber, "Phone number can't be null or empty!");

        if (!Regex.IsMatch(phoneNumber, PlPhoneNumberPattern))
            throw new ValidationException(
                "Please provide Polish phone number in correct format, starting with '+48' sign and including 9 digits");
    }

    public static void ValidatePesel(string pesel)
    {
        CheckEmptiness(pesel, "Pesel can't be null or empty!");

        if (pesel.Length != PeselLength)
            throw new ValidationException("Pesel should consist of 11 letters!");

        int[] peselDigits = pesel
            .Where(char.IsDigit)
            .Select(c => (int)char.GetNumericValue(c))
            .ToArray();

        if (peselDigits.Length != PeselLength)
            throw new ValidationException("Ensure PESEL consist of only from digits!");
        
        int checksum = 0;
        
        for (int i = 0; i < pesel.Length - 1; ++i)
            checksum += peselDigits[i] * PeselChecksumDigitCoefficients[i];
        
        int controlDigit = (10 - (checksum % 10)) % 10;
        if (controlDigit != peselDigits[PeselLength - 1])
            throw new ValidationException("Invalid PESEL");
    }
    
    private static void CheckEmptiness(string str, string errorMessage)
    {
        if (str.IsNullOrEmpty())
        {
            throw new ValidationException(errorMessage);
        }
    }
    
}