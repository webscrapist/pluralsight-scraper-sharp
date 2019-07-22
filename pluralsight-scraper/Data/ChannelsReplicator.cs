using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Data
{
    internal class ChannelsReplicator
    {
        public ChannelsReplicator(PluralsightContext pluralsightContext, EntityFactory entityFactory)
        {
            _db = pluralsightContext ?? throw new ArgumentNullException(nameof(pluralsightContext));
            _entityFactory = entityFactory ?? throw new ArgumentNullException(nameof(entityFactory));
        }

        public async Task<ReplicateResult> Replicate(IEnumerable<ChannelDto> channelDtosList,
                                                     CancellationToken cancellationToken)
        {
            if (channelDtosList == null)
            {
                return ReplicateResult.BuildEmpty();
            }

            Dictionary<string, Channel> channelsDbByName = await _db.Channels.Include(c => c.ChannelCourses)
                                                                    .ThenInclude(channelCourses => channelCourses.Course)
                                                                    .ToDictionaryAsync(c => c.Name, cancellationToken);

            var result = new ReplicateResult();

            foreach (ChannelDto channelDto in channelDtosList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                ReplicateResult replicateResult;

                if (string.IsNullOrWhiteSpace(channelDto.Name))
                {
                    Log.Error("channel without name. ChannelPageUrl: [{ChannelPageUrl}]", channelDto.Url);
                    continue;
                }

                if (channelsDbByName.TryGetValue(channelDto.Name, out Channel channelDb))
                {
                    replicateResult = Replicate(channelDto, channelDb);
                    channelsDbByName.Remove(channelDb.Name);
                }
                else
                {
                    replicateResult = Replicate(channelDto);
                }

                result.Add(replicateResult);
            }

            foreach (Channel channelDb in channelsDbByName.Values)
            {
                ReplicateResult channelDeleted = ReplicateResult.BuildChannelDeleted(channelDb.Name);
                result.Add(channelDeleted);
            }
            
            _db.Channels.RemoveRange(channelsDbByName.Values);

            await _db.SaveChangesAsync(cancellationToken);

            return result;
        }

        private ReplicateResult Replicate(ChannelDto channelDto, Channel channelDb)
        {
            bool channelModified = channelDb.Merge(channelDto, _entityFactory);

            if (channelModified)
            {
                LookUpCourses(ref channelDb);
            }

            return channelModified
                       ? ReplicateResult.BuildChannelUpdated(channelDb.Name) 
                       : ReplicateResult.BuildChannelUnchanged(channelDb.Name);
        }

        private ReplicateResult Replicate(ChannelDto channelDto)
        {
            var newChannel = new Channel(channelDto, _entityFactory);

            LookUpCourses(ref newChannel);

            _db.Channels.Add(newChannel);

            return ReplicateResult.BuildChannelCreated(newChannel.Name);
        }

        private void LookUpCourses(ref Channel channel)
        {
            foreach (ChannelCourse channelCourse in channel.ChannelCourses)
            {
                bool courseIdIsKnown = channelCourse.CourseId != 0;

                if (courseIdIsKnown)
                {
                    continue;
                }

                Course courseDb = _db.Courses.SingleOrDefault(course => course.ComparisonKey == channelCourse.Course.ComparisonKey);

                bool isNewCourse = courseDb == null;

                if (isNewCourse)
                {
                    continue;
                }

                courseDb.Merge(channelCourse.Course);

                channelCourse.Course = courseDb;
            }
        }
        
        private readonly PluralsightContext _db;
        private readonly EntityFactory _entityFactory;
    }
}
