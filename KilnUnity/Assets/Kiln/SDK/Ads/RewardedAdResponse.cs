namespace Kiln
{
    public interface IRewardedAdResponse
    {
        bool GetWithReward();
        string GetPlacementID();
    }

    public class RewardedAdResponse : IRewardedAdResponse
    {
        private bool _rewardUser = false;
        public bool RewardUser { set { _rewardUser = value; } }

        public string GetPlacementID()
        {
            throw new System.NotImplementedException();
        }

        public bool GetWithReward()
        {
            return _rewardUser;
        }
    }
}