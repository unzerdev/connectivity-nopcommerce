﻿using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Unzer.Plugin.Payments.Unzer.Services;
public class UnzerPaymentPluginManager : PaymentPluginManager
{
    protected readonly PaymentSettings _paymentSettings;
    protected readonly ISettingService _settingService;
    protected readonly IPluginService _pluginService;
    protected readonly ILocalizationService _localizationService;
    protected UnzerPaymentSettings _unzerPaymentSettings;

    protected readonly Dictionary<string, IList<IPaymentMethod>> _plugins = new();
    public UnzerPaymentPluginManager(ICustomerService customerService, IPluginService pluginService, ISettingService settingService, PaymentSettings paymentSettings, UnzerPaymentSettings unzerPaymentSettings, ILocalizationService localizationService) : base(customerService, pluginService, settingService, paymentSettings)
    {
        _paymentSettings = paymentSettings;
        _pluginService = pluginService;
        _settingService = settingService;
        _localizationService = localizationService;
        _unzerPaymentSettings = unzerPaymentSettings;
    }

    public async override Task<IList<IPaymentMethod>> LoadActivePluginsAsync(Customer customer = null, int storeId = 0, int countryId = 0)
    {
        var activePlugins = await base.LoadActivePluginsAsync(_paymentSettings.ActivePaymentMethodSystemNames, customer, storeId);

        //filter by country
        if (countryId > 0)
            activePlugins = await activePlugins.WhereAwait(async method => !(await GetRestrictedCountryIdsAsync(method)).Contains(countryId)).ToListAsync();

        var unzerPlugin = activePlugins.SingleOrDefault(p => p.PluginDescriptor.SystemName.Contains(UnzerPaymentDefaults.SystemName));
        if (unzerPlugin != null)
        {
            if (_unzerPaymentSettings.SelectedPaymentTypes.Count() > 1)
            {
                var systemName = unzerPlugin.PluginDescriptor.SystemName;
                activePlugins.Remove(unzerPlugin);

                foreach (var paymentMethod in _unzerPaymentSettings.SelectedPaymentTypes)
                {
                    if (activePlugins.Any(p => p.PluginDescriptor.SystemName.Contains(paymentMethod)))
                        continue;

                    var friendlyName = UnzerPaymentDefaults.MapPaymentTypeName(paymentMethod);

                    var curDescriptor = ClonePluginDescriptor(unzerPlugin.PluginDescriptor);

                    curDescriptor.SystemName = $"{systemName}.{paymentMethod}";
                    curDescriptor.FriendlyName = friendlyName;

                    var methDesc = UnzerPaymentDefaults.PaymentMethodDescription;
                    curDescriptor.Description = string.Format(methDesc, friendlyName);

                    var qpPaymentClone = curDescriptor.Instance<IPaymentMethod>();

                    activePlugins.Add(qpPaymentClone);

                    var key = await GetKeyAsync(customer, storeId);
                    if (_plugins.ContainsKey(key))
                    {
                        _plugins[key].Add(qpPaymentClone);
                    }
                }
            }
        }

        return activePlugins;
    }

    public override async Task<IPaymentMethod> LoadPluginBySystemNameAsync(string systemName, Customer customer = null, int storeId = 0)
    {
        if (string.IsNullOrEmpty(systemName))
            return null;

        if (!systemName.Contains(UnzerPaymentDefaults.SystemName))
            return await base.LoadPluginBySystemNameAsync(systemName, customer, storeId);

        //try to get already loaded plugin
        var key = await GetKeyAsync(customer, storeId, systemName);
        if (_plugins.ContainsKey(key))
            return _plugins[key].FirstOrDefault();

        var qpSystemName = UnzerPaymentDefaults.SystemName;

        //or get it from list of all loaded plugins or load it for the first time
        var pluginBySystemName = _plugins.TryGetValue(await GetKeyAsync(customer, storeId), out var plugins)
            && plugins.FirstOrDefault(plugin =>
                plugin.PluginDescriptor.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase)) is IPaymentMethod loadedPlugin
            ? loadedPlugin
            : (await _pluginService.GetPluginDescriptorBySystemNameAsync<IPaymentMethod>(qpSystemName, customer: customer, storeId: storeId))?.Instance<IPaymentMethod>();

        if (pluginBySystemName.PluginDescriptor.SystemName.Equals(systemName))
        {
            _plugins.Add(key, new List<IPaymentMethod> { pluginBySystemName });
            return pluginBySystemName;
        }

        var curDescriptor = ClonePluginDescriptor(pluginBySystemName.PluginDescriptor);
        var payMeth = systemName.Split('.').Last();
        var friendlyName = UnzerPaymentDefaults.MapPaymentTypeName(payMeth); ;
        curDescriptor.SystemName = systemName;
        curDescriptor.FriendlyName = friendlyName;

        var qpPaymentClone = curDescriptor.Instance<IPaymentMethod>();

        _plugins.Add(key, new List<IPaymentMethod> { qpPaymentClone });

        return qpPaymentClone;
    }

    public override bool IsPluginActive(IPaymentMethod paymentMethod)
    {
        if (paymentMethod.PluginDescriptor.SystemName.Contains(UnzerPaymentDefaults.SystemName))
        {
            return _paymentSettings.ActivePaymentMethodSystemNames
                ?.Any(systemName => UnzerPaymentDefaults.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase))
                ?? false;
        }

        return base.IsPluginActive(paymentMethod);
    }

    public override async Task<bool> IsPluginActiveAsync(string systemName, Customer customer = null, int storeId = 0)
    {
        var paymentMethod = await LoadPluginBySystemNameAsync(systemName, customer, storeId);
        return IsPluginActive(paymentMethod);

    }

    public override async Task<IList<int>> GetRestrictedCountryIdsAsync(IPaymentMethod paymentMethod)
    {
        ArgumentNullException.ThrowIfNull(paymentMethod);

        if (paymentMethod.PluginDescriptor.SystemName.Contains(UnzerPaymentDefaults.SystemName))
        {
            var settingKey = string.Format(NopPaymentDefaults.RestrictedCountriesSettingName, UnzerPaymentDefaults.SystemName);

            return await _settingService.GetSettingByKeyAsync<List<int>>(settingKey) ?? new List<int>();
        }

        return await base.GetRestrictedCountryIdsAsync(paymentMethod);
    }

    public async override Task<string> GetPluginLogoUrlAsync(IPaymentMethod meth)
    {
        var logoUrl = await base.GetPluginLogoUrlAsync(meth);

        if (!meth.PluginDescriptor.SystemName.Contains(UnzerPaymentDefaults.SystemName))
            return logoUrl;

        var methName = meth.PluginDescriptor.SystemName.Split(".");
        if (methName.Count() > 2)
        {
            var subName = methName.Last();
            logoUrl = logoUrl.Replace("logo.jpg", $"{subName}.png");
        }

        return logoUrl;
    }

    private PluginDescriptor ClonePluginDescriptor(PluginDescriptor toClone)
    {
        return new PluginDescriptor
        {
            AssemblyFileName = toClone.AssemblyFileName,
            Author = toClone.Author,
            DependsOn = toClone.DependsOn,
            Description = toClone.Description,
            DisplayOrder = toClone.DisplayOrder,
            FriendlyName = toClone.FriendlyName,
            Group = toClone.Group,
            Installed = toClone.Installed,
            LimitedToCustomerRoles = toClone.LimitedToCustomerRoles,
            SupportedVersions = toClone.SupportedVersions,
            OriginalAssemblyFile = toClone.OriginalAssemblyFile,
            LimitedToStores = toClone.LimitedToStores,
            PluginFiles = toClone.PluginFiles,
            PluginType = toClone.PluginType,
            ReferencedAssembly = toClone.ReferencedAssembly,
            ShowInPluginsList = toClone.ShowInPluginsList,
            SystemName = toClone.SystemName,
            Version = toClone.Version
        };
    }
}
