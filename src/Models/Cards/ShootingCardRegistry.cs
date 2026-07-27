using MegaCrit.Sts2.Core.Models;

namespace IreneMod.Models.Cards;

public static class ShootingCardRegistry
{
    public static bool Contains(CardModel card) =>
        card is OpenFire
            or Bullet
            or RapidFire
            or CruisingMissile
            or RollingShot
            or SuppressiveFire
            or AimForTheVitals
            or Cover
            or TripleShot
            or BlindingRound
            or ConcussionRound
            or ExplosiveRound
            or DoubleShotKit
            or DeathDeathDeath
            or SolemnMourning;
}
