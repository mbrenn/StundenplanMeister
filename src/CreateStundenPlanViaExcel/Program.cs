// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Autofac;
using BurnSystems;
using BurnSystems.Logging;
using BurnSystems.Logging.Provider;
using CommandLine;
using CreateStundenPlanViaExcel;
using DatenMeister.Actions;
using DatenMeister.Core;
using DatenMeister.Core.EMOF.Implementation;
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.Core.Models;
using DatenMeister.Core.Provider.InMemory;
using DatenMeister.Core.Runtime.Workspaces;
using DatenMeister.Integration.DotNet;
using DatenMeister.StundenPlan;
using DatenMeister.StundenPlan.Logic;
using DatenMeister.StundenPlan.Model;


var value = Parser.Default.ParseArguments<Options>(args);

var options = value.Value;
var pathInput = options.InputExcel;
var pathResult = options.OutputHtml;

Console.WriteLine("Hello, StundenplanMeister!");

var integrationSettings = new IntegrationSettings
{
    DatabasePath = "./data"
};

TheLog.AddProvider(new ConsoleProvider());

await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);
{
    Console.WriteLine("Loading the excel");

    // First of all, import the excel
    var loadAction = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Actions.__LoadExtentAction);

    loadAction.set(_DatenMeister._Actions._LoadExtentAction.dropExisting, true);
    loadAction.set(_DatenMeister._Actions._LoadExtentAction.name, "Excel Import");

    var configuration = InMemoryObject.CreateEmpty(
        _DatenMeister.TheOne.ExtentLoaderConfigs.__ExcelReferenceLoaderConfig);
    loadAction.set(_DatenMeister._Actions._LoadExtentAction.configuration, configuration);
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.filePath,
        pathInput);

    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.extentUri,
        "dm:///stundenplan");
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.workspaceId,
        WorkspaceNames.WorkspaceData);
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.sheetName,
        "events");
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.hasHeader, true);

    var actionLogic = dm.Resolve<ActionLogic>();
    await actionLogic.ExecuteAction(loadAction);

    var extent = dm.WorkspaceLogic.FindExtent(
        WorkspaceNames.WorkspaceData,
        "dm:///stundenplan") ?? throw new InvalidOperationException("Extent was not found");

    // Fix the excel stuff because the timestamp is given as an integer
    foreach (var element in extent.elements().OfType<IElement>())
    {
        // Sets the metaclass
        (element as MofElement)?.SetMetaClass(_Types.TheOne.__WeeklyPeriodicEvent);

        element.set(_Types._WeeklyPeriodicEvent.timeStart,
            StundenPlanPlugin.ConvertExcelTimeToDateTime(
                element.getOrDefault<double>(_Types._WeeklyPeriodicEvent.timeStart)));
    }

    Console.WriteLine("Identifying Conflicts");
    EventsLogic.AddConflicts(extent.elements(), extent.elements());

    Console.WriteLine("Calculate hours");
    var totalHours = EventsLogic.CalculateEventHoursPerWeek(extent.elements());

    Console.WriteLine("Now create the report");

    Console.WriteLine(extent);

    // Now create the report
    var report = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.__HtmlReportInstance);
    report.set(_DatenMeister._Reports._HtmlReportInstance.name, "The Report");

    var reportHeader = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.Elements.__ReportHeadline);
    reportHeader.set(_DatenMeister._Reports._Elements._ReportHeadline.title, "Your StundenPlan");

    // Attach the datasource
    var dynamicViewNode = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.DataViews.__DynamicSourceNode);
    dynamicViewNode.set(_DatenMeister._DataViews._DynamicSourceNode.name, "Meetings");

    var stundenPlanReportElement = InMemoryObject.CreateEmpty(DatenMeister.StundenPlan.Model._Report.TheOne.__StundenPlanReportElement);
    stundenPlanReportElement.set(DatenMeister.StundenPlan.Model._Report._StundenPlanReportElement.viewNode, dynamicViewNode);
    stundenPlanReportElement.set(DatenMeister.StundenPlan.Model._Report._StundenPlanReportElement.skipWeekend, false);
    stundenPlanReportElement.set(DatenMeister.StundenPlan.Model._Report._StundenPlanReportElement.weeks, 4);

    // Writes the conflicts
    var reportHeaderConflicts = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.Elements.__ReportHeadline);
    reportHeaderConflicts.set(_DatenMeister._Reports._Elements._ReportHeadline.title, "Conflicts");

    var conflictReport = InMemoryObject.CreateEmpty(DatenMeister.StundenPlan.Model._Report.TheOne.__HtmlConflictReport);
    conflictReport.set(DatenMeister.StundenPlan.Model._Report._HtmlConflictReport.viewNode, dynamicViewNode);

    // Writes the statistics
    var hoursTitle = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.Elements.__ReportHeadline);
    hoursTitle.set(_DatenMeister._Reports._Elements._ReportHeadline.title, "Statistics");

    var hoursParagraph = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.Elements.__ReportParagraph);
    hoursParagraph.set(_DatenMeister._Reports._Elements._ReportParagraph.paragraph, $"Meeting hours per week: {totalHours:n2}");

    var reportDefinition = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.__ReportDefinition);
    reportDefinition.set(_DatenMeister._Reports._ReportDefinition.elements, new[]
    {
        reportHeader,
        stundenPlanReportElement,
        reportHeaderConflicts,
        conflictReport,
        hoursTitle,
        hoursParagraph
    });

    report.set(_DatenMeister._Reports._HtmlReportInstance.reportDefinition, reportDefinition);
    var cssStyle =
        ResourceHelper.LoadStringFromAssembly(typeof(DummyClass), "CreateStundenPlanViaExcel.resources.style.css");
    report.set(_DatenMeister._Reports._HtmlReportInstance.cssStyleSheet, cssStyle);

    var reportSource = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.__ReportInstanceSource);
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.name, "Meetings");
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.workspaceId, WorkspaceNames.WorkspaceData);
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.path, "dm:///stundenplan");


    var reportAction = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Actions.Reports.__HtmlReportAction);
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.reportInstance, report);
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.filePath,
        pathResult);
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.name, "The Report");

    report.set(_DatenMeister._Reports._ReportInstance.sources, new[] { reportSource });

    await actionLogic.ExecuteAction(reportAction);
}

Process.Start(new ProcessStartInfo( pathResult) { UseShellExecute = true });

Console.WriteLine("And Goodbye!");
