[System.Flags]
public enum TeamColorEnum
{
    None = 0,
    Blue = 1 << 0,
    Red = 1 << 1,
    Neutral = Blue | Red
}
