using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Reports;
using DatenMeister.Reports.Generic;
using DatenMeister.Reports.Html;

namespace DatenMeister.StundenPlan.Reporting;

public class HtmlStundenPlan : IGenericReportEvaluator<HtmlReportCreator> 
{
    public bool IsRelevant(IElement element)
    {
        throw new NotImplementedException();
    }

    public void Evaluate(ReportLogic reportLogic, HtmlReportCreator reportCreator, IElement reportNode)
    {
        throw new NotImplementedException();
    }
}