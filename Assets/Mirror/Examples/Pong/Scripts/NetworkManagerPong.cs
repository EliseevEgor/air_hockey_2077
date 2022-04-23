using UnityEngine;
using UnityEngine.Assertions;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace Mirror.Examples.Pong
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerPong : NetworkManager
    {
        public Transform leftRacketSpawn;
        public Transform rightRacketSpawn;
        GameObject ball;
        GameObject computer;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? leftRacketSpawn : rightRacketSpawn;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            // spawn ball if two players
            if (numPlayers == 2)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                NetworkServer.Spawn(ball);
            }

            if (numPlayers == 1)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                computer = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Computer"));
                Assert.IsTrue(ball != null);
                Assert.IsTrue(computer != null);
                NetworkServer.Spawn(ball);
                NetworkServer.Spawn(computer);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            if (computer != null)
                NetworkServer.Destroy(computer);
            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}
