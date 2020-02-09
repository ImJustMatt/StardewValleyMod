﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeFauxMatt.CustomChores.Models;
using StardewValley;

namespace LeFauxMatt.CustomChores.Framework.Chores
{
    internal class PetTheAnimalsChore : BaseChore
    {
        private IList<FarmAnimal> _farmAnimals;
        private readonly bool _enableBarns;
        private readonly bool _enableCoops;
        private int _animalsPetted;

        public PetTheAnimalsChore(ChoreData choreData) : base(choreData)
        {
            ChoreData.Config.TryGetValue("EnableBarns", out var enableBarns);
            ChoreData.Config.TryGetValue("EnableCoops", out var enableCoops);

            _enableBarns = !(enableBarns is bool b1) || b1;
            _enableCoops = !(enableCoops is bool b2) || b2;
        }

        public override bool CanDoIt(bool today = true)
        {
            _animalsPetted = 0;

            _farmAnimals = (
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn", StringComparison.CurrentCultureIgnoreCase)) || 
                       (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop", StringComparison.CurrentCultureIgnoreCase))
                select farmAnimal).ToList();
            
            return _farmAnimals.Any();
        }

        public override bool DoIt()
        {
            foreach (var farmAnimal in _farmAnimals)
            {
                if (farmAnimal.wasPet)
                    continue;
                farmAnimal.pet(Game1.player);
                ++_animalsPetted;
            }

            return true;
        }

        public override IDictionary<string, Func<string>> GetTokens()
        {
            var tokens = base.GetTokens();
            tokens.Add("AnimalName", GetFarmAnimalName);
            tokens.Add("AnimalsPetted", GetAnimalsPetted);
            tokens.Add("WorkDone", GetAnimalsPetted);
            tokens.Add("WorkNeeded", GetWorkNeeded);
            return tokens;
        }

        private string GetFarmAnimalName()
        {
            var farmAnimals = (
                from farmAnimal in Game1.getFarm().getAllFarmAnimals()
                where (_enableBarns && farmAnimal.buildingTypeILiveIn.Value.Equals("Barn", StringComparison.CurrentCultureIgnoreCase)) ||
                      (_enableCoops && farmAnimal.buildingTypeILiveIn.Value.Equals("Coop", StringComparison.CurrentCultureIgnoreCase))
                select farmAnimal).ToList();
            return farmAnimals.Any() ? farmAnimals.Shuffle().First().Name : null;
        }
        private string GetAnimalsPetted() =>
            _animalsPetted.ToString(CultureInfo.InvariantCulture);

        private string GetWorkNeeded() =>
            _farmAnimals.Count.ToString(CultureInfo.InvariantCulture);
    }
}
