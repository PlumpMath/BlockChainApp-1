namespace Logic.Participants
{
    public class User : ExchangeUserBase
    {
        public User() { }

        public User(string name) :base()
        {
            Name = name;
        }

        public override string UniqueExchangeId()
        {
            return nameof(User).ToLowerInvariant() + Id;
        }
    }
}