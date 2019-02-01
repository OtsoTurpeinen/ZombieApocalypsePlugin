using System;
using System.Threading;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace ZombieApocalypsePlugin
{
    struct DeathSync
    {
        public PlayerDeathEvent ev;
        Role role;
        public DeathSync(PlayerDeathEvent _ev)
        {
            ev = _ev;
            role = _ev.Player.TeamRole.Role;
        }

        public bool valid
        {
            get { return ev.Player.TeamRole.Role == role; }
        }
    }
	class ZombieSpawnHandler : IEventHandlerPlayerDie
    {
		private readonly ZombieApocalypsePlugin plugin;
		private readonly Random random;
        private Queue<DeathSync> eventQue;

		public ZombieSpawnHandler(ZombieApocalypsePlugin plugin)
		{
			random = new Random();
            eventQue = new Queue<DeathSync>();
            this.plugin = plugin;
		}

        private bool ZombifyAllowed(PlayerDeathEvent ev)
        {
            int[] zombroles = plugin.GetConfigIntList("zombifiable_roles");
            int[] zombdmg = plugin.GetConfigIntList("zombify_damage_type");
            for (int i = 0; i < zombroles.Length; i++)
            {
                if ((int)ev.Player.TeamRole.Role == zombroles[i] || zombroles[i] == -1)
                {
                    for (int j = 0; j < zombdmg.Length; j++)
                    {
                        if ((int)ev.DamageTypeVar == zombdmg[j] || zombdmg[j] == -1) return true;
                    }
                }
            }
            return false;
        }

        private bool ZombiKillDamage(PlayerDeathEvent ev)
        {
            int[] zombdmg = plugin.GetConfigIntList("zombi_kill_damage_type");
            for (int i = 0; i < zombdmg.Length; i++)
            {
                if ((int)ev.DamageTypeVar == zombdmg[i] || zombdmg[i] == -1) return true;
            }
            return false;
        }

        private void ZombieResurection(PlayerDeathEvent ev)
        {
            eventQue.Enqueue(new DeathSync(ev));
            Timer timer = new Timer(ZombieResurectCallback);
            int zomb_min = plugin.GetConfigInt("zombi_rez_min_time");
            int zomb_max = plugin.GetConfigInt("zombi_rez_max_time");
            timer.Change((int)(random.NextDouble()*(float)(zomb_max-zomb_min))+zomb_min, Timeout.Infinite);
            // debug stuff
            plugin.Info(ev.Player.Name + " health " + ev.Player.GetHealth());
        }

        private int ZombieClassify(int i)
        {
            switch (i)
            {
                case (0):
                case (1):
                case (6):
                    return 0;
                case (3):
                case (5):
                case (9):
                case (10):
                case (15):
                case (16):
                case (17):
                    return 1;
                default:
                    return 2;
            }
        }

        private void ZombieResurectCallback(object o)
        {
            ((Timer)o).Dispose();
            DeathSync dev = eventQue.Dequeue();
            PlayerDeathEvent ev = dev.ev;
            plugin.Info(ev.Player.Name + " health " + ev.Player.GetHealth());
            int role_i = plugin.GetConfigInt("zombi_role");
            if (dev.valid)
            {
                int cls = ZombieClassify(role_i);
                if (cls < 2 && plugin.Server.Map.WarheadDetonated)
                {
                    return;
                }

                if (cls < 1 && plugin.Server.Map.LCZDecontaminated)
                {
                    return;
                }
                if (ev.DamageTypeVar == DamageType.WALL || ev.DamageTypeVar == DamageType.TESLA || ev.DamageTypeVar == DamageType.LURE || ev.DamageTypeVar == DamageType.DECONT) //check damage types that might get player stuck.
                {
                    ev.Player.ChangeRole((Role)role_i, true, true, true, true);
                }
                else
                {
                    ev.Player.ChangeRole((Role)role_i, true, false, true, true);
                }
            } 
        }

        private void ZombieRewards(PlayerDeathEvent ev)
        {

        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            int role_i = plugin.GetConfigInt("zombi_role");
            if (ev.Player.TeamRole.Role != (Role)role_i)
            {
                if (ZombifyAllowed(ev))
                {
                    ZombieResurection(ev);
                    plugin.Info(ev.Player.Name + " zombification! Damage type " + ev.DamageTypeVar);
                }
            }
            else
            {
                if (!ZombiKillDamage(ev))
                {
                    ZombieResurection(ev);
                    plugin.Info(ev.Player.Name + " resurecting!");
                } else
                {
                    ZombieRewards(ev);
                }
            }
        }
	}
}
