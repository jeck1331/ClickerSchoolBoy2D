namespace _Scripts.Models.Upgrade
{
    public class UpgradeStateItem
    {
        public UpgradeStateItem(int id, bool isBought = false, bool isSecret = true)
            : this(id, isBought ? 1 : 0, isSecret)
        {
        }

        public UpgradeStateItem(int id, int count, bool isSecret)
        {
            Id = id;
            Count = count;
            IsSecret = isSecret;
        }

        public bool IsSecret { get; private set; }
        public bool IsBought => Count > 0;
        public int Count { get; private set; }
        public int Id { get; }

        public void Buy() => Count++;
        public void Unsecret() => IsSecret = false;

        public void Reset()
        {
            Count = 0;
            IsSecret = Id != 0;
        }
    }
}
