using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.HtmlEngine;
using DatenMeister.Reports;
using DatenMeister.Reports.Generic;
using DatenMeister.Reports.Html;
using DatenMeister.StundenPlan.Logic;
using DatenMeister.StundenPlan.Model;
using static DatenMeister.StundenPlan.Model._Types;

namespace DatenMeister.StundenPlan.Reporting;

public class HtmlStundenPlan : IGenericReportEvaluator<HtmlReportCreator>
{
    public void Evaluate(ReportLogic reportLogic, HtmlReportCreator reportCreator, IElement reportNode)
    {
        var skipWeekend = reportNode.getOrDefault<bool>(_Report._StundenPlanReportElement.skipWeekend);
        var weeks = reportNode.getOrDefault<int>(_Report._StundenPlanReportElement.weeks);

        var viewNode =
            ReportLogic.GetViewNode(
                reportNode,
                _Report._StundenPlanReportElement.viewNode);

        var dataviewEvaluation = reportLogic.GetDataViewEvaluation();
        var elements = dataviewEvaluation.GetElementsForViewNode(viewNode).OfType<IElement>().ToList(); 

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
                var events = 
                    EventsLogic.GetEventsOnWeekDay(n, w + 1, elements); // +1 to match day numbering (1-7).ToList();

                var list = new HtmlListElement();

                foreach (var eventElement in events)
                {
                    // Assuming eventElement contains necessary information like name and time
                    var eventName = eventElement.getOrDefault<string>(_WeeklyPeriodicEvent.@name);
                    var eventTime = eventElement.getOrDefault<DateTime>(_WeeklyPeriodicEvent.@timeStart);
                    var hoursDuration = eventElement.getOrDefault<double>(_WeeklyPeriodicEvent.hoursDuration);

                    list.Items.Add($"{eventTime:HH:mm}-{eventTime.AddHours(hoursDuration):HH:mm}: {eventName}");
                }

                var cell = new HtmlTableCell(list);
                row.Add(cell);

            }

            table.AddRow(row);
        }

        reportCreator.HtmlReporter.Add(table);
    }

    public bool IsRelevant(IElement element)
    {
        var metaClass = element.getMetaClass();
        return metaClass?.equals(_Report.TheOne.__StundenPlanReportElement) == true;
    }
}