namespace _Scripts.Helpers
{
    public static class TextHelper
    {
        public static string ScoreViewFromNumber(this long number)
        {
            float numberFloat = number;
            if (numberFloat / 1000f >= 1 && numberFloat / 1_000_000f < 1)
                return GameText.Number(number / 1000m, "0.0") + GameText.Choose("K", " тыс.");
            else if (numberFloat / 1_000_000f >= 1 && numberFloat / 1_000_000_000f < 1)
                return GameText.Number(number / 1_000_000m, "0.0") + GameText.Choose("M", " млн");
            else if (numberFloat / 1_000_000_000f >= 1 && numberFloat / 1_000_000_000_000f < 1)
                return GameText.Number(number / 1_000_000_000m, "0.0") + GameText.Choose("B", " млрд");
            else if  (numberFloat / 1_000_000_000_000f >= 1)
                return GameText.Number(number / 1_000_000_000_000m, "0.0") + GameText.Choose("T", " трлн");

            return number.ToString();
        } 
    }
}