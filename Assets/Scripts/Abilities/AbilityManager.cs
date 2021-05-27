using System;
using UnityEngine;
using Grid = Systems.Grid;

namespace Abilities {
    public class AbilityManager : MonoBehaviour {
        private Ability _currentAbility = Ability.None;

        private void Awake() {
            Grid.GameEvents.OnEnterRelaxedState += ActivateAbility;
            Grid.GameEvents.OnExitRelaxedState += DeactivateAbility;
            Grid.GameEvents.OnSetCurrentAbilityTelekinesis += SetCurrentAbilityTelekinesis;
            Grid.GameEvents.OnSetCurrentAbilityAlternativeVision += SetCurrentAbilityAlternativeVision;
            Grid.GameEvents.OnSetCurrentAbilityNone += SetCurrentAbilityNone;
            OnNoneCrystalPushed();
        }

        //Used in AbilitySelectionSystem prefab
        public void OnTelekinesisCrystalPushed() {
            Grid.GameEvents.SetCurrentAbilityTelekinesis();
        }

        //Used in AbilitySelectionSystem prefab
        public void OnAlternativeVisionCrystalPushed() {
            Grid.GameEvents.SetCurrentAbilityAlternativeVision();
        }

        public void OnNoneCrystalPushed() {
            Grid.GameEvents.SetCurrentAbilityNone();
        }

        private void SetCurrentAbilityTelekinesis() {
            DeactivateAbility();
            _currentAbility = Ability.Telekinesis;
        }

        private void SetCurrentAbilityAlternativeVision() {
            DeactivateAbility();
            _currentAbility = Ability.AlternativeVision;
        }

        private void SetCurrentAbilityNone() {
            DeactivateAbility();
            Grid.GameEvents.EnterNormalState();
            _currentAbility = Ability.None;
        }

        private void DeactivateAbility() {
            switch (_currentAbility) {
                case Ability.AlternativeVision:
                    Grid.GameEvents.DeactivateAlternativeVision();
                    break;
                case Ability.Telekinesis:
                    Grid.GameEvents.DeactivateTelekinesis();
                    break;
                case Ability.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ActivateAbility() {
            switch (_currentAbility) {
                case Ability.AlternativeVision:
                    Grid.GameEvents.ActivateAlternativeVision();
                    break;
                case Ability.Telekinesis:
                    Grid.GameEvents.ActivateTelekinesis();
                    break;
                case Ability.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}