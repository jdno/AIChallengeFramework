﻿//
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
	/// <summary>
	/// The state of the game is the most important source of information for the
	/// bot. All information provided by the game engine gets processed and stored.
	/// </summary>
	public class State
	{
		/// <summary>
		/// The name of your bot, as it is given by the game engine.
		/// </summary>
		/// <value>The name of your bot.</value>
		public string MyName { get; set; }

		/// <summary>
		/// The name of the enemy's bot, as it is given by the game engine.
		/// </summary>
		/// <value>The name of the enemy.</value>
		public string EnemyName { get; set; }

		/// <summary>
		/// This map contains all information that is available.
		/// </summary>
		/// <value>The complete map.</value>
		public Map CompleteMap { get; set; }

		/// <summary>
		/// This map only contains the part that is visible to the user.
		/// </summary>
		/// <value>The visible map.</value>
		public Map VisibleMap { get; set; }

		/// <summary>
		/// Keeps track which player owns which continent.
		/// </summary>
		/// <value>The owned continents.</value>
		public Dictionary<Continent, string> OwnedContinents { get; private set; }

		/// <summary>
		/// Keeps track of how many armies you can place per turn.
		/// </summary>
		/// <value>My armies per turn.</value>
		public int MyArmiesPerTurn { get; set; }

		/// <summary>
		/// Keeps track of how many armies your enemy can place per turn.
		/// </summary>
		/// <value>The enemy armies per turn.</value>
		public int EnemyArmiesPerTurn { get; set; }

		/// <summary>
		/// The moves of your opponent in the last turn. This lists gets
		/// refreshed every turn, so if you want to recognize trends in the
		/// enemy bot, please do no rely on this attribute.
		/// </summary>
		/// <value>The enemy moves in the last turn.</value>
		public List<Move> EnemyMoves { get; set; }

		public State ()
		{
			CompleteMap = new Map ();
			VisibleMap = new Map ();
			OwnedContinents = new Dictionary<Continent, string> ();
			EnemyMoves = new List<Move> ();
		}

		/// <summary>
		/// Check if a player currently is entitled to rewards for holding a
		/// country. Also keep track of changes to correctly calculate the
		/// number of armies the users are entitled to per turn.
		/// </summary>
		public void CheckRewards ()
		{
			string owner;

			foreach (Continent c in VisibleMap.Continents) {
				owner = c.OwnedBy ();

				if (owner.Equals (null)) {
					if (OwnedContinents.ContainsKey (c)) {
						if (OwnedContinents [c].Equals (MyName)) {
							MyArmiesPerTurn -= c.Reward;
						} else {
							EnemyArmiesPerTurn -= c.Reward;
						}

						OwnedContinents.Remove (c);
					}
				} else if (owner.Equals (MyName)) {
					if (!OwnedContinents.ContainsKey (c)) {
						OwnedContinents.Add (c, MyName);
						MyArmiesPerTurn += c.Reward;
					}
				} else {
					if (!OwnedContinents.ContainsKey (c)) {
						OwnedContinents.Add (c, MyName);
						EnemyArmiesPerTurn += c.Reward;
					}
				}
			}
		}

		/// <summary>
		/// Process a AttackTransferMove, and updates the number of armies
		/// in the source and target region.
		/// </summary>
		/// <param name="move">Move.</param>
		public void ProcessAttackTransferMove (AttackTransferMove move)
		{
			if (VisibleMap.Regions.ContainsKey (move.SourceRegion.Id)) {
				Region sourceRegion = VisibleMap.Regions[move.SourceRegion.Id];
				sourceRegion.Armies -= move.Armies;
			}

			if (VisibleMap.Regions.ContainsKey (move.TargetRegion.Id)) {
				Region targetRegion = VisibleMap.Regions[move.TargetRegion.Id];
				targetRegion.Armies += move.Armies;
			}
		}

		/// <summary>
		/// Processes a PlaceArmiesMove, and updates the number of armies
		/// in the target region.
		/// </summary>
		/// <param name="move">Move.</param>
		public void ProcessPlaceArmiesMove (PlaceArmiesMove move)
		{
			if (VisibleMap.Regions.ContainsKey (move.Region.Id)) {
				Region region = VisibleMap.Regions[move.Region.Id];
				region.Armies -= move.Armies;
			}
		}

		/// <summary>
		/// Updates the map.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="regionId">Region identifier.</param>
		/// <param name="armies">Armies.</param>
		public void UpdateMap (string player, int regionId, int armies)
		{
			Region region;

			if (!VisibleMap.Regions.ContainsKey (regionId)) {
				VisibleMap.AddRegion (CompleteMap.Regions [regionId]);
			}

			region = VisibleMap.Regions [regionId];
			region.Owner = player;
			region.Armies = armies;
		}
	}
}
