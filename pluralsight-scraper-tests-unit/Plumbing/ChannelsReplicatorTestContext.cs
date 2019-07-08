using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using VH.PluralsightScraper.Data;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Tests.Unit.Plumbing
{
    internal class ChannelsReplicatorTestContext
    {
        public void given_channels_is_null()
        {
            _channelsToReplicate = null;
        }

        public void given_channels_is_empty()
        {
            _channelsToReplicate = new ChannelDto[] { };
        }

        public void given_channel_created()
        {
            ChannelDto dto = CreateChannelDto(FOO_CHANNEL_NAME_CREATED);

            _channelsToReplicate = new List<ChannelDto>(_channelsToReplicate)
                                   {
                                       dto
                                   };
        }
        
        public void given_channel_updated()
        {
            ChannelDto channelDto = CreateChannelDto(FOO_CHANNEL_NAME_UPDATED);

            List<Course> onlyOneCourse = channelDto.Courses
                                                   .Select(_courseFactory.ConvertFromDto)
                                                   .Select(c => c.Course)
                                                   .Take(1)
                                                   .ToList();
            
            var channelInDb = new Channel(channelDto.Name, channelDto.Url, onlyOneCourse);

            _channelsDbRecords = new List<Channel>(_channelsDbRecords)
                                 {
                                     channelInDb
                                 };

            _channelsToReplicate = new List<ChannelDto>(_channelsToReplicate)
                                   {
                                       channelDto
                                   };
        }

        public void given_channel_unchanged()
        {
            ChannelDto dto = CreateChannelDto(FOO_CHANNEL_NAME_UNCHANGED);

            List<Course> courses = dto.Courses
                                      .Select(_courseFactory.ConvertFromDto)
                                      .Select(c => c.Course)
                                      .ToList();
            
            var channelInDb = new Channel(dto.Name, dto.Url, courses);

            _channelsDbRecords = new List<Channel>(_channelsDbRecords)
                                 {
                                     channelInDb
                                 };

            _channelsToReplicate = new List<ChannelDto>(_channelsToReplicate)
                                   {
                                       dto
                                   };
        }

        public void given_channel_deleted()
        {
            ChannelDto channelDto = CreateChannelDto(FOO_CHANNEL_NAME_DELETED);

            List<Course> courses = channelDto.Courses
                                             .Select(_courseFactory.ConvertFromDto)
                                             .Select(c => c.Course)
                                             .ToList();
            
            var channelInDb = new Channel(channelDto.Name, channelDto.Url, courses);

            _channelsDbRecords = new List<Channel>(_channelsDbRecords)
                                 {
                                     channelInDb
                                 };
        }

        public void given_task_was_canceled()
        {
            _isReplicationCanceled = true;
            _expectingException = true;
        }

        public async Task when_replicate()
        {
            var dbContextFactory = new DbContextFactory();
            
            await SeedDataInDatabase(dbContextFactory, new CancellationToken());

            try
            {
                var cancellationToken = new CancellationToken(_isReplicationCanceled);

                using (PluralsightContext dbContext = await dbContextFactory.Create(cancellationToken))
                {
                    var sut = new ChannelsReplicator(dbContext);
                    _result = await sut.Replicate(_channelsToReplicate, cancellationToken);
                }
            }
            catch (Exception e)
            {
                if (_expectingException)
                {
                    _exception = e;
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task SeedDataInDatabase(DbContextFactory dbContextFactory, CancellationToken cancellationToken)
        {
            using (PluralsightContext dbContext = await dbContextFactory.Create(cancellationToken))
            {
                await dbContext.Channels.AddRangeAsync(_channelsDbRecords, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public void should_return_empty_results()
        {
            _result.ChannelsCreatedCount.Should().Be(0);
            _result.ChannelsDeletedCount.Should().Be(0);
            _result.ChannelsUpdatedCount.Should().Be(0);
            _result.ChannelsUnchangedCount.Should().Be(0);
            _result.TotalChannelsCount.Should().Be(0);
            _result.Details.Should().BeEmpty();
        }

        public void should_return_channel_created()
        {
            _result.ChannelsCreatedCount.Should().Be(1);
            _result.ChannelsDeletedCount.Should().Be(0);
            _result.ChannelsUpdatedCount.Should().Be(0);
            _result.ChannelsUnchangedCount.Should().Be(0);
            _result.TotalChannelsCount.Should().Be(1);

            ReplicateResultDetail[] details = _result.Details.ToArray();

            details.Length.Should().Be(1);

            details[0].Action.Should().Be(ReplicationAction.Created);
            details[0].ChannelName.Should().Be(FOO_CHANNEL_NAME_CREATED);
        }

        public void should_return_channel_updated()
        {
            _result.ChannelsCreatedCount.Should().Be(0);
            _result.ChannelsDeletedCount.Should().Be(0);
            _result.ChannelsUpdatedCount.Should().Be(1);
            _result.ChannelsUnchangedCount.Should().Be(0);
            _result.TotalChannelsCount.Should().Be(1);

            ReplicateResultDetail[] details = _result.Details.ToArray();

            details.Length.Should().Be(1);

            details[0].Action.Should().Be(ReplicationAction.Updated);
            details[0].ChannelName.Should().Be(FOO_CHANNEL_NAME_UPDATED);
        }

        public void should_return_channel_unchanged()
        {
            _result.ChannelsCreatedCount.Should().Be(0);
            _result.ChannelsDeletedCount.Should().Be(0);
            _result.ChannelsUpdatedCount.Should().Be(0);
            _result.ChannelsUnchangedCount.Should().Be(1);
            _result.TotalChannelsCount.Should().Be(1);

            ReplicateResultDetail[] details = _result.Details.ToArray();

            details.Length.Should().Be(1);

            details[0].Action.Should().Be(ReplicationAction.Unchanged);
            details[0].ChannelName.Should().Be(FOO_CHANNEL_NAME_UNCHANGED);
        }

        public void should_return_channel_deleted()
        {
            _result.ChannelsCreatedCount.Should().Be(0);
            _result.ChannelsDeletedCount.Should().Be(1);
            _result.ChannelsUpdatedCount.Should().Be(0);
            _result.ChannelsUnchangedCount.Should().Be(0);
            _result.TotalChannelsCount.Should().Be(1);

            ReplicateResultDetail[] details = _result.Details.ToArray();

            details.Length.Should().Be(1);

            details[0].Action.Should().Be(ReplicationAction.Deleted);
            details[0].ChannelName.Should().Be(FOO_CHANNEL_NAME_DELETED);
        }

        public void should_return_channel_added_changed_removed_unchanged()
        {
            _result.ChannelsCreatedCount.Should().Be(1);
            _result.ChannelsDeletedCount.Should().Be(1);
            _result.ChannelsUpdatedCount.Should().Be(1);
            _result.ChannelsUnchangedCount.Should().Be(1);
            _result.TotalChannelsCount.Should().Be(4);

            ReplicateResultDetail[] details = _result.Details.ToArray();

            details.Length.Should().Be(4);

            details[0].Action.Should().Be(ReplicationAction.Created);
            details[1].Action.Should().Be(ReplicationAction.Updated);
            details[2].Action.Should().Be(ReplicationAction.Unchanged);
            details[3].Action.Should().Be(ReplicationAction.Deleted);
            
            details[0].ChannelName.Should().Be(FOO_CHANNEL_NAME_CREATED);
            details[1].ChannelName.Should().Be(FOO_CHANNEL_NAME_UPDATED);
            details[2].ChannelName.Should().Be(FOO_CHANNEL_NAME_UNCHANGED);
            details[3].ChannelName.Should().Be(FOO_CHANNEL_NAME_DELETED);
        }

        public void should_throw_exception()
        {
            _exception.Should().NotBeNull();
        }
        
        private static ChannelDto CreateChannelDto(string channelName)
        {
            CourseDto course0 = CourseDto.Create("foo-course-name-0", "foo-course-level-0", "11/11/11");
            CourseDto course1 = CourseDto.Create("foo-course-name-1", "foo-course-level-1", "March 13, 2014");
            
            return new ChannelDto("foo-url", channelName, courses: new[] { course0, course1 });
        }

        private const string FOO_CHANNEL_NAME_CREATED   = "foo-channel-name-created";
        private const string FOO_CHANNEL_NAME_DELETED   = "foo-channel-name-deleted";
        private const string FOO_CHANNEL_NAME_UPDATED   = "foo-channel-name-updated";
        private const string FOO_CHANNEL_NAME_UNCHANGED = "foo-channel-name-unchanged";
        
        private IEnumerable<Channel> _channelsDbRecords = new Channel[] { };
        private IEnumerable<ChannelDto> _channelsToReplicate = new ChannelDto[] { };
        private bool _isReplicationCanceled;
        private bool _expectingException;
        private ReplicateResult _result;
        private Exception _exception;
        private readonly CourseFactory _courseFactory = new CourseFactory();
    }
}
