namespace my_journey_journal.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public string EntryName { get; set; }
        public string? EntryDetails { get; set; }
        public DateTime? DateCreated { get; set; }


        public JournalEntry()
        {
            
        }
    }
}
