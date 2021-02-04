namespace Kiln
{
    public interface ILeaderboardEntry
    {
        double GetScore();
        int GetRank();
        IPlayer GetPlayer();
        string ToString();
    }

    public class LeaderboardEntry : ILeaderboardEntry
    {
        private double _score;
        private int _rank;
        private Player _player;

        public LeaderboardEntry(double score, int rank, Player player)
        {
            _score = score;
            _rank = rank;
            _player = player;
        }

        public double GetScore()
        {
            return _score;
        }
        public int GetRank()
        {
            return _rank;
        }

        public IPlayer GetPlayer()
        {
            return _player;
        }

        new public string ToString()
        {
            return $"Player {GetPlayer().GetName()} - score: {GetScore()}, rank: {GetRank()}";
        }

    }   
}