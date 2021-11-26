namespace Web_API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public User? User { get; set; }
    }
}
