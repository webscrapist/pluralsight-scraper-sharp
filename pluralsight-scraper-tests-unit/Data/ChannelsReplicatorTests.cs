using System.Threading.Tasks;
using NUnit.Framework;
using VH.PluralsightScraper.Tests.Unit.Plumbing;

namespace VH.PluralsightScraper.Tests.Unit.Data
{
    [TestFixture]
    public class ChannelsReplicatorTests
    {
        [Test]
        public async Task when_replicate_given_channels_is_null_should_return_empty_results()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channels_is_null();

            await _.when_replicate();

            _.should_return_empty_results();
        }

        [Test]
        public async Task when_replicate_given_channels_is_empty_should_return_empty_results()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channels_is_empty();

            await _.when_replicate();

            _.should_return_empty_results();
        }

        [Test]
        public async Task when_replicate_given_channel_created_should_return_channel_created()
        {
            var _ = new ChannelsReplicatorTestContext();
            
            _.given_channel_created();

            await _.when_replicate();

            _.should_return_channel_created();
        }

        [Test]
        public async Task when_replicate_given_channel_updated_should_return_channel_updated()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channel_updated();

            await _.when_replicate();

            _.should_return_channel_updated();
        }

        [Test]
        public async Task when_replicate_given_channel_unchanged_should_return_channel_unchanged()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channel_unchanged();

            await _.when_replicate();

            _.should_return_channel_unchanged();
        }

        [Test]
        public async Task when_replicate_given_channel_delete_should_return_channel_removed()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channel_deleted();

            await _.when_replicate();

            _.should_return_channel_deleted();
        }

        [Test]
        public async Task when_replicate_given_channel_created_updated_deleted_unchanged_should_return_channel_created_updated_deleted_unchanged()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_channel_created();
            _.given_channel_updated();
            _.given_channel_deleted();
            _.given_channel_unchanged();

            await _.when_replicate();

            _.should_return_channel_added_changed_removed_unchanged();
        }

        [Test]
        public async Task when_replicate_given_task_was_canceled_should_throw_exception()
        {
            var _ = new ChannelsReplicatorTestContext();

            _.given_task_was_canceled();

            await _.when_replicate();

            _.should_throw_exception();
        }
    }
}
