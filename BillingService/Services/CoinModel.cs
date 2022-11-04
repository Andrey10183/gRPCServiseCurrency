namespace BillingService.Services
{
    public class CoinModel
    {
        public static int currentId;
        public static CoinModel longestCoin;

        public CoinModel()
        {
            Id = ++currentId;
            History += "emission";
            if (longestCoin==null) longestCoin = this;
        }

        public int Id { get; set; }

        public string History { get; set; }

        public void Transfer(string destName)
        {
            History += "-" + destName;
            if (ChainLength(this) > ChainLength(longestCoin))
                longestCoin = this;
        }

        private int ChainLength(CoinModel coin)
        {
            var result = 0;
            foreach (var item in coin.History)
            {
                if (item == '-')
                    result++;
            }
            return result + 1;
        }
    }
}