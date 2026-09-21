namespace TheEyeOfOden.Radar
{
    public enum EntityCategory
    {
        Unknown,
        Hostile,
        AlertedHostile,
        Passive,
        Tamed,
        Boss,
        Player,
        Npc
    }

    public enum DisplayMode
    {
        DotsOnly,
        IconsWithDotsFallback,
        IconsOnly
    }
}
