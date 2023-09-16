using Domain.Shared;

namespace Domain.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            //this.Id = NewSequentialGuid.Create();
            this.CreatedDate = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }

        public string? CreatedByName { get; set; }

        public string? CreatedByIp { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedByIp { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}