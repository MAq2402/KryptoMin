namespace KryptoMin.Domain.Entities
{
    public abstract class Entity
    {
        protected Entity(Guid partitionKey, Guid rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public Guid PartitionKey { get; protected set; }
        public Guid RowKey { get; protected set; }
        public string Id => string.Concat(RowKey.ToString(), PartitionKey.ToString());

        public override bool Equals(object obj)
        {
            var other = obj as Entity;

            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(other.Id))
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}