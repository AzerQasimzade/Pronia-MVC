namespace ProniaProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public bool? Status { get; set; }
        public decimal Total { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
