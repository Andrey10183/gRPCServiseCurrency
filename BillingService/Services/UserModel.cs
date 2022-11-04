namespace BillingService.Services
{
    public class UserModel
    {
        public string Name { get; set; }
        public int Amount { get { return Coins.Count; } }
        public int Raiting { get; set; }

        public List<CoinModel> Coins = new();
    }
}