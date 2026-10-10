namespace _Scripts.Models.Upgrade
{
    public class UpgradeStateItem
    {
        private int id;
        private bool isBought = false;
        private bool isSecret = true;

        public UpgradeStateItem(int id, bool isBought = false, bool isSecret = true)
        {
            this.id = id;
            
            if (isBought) Buy();
            if (!isSecret) Unsecret();
        }
        
        public bool IsSecret => isSecret;
        public bool IsBought => isBought;
        public int Id => id;
        
        public void Buy() => isBought = true;
        public void Unsecret() => isSecret = false;

        public void Reset()
        {
            isBought = false;
            isSecret = id != 0;
        }
    }
}