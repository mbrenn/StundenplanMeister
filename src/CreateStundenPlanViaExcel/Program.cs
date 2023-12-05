﻿// See https://aka.ms/new-console-template for more information

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
    var loadAction = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Actions.__LoadExtentAction);

    loadAction.set(_DatenMeister._Actions._LoadExtentAction.dropExisting, true);
    loadAction.set(_DatenMeister._Actions._LoadExtentAction.name, "Excel Import");

    var configuration = InMemoryObject.CreateEmpty(
        _DatenMeister.TheOne.ExtentLoaderConfigs.__ExcelReferenceLoaderConfig);
    loadAction.set(_DatenMeister._Actions._LoadExtentAction.configuration, configuration);
    configuration.set(
        _DatenMeister._ExtentLoaderConfigs._ExcelLoaderConfig.filePath,
        "C:\\Users\\mbren\\OneDrive\\Dokumente\\Meetings.xlsx");

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
        "dm:///stundenplan");

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

    var reportDefinition = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.__ReportDefinition);
    reportDefinition.set(_DatenMeister._Reports._ReportDefinition.elements, new[]
    {
        reportHeader,
        stundenPlanReportElement
    });

    report.set(_DatenMeister._Reports._HtmlReportInstance.reportDefinition, reportDefinition);

    var reportSource = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Reports.__ReportInstanceSource);
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.name, "Meetings");
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.workspaceId, WorkspaceNames.WorkspaceData);
    reportSource.set(_DatenMeister._Reports._ReportInstanceSource.path, "dm:///stundenplan");


    var reportAction = InMemoryObject.CreateEmpty(_DatenMeister.TheOne.Actions.Reports.__HtmlReportAction);
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.reportInstance, report);
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.filePath,
        "C:\\Users\\mbren\\OneDrive\\Dokumente\\Meetings.html");
    reportAction.set(_DatenMeister._Actions._Reports._HtmlReportAction.name, "The Report");



    report.set(_DatenMeister._Reports._ReportInstance.sources, new[] { reportSource });

    await actionLogic.ExecuteAction(reportAction);
}


Console.WriteLine("And Goodbye!");