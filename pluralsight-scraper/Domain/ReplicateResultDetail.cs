namespace VH.PluralsightScraper.Domain
{
    internal class ReplicateResultDetail
    {
        public string ChannelName { get; }

        public ReplicationAction Action { get; }

        public ReplicateResultDetail(string channelName, ReplicationAction action)
        {
            ChannelName = channelName;
            Action = action;
        }
    }
}
