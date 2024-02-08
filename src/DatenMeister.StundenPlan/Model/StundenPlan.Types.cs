#nullable enable
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.EMOF.Implementation;

// ReSharper disable RedundantNameQualifier
// Created by DatenMeister.SourcecodeGenerator.ClassTreeGenerator Version 1.2.0.0
namespace DatenMeister.StundenPlan.Model
{
    public class _Types
    {
        public class _WeeklyPeriodicEvent
        {
            public static string @name = "name";
            public IElement? @_name = null;

            public static string @timeStart = "timeStart";
            public IElement? @_timeStart = null;

            public static string @hoursDuration = "hoursDuration";
            public IElement? @_hoursDuration = null;

            public static string @weekInterval = "weekInterval";
            public IElement? @_weekInterval = null;

            public static string @weekOffset = "weekOffset";
            public IElement? @_weekOffset = null;

            public static string @onMonday = "onMonday";
            public IElement? @_onMonday = null;

            public static string @onTuesday = "onTuesday";
            public IElement? @_onTuesday = null;

            public static string @onWednesday = "onWednesday";
            public IElement? @_onWednesday = null;

            public static string @onThursday = "onThursday";
            public IElement? @_onThursday = null;

            public static string @onFriday = "onFriday";
            public IElement? @_onFriday = null;

            public static string @onSaturday = "onSaturday";
            public IElement? @_onSaturday = null;

            public static string @onSunday = "onSunday";
            public IElement? @_onSunday = null;

        }

        public _WeeklyPeriodicEvent @WeeklyPeriodicEvent = new _WeeklyPeriodicEvent();
        public MofObjectShadow @__WeeklyPeriodicEvent = new MofObjectShadow("dm:///types/#954ad359-1a92-4661-bc58-83ea4517d493");

        public class _WeeklyScheduleView
        {
            public static string @name = "name";
            public IElement? @_name = null;

            public static string @collectionUri = "collectionUri";
            public IElement? @_collectionUri = null;

            public static string @weeks = "weeks";
            public IElement? @_weeks = null;

            public static string @skipWeekend = "skipWeekend";
            public IElement? @_skipWeekend = null;

        }

        public _WeeklyScheduleView @WeeklyScheduleView = new _WeeklyScheduleView();
        public MofObjectShadow @__WeeklyScheduleView = new MofObjectShadow("dm:///types/#e722b04b-4392-4695-ada4-168397d7edc0");

        public class _ConflictingSchedule
        {
            public static string @firstSchedule = "firstSchedule";
            public IElement? @_firstSchedule = null;

            public static string @secondSchedule = "secondSchedule";
            public IElement? @_secondSchedule = null;

        }

        public _ConflictingSchedule @ConflictingSchedule = new _ConflictingSchedule();
        public MofObjectShadow @__ConflictingSchedule = new MofObjectShadow("dm:///types/#79050f11-3a68-4783-98ea-481da6b39728");

        public static readonly _Types TheOne = new _Types();

    }

    public class _Forms
    {
        public class _SchedulerForm
        {
            public static string @name = "name";
            public IElement? @_name = null;

            public static string @title = "title";
            public IElement? @_title = null;

            public static string @isReadOnly = "isReadOnly";
            public IElement? @_isReadOnly = null;

            public static string @isAutoGenerated = "isAutoGenerated";
            public IElement? @_isAutoGenerated = null;

            public static string @hideMetaInformation = "hideMetaInformation";
            public IElement? @_hideMetaInformation = null;

            public static string @originalUri = "originalUri";
            public IElement? @_originalUri = null;

            public static string @creationProtocol = "creationProtocol";
            public IElement? @_creationProtocol = null;

        }

        public _SchedulerForm @SchedulerForm = new _SchedulerForm();
        public MofObjectShadow @__SchedulerForm = new MofObjectShadow("dm:///types/#9b107f7e-59c0-4164-b761-78764223afa2");

        public static readonly _Forms TheOne = new _Forms();

    }

    public class _Report
    {
        public class _StundenPlanReportElement
        {
            public static string @viewNode = "viewNode";
            public IElement? @_viewNode = null;

            public static string @weeks = "weeks";
            public IElement? @_weeks = null;

            public static string @skipWeekend = "skipWeekend";
            public IElement? @_skipWeekend = null;

        }

        public _StundenPlanReportElement @StundenPlanReportElement = new _StundenPlanReportElement();
        public MofObjectShadow @__StundenPlanReportElement = new MofObjectShadow("dm:///types/#fefebd7f-03b5-49c9-9118-491c33f87956");

        public class _HtmlConflictReport
        {
            public static string @viewNode = "viewNode";
            public IElement? @_viewNode = null;

        }

        public _HtmlConflictReport @HtmlConflictReport = new _HtmlConflictReport();
        public MofObjectShadow @__HtmlConflictReport = new MofObjectShadow("dm:///types/#fef1fd2f-03b5-49c9-9118-491c33f87956");

        public static readonly _Report TheOne = new _Report();

    }

}
