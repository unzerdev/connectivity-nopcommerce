using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Unzer.Plugin.Payments.Unzer.Components
{
    public class PaymentInfoViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.Unzer/Views/PaymentInfo.cshtml");
        }
    }
}
