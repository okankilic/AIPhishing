using AIPhishing.Common.Extensions;
using AIPhishing.Common.Models;

namespace AIPhishing.Business.Enums;

public class EnumBusiness : IEnumBusiness
{
    public IEnumerable<SelectItemModel<int>> GetEnums<T>() where T : Enum
    {
        return (from T value in Enum.GetValues(typeof(T))
            select new SelectItemModel<int>(value.GetHashCode(), value.GetDescription()))
            .ToList();
    }
}