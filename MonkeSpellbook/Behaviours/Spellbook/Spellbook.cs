using System.Collections.Generic;
using MonkeSpellbook.Behaviours.Spells;
using PDollarGestureRecognizer;
using UnityEngine;

namespace MonkeSpellbook.Behaviours.Spellbook
{
    public class Spellbook : MonoBehaviour
    {
        private readonly Spell[] _spells =
        {
            new CloudGrab()
        };

        private Dictionary<string, Spell> _spellMap;

        private void Awake()
        {
            _spellMap = new Dictionary<string, Spell>();

            foreach (var spell in _spells)
            {
                spell.Initialise();
                _spellMap[spell.Name] = spell;
            }
        }

        public void HandleGesture(Result result)
        {
            if (result.Score < 0.8f)
                return;

            if (!_spellMap.TryGetValue(result.GestureClass, out var spell))
                return;

            spell.IsActive = !spell.IsActive;

            if (spell.IsActive)
                spell.Activate();
            else
                spell.Deactivate();
        }
    }
}