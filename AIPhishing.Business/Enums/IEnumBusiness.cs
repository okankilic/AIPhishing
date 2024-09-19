using AIPhishing.Common.Models;

namespace AIPhishing.Business.Enums;

public interface IEnumBusiness
{
    IEnumerable<SelectItemModel<int>> GetEnums<T>() where T : Enum;
}