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
	/// A map is the top-level organizational unit of the gaming board. It contains
	/// of several continents.
	/// </summary>
	public class Map
	{
		/// <summary>
		/// A map consists of several continents.
		/// </summary>
		/// <value>The continents.</value>
		public List<Continent> Continents { get; private set; }

		/// <summary>
		/// Lookup regions on this map using their ID.
		/// </summary>
		/// <value>The regions.</value>
		public Dictionary<int, Region> Regions { get; private set; }

		public Map ()
		{
			Continents = new List<Continent> ();
			Regions = new Dictionary<int, Region> ();

			Logger.Info ("Map:\tInitialized.");
		}

		/// <summary>
		/// Adds the continent to the map.
		/// </summary>
		/// <param name="continent">Continent.</param>
		public void AddContinent (Continent continent)
		{
			if (!Continents.Contains (continent)) {
				Continents.Add (continent);
			}

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("Map:\tContinent {0} added.", continent.Id));
			}
		}

		/// <summary>
		/// Adds the region to the map.
		/// </summary>
		/// <param name="region">Region.</param>
		public void AddRegion (Region region)
		{
			Regions.Add (region.Id, region);

			if (Logger.IsDebug ()) {
				Logger.Debug (string.Format("Map:\tRegion {0} added.", region.Id));
			}
		}

		/// <summary>
		/// Return the continent with the given identifier.
		/// </summary>
		/// <returns>The continent with the given identifier.</returns>
		/// <param name="id">Identifier.</param>
		public Continent ContinentForId (int id)
		{
			foreach (Continent c in Continents) {
				if (c.Id == id) {
					return c;
				}
			}

			return null;
		}
	}
}

