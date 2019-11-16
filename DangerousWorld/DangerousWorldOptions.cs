using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;

using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PierreStirnweiss.DangerousWorld
{
    public enum FallDamageDifficulty
    {
        [Option("None", "Game default. No damage from falling")]
        None,
        [Option("Mild", "Reduced amount of damage. No Crippling/Death of Dupes for higher falls")]
        Mild,
        [Option("Normal", "Standard amount of damage. Crippled effect for higher falls. No instant death.")]
        Normal,
        [Option("High", "Standard amount of damage. Crippled and instant death activated.")]
        High,
        [Option("Custom", "Read options in the config/FallDamageConfig.json if present, otherwise fall back to Normal")]
        Custom
    }

    public struct FallDamageOptions
    {
        public int damageHeightLimit;
        public int damageDivider;

        public bool crippleEnabled;
        public int crippleHeightLimit;
        public int crippledDuration;
        public int crippledModifier;

        public bool deathEnabled;
        public int deathHeightLimit;

        public override string ToString()
        {
            return ("DangerousWorld.FallDamageOptions:\r\n Difficulty: {0}\r\n DamageHeightLimit: {1}\r\n" +
                " DamageDivider: {2}\r\n CrippleEnabled: {3}\r\n CrippleHeightLimit: {4}\r\n CrippledDuration: {5}\r\n" +
                " CrippledModifier: {6}\r\n DeathEnabled: {7}\r\n DeathHeightLimit: {8}").
                F(DangerousWorldOptions.Instance.FallDamageDifficultyOption.ToString(),
                DangerousWorldOptions.Instance.fallOptions.damageHeightLimit,
                DangerousWorldOptions.Instance.fallOptions.damageDivider,
                DangerousWorldOptions.Instance.fallOptions.crippleEnabled,
                DangerousWorldOptions.Instance.fallOptions.crippleHeightLimit,
                DangerousWorldOptions.Instance.fallOptions.crippledDuration,
                DangerousWorldOptions.Instance.fallOptions.crippledModifier,
                DangerousWorldOptions.Instance.fallOptions.deathEnabled,
                DangerousWorldOptions.Instance.fallOptions.deathHeightLimit);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    [RestartRequired]
    class DangerousWorldOptions : POptions.SingletonOptions<DangerousWorldOptions>
    {
        [Option("Fall damage difficulty", "How lethal falling should be.")]
        [JsonProperty]
        public FallDamageDifficulty FallDamageDifficultyOption { get; set; }

        [Option("Aggressive Critters (experimental)", "The Critter strikes back.")]//\r\nIndividual species options can be found config/AggroSpeciesConfig.json")]
        [JsonProperty]
        public bool AggroCrittersOption { get; set; }

        public FallDamageOptions fallOptions;
        public DangerousWorldOptions()
        {
            FallDamageDifficultyOption = FallDamageDifficulty.Normal;
            AggroCrittersOption = true;
        }

        public void InitialiseOptions()
        {
            var baseDir = POptions.GetModDir(Assembly.GetExecutingAssembly());

            switch (this.FallDamageDifficultyOption)
            {
                case FallDamageDifficulty.None:
                case FallDamageDifficulty.Mild:
                    fallOptions = new FallDamageOptions
                    {
                        damageHeightLimit = 2,
                        damageDivider = 2,
                        crippleEnabled = false,
                        crippleHeightLimit = 5,
                        crippledDuration = 10,
                        crippledModifier = -20,
                        deathEnabled = false,
                        deathHeightLimit = 20
                    };
                    break;
                case FallDamageDifficulty.Normal:
                    fallOptions = new FallDamageOptions
                    {
                        damageHeightLimit = 2,
                        damageDivider = 1,
                        crippleEnabled = true,
                        crippleHeightLimit = 5,
                        crippledDuration = 10,
                        crippledModifier = -20,
                        deathEnabled = false,
                        deathHeightLimit = 20
                    };
                    break;
                case FallDamageDifficulty.High:
                    fallOptions = new FallDamageOptions
                    {
                        damageHeightLimit = 2,
                        damageDivider = 1,
                        crippleEnabled = true,
                        crippleHeightLimit = 5,
                        crippledDuration = 10,
                        crippledModifier = -20,
                        deathEnabled = true,
                        deathHeightLimit = 20
                    };
                    break;
                case FallDamageDifficulty.Custom:
                    String options = String.Empty;
                    if (File.Exists(Path.Combine(baseDir, "config/FallDamageConfig.json")))
                    {
#if DEBUG
                        PUtil.LogDebug("FallDamage custom option file exists");
#endif
                        options = File.ReadAllText(Path.Combine(baseDir, "config/FallDamageConfig.json"));
                    }
                    else
                    {
#if DEBUG
                        PUtil.LogDebug("FallDamage custom option file does not exists");
#endif
                        goto case FallDamageDifficulty.Normal;
                    }
                    try
                    {
                        fallOptions = JsonConvert.DeserializeObject<FallDamageOptions>(options);
                    }
                    catch (JsonException e)
                    {
                        goto case FallDamageDifficulty.Normal;
                    }
                    break;
            
            }
#if DEBUG
            PUtil.LogDebug(("Resulting options FallDamage: {0}").F(fallOptions.ToString()));
#endif
        }
    }
}
