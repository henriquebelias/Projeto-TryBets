using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TryBets.Odds.Models;

namespace TryBets.Odds.Repository;

public class OddRepository : IOddRepository
{
    protected readonly ITryBetsContext _context;

    public OddRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public Match Patch(int MatchId, int TeamId, string BetValue)
    {
        string BetValueConverted = BetValue.Replace(",", ".");
        decimal BetValueDecimal = Decimal.Parse(BetValueConverted, CultureInfo.InvariantCulture);

        Match foundMatch =
            _context.Matches.FirstOrDefault(m => m.MatchId == MatchId)
            ?? throw new BadHttpRequestException("Invalid Match ID");

        Team foundTeam =
            _context.Teams.FirstOrDefault(t => t.TeamId == TeamId)
            ?? throw new BadHttpRequestException("Team Not Founded");

        if (foundMatch!.MatchTeamAId == TeamId)
        {
            foundMatch.MatchTeamAValue += BetValueDecimal;
        }
        else if (foundMatch.MatchTeamBId == TeamId)
        {
            foundMatch.MatchTeamBValue += BetValueDecimal;
        }
        else
        {
            throw new BadHttpRequestException("Invalid Match ID");
        }

        _context.Matches.Update(foundMatch);
        _context.SaveChanges();

        return new Match
        {
            MatchId = foundMatch.MatchId,
            MatchDate = foundMatch.MatchDate,
            MatchTeamAId = foundMatch.MatchTeamAId,
            MatchTeamBId = foundMatch.MatchTeamBId,
            MatchTeamAValue = foundMatch.MatchTeamAValue,
            MatchTeamBValue = foundMatch.MatchTeamBValue,
            MatchFinished = foundMatch.MatchFinished,
            MatchWinnerId = foundMatch.MatchWinnerId,
        };
    }
}
