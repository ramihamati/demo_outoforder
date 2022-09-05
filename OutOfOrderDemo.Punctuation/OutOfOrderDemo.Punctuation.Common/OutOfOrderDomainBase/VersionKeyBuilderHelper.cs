using Microsoft.AspNetCore.WebUtilities;

namespace OutOfOrderDemo.Punctuation.Common;

public abstract class VersionKeyBuilderHelper : ValueObject<string>
{
    public static VersionKey CreateVersionKey(VersionPrefix keyPrefix, Guid id)
    {
        return $"{keyPrefix.Value}-{Minify(id)}";
    }

    private static string Minify(Guid guid)
    {
        return WebEncoders.Base64UrlEncode(
            Convert.FromHexString(
                guid.ToString().Replace("-", "")));
    }
}
