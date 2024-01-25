using Reptile;
using UnityEngine;

namespace MoreMap
{
    public class TempRacerLink : MonoBehaviour
    {
        private MapPin linkedPin;

        public void SetPin(MapPin pin)
        {
            linkedPin = pin;
        }

        private void OnDestroy()
        {
            Core.Logger.LogInfo("Linked temp racer is being destroyed!");
            if (PinManager.Instance != null)
            {
                if (PinManager.Instance.characterPins.Contains(linkedPin)) PinManager.Instance.characterPins.Remove(linkedPin);
                if (PinManager.Instance.tempRacerLinks.ContainsKey(linkedPin)) PinManager.Instance.tempRacerLinks.Remove(linkedPin);
                PinManager.Instance.RemovePin(linkedPin);
            }
            if (linkedPin != null) GameObject.Destroy(linkedPin.gameObject);
        }
    }
}
