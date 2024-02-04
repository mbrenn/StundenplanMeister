using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.Core.Provider.InMemory;
using DatenMeister.StundenPlan.Model;
using System.Reflection.Metadata.Ecma335;
using static DatenMeister.StundenPlan.Model._Types;

namespace DatenMeister.StundenPlan.Logic
{
    public static class EventsLogic
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

        public static IEnumerable<IElement> GetConflicts(IEnumerable<IElement> allElements)
        {
            var allElementsAsAList = allElements.ToList();
            var result = new List<IElement>();

            // First, store the list, we make a triangle approach by comparing all elements to be compared by later elements
            for (var n = 0; n < allElementsAsAList.Count() - 1; n++)
            {
                for (var m = n + 1; m < allElementsAsAList.Count(); m++)
                {
                    var first = allElementsAsAList[n];
                    var second = allElementsAsAList[m];

                    if (IsConflicting(first, second))
                    {
                        var conflict = InMemoryObject.CreateEmpty(
                            _Types.TheOne.__ConflictingSchedule);
                        conflict.set(_ConflictingSchedule.firstSchedule, first);
                        conflict.set(_ConflictingSchedule.secondSchedule, second);
                        result.Add(conflict);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets or sets whether two events are conflicting to each other
        /// </summary>
        /// <param name="first">First element</param>
        /// <param name="second">Second Element</param>
        /// <returns>true, if conflicting</returns>
        private static bool IsConflicting(IElement first, IElement second)
        {
            // An event is not conflicting, if there
            // - are no conflicting days
            // - are no overlapping times OR
            // - in case the intervals have a common divisor, the interval offset are not the same
            var firstStart = first.getOrDefault<double>(_Types._WeeklyPeriodicEvent.timeStart);
            var firstDuration = first.getOrDefault<double>(_Types._WeeklyPeriodicEvent.hoursDuration);
            var firstInterval = first.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekInterval);
            var firstOffset = first.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekOffset);
            var firstEnd = firstStart + firstDuration;

            var secondStart = second.getOrDefault<double>(_Types._WeeklyPeriodicEvent.timeStart);
            var secondDuration = second.getOrDefault<double>(_Types._WeeklyPeriodicEvent.hoursDuration);
            var secondInterval = second.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekInterval);
            var secondOffset = second.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekOffset);
            var secondEnd = secondStart + secondDuration;

            // There are no conflicting days
            if (!(
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onMonday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onMonday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onTuesday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onTuesday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onWednesday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onWednesday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onThursday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onThursday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onFriday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onFriday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSaturday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSaturday)
                ||
                first.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSunday) && second.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSunday)
                ))
            {
                return false;
            }

            // There are no conflicting time slots
            if (firstStart >= secondEnd || secondStart >= firstEnd)
            {
                return false;
            }

            // Check, that the intervals have a common divider
            var low = Math.Min(firstInterval, secondInterval);
            var high = Math.Max(firstInterval, secondInterval);
            
            var residual = high % low;
            if (residual == 0 && low != 0)
            {
                // Ok, we do have a common divider, now let's figure out whether the intervals are conflicting
                // First, normalize the offsets that they are not carrying the intervals
                var firstCorrectedOffset = firstOffset % firstInterval;
                var secondCorrectedOffset = secondOffset % secondInterval;

                // Second, normalize them on the smaller interval
                firstCorrectedOffset = firstCorrectedOffset % low;
                secondCorrectedOffset = secondCorrectedOffset % low;

                // Now check, that both are different, that means they are in alternative weeks
                if ( firstCorrectedOffset != secondCorrectedOffset)
                {
                    return false;
                }
            }
            
            // We have a matching time, matching time interval and matching days. It must be a conflict
            return true;
        }
    }
}
