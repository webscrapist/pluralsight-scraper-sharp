using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VH.PluralsightScraper.Domain;
using VH.PluralsightScraper.Dtos;

namespace VH.PluralsightScraper.Data
{
    internal class ChannelsReplicator
    {
        public ChannelsReplicator(PluralsightContext pluralsightContext)
        {
            _db = pluralsightContext ?? throw new ArgumentNullException(nameof(pluralsightContext));
        }

        public async Task<ReplicateResult> Replicate(IEnumerable<ChannelDto> dtosList,
                                                     CancellationToken cancellationToken)
        {
            if (dtosList == null)
            {
                return ReplicateResult.BuildEmpty();
            }

            Dictionary<string, Channel> channelsByName = await _db.Channels
                                                                  .Include(c => c.ChannelCourses)
                                                                  .ThenInclude(channelCourses => channelCourses.Course)
                                                                  .ToDictionaryAsync(c => c.Name, cancellationToken);

            var result = new ReplicateResult();

            foreach (ChannelDto dto in dtosList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                ReplicateResult replicateResult;

                if (channelsByName.TryGetValue(dto.Name, out Channel channelDb))
                {
                    replicateResult = Replicate(dto, channelDb);
                    channelsByName.Remove(channelDb.Name);
                }
                else
                {
                    replicateResult = Replicate(dto);
                }

                result.Add(replicateResult);
            }

            foreach (Channel channelDb in channelsByName.Values)
            {
                ReplicateResult channelDeleted = ReplicateResult.BuildChannelDeleted(channelDb.Name);
                result.Add(channelDeleted);
            }
            
            _db.Channels.RemoveRange(channelsByName.Values);

            await _db.SaveChangesAsync(cancellationToken);

            return result;
        }

        private ReplicateResult Replicate(ChannelDto channelDto, Channel channelDb)
        {
            bool channelModified = channelDb.Merge(channelDto);

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
            var newChannel = new Channel(channelDto);

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

                Course courseDb = _db.Courses.SingleOrDefault(c => string.Equals(c.Name,
                                                                                 channelCourse.Course.Name,
                                                                                 StringComparison.CurrentCultureIgnoreCase));

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
    }
}
