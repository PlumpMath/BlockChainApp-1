﻿using System.Collections.Generic;
using Logic.ExchangeUsers;
using Logic.Participants;
using Utilities.Common;

namespace Logic.Fabrics
{
    public interface IIndividualUserFabric : IFabricBase<IndividualUser> { }

    public class IndividualUserFabric : FabricBase<IndividualUser>, IIndividualUserFabric
    {
        protected override IEnumerable<string> EntityNames => new string[]
        {
            "Noah",
            "Liam",
            "Mason",
            "Jacob",
            "William",
            "Ethan",
            "James",
            "Alexander",
            "Michael",
            "Benjamin",
            "Elijah",
            "Daniel",
            "Aiden",
            "Logan",
            "Matthew",
            "Lucas",
            "Jackson",
            "David",
            "Oliver",
            "Jayden",
            "Joseph",
            "Gabriel",
            "Samuel",
            "Carter",
            "Anthony",
            "John",
            "Dylan",
            "Luke",
            "Henry",
            "Andrew",
            "Isaac",
            "Christopher",
            "Joshua",
            "Wyatt",
            "Sebastian",
            "Owen",
            "Caleb",
            "Nathan",
            "Ryan",
            "Jack",
            "Hunter",
            "Levi",
            "Christian",
            "Jaxon",
            "Julian",
            "Landon",
            "Grayson",
            "Jonathan",
            "Isaiah",
            "Charles",
            "Thomas",
            "Aaron",
            "Eli",
            "Connor",
            "Jeremiah",
            "Cameron",
            "Josiah",
            "Adrian",
            "Colton",
            "Jordan",
            "Brayden",
            "Nicholas",
            "Robert",
            "Angel",
            "Hudson",
            "Lincoln",
            "Evan",
            "Dominic",
            "Austin",
            "Gavin",
            "Nolan",
            "Parker",
            "Adam",
            "Chase",
            "Jace",
            "Ian",
            "Cooper",
            "Easton",
            "Kevin",
            "Jose",
            "Tyler",
            "Brandon",
            "Asher",
            "Jaxson",
            "Mateo",
            "Jason",
            "Ayden",
            "Zachary",
            "Carson",
            "Xavier",
            "Leo",
            "Ezra",
            "Bentley",
            "Sawyer",
            "Kayden",
            "Blake",
            "Nathaniel"
        };
    }
}