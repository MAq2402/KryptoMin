namespace KryptoMin.Domain.Entities
{
    public abstract class Entity
    {
        public Guid PartitionKey { get; protected set; }
        public Guid RowKey { get; protected set; }
    }
}