import * as Mof from '/js/datenmeister/Mof.js'
import {ObjectType} from '/js/datenmeister/Mof.js'
import * as FormFactory from '/js/datenmeister/forms/FormFactory.js'
import * as StundenPlanTypes from './DatenMeister.StundenPlan.js';
import {FormType, IObjectFormElement} from '/js/datenmeister/forms/Interfaces.js';
import {IFormConfiguration} from '/js/datenmeister/forms/IFormConfiguration.js';
import * as ClientItems from '/js/datenmeister/client/Items.js';
import * as Navigator from '/js/datenmeister/Navigator.js';
import {_DatenMeister} from '/js/datenmeister/models/DatenMeister.class.js';

export function init() {
    FormFactory.registerObjectForm(
        StundenPlanTypes._Forms.__SchedulerForm_Uri,
        () => {
            return new StundenPlanForm();
        }
    );

    /**
     * Add the tests, only if the test framework is loaded
     */
    if (typeof describe !== 'function') return;

    describe('StundenPlanMeister Tests', function () {
        it('Create Empty Table', function () {
            const container = $('#test_area');
            const table = new WeeklyCalenderControl(container);
            table.createTable(
                {
                    weeks: 4
                }
            );
        });

        it('Test Event Manager', function () {
            const event1 = new Mof.DmObject();
            event1.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.onTuesday, true);
            event1.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, "09:00");
            event1.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval, 1);
            event1.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset, 0);
            event1.set(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, "Event1");

            const event2 = new Mof.DmObject();
            event2.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.onWednesday, true);
            event2.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, "09:00");
            event2.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval, 2);
            event2.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset, 0);
            event2.set(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, "Event2");

            const event3 = new Mof.DmObject();
            event3.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.onWednesday, true);
            event3.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, "09:00");
            event3.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval, 2);
            event3.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset, 1);
            event3.set(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, "Event3");

            const event4 = new Mof.DmObject();
            event4.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.onThursday, true);
            event4.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, "09:00");
            event4.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval, 1);
            event4.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset, 0);
            event4.set(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, "Event4");

            const event5 = new Mof.DmObject();
            event5.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.onThursday, true);
            event5.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, "08:00");
            event5.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval, 1);
            event5.set(StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset, 0);
            event5.set(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, "Event5");

            const manager = new PeriodicEventManager(
                [event1, event2, event3, event4, event5]
            );

            // Check single Tuesday
            const tuesdayFirstWeek = manager.getEventsOnWeekday(0, 2);
            chai.assert(tuesdayFirstWeek.length === 1, "Tuesday first length");
            chai.assert(
                tuesdayFirstWeek[0].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event1",
                "Event1 not on first Tuesday");

            const tuesdaySecondWeek = manager.getEventsOnWeekday(1, 2);
            chai.assert(tuesdaySecondWeek.length === 1, "Tuesday second week");
            chai.assert(
                tuesdaySecondWeek[0].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event1",
                "Event1 not on second tuesday");

            // Check odd and even weeks
            const wednesdayFirstWeek = manager.getEventsOnWeekday(0, 3);
            chai.assert(wednesdayFirstWeek.length === 1,
                "Wednesday first week length");
            chai.assert(
                wednesdayFirstWeek[0].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event2",
                "Event2 not on first Wednesday");

            const wednesdaySecondWeek = manager.getEventsOnWeekday(1, 3);
            chai.assert(wednesdaySecondWeek.length === 1,
                "Wednesday second week length");
            chai.assert(
                wednesdaySecondWeek[0].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event3",
                "Event3 not on second Wednesday");

            // Check Ordering
            const thursdayFirstWeek = manager.getEventsOnWeekday(0, 4);
            chai.assert(thursdayFirstWeek.length === 2,
                "Thursday first week");
            chai.assert(
                thursdayFirstWeek[0].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event5",
                "Event5 as first Thursday event");
            chai.assert(
                thursdayFirstWeek[1].get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String) === "Event4",
                "Event4 as second Thursday event");
        });
    });
}

class StundenPlanForm implements IObjectFormElement {

    element: Mof.DmObject;

    workspace: string;
    extentUri: string;
    itemUrl: string;
    formElement: Mof.DmObject;
    formType: FormType;

    async createFormByObject(parent: JQuery<HTMLElement>, configuration: IFormConfiguration): Promise<void> {
        const domContainer = $("<span>Loading Schedule</span>");
        parent.append(domContainer);

        const calenderConfiguration: IWeeklyCalenderConfiguration = {};

        const foundItem = await ClientItems.getObjectByUri(
            this.workspace,
            this.itemUrl);
        let packagedElements: any[];
        if (foundItem.metaClass.uri === StundenPlanTypes._Types.__WeeklyScheduleView_Uri) {
            const queryUrl = foundItem.get(StundenPlanTypes._Types._WeeklyScheduleView.collectionUri);
            packagedElements = await ClientItems.getElements(queryUrl);
            calenderConfiguration.weeks = 
                foundItem.get(StundenPlanTypes._Types._WeeklyScheduleView.weeks, ObjectType.Number);
            calenderConfiguration.skipWeekend = 
                foundItem.get(StundenPlanTypes._Types._WeeklyScheduleView.skipWeekend, ObjectType.Boolean);
            
        } else {
            // Gets the elements from the package
            packagedElements = foundItem.get(_DatenMeister._CommonTypes._Default._Package.packagedElement, Mof.ObjectType.Array);
        }

        if (packagedElements === undefined || packagedElements === null) {
            parent.append($("<span>The element did not include the packagedElements"));
        }

        const calendarControl = new WeeklyCalenderControl(domContainer);

        // Go through the elements and skip the ones which are not periodic dates
        for (const n in packagedElements) {
            const item = packagedElements[n] as Mof.DmObject;
            if (item.metaClass.uri !== StundenPlanTypes._Types.__WeeklyPeriodicEvent_Uri) {
                continue;
            }

            calendarControl.addPeriodicEvent(item);
        }

        // Create the calendarControl
        domContainer.empty();

        calendarControl.createTable(calenderConfiguration
        );
    }

    refreshForm(): void {
        return;
    }

    async storeFormValuesIntoDom?(reuseExistingElement?: boolean): Promise<Mof.DmObject> {
        return new Mof.DmObject();
    }
}

export interface IWeeklyCalenderConfiguration
{
    /**
     * Number of weeks to be shown
     */
    weeks?: number; 

    /**
     * Gives information, whether the weeks shall be shown as the first column
     */
    showWeeks?: boolean; 

    /**
     * Gives information whether the weekend days shall be skipped and not to be shown
     */
    skipWeekend?: boolean;
}

export class WeeklyCalenderControl {
    container: JQuery<HTMLElement>;
    table: JQuery<HTMLElement>;

    /**
     * Stores each cell in the array
     */
    cells: JQuery<HTMLElement>[] = new Array<JQuery<HTMLElement>>;

    /**
     * Stores the events that shall be shown. 
     * When the table will be created, the elements will be added
     */
    events: Mof.DmObject[];

    /**
     * Stores the flag whether the table has already been created
     */
    isTableCreated: boolean;

    /**
     * Initializes a new instance of the object
     * @param container Container which is used to append the table
     */
    constructor(container: JQuery<HTMLElement>) {
        this.container = container;
        this.isTableCreated = false;
        this.events = new Array<Mof.DmObject>;
    }

    createTable(configuration: IWeeklyCalenderConfiguration) {
        // Performs the default configuration, if not already set
        if (configuration.weeks === undefined || configuration.weeks === 0) { configuration.weeks = 4; }
        if (configuration.showWeeks === undefined) { configuration.showWeeks = true; }
        if (configuration.skipWeekend === undefined) { configuration.skipWeekend = false; }

        // Deletes the existing content, if a table already has been created
        if (this.isTableCreated && this.table !== undefined) {
            this.table.remove();
            this.cells.length = 0;
        }

        // Creates the manager
        const manager = new PeriodicEventManager(this.events);
        
        // Now, we are live.. The table is created
        const table = $("<table class='stundenplan'></table>");

        for (let n = 0; n < configuration.weeks; n++) {
            const row = $("<tr></tr>");

            // Creates the first column containing the weekdays
            if (configuration.showWeeks) {
                const cellWeek = $("<td class='stundenplan-week'></td>");
                cellWeek.text("Week " + (n + 1).toString());
                row.append(cellWeek);
            }

            // Now, create the cells for each workday. One Cell per workday
            const numberOfDays = configuration.skipWeekend ? 5 : 7;
            for (let day = 1; day <= numberOfDays; day++) {
                const cell = $("<td></td>");
                const weekDay = $("<div class='stundenplan-weekday'></div>");
                weekDay.text(getWeekDay(day));
                cell.append(weekDay);

                // Gets the events at that date
                const events = manager.getEventsOnWeekday(n, day);
                for (const m in events) {
                    const eventItem = events[m];

                    const weekEvent = $("<div class='stundenplan-event'></div>");
                    const theEvent = $("<a></a>");
                    const timeStart = eventItem.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, Mof.ObjectType.String);
                    const title = eventItem.get(StundenPlanTypes._Types._WeeklyPeriodicEvent._name_, Mof.ObjectType.String);
                    theEvent.text(timeStart + " " + title);
                    theEvent.attr('href', Navigator.getLinkForNavigateToMofItem(eventItem));
                    weekEvent.append(theEvent);
                    cell.append(weekEvent);
                }

                this.cells.push(cell);
                                
                // Adds cell to the row
                row.append(cell);
            }

            table.append(row);
        }

        this.container.append(table);
        this.table = table;
    }

    /**
     * Adds one item to the table. 
     * Currently, if the table already has been created, we cannot add 
     * an additional item to that
     * @param event Event that shall be added
     */
    addPeriodicEvent(event: Mof.DmObject) {
        this.events.push(event);
    }
}

/**
 * Some helper class which helps to organize the periodic events
 */
class PeriodicEventManager {
    private readonly events: Mof.DmObject[];

    constructor(events: Mof.DmObject[]) {
        this.events = events;
    }

    /**
     * Gets the events on a certain weekday within a certain week
     * @param week # of week to be evaluated
     * @param day Weekday to be evaluated
     */
    getEventsOnWeekday(week: number, day: number) {
        const foundEvents = new Array<Mof.DmObject>();
        for (const n in this.events) {
            const event = this.events[n];
            if (!isPeriodicEventApplicableOnWeekday(event, day)) {
                continue;
            }

            const weekOffset = event.get(
                    StundenPlanTypes._Types._WeeklyPeriodicEvent.weekOffset,
                    Mof.ObjectType.Number)
                ?? 0;
            let weekInterval = event.get(
                StundenPlanTypes._Types._WeeklyPeriodicEvent.weekInterval,
                Mof.ObjectType.Number);
            if (weekInterval === undefined || weekInterval < 1) {
                weekInterval = 1;
            }

            // Checks, if we are in the right week! 
            if ((week - weekOffset) % weekInterval !== 0) {
                continue;
            }

            // Ok, success, add it! 
            foundEvents.push(event);
        }

        // Now sort the entries according starting date
        foundEvents.sort((x, y) => {
            const startX = x.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, Mof.ObjectType.String);
            const startY = y.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.timeStart, Mof.ObjectType.String);

            return startX === startY ? 0 : startX < startY ? -1 : 1;
        });
        
        return foundEvents;
    }
}

/**
 * Checks whether the periodic event is applicable on a certain weekday 
 * @param event Event to be checked
 * @param dayNumber Number of the day
 */
function isPeriodicEventApplicableOnWeekday(event: Mof.DmObject, dayNumber: number){
    switch(dayNumber) {
        case 1:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onMonday, Mof.ObjectType.Boolean);
        case 2:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onTuesday, Mof.ObjectType.Boolean);
        case 3:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onWednesday, Mof.ObjectType.Boolean);
        case 4:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onThursday, Mof.ObjectType.Boolean);
        case 5:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onFriday, Mof.ObjectType.Boolean);
        case 6:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onSaturday, Mof.ObjectType.Boolean);
        case 7:
            return event.get(StundenPlanTypes._Types._WeeklyPeriodicEvent.onSunday, Mof.ObjectType.Boolean);
    }
}

/**
 * Gets the name of the weekday
 * @param dayNumber # of the day. 1 is Monday, 7 is Sunday
 * @returns The name
 */
function getWeekDay(dayNumber: number) {
    if (dayNumber < 1 || dayNumber > 7) {
        throw "Invalid argument, must be between 1 and 7";
    }

    switch (dayNumber) {
        case 1:
            return "Monday";
        case 2:
            return "Tuesday";
        case 3:
            return "Wednesday";
        case 4:
            return "Thursday";
        case 5:
            return "Friday";
        case 6:
            return "Saturday";
        case 7:
            return "Sunday";
    }
}