using Smod2;
using Smod2.Attributes;
using Smod2.API;

namespace ZombieApocalypsePlugin
{
	[PluginDetails(
		author = "DrTeaSpoon",
		name = "Zombie Apocalypse",
		description = "Pluging for cool zombie stuff",
		id = "drteaspoon.zombie.plugin",
		version = "1.9",
		SmodMajor = 3,
		SmodMinor = 0,
		SmodRevision = 0
		)]
	class ZombieApocalypsePlugin : Plugin
	{
		public override void OnDisable()
		{
			this.Info(this.Details.name + " was disabled");
		}

		public override void OnEnable()
        {
            this.Info(this.Details.name + " has loaded");
        }

		public override void Register()
		{
            this.AddConfig(new Smod2.Config.ConfigSetting("zombifiable_roles", new int[8] { 1, 4, 6, 8, 11, 12, 13, 15 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "Roles that can turn into zombies."));
            this.AddConfig(new Smod2.Config.ConfigSetting("zombify_damage_type", new int[2] { 18, 19 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "Damage types that induce zombification."));
            this.AddConfig(new Smod2.Config.ConfigSetting("zombi_rez_min_time", 2000, Smod2.Config.SettingType.NUMERIC, true, "Min Zombie resurection time"));
            this.AddConfig(new Smod2.Config.ConfigSetting("zombi_rez_max_time", 3000, Smod2.Config.SettingType.NUMERIC, true, "Max Zombie resurection time"));
            this.AddConfig(new Smod2.Config.ConfigSetting("zombi_kill_damage_type", new int[7] { 17,10, 5, 3, 2, 1, 0 }, Smod2.Config.SettingType.NUMERIC_LIST, true, "Damage types that can kill zombies completely. Others let the zombie resurect."));
            this.AddConfig(new Smod2.Config.ConfigSetting("zombi_role", (int)Role.SCP_049_2, Smod2.Config.SettingType.NUMERIC, true, "Role that acts as Zombie."));
            this.AddEventHandlers(new ZombieSpawnHandler(this));
		}
	}
}
