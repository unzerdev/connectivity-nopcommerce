using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Unzer.Plugin.Payments.Unzer.Models;

namespace Unzer.Plugin.Payments.Unzer.Validators
{
    /// <summary>
    /// Represents configuration model validator
    /// </summary>
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region Ctor

        public ConfigurationValidator(ILocalizationService localizationService)
        {
            //RuleFor(model => model.GateWayURL)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Unzer.Fields.GateWayURL.Required"));

            //RuleFor(model => model.AutoCapture)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Unzer.Fields.AutoCapture.Required"));

            //RuleFor(model => model.TextOnStatement)
            //    .MaximumLength(22)
            //    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Unzer.Fields.TextOnStatement.NoLongerThen22Chars"))
            //    .Matches("[ -~]+")
            //    .WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payments.Unzer.Fields.TextOnStatement.MustBeReadable"));
        }

        #endregion
    }
}