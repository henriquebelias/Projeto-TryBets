using System.Linq;
using Microsoft.EntityFrameworkCore;
using TryBets.Bets.DTO;
using TryBets.Bets.Models;

namespace TryBets.Bets.Repository;

public class BetRepository : IBetRepository
{
    protected readonly ITryBetsContext _context;

    public BetRepository(ITryBetsContext context)
    {
        _context = context;
    }

    public BetDTOResponse Post(BetDTORequest betRequest, string email)
    {
        User foundUser =
            _context.Users.FirstOrDefault(x => x.Email == email)!
            ?? throw new Exception("User not founded");

        Match foundMatch =
            _context.Matches.FirstOrDefault(m => m.MatchId == betRequest.MatchId)!
            ?? throw new Exception("Match not founded");

        Team foundTeam =
            _context.Teams.FirstOrDefault(t => t.TeamId == betRequest.TeamId)!
            ?? throw new Exception("Team not founded");

        if (foundMatch.MatchFinished)
        {
            throw new Exception("Match finished");
        }

        if (
            foundMatch.MatchTeamAId != betRequest.TeamId
            && foundMatch.MatchTeamBId != betRequest.TeamId
        )
        {
            throw new Exception("Team is not in this match");
        }

        Bet newBet = new()
        {
            UserId = foundUser.UserId,
            MatchId = betRequest.MatchId,
            TeamId = betRequest.TeamId,
            BetValue = betRequest.BetValue,
        };

        _context.Bets.Add(newBet);
        _context.SaveChanges();

        Bet createdBet = _context
            .Bets.Include(b => b.Team)
            .Include(b => b.Match)
            .Where(b => b.BetId == newBet.BetId)
            .FirstOrDefault()!;

        return new BetDTOResponse
        {
            BetId = createdBet.BetId,
            MatchId = createdBet.MatchId,
            TeamId = createdBet.TeamId,
            BetValue = createdBet.BetValue,
            MatchDate = createdBet.Match!.MatchDate,
            TeamName = createdBet.Team!.TeamName,
            Email = createdBet.User!.Email,
        };
    }

    public BetDTOResponse Get(int BetId, string email)
    {
        User foundUser =
            _context.Users.FirstOrDefault(x => x.Email == email)!
            ?? throw new Exception("User not founded");

        Bet foundBet =
            _context
                .Bets.Include(b => b.Team)
                .Include(b => b.Match)
                .Where(b => b.BetId == BetId)
                .FirstOrDefault()! ?? throw new Exception("Bet not founded");

        if (foundBet.User!.Email != email)
        {
            throw new Exception("Bet view not allowed");
        }

        return new BetDTOResponse
        {
            BetId = foundBet.BetId,
            MatchId = foundBet.MatchId,
            TeamId = foundBet.TeamId,
            BetValue = foundBet.BetValue,
            MatchDate = foundBet.Match!.MatchDate,
            TeamName = foundBet.Team!.TeamName,
            Email = foundBet.User!.Email,
        };
    }
}
