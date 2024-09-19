using AIPhishing.Business.Enums;
using AIPhishing.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.Web.Controllers;

public class EnumsController(IEnumBusiness enumBusiness)
    : BaseApiController
{
    private readonly IEnumBusiness _enumBusiness = enumBusiness ?? throw new ArgumentNullException(nameof(enumBusiness));

    [HttpGet("attack-types")]
    public IActionResult GetAttackTypes()
    {
        var result = _enumBusiness.GetEnums<AttackTypeEnum>();

        return OkApiResult(result);
    }
}