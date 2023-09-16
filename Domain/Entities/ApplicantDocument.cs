namespace Domain.Entities
{
    public class ApplicantDocument : BaseEntity
    {
        public Guid ApplicantId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Comment { get; set; }
        public string FileUrl { get; set; }

        public Guid DocuemntType { get; set; }

        public string DocumentTitle { get; set; }
        public string? DocuemntTypeName { get; set; }
    }
}