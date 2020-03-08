using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using STRINGS;
using TUNING;
using Klei.AI;
using UnityEngine;

namespace MurderBot
{
    class MurderBotConfig : IEntityConfig
    {
        public const string ID = "MurderBot";
        public const string NAME = "Murder Bot";
        public const string DESCRIPTION = "A murderous little robot";
        public const float BATTERY_DEPLETION_RATE = 40f;
        public const float BATTERY_CAPACITY = 21000f;
        public static float MASS = 25f;


        public GameObject CreatePrefab()
        {
            string name = (string) MurderBotConfig.NAME;
            string desc = (string) MurderBotConfig.DESCRIPTION;
            string id = (string)MurderBotConfig.ID;
            double mass = (double) SweepBotConfig.MASS;
            EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
            KAnimFile anim = Assets.GetAnim((HashedString)"sweep_bot_kanim");
            EffectorValues decor = none;
            EffectorValues noise = new EffectorValues();
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id, name, desc, (float)mass, anim, "idle", Grid.SceneLayer.Creatures, 1, 1, decor, noise, SimHashes.Creature, (List<Tag>)null, 293f);

            placedEntity.AddOrGet<LoopingSounds>();
            placedEntity.GetComponent<KBatchedAnimController>().isMovable = true;
            KPrefabID kprefabId = placedEntity.AddOrGet<KPrefabID>();
            kprefabId.AddTag(GameTags.Creature, false);
            placedEntity.AddComponent<Pickupable>();
            placedEntity.AddOrGet<Clearable>().isClearable = false;

            Trait trait = Db.Get().CreateTrait("MurderBotBaseTrait", name, name, (string)null, false, (ChoreGroup[])null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 21000f, name, false, false, true));
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -40f, name, false, false, true));
            Modifiers modifiers = placedEntity.AddOrGet<Modifiers>();
            modifiers.initialTraits = new string[1]
            {
                "MurderBotBaseTrait"
            };
            modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
            modifiers.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
            placedEntity.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity((KAnimHashedString)"snapto_pivot", false);
            placedEntity.AddOrGet<Traits>();
            placedEntity.AddOrGet<CharacterOverlay>();
            placedEntity.AddOrGet<Effects>();
            placedEntity.AddOrGetDef<AnimInterruptMonitor.Def>();
            placedEntity.AddOrGetDef<StorageUnloadMonitor.Def>();
            placedEntity.AddOrGetDef<RobotBatteryMonitor.Def>();
            placedEntity.AddOrGetDef<SweetBotReactMonitor.Def>();
            placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
            placedEntity.AddOrGetDef<SweepBotTrappedMonitor.Def>();
            placedEntity.AddOrGet<AnimEventHandler>();

            SymbolOverrideControllerUtil.AddToPrefab(placedEntity);
            placedEntity.AddOrGet<OrnamentReceptacle>().AddDepositTag(GameTags.PedestalDisplayable);
            placedEntity.AddOrGet<DecorProvider>();
            placedEntity.AddOrGet<UserNameable>();
            placedEntity.AddOrGet<CharacterOverlay>();
            placedEntity.AddOrGet<ItemPedestal>();
            Navigator navigator = placedEntity.AddOrGet<Navigator>();
            navigator.NavGridName = "WalkerBabyNavGrid";
            navigator.CurrentNavType = NavType.Floor;
            navigator.defaultSpeed = 1f;
            navigator.updateProber = true;
            navigator.maxProbingRadius = 32;
            navigator.sceneLayer = Grid.SceneLayer.Creatures;
            kprefabId.AddTag(GameTags.Creatures.Walker, false);
            ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef)new FallStates.Def(), true).Add((StateMachine.BaseDef)new AnimInterruptStates.Def(), true).Add((StateMachine.BaseDef)new SweepBotTrappedStates.Def(), true).Add((StateMachine.BaseDef)new ReturnToChargeStationStates.Def(), true).Add((StateMachine.BaseDef)new IdleStates.Def(), true);
            placedEntity.AddOrGet<LoopingSounds>();
            EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Robots.Models.SweepBot, (string)null);

            return placedEntity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
            StorageUnloadMonitor.Instance smi = inst.GetSMI<StorageUnloadMonitor.Instance>();
            smi.sm.internalStorage.Set(inst.GetComponents<Storage>()[1], smi);
            inst.GetComponent<OrnamentReceptacle>();
            inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
        }
    }
}
