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
	/// <summary>
	/// A continent contains multiple regions, and provides a reward to the
	/// player who holds all those regions.
	/// 
	/// A continent is identified by its ID, and provides methods to access
	/// its regions and check if it is owned by some one.
	/// </summary>
	public class Continent
	{
		/// <summary>
		/// If the classic map is played, you can use these enums to map between
		/// readable names and the IDs in the system. Please note that, if for
		/// whatever reason, another map is played, those mappings are meaningless.
		/// </summary>
		public enum Name
		{
			NorthAmerica = 1,
			SouthAmerica = 2,
			Europe = 3,
			Africa = 4,
			Asia = 5,
			Australia = 6
		}

		/// <summary>
		/// A continent is identified by a single integer value.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id { get; private set; }

		/// <summary>
		/// A continent offers a reward for the player that fully owns it.
		/// </summary>
		/// <value>The reward.</value>
		public int Reward { get; set; }

		/// <summary>
		/// A continent consists of several regions.
		/// </summary>
		/// <value>The regions.</value>
		public List<Region> Regions { get; private set; }

		/// <summary>
		/// A border territory is a region that has a connection to a
		/// foreign continent.
		/// </summary>
		private int numberOfBorderTerritories = -1;

		/// <summary>
		/// Every connection between a region of this continent and
		/// neighboring regions on other continents counts as an invasion
		/// path.
		/// </summary>
		private int numberOfInvasionPaths = -1;

		/// <summary>
		/// Every continent has a priority, which indicates how "interesting"
		/// it is. You can use this value to give your bot directions, and
		/// extend the method if the default algorithm does not fit your plan.
		/// </summary>
		private int priority = -1;

		public Continent (int id, int reward)
		{
			this.Id = id;
			this.Reward = reward;
			Regions = new List<Region> ();

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("Continent:\tInitialized with id {0} and reward {1}.", id, reward));
			} else {
				Logger.Info (string.Format("Continent:\tInitialized with id {0}.", id));
			}
		}

		/// <summary>
		/// Adds a region to the continent.
		/// </summary>
		/// <param name="region">Region.</param>
		public void AddRegion (Region region)
		{
			if (!Regions.Contains(region)) Regions.Add (region);

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("Continent:\tRegion {0} added to continent {1}", region.Id, Id));
			}
		}

		/// <summary>
		/// Returns the owner of a continent. If the continent is
		/// not fully owned by a single player, null is returned.
		/// </summary>
		/// <returns>The owner of the continent, or <c>null</c>.</returns>
		public string OwnedBy () {
			if (Regions.Count > 0) {
				string owner = Regions [0].Owner;

				foreach (Region r in Regions) {
					if (!owner.Equals (r.Owner)) {
						return "unkown";
					}
				}

				return owner;
			} else {
				return "unknown";
			}
		}


		/// <summary>
		/// Calculate the number of border territories.
		/// </summary>
		/// <returns>The of border territories.</returns>
		public int NumberOfBorderTerritories () {
			if (numberOfBorderTerritories > 0) {
				return numberOfBorderTerritories;
			}
				
			HashSet<Region> borderTerritories = new HashSet<Region> ();

			foreach (Region r in Regions) {
				foreach (Region n in r.Neighbors) {
					if (!n.Continent.Equals (this)) {
						borderTerritories.Add (r);
					}
				}
			}

			numberOfBorderTerritories = borderTerritories.Count;

			return numberOfBorderTerritories;
		}

		/// <summary>
		/// Calculate the numbers the of invasion paths.
		/// </summary>
		/// <returns>The number of invasion paths.</returns>
		public int NumberOfInvasionPaths () {
			if (numberOfInvasionPaths > 0) {
				return numberOfInvasionPaths;
			}

			numberOfInvasionPaths = 0;

			foreach (Region r in Regions) {
				foreach (Region n in r.Neighbors) {
					if (!n.Continent.Equals (this)) {
						numberOfInvasionPaths += r.NumberOfEnemyNeighbors ();
					}
				}
			}

			return numberOfInvasionPaths;
		}

		/// <summary>
		/// Calculate the priority of this continent, which is the product of the
		/// number of invasion paths and the number of border territories.
		/// 
		/// If you need another algorithm to calculate the priority, simple extend
		/// the class and overwrite this method.
		/// </summary>
		/// <returns>The priority.</returns>
		virtual public int Priority ()
		{
			if (priority >= 0) {
				return priority;
			}

			priority = NumberOfInvasionPaths () * NumberOfBorderTerritories ();

			return priority;
		}
	}
}

