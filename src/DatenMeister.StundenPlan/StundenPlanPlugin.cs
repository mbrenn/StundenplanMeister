using DatenMeister.Core;
using DatenMeister.Core.Runtime.Workspaces;
using DatenMeister.Extent.Manager;
using DatenMeister.Extent.Manager.Extents.Configuration;
using DatenMeister.Extent.Manager.ExtentStorage;
using DatenMeister.Plugins;
using DatenMeister.StundenPlan.Model;
using DatenMeister.WebServer.Library.PageRegistration;

namespace DatenMeister.StundenPlan;

public class StundenPlanPlugin : IDatenMeisterPlugin
{
    private readonly IWorkspaceLogic _workspaceLogic;
    private readonly IScopeStorage _scopeStorage;
    
    private const string ExtentTypeName = "StundenPlan";
    
    public const string ViewModeName = "StundenPlan";

    public const string UriStundenPlanForm = "dm:///forms.stundenplan.datenmeister/";
    public const string UriStundenPlanTypes = "dm:///types.stundenplan.datenmeister/";

    public StundenPlanPlugin(IWorkspaceLogic workspaceLogic, IScopeStorage scopeStorage)
    {
        _workspaceLogic = workspaceLogic;
        _scopeStorage = scopeStorage;
    }
    
    public void Start(PluginLoadingPosition position)
    {
        // Adds the types and the forms
        var extentManager = new ExtentManager(_workspaceLogic, _scopeStorage);
        extentManager.CreateExtentByResource(
            typeof(StundenPlanPlugin),
            "DatenMeister.StundenPlan.xmi.StundenPlan.Forms.xml",
            UriStundenPlanForm,
            "Management");
        
        extentManager.CreateExtentByResource(
            typeof(StundenPlanPlugin),
            "DatenMeister.StundenPlan.xmi.StundenPlan.Types.xml",
            UriStundenPlanTypes,
            "Types");

        // Adds the extent type
        var extentSettings = _scopeStorage.Get<ExtentSettings>();
        var extentSetting =
            new ExtentType(ExtentTypeName);
        extentSetting.rootElementMetaClasses.Add(_Types.TheOne.__WeeklyPeriodicEvent);
        
        extentSettings.extentTypeSettings.Add(extentSetting);

        // Adds the javascript
        var pluginLogic = new PageRegistrationLogic(_scopeStorage.Get<PageRegistrationData>());
        pluginLogic.AddJavaScriptFromResource(
            typeof(StundenPlanPlugin),
            "DatenMeister.StundenPlan.resources.stundenplan.js",
            "stundenplan.js",
            "../../Apps/Datenmeister.StundenPlan/resources/stundenPlan.js");
        pluginLogic.AddJavaScriptFromResource(
            typeof(StundenPlanPlugin),
            "DatenMeister.StundenPlan.resources.DatenMeister.StundenPlan.js",
            "DatenMeister.StundenPlan.js",
            "../../Apps/Datenmeister.StundenPlan/resources/DatenMeister.StundenPlan.js",
            RegistrationType.OnlyEmbedding);

        pluginLogic.AddCssFileFromResource(
            typeof(StundenPlanPlugin),
            "DatenMeister.StundenPlan.resources.stundenplan.css",
            "stundenplan.css",
            "../../Apps/Datenmeister.StundenPlan/resources/stundenplan.css");
    }
}