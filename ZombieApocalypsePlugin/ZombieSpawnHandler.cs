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
        public bool skip;
        public DeathSync(PlayerDeathEvent _ev)
        {
            ev = _ev;
            role = _ev.Player.TeamRole.Role;
            skip = false;
        }

        public bool valid
        {
            get { return ev.Player.TeamRole.Role == role; }
        }
    }
	class ZombieSpawnHandler : IEventHandlerPlayerDie, IEventHandlerDisconnect
    {
		private readonly ZombieApocalypsePlugin plugin;
		private readonly Random random;
        private List<DeathSync> eventQue;

		public ZombieSpawnHandler(ZombieApocalypsePlugin plugin)
		{
			random = new Random();
            eventQue = new List<DeathSync>();
            this.plugin = plugin;
		}


        public void OnDisconnect(DisconnectEvent ev)
        {
            for (int i = eventQue.Count-1; i > -1; i--)
            {
                if (eventQue[i].ev.Player.IpAddress == ev.Connection.IpAddress)
                {
                    DeathSync d = eventQue[i];
                    d.skip = true;
                    eventQue[i] = d;
                    plugin.Info(ev.Connection.IpAddress + " escaped the resurection by disconnecting! Hopefully crash averted.");
                }
            }
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
            eventQue.Add(new DeathSync(ev));
            Timer timer = new Timer(ZombieResurectCallback);
            int zomb_min = plugin.GetConfigInt("zombi_rez_min_time");
            int zomb_max = plugin.GetConfigInt("zombi_rez_max_time");
            timer.Change((int)(random.NextDouble()*(float)(zomb_max-zomb_min))+zomb_min, Timeout.Infinite);
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
            DeathSync dev = eventQue[0];
            eventQue.RemoveAt(0);
            if (dev.skip) return;
            PlayerDeathEvent ev = dev.ev;
            int role_i = plugin.GetConfigInt("zombi_role");
            if (dev.valid)
            {
                if (ev.DamageTypeVar == DamageType.WALL || ev.DamageTypeVar == DamageType.TESLA || ev.DamageTypeVar == DamageType.LURE || ev.DamageTypeVar == DamageType.DECONT) //check damage types that might get player stuck.
                {
                    //Thesse are to check what level of the map is disabled.
                    //If player dies in a way that is deemed problematic (makes the player stuck) then we want to respawn at normal habitat.
                    int cls = ZombieClassify(role_i);
                    if (cls < 2 && plugin.Server.Map.WarheadDetonated)
                    {
                        return;
                    }
                    if (cls < 1 && plugin.Server.Map.LCZDecontaminated)
                    {
                        return;
                    }
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
            if (ev.Player != null && ev.Player.TeamRole.Role != (Role)role_i)
            {
                if (ZombifyAllowed(ev))
                {
                    ZombieResurection(ev);
                }
            }
            else
            {
                if (!ZombiKillDamage(ev))
                {
                    ZombieResurection(ev);
                } else
                {
                    ZombieRewards(ev);
                }
            }
        }
	}
}
