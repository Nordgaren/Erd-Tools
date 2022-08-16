using PropertyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erd_Tools.Models
{
    public class Enemy : Chr
    {
        public Enemy(PHPointer enemyIns, ErdHook hook) : base(enemyIns, hook)
        {

        }
    }
}
