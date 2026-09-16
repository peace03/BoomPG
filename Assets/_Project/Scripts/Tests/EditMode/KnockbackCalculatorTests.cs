using BoomPG.Gameplay.Combat;
using BoomPG.Gameplay.Config;
using NUnit.Framework;
using UnityEngine;

namespace BoomPG.Tests.EditMode
{
    /// <summary>기본 설정의 경계와 연속 넉백 합성 회귀를 검증한다.</summary>
    public class KnockbackCalculatorTests
    {
        private CombatConfig _config;

        /// <summary>실제 기본 설정으로 각 테스트를 격리한다.</summary>
        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<CombatConfig>();
        }

        /// <summary>테스트가 만든 설정 인스턴스를 정리한다.</summary>
        [TearDown]
        public void TearDown()
        {
            ScriptableObject.DestroyImmediate(_config);
        }

        /// <summary>거리(m) 경계별 피해(HP)와 넉백(m/s)을 검증한다.</summary>
        [TestCase(0f, 20f, 18f)]
        [TestCase(1f, 20f, 18f)]
        [TestCase(1.01f, 12f, 13f)]
        [TestCase(4.5f, 3f, 4f)]
        public void TryGetSplash_UsesFirstInclusiveBand(float distance, float damage, float speed)
        {
            Assert.AreEqual(4, _config.SplashBands.Length);
            Assert.IsTrue(KnockbackCalculator.TryGetSplash(_config, distance,
                out float actualDamage, out float actualSpeed));
            Assert.AreEqual(damage, actualDamage, 0.001f);
            Assert.AreEqual(speed, actualSpeed, 0.001f);
        }

        /// <summary>반경 밖에서는 피해와 속도가 남지 않아야 한다.</summary>
        [Test]
        public void TryGetSplash_OutsideRadius_ReturnsFalseAndZero()
        {
            Assert.IsFalse(KnockbackCalculator.TryGetSplash(_config, 4.51f,
                out float damage, out float speed));
            Assert.AreEqual(0f, damage, 0.001f);
            Assert.AreEqual(0f, speed, 0.001f);
        }

        /// <summary>수평 폭발에도 상승 성분을 유지한다.</summary>
        [Test]
        public void GetDirection_HorizontalTarget_ReturnsUpwardUnitVector()
        {
            Vector3 result = KnockbackCalculator.GetDirection(Vector3.zero, Vector3.right, 0.35f);
            Assert.Greater(result.y, 0f);
            Assert.AreEqual(1f, result.magnitude, 0.001f);
        }

        /// <summary>폭심과 대상이 같아도 넉백 방향이 사라지지 않는다.</summary>
        [Test]
        public void GetDirection_CoincidentTarget_ReturnsUp()
        {
            AssertVector(Vector3.up, KnockbackCalculator.GetDirection(Vector3.zero, Vector3.zero, 0.35f));
        }

        /// <summary>같은 방향의 속도(m/s)를 더한다.</summary>
        [Test]
        public void Blend_Additive_AddsVelocity()
        {
            AssertVector(new Vector3(0f, 0f, 22f), Blend(18f, 4f, KnockbackBlendMode.Additive));
        }

        /// <summary>가산 속도가 25 m/s를 넘지 않는다.</summary>
        [Test]
        public void Blend_Additive_ClampsSpeed()
        {
            Assert.AreEqual(25f, Blend(18f, 18f, KnockbackBlendMode.Additive).magnitude, 0.001f);
        }

        /// <summary>반대 방향의 같은 속도는 상쇄한다.</summary>
        [Test]
        public void Blend_Additive_CancelsOppositeVelocity()
        {
            Assert.AreEqual(0f, Blend(18f, -18f, KnockbackBlendMode.Additive).magnitude, 0.001f);
        }

        /// <summary>약한 폭발이 강한 넉백을 지우지 않는다.</summary>
        [Test]
        public void Blend_KeepStronger_PreservesStrongerCurrentVelocity()
        {
            AssertVector(new Vector3(0f, 0f, 18f), Blend(18f, 4f, KnockbackBlendMode.KeepStronger));
        }

        /// <summary>기존 속도의 절반과 신규 속도를 더한다.</summary>
        [Test]
        public void Blend_AdditiveDamped_HalvesCurrentVelocity()
        {
            AssertVector(new Vector3(0f, 0f, 13f), Blend(18f, 4f, KnockbackBlendMode.AdditiveDamped));
        }

        /// <summary>착지 지점을 조정할 수 있도록 좌우 입력의 방향과 크기를 유지한다.</summary>
        [Test]
        public void ProjectLateralInput_PerpendicularInput_PassesThrough()
        {
            Vector3 result = KnockbackCalculator.ProjectLateralInput(Vector3.right, new Vector3(0f, 0f, 18f));
            AssertVector(Vector3.right, result);
            Assert.AreEqual(1f, result.magnitude, 0.001f);
        }

        /// <summary>날아가는 방향으로 입력해도 넉백을 가속할 수 없다.</summary>
        [Test]
        public void ProjectLateralInput_ForwardInput_ReturnsZero()
        {
            Vector3 result = KnockbackCalculator.ProjectLateralInput(Vector3.forward, new Vector3(0f, 0f, 18f));
            Assert.AreEqual(0f, result.magnitude, 0.001f);
        }

        /// <summary>반대 방향 입력으로 브레이크를 걸어 넉백을 취소할 수 없다.</summary>
        [Test]
        public void ProjectLateralInput_BackwardInput_ReturnsZero()
        {
            Vector3 result = KnockbackCalculator.ProjectLateralInput(Vector3.back, new Vector3(0f, 0f, 18f));
            Assert.AreEqual(0f, result.magnitude, 0.001f);
        }

        /// <summary>수직 넉백에는 좌우 축을 정의할 수 없으므로 입력을 유지한다.</summary>
        [Test]
        public void ProjectLateralInput_NoHorizontalKnockback_ReturnsInput()
        {
            Vector3 moveDirection = Vector3.right;
            Vector3 result = KnockbackCalculator.ProjectLateralInput(moveDirection, new Vector3(0f, 12f, 0f));
            AssertVector(moveDirection, result);
        }

        private static Vector3 Blend(float current, float incoming, KnockbackBlendMode mode)
        {
            return KnockbackCalculator.Blend(Vector3.forward * current, Vector3.forward * incoming, mode, 25f);
        }

        private static void AssertVector(Vector3 expected, Vector3 actual)
        {
            Assert.AreEqual(expected.x, actual.x, 0.001f);
            Assert.AreEqual(expected.y, actual.y, 0.001f);
            Assert.AreEqual(expected.z, actual.z, 0.001f);
        }
    }
}
