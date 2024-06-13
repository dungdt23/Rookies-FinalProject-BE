namespace AssetManagement.Domain.Common
{
    public class BaseEntity
    {
        public virtual Guid Id { get; set; } = Guid.NewGuid();
        public required DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public required bool IsDeleted { get; set; } = false;

    }
}
