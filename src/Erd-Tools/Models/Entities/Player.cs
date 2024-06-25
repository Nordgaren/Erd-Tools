using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;

namespace Erd_Tools.Models.Entities
{
    public class Player : Chr
    {
        public Player(PHPointer enemyIns, ErdHook hook) : base(enemyIns, hook)
        {

        }

        public void LevelUpPlayer(int vigor, int mind, int endurance, int strength, int dexterity, int intelligence, int faith, int arcane)
        {
            _hook.LevelUpPlayer(vigor, mind, endurance, strength, dexterity, intelligence, faith, arcane);
        }

        private int _vigor;
        public int Vigor
        {
            get => _vigor;
            set => _vigor = value;
        }
        private int _mind;
        public int Mind
        {
            get => _mind;
            set => _mind = value;
        }

        private int _endurance;
        public int Endurance
        {
            get => _endurance;
            set => _endurance = value;
        }

        private int _strength;
        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        private int _dexterity;
        public int Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        private int _intelligence;
        public int Intelligence
        {
            get => _intelligence;
            set => _intelligence = value;
        }

        private int _faith;
        public int Faith
        {
            get => _faith;
            set => _faith = value;
        }

        private int _arcane;
        public int Arcane
        {
            get => _arcane;
            set => _arcane = value;
        }
    }
}