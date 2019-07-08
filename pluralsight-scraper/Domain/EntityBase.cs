namespace VH.PluralsightScraper.Domain
{
    internal abstract class EntityBase : IDomainEntity
    {
        public int Id { get; private set; }
    }
}
