namespace SqlGenerator.Discover
{
    internal interface IDiscoverFieldDefFactory
    {
        IFieldDefStrategy GetDiscoverStrategy(DiscoverStrategy strategy);
    }
}