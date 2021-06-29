using System.Collections.Generic;

namespace PoC.Example.Abstract.Capabilities.Common
{
    public class Person
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string PersonalNumber { get; set; }
        public string Id { get; set; }

        public List<Person> FavoriteFootballPlayers { get; set; } = new List<Person>();
    }
}