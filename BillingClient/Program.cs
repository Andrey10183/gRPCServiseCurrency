using Grpc.Net.Client;
using BillingClient;
using Grpc.Core;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7011");
var client = new BillingServ.BillingServClient (channel);

async Task PrintUserList()
{
    Console.WriteLine("Balance");
    using (var call = client.ListUsers(new None()))
    {
        while (await call.ResponseStream.MoveNext())
        {
            var current = call.ResponseStream.Current;
            Console.WriteLine("{0},{1}", current.Name, current.Amount);
        }
    }
}

await PrintUserList();

Console.WriteLine("Coin emmision - 10 coin");
var responce = new Response();
var emission = new EmissionAmount() { Amount = 10 };
responce = client.CoinsEmission(emission);
Console.WriteLine(responce.Comment);

await PrintUserList();

Console.WriteLine("Coin transaction boris->maria - 5 coin");
var data = new MoveCoinsTransaction() { SrcUser = "boris", DstUser = "maria", Amount = 5 };
responce = client.MoveCoins(data);
Console.WriteLine(responce.Comment);

await PrintUserList();


Console.WriteLine("Requesting longest history coin");
var none = new None();
var longestCoin = client.LongestHistoryCoin(none);
Console.WriteLine("id = {0}, history = {1}", longestCoin.Id, longestCoin.History);

Console.ReadKey();