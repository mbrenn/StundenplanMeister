using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Functions.Queries;
using DatenMeister.Core.Helper;
using DatenMeister.HtmlEngine;
using DatenMeister.Reports;
using DatenMeister.Reports.Generic;
using DatenMeister.Reports.Html;
using DatenMeister.StundenPlan.Logic;
using DatenMeister.StundenPlan.Model;

namespace DatenMeister.StundenPlan.Reporting
{
    internal class HtmlConflictReport : IGenericReportEvaluator<HtmlReportCreator>
    {
        public void Evaluate(ReportLogic reportLogic, HtmlReportCreator reportCreator, IElement reportNode)
        {
            var viewNode =
            ReportLogic.GetViewNode(
                reportNode,
                _Report._StundenPlanReportElement.viewNode);

            var dataviewEvaluation = reportLogic.GetDataViewEvaluation();
            var conflicts = dataviewEvaluation.GetElementsForViewNode(viewNode).WhenMetaClassIs(_Types.TheOne.__ConflictingSchedule)
                .OfType<IElement>().ToList();

            if (conflicts.Count == 0)
            {
                reportCreator.HtmlReporter.Add(
                    new HtmlParagraph("No conflicts in meetings"));
            }
            else
            {
                var list = new HtmlListElement();
                foreach (var conflict in conflicts)
                {
                    var event1Name = conflict
                        .getOrDefault<IElement>(_Types._ConflictingSchedule.firstSchedule)
                        .getOrDefault<string>(_Types._WeeklyPeriodicEvent.name);
                    var event2Name = conflict
                        .getOrDefault<IElement>(_Types._ConflictingSchedule.secondSchedule)
                        .getOrDefault<string>(_Types._WeeklyPeriodicEvent.name);
                    var text = $"\"{event1Name}\" conflicts with \"{event2Name}\"";
                    list.Items.Add(text);
                }

                reportCreator.HtmlReporter.Add(list);
            }
        }

        public bool IsRelevant(IElement element)
        {
            var metaClass = element.getMetaClass();
            return metaClass?.equals(_Report.TheOne.__HtmlConflictReport) == true;
        }
    }
}
