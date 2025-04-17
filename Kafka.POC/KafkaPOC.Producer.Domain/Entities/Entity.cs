namespace KafkaPOC.Producer.Domain.Entities
{
    public abstract class Entity<T>
    {
        #region Props
        public virtual T Id { get; protected set; }

        public DateTime CreationDate { get; protected set; }
        #endregion
    }
}
