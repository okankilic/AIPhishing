using AIPhishing.Business.Enums;
using AIPhishing.Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class EnumsController : BaseApiController
{
    private readonly IEnumBusiness _enumBusiness;
    
    public EnumsController(
        IHttpContextAccessor httpContextAccessor,
        IEnumBusiness enumBusiness)
        : base(httpContextAccessor)
    {
        _enumBusiness = enumBusiness ?? throw new ArgumentNullException(nameof(enumBusiness));
    }

    // [HttpGet("attack-types")]
    // public IActionResult GetAttackTypes()
    // {
    //     var result = _enumBusiness.GetEnums<AttackTypeEnum>();
    //
    //     return OkApiResult(result);
    // }
}