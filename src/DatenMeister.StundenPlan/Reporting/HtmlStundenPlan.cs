using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.HtmlEngine;
using DatenMeister.Reports;
using DatenMeister.Reports.Generic;
using DatenMeister.Reports.Html;
using DatenMeister.StundenPlan.Model;

namespace DatenMeister.StundenPlan.Reporting;

public class HtmlStundenPlan : IGenericReportEvaluator<HtmlReportCreator> 
{
    public bool IsRelevant(IElement element)
    {

        var metaClass = element.getMetaClass();
        return metaClass?.equals(_Report.TheOne.__StundenPlanReportElement) == true;
    }

    public void Evaluate(ReportLogic reportLogic, HtmlReportCreator reportCreator, IElement reportNode)
    {
        reportCreator.HtmlReporter.Add(
            new HtmlEngine.HtmlHeadline("We are having a headline", 1));
        var skipWeekend = reportNode.getOrDefault<bool>(_Report._StundenPlanReportElement.skipWeekend);
        var weeks = reportNode.getOrDefault<int>(_Report._StundenPlanReportElement.weeks);

        var viewNode =
            ReportLogic.GetViewNode(
                reportNode,
                _Report._StundenPlanReportElement.viewNode);
        
        var dataviewEvaluation = reportLogic.GetDataViewEvaluation();
        var elements = dataviewEvaluation.GetElementsForViewNode(viewNode);

        // Ok, now we create the table
        var table = new HtmlTable();

        // The week rows
        var weekRow = new HtmlTableRow();
        weekRow.Add(new HtmlTableCell("Monday"));
        weekRow.Add(new HtmlTableCell("Tuesday"));
        weekRow.Add(new HtmlTableCell("Wednesday"));
        weekRow.Add(new HtmlTableCell("Thursday"));
        weekRow.Add(new HtmlTableCell("Friday"));
        if (!skipWeekend)
        {
            weekRow.Add(new HtmlTableCell("Saturday"));
            weekRow.Add(new HtmlTableCell("Sunday"));
        }

        table.AddRow(weekRow);

        // The weeks
        for (var n = 0; n < weeks; n++)
        {
            var row = new HtmlTableRow();
            for (var w = 0; w < (skipWeekend ? 5 : 7); w++)
            {
                var cell = new HtmlTableCell("-");
                row.Add(cell);
            }

            table.AddRow(row);
        }

        reportCreator.HtmlReporter.Add(table);
    }
}