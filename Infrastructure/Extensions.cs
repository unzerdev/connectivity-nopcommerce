using Nop.Core.Domain.Common;

namespace Unzer.Plugin.Payments.Unzer.Infrastructure;
public static class Extensions
{
    public static bool IsSameAddress(this Address compareTo, Address isSameAdress)
    {
        var isNotSame = compareTo.FirstName != isSameAdress.FirstName || compareTo.LastName != isSameAdress.LastName ||
            compareTo.Address1 != isSameAdress.Address1 || compareTo.Address2 != isSameAdress.Address2 ||
            compareTo.City != isSameAdress.City || compareTo.ZipPostalCode != isSameAdress.ZipPostalCode ||
            compareTo.CountryId != isSameAdress.CountryId || compareTo.StateProvinceId != isSameAdress.StateProvinceId;

        return !isNotSame;
    }
}
