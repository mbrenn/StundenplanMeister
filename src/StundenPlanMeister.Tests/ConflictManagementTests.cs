using BurnSystems.Logging;
using BurnSystems.Logging.Provider;
using DatenMeister.Core;
using DatenMeister.Core.EMOF.Interface.Reflection;
using DatenMeister.Core.Helper;
using DatenMeister.Core.Provider.InMemory;
using DatenMeister.Integration.DotNet;
using DatenMeister.StundenPlan.Logic;
using DatenMeister.StundenPlan.Model;

namespace StundenPlanMeister.Tests
{
    internal class ConflictManagementTests
    {
        [Test]
        public async Task TestConflictEngineOnSameDay()
        {
            var integrationSettings = new IntegrationSettings
            {
                DatabasePath = "./data"
            };

            TheLog.AddProvider(new ConsoleProvider());

            await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);
            var tests = new List<IElement>();

            var event1 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event2 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event3 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);

            event1.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event1.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event1.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event1.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event1.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event1.set(_Types._WeeklyPeriodicEvent.name, "Event 1");

            event2.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event2.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event2.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event2.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event2.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event2.set(_Types._WeeklyPeriodicEvent.name, "Event 2");

            event3.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event3.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event3.set(_Types._WeeklyPeriodicEvent.onTuesday, true);
            event1.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event3.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event3.set(_Types._WeeklyPeriodicEvent.name, "Event 3");

            var events = EventsLogic.GetConflicts(new[] { event1, event2, event3 }).ToList();

            Assert.That(events.Count, Is.EqualTo(1));
            var firstConflict = events[0];
            var firstSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.firstSchedule);
            var secondSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.secondSchedule);

            Assert.That(firstSchedule, Is.Not.Null);
            Assert.That(secondSchedule, Is.Not.Null);

            Assert.That(firstSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 1"));
            Assert.That(secondSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 2"));
        }

        [Test]
        public async Task TestConflictEngineOnOverlapping()
        {
            var integrationSettings = new IntegrationSettings
            {
                DatabasePath = "./data"
            };

            TheLog.AddProvider(new ConsoleProvider());

            await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);
            var tests = new List<IElement>();

            var event1 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event2 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event3 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);

            event1.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event1.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event1.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event1.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event1.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event1.set(_Types._WeeklyPeriodicEvent.name, "Event 1");

            event2.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event2.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event2.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event2.set(_Types._WeeklyPeriodicEvent.timeStart, 10);
            event2.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event2.set(_Types._WeeklyPeriodicEvent.name, "Event 2");

            event3.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event3.set(_Types._WeeklyPeriodicEvent.weekInterval, 1);
            event3.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event3.set(_Types._WeeklyPeriodicEvent.timeStart, 11);
            event3.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event3.set(_Types._WeeklyPeriodicEvent.name, "Event 3");

            var events = EventsLogic.GetConflicts(new[] { event1, event2, event3 }).ToList();

            Assert.That(events.Count, Is.EqualTo(2));
            var firstConflict = events[0];
            var firstSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.firstSchedule);
            var secondSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.secondSchedule);

            Assert.That(firstSchedule, Is.Not.Null);
            Assert.That(secondSchedule, Is.Not.Null);

            Assert.That(firstSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 1"));
            Assert.That(secondSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 2"));
        }

        [Test]
        public async Task TestConflictEngineAlternativeWeeks()
        {
            var integrationSettings = new IntegrationSettings
            {
                DatabasePath = "./data"
            };

            TheLog.AddProvider(new ConsoleProvider());

            await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);
            var tests = new List<IElement>();

            var event1 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event2 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);
            var event3 = InMemoryObject.CreateEmpty(_Types.TheOne.__WeeklyPeriodicEvent);

            event1.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event1.set(_Types._WeeklyPeriodicEvent.weekInterval, 2);
            event1.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event1.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event1.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event1.set(_Types._WeeklyPeriodicEvent.name, "Event 1");

            event2.set(_Types._WeeklyPeriodicEvent.weekOffset, 0);
            event2.set(_Types._WeeklyPeriodicEvent.weekInterval, 2);
            event2.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event2.set(_Types._WeeklyPeriodicEvent.timeStart, 10);
            event2.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event2.set(_Types._WeeklyPeriodicEvent.name, "Event 2");

            event3.set(_Types._WeeklyPeriodicEvent.weekOffset, 1);
            event3.set(_Types._WeeklyPeriodicEvent.weekInterval, 2);
            event3.set(_Types._WeeklyPeriodicEvent.onMonday, true);
            event3.set(_Types._WeeklyPeriodicEvent.timeStart, 9);
            event3.set(_Types._WeeklyPeriodicEvent.hoursDuration, 2);
            event3.set(_Types._WeeklyPeriodicEvent.name, "Event 3");

            var events = EventsLogic.GetConflicts(new[] { event1, event2, event3 }).ToList();

            Assert.That(events.Count, Is.EqualTo(1));
            var firstConflict = events[0];
            var firstSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.firstSchedule);
            var secondSchedule = firstConflict.getOrDefault<IObject>(_Types._ConflictingSchedule.secondSchedule);

            Assert.That(firstSchedule, Is.Not.Null);
            Assert.That(secondSchedule, Is.Not.Null);

            Assert.That(firstSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 1"));
            Assert.That(secondSchedule.getOrDefault<string>(_Types._WeeklyPeriodicEvent.name), Is.EqualTo("Event 2"));
        }
    }
}
