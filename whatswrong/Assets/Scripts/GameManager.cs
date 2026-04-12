using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public string ChosenDaddy;
        public string DaddyReason;
        public string DaddyPersonality;
        public string Hint1;
        public string Object1;
        public string Hint2;
        public string Object2;
        public string Hint3;
        public string Object3;
        public int CluesRevealed = 0;

        public Dictionary<string, Daddy> Daddies = new Dictionary<string, Daddy>();

        public void Start()
        {
            if (GameSetup.Daddies != null)
                Daddies = GameSetup.Daddies;

            if (!string.IsNullOrEmpty(GameSetup.ChosenDaddyName))
                ChosenDaddy = GameSetup.ChosenDaddyName;

            PickRandomReason();
        }

        public void PickRandomReason()
        {
            if (Daddies.Count == 0)
            {
                Debug.LogError("No daddies to pick from");
                return;
            }

            if (!Daddies.ContainsKey(ChosenDaddy))
            {
                Debug.LogError("Chosen daddy not found: " + ChosenDaddy);
                return;
            }

            var chosenDaddy = Daddies[ChosenDaddy];
            int index = Random.Range(0, chosenDaddy.Reason.Count);
            var randomReason = Daddies[ChosenDaddy].Reason[index];

            DaddyPersonality = Daddies[ChosenDaddy].Personality;
            DaddyReason = randomReason.ReasonText;
            Hint1 = randomReason.Hint1;
            Hint2 = randomReason.Hint2;
            Hint3 = randomReason.Hint3;
            Object1 = randomReason.Object1;
            Object2 = randomReason.Object2;
            Object3 = randomReason.Object3;
        }

        public string RevealClue(string objectId)
        {
            if (objectId == Object1)
            {
                CluesRevealed++;
                return Hint1;
            }
            else if (objectId == Object2)
            {
                CluesRevealed++;
                return Hint2;
            }
            else if (objectId == Object3)
            {
                CluesRevealed++;
                return Hint3;
            }

            return "...";
        }
    }
}
