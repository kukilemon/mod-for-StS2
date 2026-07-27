using IreneMod.Models.Cards;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models;

public sealed class IreneCardPool : CardPoolModel
{
    // Pure-code milestone: reuse Ironclad presentation resources until Irene art is added.
    public override string Title => "ironclad";
    public override string EnergyColorName => "ironclad";
    public override string CardFrameMaterialPath => "card_frame_red";
    public override Color DeckEntryCardColor => new("8B2942");
    public override Color EnergyOutlineColor => new("54202D");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards() =>
    [
        ModelDb.Card<IreneStrike>(),
        ModelDb.Card<IreneDefend>(),
        ModelDb.Card<StandardLantern>(),
        ModelDb.Card<OpenFire>(),
        ModelDb.Card<RapidFire>(),
        ModelDb.Card<CruisingMissile>(),
        ModelDb.Card<HoldBreath>(),
        ModelDb.Card<RollingShot>(),
        ModelDb.Card<SuppressiveFire>(),
        ModelDb.Card<AimForTheVitals>(),
        ModelDb.Card<Cover>(),
        ModelDb.Card<TripleShot>()
        ,ModelDb.Card<LoadAmmunition>()
        ,ModelDb.Card<BlindingRound>()
        ,ModelDb.Card<ConcussionRound>()
        ,ModelDb.Card<ReinforcedCover>()
        ,ModelDb.Card<HomemadeAmmunition>()
        ,ModelDb.Card<QuickReload>()
        ,ModelDb.Card<TracerRound>()
        ,ModelDb.Card<ExplosiveRound>()
        ,ModelDb.Card<WeaponModification>()
        ,ModelDb.Card<SolemnMourning>()
        ,ModelDb.Card<DoubleShotKit>()
        ,ModelDb.Card<DualWieldForm>()
        ,ModelDb.Card<DeathDeathDeath>()
        ,ModelDb.Card<AdjustStance>()
        ,ModelDb.Card<LungeStep>()
        ,ModelDb.Card<RisingThrust>()
        ,ModelDb.Card<PursuingThrust>()
        ,ModelDb.Card<ArmorPiercingThrust>()
        ,ModelDb.Card<AerialPursuit>()
        ,ModelDb.Card<FencingEtiquette>()
        ,ModelDb.Card<AdvancingSlash>()
        ,ModelDb.Card<SweepingSlash>()
        ,ModelDb.Card<AerialIntercept>()
        ,ModelDb.Card<PerfectParry>()
        ,ModelDb.Card<Launch>()
        ,ModelDb.Card<ChainThrust>()
        ,ModelDb.Card<DefensiveCounter>()
        ,ModelDb.Card<MoonlightSword>()
        ,ModelDb.Card<GravityReversal>()
        ,ModelDb.Card<SunSword>()
        ,ModelDb.Card<SwordWindSuppression>()
        ,ModelDb.Card<DuelStance>()
        ,ModelDb.Card<Judgment>()
        ,ModelDb.Card<SwordGunConcerto>()
        ,ModelDb.Card<MeteorSword>()
        ,ModelDb.Card<GaleFinalThrust>()
        ,ModelDb.Card<LightTheWay>()
        ,ModelDb.Card<PrepareUnderLantern>()
        ,ModelDb.Card<BorrowLight>()
        ,ModelDb.Card<LightMatch>()
        ,ModelDb.Card<LanternStrike>()
        ,ModelDb.Card<ThrowSpark>()
        ,ModelDb.Card<Disarm>()
        ,ModelDb.Card<NightWatch>()
        ,ModelDb.Card<LanternBearer>()
        ,ModelDb.Card<RecycleEmbers>()
        ,ModelDb.Card<Guide>()
        ,ModelDb.Card<RecallInLight>()
        ,ModelDb.Card<LightingRitual>()
        ,ModelDb.Card<EmberShield>()
        ,ModelDb.Card<GuardTheFlame>()
        ,ModelDb.Card<AddLampOil>()
        ,ModelDb.Card<EvolutionRitual>()
        ,ModelDb.Card<IreneMetamorphosis>()
        ,ModelDb.Card<HolyLightBaptism>()
        ,ModelDb.Card<BrilliantLantern>()
        ,ModelDb.Card<ThrowFlame>()
        ,ModelDb.Card<ReturnHome>()
        ,ModelDb.Card<ExpandedLantern>()
        ,ModelDb.Card<FlameImpact>()
        ,ModelDb.Card<FinalForm>()
        ,ModelDb.Card<EternalLantern>()
        ,ModelDb.Card<SeaBornTransformation>()
    ];
}
