using System.Net.Http;

namespace TryBets.Bets.Services;

public class OddService : IOddService
{
    private readonly HttpClient _client;

    public OddService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object> UpdateOdd(int MatchId, int TeamId, decimal BetValue)
    {
        var response = await _client.GetAsync(
            $"https://localhost:5004/api/odd/{MatchId}/{TeamId}/{BetValue}"
        );

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<object>();

            return content!;
        }

        return default!;
    }
}
