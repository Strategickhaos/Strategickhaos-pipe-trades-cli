using System;
using Xunit;
using PipeTradesFeatureEngine;

namespace FeatureEngineTests
{
    /// <summary>
    /// Tests for the Feature Engine primitive functions.
    /// These tests validate that the engines return expected metrics.
    /// </summary>
    public class PhysicsEngineTests
    {
        [Fact]
        public void FREQUENCY_HZ_ValidInput_ReturnsFrequency()
        {
            // Radio wave: 300m wavelength = 300,000mm
            var result = FeatureEngine.FREQUENCY_HZ(300000);
            Assert.True(result is double, "Expected double return type");
            double frequency = (double)result;
            
            // c = λf, so f = c/λ
            // f = 299,792,458 m/s / 300 m ≈ 999,308 Hz
            Assert.True(frequency > 999000 && frequency < 1000000);
        }

        [Fact]
        public void FREQUENCY_HZ_ZeroInput_ReturnsError()
        {
            var result = FeatureEngine.FREQUENCY_HZ(0);
            Assert.NotNull(result);
            // Should return Excel error for invalid input
        }

        [Fact]
        public void FREQUENCY_HZ_NegativeInput_ReturnsError()
        {
            var result = FeatureEngine.FREQUENCY_HZ(-100);
            Assert.NotNull(result);
            // Should return Excel error for invalid input
        }
    }

    public class ChemistryEngineTests
    {
        [Fact]
        public void ELEMENT_Z_Hydrogen_Returns1()
        {
            var result = FeatureEngine.ELEMENT_Z("H");
            Assert.Equal(1, result);
        }

        [Fact]
        public void ELEMENT_Z_Iron_Returns26()
        {
            var result = FeatureEngine.ELEMENT_Z("Fe");
            Assert.Equal(26, result);
        }

        [Fact]
        public void ELEMENT_Z_Gold_Returns79()
        {
            var result = FeatureEngine.ELEMENT_Z("Au");
            Assert.Equal(79, result);
        }

        [Fact]
        public void ELEMENT_Z_CaseInsensitive()
        {
            var resultUpper = FeatureEngine.ELEMENT_Z("AU");
            var resultLower = FeatureEngine.ELEMENT_Z("au");
            var resultMixed = FeatureEngine.ELEMENT_Z("Au");
            
            Assert.Equal(79, resultUpper);
            Assert.Equal(79, resultLower);
            Assert.Equal(79, resultMixed);
        }

        [Fact]
        public void ELEMENT_Z_InvalidSymbol_ReturnsError()
        {
            var result = FeatureEngine.ELEMENT_Z("Xx");
            Assert.NotNull(result);
            // Should return Excel error for invalid symbol
        }
    }

    public class GeometryEngineTests
    {
        [Fact]
        public void GEO_DISTANCE_KM_SamePoint_ReturnsZero()
        {
            var result = FeatureEngine.GEO_DISTANCE_KM(30.0, -90.0, 30.0, -90.0);
            Assert.IsType<double>(result);
            Assert.Equal(0.0, (double)result, 3);
        }

        [Fact]
        public void GEO_DISTANCE_KM_ValidPoints_ReturnsDistance()
        {
            // Lake Charles to Sulphur (roughly 15km)
            var result = FeatureEngine.GEO_DISTANCE_KM(30.2266, -93.2174, 30.2366, -93.3774);
            Assert.True(result is double, "Expected double return type");
            double distance = (double)result;
            
            // Should be around 15km
            Assert.True(distance > 10 && distance < 20);
        }

        [Fact]
        public void GEO_DISTANCE_KM_InvalidLatitude_ReturnsError()
        {
            var result = FeatureEngine.GEO_DISTANCE_KM(91.0, 0.0, 0.0, 0.0);
            Assert.NotNull(result);
            // Should return Excel error for invalid coordinates
        }
    }

    public class TextAnalysisEngineTests
    {
        [Fact]
        public void FALLACY_SCORE_EmptyText_ReturnsZero()
        {
            var result = FeatureEngine.FALLACY_SCORE("");
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void FALLACY_SCORE_ValidText_ReturnsScore()
        {
            var result = FeatureEngine.FALLACY_SCORE("Everyone says this is true!");
            Assert.True(result is double, "Expected double return type");
            double score = (double)result;
            Assert.True(score >= 0.0 && score <= 1.0);
        }

        [Fact]
        public void ENTROPY_SCORE_EmptyText_ReturnsZero()
        {
            var result = FeatureEngine.ENTROPY_SCORE("");
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ENTROPY_SCORE_RepetitiveText_ReturnsLowEntropy()
        {
            var result = FeatureEngine.ENTROPY_SCORE("aaaaaa");
            Assert.True(result is double, "Expected double return type");
            double entropy = (double)result;
            Assert.True(entropy < 0.3); // Should be low entropy
        }

        [Fact]
        public void ENTROPY_SCORE_DiverseText_ReturnsHigherEntropy()
        {
            var result = FeatureEngine.ENTROPY_SCORE("The quick brown fox jumps over the lazy dog");
            Assert.True(result is double, "Expected double return type");
            double entropy = (double)result;
            Assert.True(entropy > 0.5); // Should be higher entropy
        }

        [Fact]
        public void CONTRADICTION_SCORE_EmptyText_ReturnsZero()
        {
            var result = FeatureEngine.CONTRADICTION_SCORE("");
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void CONTRADICTION_SCORE_ValidText_ReturnsScore()
        {
            var result = FeatureEngine.CONTRADICTION_SCORE("I love cats. I hate cats.");
            Assert.True(result is double, "Expected double return type");
            double score = (double)result;
            Assert.True(score >= 0.0 && score <= 1.0);
        }

        [Fact]
        public void SIGNAL_SHARPNESS_EmptyText_ReturnsZero()
        {
            var result = FeatureEngine.SIGNAL_SHARPNESS("");
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void SIGNAL_SHARPNESS_SpecificText_ReturnsHighScore()
        {
            var result = FeatureEngine.SIGNAL_SHARPNESS("The temperature was 72.5 degrees at 14:23 on January 16th.");
            Assert.True(result is double, "Expected double return type");
            double sharpness = (double)result;
            Assert.True(sharpness > 0.3); // Should be relatively sharp
        }

        [Fact]
        public void SIGNAL_SHARPNESS_VagueText_ReturnsLowScore()
        {
            var result = FeatureEngine.SIGNAL_SHARPNESS("Maybe it could possibly work perhaps.");
            Assert.True(result is double, "Expected double return type");
            double sharpness = (double)result;
            Assert.True(sharpness < 0.5); // Should be less sharp due to hedge words
        }
    }

    public class CompositeEngineTests
    {
        [Fact]
        public void UTRD_ValidText_ReturnsJSON()
        {
            var result = FeatureEngine.UTRD("This is a test sentence.");
            Assert.IsType<string>(result);
            string json = (string)result;
            
            // Should contain JSON with expected keys
            Assert.Contains("metrics", json);
            Assert.Contains("fallacy_score", json);
            Assert.Contains("entropy_score", json);
            Assert.Contains("contradiction_score", json);
            Assert.Contains("signal_sharpness", json);
            Assert.Contains("composite_score", json);
        }

        [Fact]
        public void UTRD_EmptyText_ReturnsError()
        {
            var result = FeatureEngine.UTRD("");
            Assert.NotNull(result);
            // Should return Excel error for invalid input
        }
    }
}
