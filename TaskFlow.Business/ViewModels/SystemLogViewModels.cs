namespace TaskFlow.Business.ViewModels
{
    public class SystemLogViewModel
    {
        public Guid guid { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int? UserId { get; set; }
        public int? StudentId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int AffectedRecord { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
