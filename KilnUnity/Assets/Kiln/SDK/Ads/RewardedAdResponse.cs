namespace Kiln
{
    public interface IRewardedAdResponse
    {
        bool getWithReward();
        string getPlacementID();
    }

    public class RewardedAdResponse : IRewardedAdResponse
    {
        protected bool _rewardUser = false;
        public bool RewardUser { set { _rewardUser = value; } }

        public string getPlacementID()
        {
            throw new System.NotImplementedException();
        }

        public bool getWithReward()
        {
            return _rewardUser;
        }
    }
}