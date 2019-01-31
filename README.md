# ZombieApocalypsePlugin
Zombie Apocalypse Plugin for SCP: Secret Laboratory smod server.

## Install instructions
Drop the .dll file into your `SCP Secret Laboratory Dedicated Server\sm_plugins` folder.
For extra config stuff remeber to edit your `%AppData%\Roaming\SCP Secret Laboratory\config_gameplay.txt`

## Requirements to build
You need the smod source https://github.com/Grover-c13/Smod2

Create folder for this project within your Smod2 folder

## Features
When player dies to damage from zombie or scp-49, they resurect after short duration. Zombies can only be killed permanently with Grenades, Crushing or Tesla (plus obvious nuke and decontamination).

## Configs

Config Option | Value Type | Default Value | Description
------------ | ------------- | ------------- | -------------
zombifiable_roles | Int List | 1, 4, 6, 8, 11, 12, 13, 15 | Roles that can become zombies, -1 for any
zombify_damage_type | Int List | 18, 19 | Damage types that turn players into zombies, -1 for any
zombi_rez_min_time | Int | 2000 | Minimum time zombie resurects (milliseconds)
zombi_rez_max_time | Int | 3000 | Maxumum time zombie resurects (milliseconds)
zombi_kill_damage_type | Int | 17,10, 5, 3, 2, 1, 0 | Damage types that permanently kills a zombie
zombi_role | Int | 10 | Role that counts as zombie (you can have your 173 apocalypse if you want)
