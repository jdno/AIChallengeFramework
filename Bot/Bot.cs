//
//  Copyright 2014  jdno
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
using System;
using System.Collections.Generic;

namespace AIChallengeFramework
{
	public class Bot
	{
		/// <summary>
		/// This is the name your bot gets assigned by the game engine.
		/// You have no control over this, and its not the name that gets
		/// displayed on the website.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// The framework keeps track for you how much units you can place
		/// in a turn. The number is updated everytime you conquer or lose
		/// a continent.
		/// </summary>
		/// <value>The armies per turn.</value>
		public int ArmiesPerTurn { get; set; }

		/// <summary>
		/// This is a list of all turns a bot peformed last turn. You can use
		/// it to analyze the behavior of the enemy or improve yours.
		/// </summary>
		/// <value>The moves last turn.</value>
		public List<Move> MovesLastTurn { get; set; }

		/// <summary>
		/// Keeps track which continent you own.
		/// </summary>
		/// <value>The owned continents.</value>
		public HashSet<Continent> OwnedContinents { get; private set; }

		/// <summary>
		/// To function properly, every bot is initialized with the State
		/// (even if you don't actually use it).
		/// </summary>
		public readonly State State;

		public Bot (State state)
		{
			this.State = state;
			Name = "unset";
			ArmiesPerTurn = 5;
			MovesLastTurn = new List<Move> ();
			OwnedContinents = new HashSet<Continent> ();
		}

		/// <summary>
		/// At the start of the game, the game engine presents you with twelve
		/// regions from which you can choose your starting position. You MUST
		/// return 6 preferred starting regions.
		/// </summary>
		/// <returns>The six starting regions.</returns>
		/// <param name="regions">Regions.</param>
		virtual public List<Region> PreferredStartingRegions (List<Region> regions)
		{
			return regions.GetRange (0, 6);
		}

		/// <summary>
		/// The game engine asks the bot every turn which regions he wants to
		/// put his new units. You do not have to put those new units on the
		/// field, and can simply return an empty list, but in this case you
		/// loose the units forever.
		/// </summary>
		/// <returns>The armies.</returns>
		virtual public List<PlaceArmiesMove> PlaceArmies ()
		{
			return new List<PlaceArmiesMove> ();
		}

		/// <summary>
		/// The game engine asks the bot every turn for his attack or transfers
		/// moves. Create a list of your moves and return for further processing
		/// by the parser.
		/// </summary>
		/// <returns>The or transfer.</returns>
		virtual public List<AttackTransferMove> AttackOrTransfer ()
		{
			return new List<AttackTransferMove> ();
		}

		/// <summary>
		/// Checks if you own a given continent.
		/// </summary>
		/// <returns><c>true</c>, if continent was owned, <c>false</c> otherwise.</returns>
		/// <param name="continent">Continent.</param>
		public bool OwnsContinent (Continent continent)
		{
			if (OwnedContinents.Contains (continent)) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Called whenever you conquer a continent, updates your armies.
		/// </summary>
		/// <param name="continent">Continent.</param>
		public void GainContinent (Continent continent)
		{
			if (!OwnedContinents.Contains (continent)) {
				OwnedContinents.Add (continent);
				ArmiesPerTurn += continent.Reward;
			}
		}

		/// <summary>
		/// Called whenever you conquer a continent, updates your armies.
		/// </summary>
		/// <param name="continent">Continent.</param>
		public void LoseContinent (Continent continent)
		{
			if (OwnedContinents.Contains (continent)) {
				OwnedContinents.Remove (continent);
				ArmiesPerTurn -= continent.Reward;
			}
		}
	}
}

