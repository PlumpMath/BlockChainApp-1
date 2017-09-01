using System;
using Logic.Interfaces;

namespace Logic.Entitites
{
    public class User : ExchangeUserBase
    {
        public User(string name) :base()
        {
            Name = name;
        }
    }
}