using NUnit.Framework;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Feedback;

namespace VH.PluralsightScraper.Tests.Unit.Feedback
{
    [TestFixture]
    public class ConsoleViewTests
    {
        [Test]
        public void when_method_name_given_condition_should_expectation()
        {
            // arrange
            ConsoleView.ShowGettingChannels();
            ReplicateResult replicateResult = ReplicateResult.BuildChannelCreated(channelName: "foo channel");

            // act
            ConsoleView.Show(replicateResult);
        }
    }
}
