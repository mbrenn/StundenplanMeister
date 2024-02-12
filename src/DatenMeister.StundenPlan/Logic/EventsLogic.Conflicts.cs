using DatenMeister.Core.EMOF.Implementation;
using DatenMeister.Core.EMOF.Interface.Common;
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Functions.Queries;
using DatenMeister.Core.Helper;
using DatenMeister.Core.Provider.InMemory;
using DatenMeister.StundenPlan.Model;
using static DatenMeister.StundenPlan.Model._Types;

namespace DatenMeister.StundenPlan.Logic
{
    public static partial class EventsLogic
    {

        /// <summary>
        ///  Add all conflicts to the intended target collection
        /// </summary>
        /// <param name="targetCollection">Creates the Target collection</param>
        /// <param name="allElements">All Events to be parsed</param>
        public static void AddConflicts(IReflectiveCollection targetCollection, IReflectiveCollection allElements)
        {
            var factory = new MofFactory(targetCollection);

            foreach (var conflict in GetConflicts(allElements, factory))
            {
                targetCollection.add(conflict);
            }
        }

        /// <summary>
        /// Gets an enumeration of all conflicts out of the events within the all elements
        /// </summary>
        /// <param name="allElements">Events to be parsed</param>
        /// <param name="factory">Opotional factory to be used, can be null, in case the elements shall just be created temporarily</param>
        /// <returns>Enumeration of elements</returns>
        public static IEnumerable<IElement> GetConflicts(IReflectiveCollection allElements, IFactory? factory = null)
        {
            var allElementsAsAList = allElements.
                WhenMetaClassIs(_Types.TheOne.__WeeklyPeriodicEvent).OfType<IElement>().ToList();
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
                        var conflict =
                            factory != null
                            ? factory.create(_Types.TheOne.__ConflictingSchedule)
                            : InMemoryObject.CreateEmpty(_Types.TheOne.__ConflictingSchedule);
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
            var firstStart = first.getOrDefault<DateTime>(_Types._WeeklyPeriodicEvent.timeStart);
            var firstDuration = first.getOrDefault<double>(_Types._WeeklyPeriodicEvent.hoursDuration);
            var firstInterval = first.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekInterval);
            var firstOffset = first.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekOffset);
            var firstEnd = firstStart + TimeSpan.FromHours(firstDuration);

            var secondStart = second.getOrDefault<DateTime>(_Types._WeeklyPeriodicEvent.timeStart);
            var secondDuration = second.getOrDefault<double>(_Types._WeeklyPeriodicEvent.hoursDuration);
            var secondInterval = second.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekInterval);
            var secondOffset = second.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekOffset);
            var secondEnd = secondStart + TimeSpan.FromHours(secondDuration);

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
                if (firstCorrectedOffset != secondCorrectedOffset)
                {
                    return false;
                }
            }

            // We have a matching time, matching time interval and matching days. It must be a conflict
            return true;
        }
    }
}
