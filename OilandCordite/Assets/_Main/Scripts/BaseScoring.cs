public class BaseScoring
{
    //Flat Scores
    public const int PAR_TIME_SCORE = 5000;
    public const int COMBO_BONUS = 100;

    //Score Parameters
    public const float COMBO_TIME = 4f;
    public const int MAX_COMBO = 10;

    public enum Rank
    {
        Emerald = 0,
        Gold = 1,
        Silver = 2,
        Bronze = 3,
        None = 4,
    }
}
