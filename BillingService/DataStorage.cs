using BillingService.Services;

namespace BillingServer
{
    public class DataStorage
    {
        public List<UserModel> users = new()
        {
            new UserModel {Name="boris", Raiting = 5000},
            new UserModel {Name="maria", Raiting = 1000},
            new UserModel {Name="oleg", Raiting = 800},
        };

        public CoinModel LongestHistoryCoin = new();

        public DataStorage()
        {
            Data = users;
        }

        public List<UserModel> Data { get; set; }
    }
}
