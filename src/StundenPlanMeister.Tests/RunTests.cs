using BurnSystems.Logging;
using BurnSystems.Logging.Provider;
using DatenMeister.Core;
using DatenMeister.Core.Runtime.Workspaces;
using DatenMeister.Integration.DotNet;
using DatenMeister.StundenPlan;

namespace StundenPlanMeister.Tests
{
    public class RunTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestWhetherStundenMeisterIsIncludedAsync()
        {
            Console.WriteLine(StundenPlanPlugin.ViewModeName);

            var integrationSettings = new IntegrationSettings
            {
                DatabasePath = "./data"
            };

            TheLog.AddProvider(new ConsoleProvider());

            await using var dm = await GiveMe.DatenMeisterAsync(integrationSettings);
            {
                // Check, if form is available
                var formsExtent = dm.WorkspaceLogic.FindExtent(
                    WorkspaceNames.WorkspaceManagement,
                    StundenPlanPlugin.UriStundenPlanForm);
                Assert.That(formsExtent, Is.Not.Null);

                // Check, if types is available
                var typesExtent = dm.WorkspaceLogic.FindExtent(
                    WorkspaceNames.WorkspaceTypes,
                    StundenPlanPlugin.UriStundenPlanTypes);
                Assert.That(typesExtent, Is.Not.Null);
            }
        }
    }
}