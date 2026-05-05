namespace _Scripts.Helpers
{
    public static class TextHelper
    {
        public static string ScoreViewFromNumber(this ulong number)
        {
            float numberFloat = number;
            if (numberFloat / 1000f >= 1 && numberFloat / 1_000_000f < 1)
                return $"{(numberFloat / 1000f).ToString("0.0")}K";
            else if (numberFloat / 1_000_000f >= 1 && numberFloat / 1_000_000_000f < 1)
                return $"{(numberFloat / 1_000_000f).ToString("0.0")}M";
            else if (numberFloat / 1_000_000_000f >= 1 && numberFloat / 1_000_000_000_000f < 1)
                return $"{(numberFloat / 1_000_000_000f).ToString("0.0")}B";
            else if  (numberFloat / 1_000_000_000_000f >= 1)
                return $"{(numberFloat / 1_000_000_000_000f).ToString("0.0")}T";

            return number.ToString();
        } 
    }
}