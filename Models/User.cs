namespace Web_API.Models
{
    public class User
    {
        public string Email { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;        
        public bool Enabled { get; set; }
        public List<Order>? Order { get; set; }
    }
}
