using DatenMeister.Core.EMOF.Interface.Common;
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Functions.Queries;
using DatenMeister.Core.Helper;
using DatenMeister.StundenPlan.Model;

namespace DatenMeister.StundenPlan.Logic
{
    public static partial class EventsLogic
    {
        /// <summary>
        /// Calculates the number of hours per week just being used for the given events
        /// </summary>
        /// <param name="allElements">Contains all elements to be assessed</param>
        /// <returns></returns>
        public static double CalculateEventHoursPerWeek(IReflectiveCollection allElements)
        {
            var allElementsAsAList = allElements.
                WhenMetaClassIs(_Types.TheOne.__WeeklyPeriodicEvent).OfType<IElement>().ToList();

            var totalHours = 0.0;

            foreach (var element in allElementsAsAList) {
                var eventHours = element.getOrDefault<double>(_Types._WeeklyPeriodicEvent.hoursDuration);
                double factor = (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onMonday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onTuesday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onWednesday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onThursday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onFriday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSaturday) ? 1 : 0)
                    + (element.getOrDefault<bool>(_Types._WeeklyPeriodicEvent.onSunday) ? 1 : 0);

                factor /= Math.Max(1, element.getOrDefault<int>(_Types._WeeklyPeriodicEvent.weekInterval));

                totalHours += eventHours * factor;
            }

            return totalHours;
        }
    }
}
