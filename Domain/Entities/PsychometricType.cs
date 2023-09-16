namespace Domain.Entities
{
    public class PsychometricType
    {
        public PsychometricType()
        {
            // IsDeleted = false;
            // CreatedDate = DateTime.Now;
        }

        public string Name { get; set; }
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}