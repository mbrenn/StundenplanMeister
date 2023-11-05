// See https://aka.ms/new-console-template for more information

using Autofac;
using BurnSystems.Logging;
using BurnSystems.Logging.Provider;
using DatenMeister.Actions;
using DatenMeister.Core;
using DatenMeister.Core.Models;
using DatenMeister.Core.Provider.InMemory;
using DatenMeister.Core.Runtime.Workspaces;
using DatenMeister.Integration.DotNet;

Console.WriteLine("Hello, StundenplanMeister!");

var integrationSettings = new IntegrationSettings
{
    DatabasePath = "./data"
};

TheLog.AddProvider(new ConsoleProvider());

await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);

{
    // First of all, import the excel
    var action = InMemoryObject.CreateEmpty(
        _DatenMeister.TheOne.Actions.__LoadExtentAction);

    action.set(_DatenMeister._Actions._LoadExtentAction.dropExisting, true);
    action.set(_DatenMeister._Actions._LoadExtentAction.name, "Excel Import");

    var configuration = InMemoryObject.CreateEmpty(
        _DatenMeister.TheOne.ExtentLoaderConfigs.__ExcelReferenceLoaderConfig);
    action.set(_DatenMeister._Actions._LoadExtentAction.configuration, configuration);
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.filePath,
        "C:\\Users\\mbren\\OneDrive\\Dokumente\\Meetings.xlsx");

    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.extentUri,
        "dm:///stundenplan");
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.sheetName, 
        "events");
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.hasHeader, true);

    var actionLogic = dm.Resolve<ActionLogic>();
    await actionLogic.ExecuteAction(action);

    var extent = dm.WorkspaceLogic.FindExtent(
        WorkspaceNames.WorkspaceData,
        "dm:///stundenplan");
    
    Console.WriteLine(extent);
}


Console.WriteLine("And Goodbye!");