using DatenMeister.Core.EMOF.Implementation;
using DatenMeister.Core.EMOF.Interface.Common;
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.StundenPlan.Model;
using static DatenMeister.StundenPlan.Model._Types;

namespace DatenMeister.StundenPlan.Logic
{
    public static partial class EventsLogic
    {
        /// <summary>
        /// Checks whether the event is on a certain weekday active
        /// </summary>
        /// <param name="weekDay">Day of the week being requested. 1 is Monday</param>
        /// <param name="element">Element to be evaluated</param>
        public static bool IsEventOnWeekday(int weekDay, IElement element)
        {
            return weekDay switch
            {
                1 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onMonday) == 1,
                2 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onTuesday) == 1,
                3 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onWednesday) == 1,
                4 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onThursday) == 1,
                5 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onFriday) == 1,
                6 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onSaturday) == 1,
                7 => element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.onSunday) == 1,
                _ => false
            };
        }

        /// <summary>
        /// Returns a list of all elements which fit to the given week and weekday
        /// </summary>
        /// <param name="week">Week to be evaluated, starting with 0 as the first week</param>
        /// <param name="weekDay">Day of the week, 1 is Monday</param>
        /// <param name="allElements">Collection of all elements to filter</param>
        /// <returns>Enumerating the events</returns>
        public static IEnumerable<IElement> GetEventsOnWeekDay(int week, int weekDay, IEnumerable<IElement> allElements)
        {
            return allElements.Where(element =>
            {
                var eventWeekInterval = element.getOrDefault<int>(_WeeklyPeriodicEvent.@weekInterval);
                var eventWeekOffset = element.getOrDefault<int>(_WeeklyPeriodicEvent.@weekOffset);

                // Calculate the effective week number for the element
                var effectiveWeek = week - eventWeekOffset;

                // Check if the effective week is divisible by the event's weekInterval
                if (effectiveWeek >= 0 && effectiveWeek % eventWeekInterval == 0)
                {
                    // Check if the event occurs on the specified weekday
                    var found = IsEventOnWeekday(weekDay, element);
                    return found;
                }

                return false;
            }).OrderBy(
                x => x.getOrDefault<DateTime>(_WeeklyPeriodicEvent.timeStart));
        }
    }
}
