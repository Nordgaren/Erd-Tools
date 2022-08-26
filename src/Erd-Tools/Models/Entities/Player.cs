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
    }
}
