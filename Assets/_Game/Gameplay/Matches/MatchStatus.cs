namespace TerraToss.Gameplay
{
    /// <summary>Resolution state of a match.</summary>
    public enum MatchStatus
    {
        /// <summary>The match is still being played.</summary>
        InProgress,

        /// <summary>The match has been won.</summary>
        Won,

        /// <summary>The match has been lost (e.g. shots exhausted without a valid hit).</summary>
        Lost
    }
}
