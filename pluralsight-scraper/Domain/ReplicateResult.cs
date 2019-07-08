using System.Collections.Generic;
using System.Linq;

namespace VH.PluralsightScraper.Domain
{
    internal class ReplicateResult
    {
        public IEnumerable<ReplicateResultDetail> Details => _details;

        public int ChannelsCreatedCount => _details.Count(_ => _.Action == ReplicationAction.Created);

        public int ChannelsUpdatedCount => _details.Count(_ => _.Action == ReplicationAction.Updated);

        public int ChannelsDeletedCount => _details.Count(_ => _.Action == ReplicationAction.Deleted);
        
        public int ChannelsUnchangedCount => _details.Count(_ => _.Action == ReplicationAction.Unchanged);

        public int TotalChannelsCount => _details.Count;

        public ReplicateResult()
        {
            _details = new List<ReplicateResultDetail>();
        }
        
        public void Add(ReplicateResult replicateResult)
        {
            _details.AddRange(replicateResult.Details);
        }
        
        public static ReplicateResult BuildChannelCreated(string channelName)
        {
            return new ReplicateResult(new[] { new ReplicateResultDetail(channelName, ReplicationAction.Created) });
        }

        public static ReplicateResult BuildChannelUpdated(string channelName)
        {
            return new ReplicateResult(new[] { new ReplicateResultDetail(channelName, ReplicationAction.Updated) });
        }

        public static ReplicateResult BuildChannelUnchanged(string channelName)
        {
            return new ReplicateResult(new[] { new ReplicateResultDetail(channelName, ReplicationAction.Unchanged) });
        }

        public static ReplicateResult BuildChannelDeleted(string channelName)
        {
            return new ReplicateResult(new[] { new ReplicateResultDetail(channelName, ReplicationAction.Deleted) });
        }

        public static ReplicateResult BuildEmpty()
        {
            return new ReplicateResult(new ReplicateResultDetail[] { });
        }

        private ReplicateResult(IEnumerable<ReplicateResultDetail> details)
        {
            _details = new List<ReplicateResultDetail>();
            _details.AddRange(details);
        }

        private readonly List<ReplicateResultDetail> _details;
    }
}
