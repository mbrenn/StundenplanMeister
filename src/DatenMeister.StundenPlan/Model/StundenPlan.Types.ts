// Created by DatenMeister.SourcecodeGenerator.TypeScriptInterfaceGenerator Version 1.0.0.0
export namespace _Types
{
        export class _WeeklyPeriodicEvent
        {
            static _name_ = "name";
            static timeStart = "timeStart";
            static hoursDuration = "hoursDuration";
            static weekInterval = "weekInterval";
            static weekOffset = "weekOffset";
            static onMonday = "onMonday";
            static onTuesday = "onTuesday";
            static onWednesday = "onWednesday";
            static onThursday = "onThursday";
            static onFriday = "onFriday";
            static onSaturday = "onSaturday";
            static onSunday = "onSunday";
        }

        export const __WeeklyPeriodicEvent_Uri = "dm:///types/#954ad359-1a92-4661-bc58-83ea4517d493";
        export class _WeeklyScheduleView
        {
            static _name_ = "name";
            static collectionUri = "collectionUri";
            static weeks = "weeks";
            static skipWeekend = "skipWeekend";
        }

        export const __WeeklyScheduleView_Uri = "dm:///types/#e722b04b-4392-4695-ada4-168397d7edc0";
        export class _ConflictingSchedule
        {
            static firstSchedule = "firstSchedule";
            static secondSchedule = "secondSchedule";
        }

        export const __ConflictingSchedule_Uri = "dm:///types/#79050f11-3a68-4783-98ea-481da6b39728";
}

export namespace _Forms
{
        export class _SchedulerForm
        {
            static _name_ = "name";
            static title = "title";
            static isReadOnly = "isReadOnly";
            static isAutoGenerated = "isAutoGenerated";
            static hideMetaInformation = "hideMetaInformation";
            static originalUri = "originalUri";
            static creationProtocol = "creationProtocol";
        }

        export const __SchedulerForm_Uri = "dm:///types/#9b107f7e-59c0-4164-b761-78764223afa2";
}

export namespace _Report
{
        export class _StundenPlanReportElement
        {
            static viewNode = "viewNode";
            static weeks = "weeks";
            static skipWeekend = "skipWeekend";
        }

        export const __StundenPlanReportElement_Uri = "dm:///types/#fefebd7f-03b5-49c9-9118-491c33f87956";
}

