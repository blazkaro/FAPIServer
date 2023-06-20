namespace FAPIServer.Validation.Contexts;

public class DPoPValidationParameters
{
    public DPoPValidationParameters(Uri validHtu, string validHtm)
    {
        if (string.IsNullOrEmpty(validHtm))
            throw new ArgumentException($"'{nameof(validHtm)}' cannot be null or empty.", nameof(validHtm));

        ValidHtu = validHtu ?? throw new ArgumentNullException(nameof(validHtu));
        ValidHtm = validHtm;
    }

    public DPoPValidationParameters(Uri validHtu, string validHtm, byte[]? validPkh, byte[]? validAth)
        : this(validHtu, validHtm)
    {
        ValidPkh = validPkh;
        ValidAth = validAth;
    }

    public Uri ValidHtu { get; set; }
    public string ValidHtm { get; set; }
    public byte[]? ValidPkh { get; set; }
    public byte[]? ValidAth { get; set; }
}
