using System;
using UnityEngine;

namespace Systems {
    public class PositionSecretary : MonoBehaviour {
        public Transform playerTransform;

        private const float TIME_BETWEEN_POSITION_NOTES = 0.5f;

        private float _timeSinceLastPositionNote;
        private DataExporter _dataExporter;

        private void Start() {
            _dataExporter = Grid.DataExporter;
        }

        private void NotePlayersPosition() {
            _dataExporter.NotePosition(playerTransform.position);
        }

        private void FixedUpdate() {
            if (_timeSinceLastPositionNote >= TIME_BETWEEN_POSITION_NOTES) {
                NotePlayersPosition();
                _timeSinceLastPositionNote -= TIME_BETWEEN_POSITION_NOTES;
            }
            else {
                _timeSinceLastPositionNote += Time.deltaTime;
            }
        }
    }
}