using Billing;
using BillingServer;
using Grpc.Core;

namespace BillingService.Services
{

    public class MyBillingService : BillingServ.BillingServBase
    {
        private readonly ILogger<MyBillingService> _logger;
        private readonly DataStorage _data;

        public MyBillingService(ILogger<MyBillingService> logger, DataStorage data)
        {
            _logger = logger;
            _data = data;
        }

        public override async Task ListUsers(None request, IServerStreamWriter<UserProfile> responseStream, ServerCallContext context)
        {
            foreach(var user in _data.users)
            {
                var userProto = new UserProfile() { Name = user.Name, Amount = user.Amount };
                await responseStream.WriteAsync(userProto);
            }
        }

        public override async Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            return await Task.FromResult(EmisionPerforming(request.Amount));
        }

        public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
        {
            var srcUser = _data.users.Where(x => x.Name == request.SrcUser).First();
            var destUser = _data.users.Where(x => x.Name == request.DstUser).First();

            if (srcUser == null || destUser == null)
                return Task.FromResult(new Response() { Status = Response.Types.Status.Failed, Comment = "Source/Destinate user not found" });

            if (srcUser.Amount < request.Amount)
                return Task.FromResult(new Response() { Status = Response.Types.Status.Failed, Comment = "Source user don't have enough coins" });

            var toTansact = request.Amount;
            while (toTansact > 0) 
            {
                var coin = srcUser.Coins[0];
                coin.Transfer(destUser.Name);
                srcUser.Coins.RemoveAt(0);
                destUser.Coins.Add(coin);
                toTansact--;
            }
            
            return Task.FromResult(new Response() { Status = Response.Types.Status.Ok, Comment = "Transaction compleeted" });
        }

        public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
        {
            return Task.FromResult(new Coin() { History = CoinModel.longestCoin.History, Id = CoinModel.longestCoin.Id });
        }

        private Response EmisionPerforming(long amount)
        {
            long remainAmaunt = amount;
            long totalAmount = amount;

            if (amount < _data.users.Count)
                return new Response() { Status = Response.Types.Status.Failed, Comment = "Emission is not enough" };

            foreach(var user in _data.users)
            {
                var coin = new CoinModel();
                coin.Transfer(user.Name);
                user.Coins.Add(coin);
            }

            totalAmount -= _data.users.Count;
            remainAmaunt -= _data.users.Count;

            var TotalRaiting = 0;
            foreach (var user in _data.users)
            {
                TotalRaiting += user.Raiting;
            }

            var usersSorted = _data.users.OrderByDescending(x => x.Raiting);

            foreach (var user in usersSorted)
            {
                var gain = (double)user.Raiting / (double)TotalRaiting;
                var coinsCount = (long)Math.Round((double)totalAmount * gain, MidpointRounding.AwayFromZero);
                if (coinsCount == 0 && remainAmaunt > 0) coinsCount = 1;
                if (remainAmaunt - coinsCount >= 0)
                {
                    for (var i = 0; i < coinsCount; i++)
                    {
                        var coin = new CoinModel();
                        coin.Transfer(user.Name);
                        user.Coins.Add(coin);
                    }
                    remainAmaunt -= coinsCount;
                }
            }

            return new Response() { Status = Response.Types.Status.Ok, Comment = "Emission compleeted" };
        }
    }
}