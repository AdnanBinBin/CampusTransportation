namespace DAL.DB.Model
{
    public class SharedVehicule
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public int Capacity { get; set; }
        public int DriverId { get; set; }
    }
}
