using System;
using System.Collections.Generic;
using System.Linq;
using ExcelDna.Integration;
using Newtonsoft.Json;

namespace PipeTradesFeatureEngine
{
    /// <summary>
    /// Feature Engine - Primitive feature functions for Excel
    /// 
    /// This is the spine of the vision-to-computation architecture.
    /// Each function transforms heterogeneous inputs into measurable outputs.
    /// No interpretation. No "truth". Only metrics.
    /// 
    /// Architecture:
    /// - Interface: Excel-DNA (this file exposes functions to Excel)
    /// - Orchestration: C# (normalizes inputs, dispatches to engines)
    /// - Engines: Pure functions (return numbers, vectors, labels)
    /// </summary>
    public static class FeatureEngine
    {
        // ============================================================
        // PHYSICAL CONSTANTS
        // ============================================================
        
        private const double SPEED_OF_LIGHT_M_PER_S = 299792458.0;
        private const double EARTH_RADIUS_KM = 6371.0;
        
        // ============================================================
        // PERIODIC TABLE (Simplified)
        // ============================================================
        
        private static readonly Dictionary<string, int> PeriodicTable = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"H", 1}, {"He", 2}, {"Li", 3}, {"Be", 4}, {"B", 5},
            {"C", 6}, {"N", 7}, {"O", 8}, {"F", 9}, {"Ne", 10},
            {"Na", 11}, {"Mg", 12}, {"Al", 13}, {"Si", 14}, {"P", 15},
            {"S", 16}, {"Cl", 17}, {"Ar", 18}, {"K", 19}, {"Ca", 20},
            {"Sc", 21}, {"Ti", 22}, {"V", 23}, {"Cr", 24}, {"Mn", 25},
            {"Fe", 26}, {"Co", 27}, {"Ni", 28}, {"Cu", 29}, {"Zn", 30},
            {"Ga", 31}, {"Ge", 32}, {"As", 33}, {"Se", 34}, {"Br", 35},
            {"Kr", 36}, {"Rb", 37}, {"Sr", 38}, {"Y", 39}, {"Zr", 40},
            {"Nb", 41}, {"Mo", 42}, {"Tc", 43}, {"Ru", 44}, {"Rh", 45},
            {"Pd", 46}, {"Ag", 47}, {"Cd", 48}, {"In", 49}, {"Sn", 50},
            {"Sb", 51}, {"Te", 52}, {"I", 53}, {"Xe", 54}, {"Cs", 55},
            {"Ba", 56}, {"La", 57}, {"Ce", 58}, {"Pr", 59}, {"Nd", 60},
            {"Pm", 61}, {"Sm", 62}, {"Eu", 63}, {"Gd", 64}, {"Tb", 65},
            {"Dy", 66}, {"Ho", 67}, {"Er", 68}, {"Tm", 69}, {"Yb", 70},
            {"Lu", 71}, {"Hf", 72}, {"Ta", 73}, {"W", 74}, {"Re", 75},
            {"Os", 76}, {"Ir", 77}, {"Pt", 78}, {"Au", 79}, {"Hg", 80},
            {"Tl", 81}, {"Pb", 82}, {"Bi", 83}, {"Po", 84}, {"At", 85},
            {"Rn", 86}, {"Fr", 87}, {"Ra", 88}, {"Ac", 89}, {"Th", 90},
            {"Pa", 91}, {"U", 92}, {"Np", 93}, {"Pu", 94}, {"Am", 95},
            {"Cm", 96}, {"Bk", 97}, {"Cf", 98}, {"Es", 99}, {"Fm", 100},
            {"Md", 101}, {"No", 102}, {"Lr", 103}, {"Rf", 104}, {"Db", 105},
            {"Sg", 106}, {"Bh", 107}, {"Hs", 108}, {"Mt", 109}, {"Ds", 110},
            {"Rg", 111}, {"Cn", 112}, {"Nh", 113}, {"Fl", 114}, {"Mc", 115},
            {"Lv", 116}, {"Ts", 117}, {"Og", 118}
        };
        
        // ============================================================
        // ENGINE 1: WAVELENGTH → FREQUENCY
        // ============================================================
        
        /// <summary>
        /// Converts wavelength in millimeters to frequency in Hertz.
        /// 
        /// This is a pure physics transform: λ → f
        /// No interpretation. Just the relationship c = λf.
        /// 
        /// Example: =FREQUENCY_HZ(300000) returns 999308.19 Hz (roughly 1 kHz for 300m wave)
        /// </summary>
        /// <param name="wavelength_mm">Wavelength in millimeters</param>
        /// <returns>Frequency in Hertz (Hz)</returns>
        [ExcelFunction(
            Name = "FREQUENCY_HZ",
            Description = "Convert wavelength (mm) to frequency (Hz)",
            Category = "Feature Engine - Physics"
        )]
        public static object FREQUENCY_HZ(double wavelength_mm)
        {
            if (wavelength_mm <= 0)
                return ExcelError.ExcelErrorNum;
            
            try
            {
                double wavelength_m = wavelength_mm / 1000.0;
                double frequency_hz = SPEED_OF_LIGHT_M_PER_S / wavelength_m;
                return frequency_hz;
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 2: ELEMENT SYMBOL → ATOMIC NUMBER
        // ============================================================
        
        /// <summary>
        /// Converts element symbol to atomic number (Z).
        /// 
        /// This maps categorical data (element name) to ordinal data (position).
        /// Enables numerical operations on chemical elements.
        /// 
        /// Example: =ELEMENT_Z("Fe") returns 26 (Iron)
        /// Example: =ELEMENT_Z("Au") returns 79 (Gold)
        /// </summary>
        /// <param name="symbol">Chemical element symbol (e.g., "H", "Fe", "Au")</param>
        /// <returns>Atomic number (Z)</returns>
        [ExcelFunction(
            Name = "ELEMENT_Z",
            Description = "Get atomic number from element symbol",
            Category = "Feature Engine - Chemistry"
        )]
        public static object ELEMENT_Z(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return ExcelError.ExcelErrorValue;
            
            try
            {
                string cleanSymbol = symbol.Trim();
                
                if (PeriodicTable.TryGetValue(cleanSymbol, out int atomicNumber))
                {
                    return atomicNumber;
                }
                
                return ExcelError.ExcelErrorValue;
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 3: GPS → DISTANCE
        // ============================================================
        
        /// <summary>
        /// Calculate great-circle distance between two GPS coordinates.
        /// 
        /// Uses Haversine formula. Returns distance in kilometers.
        /// This is geometric anchor - turning position into measurable separation.
        /// 
        /// Example: =GEO_DISTANCE_KM(30.2266, -93.2174, 30.2366, -93.3774)
        /// Returns distance between Lake Charles and Sulphur, LA
        /// </summary>
        /// <param name="lat1">Latitude of point 1 (decimal degrees)</param>
        /// <param name="lon1">Longitude of point 1 (decimal degrees)</param>
        /// <param name="lat2">Latitude of point 2 (decimal degrees)</param>
        /// <param name="lon2">Longitude of point 2 (decimal degrees)</param>
        /// <returns>Distance in kilometers</returns>
        [ExcelFunction(
            Name = "GEO_DISTANCE_KM",
            Description = "Calculate distance between GPS coordinates (km)",
            Category = "Feature Engine - Geometry"
        )]
        public static object GEO_DISTANCE_KM(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                // Validate coordinates
                if (Math.Abs(lat1) > 90 || Math.Abs(lat2) > 90 || 
                    Math.Abs(lon1) > 180 || Math.Abs(lon2) > 180)
                {
                    return ExcelError.ExcelErrorNum;
                }
                
                // Convert to radians
                double lat1Rad = lat1 * Math.PI / 180.0;
                double lat2Rad = lat2 * Math.PI / 180.0;
                double dLat = (lat2 - lat1) * Math.PI / 180.0;
                double dLon = (lon2 - lon1) * Math.PI / 180.0;
                
                // Haversine formula
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                          Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                          Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double distance_km = EARTH_RADIUS_KM * c;
                
                return distance_km;
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 4: TEXT → FALLACY SCORE (STUB)
        // ============================================================
        
        /// <summary>
        /// Detect logical fallacies in text.
        /// 
        /// STUB IMPLEMENTATION: Returns random score for now.
        /// In production, this would call Claude/Ollama or use a trained model.
        /// 
        /// Returns: score from 0.0 (no fallacies) to 1.0 (high fallacy density)
        /// 
        /// Example: =FALLACY_SCORE("Everyone says it's true, so it must be!")
        /// Should detect argumentum ad populum
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <returns>Fallacy score (0.0 - 1.0)</returns>
        [ExcelFunction(
            Name = "FALLACY_SCORE",
            Description = "Detect logical fallacies in text (stub)",
            Category = "Feature Engine - Text Analysis"
        )]
        public static object FALLACY_SCORE(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;
            
            try
            {
                // STUB: Simple heuristic for demonstration
                // In production: call LLM or use trained classifier
                string lower = text.ToLower();
                int fallacyIndicators = 0;
                
                // Check for common fallacy patterns
                if (lower.Contains("everyone") || lower.Contains("nobody"))
                    fallacyIndicators++;
                if (lower.Contains("always") || lower.Contains("never"))
                    fallacyIndicators++;
                if (lower.Contains("obviously") || lower.Contains("clearly"))
                    fallacyIndicators++;
                if (lower.Contains("must be") && lower.Contains("because"))
                    fallacyIndicators++;
                
                // Normalize to 0-1 scale
                double score = Math.Min(fallacyIndicators / 4.0, 1.0);
                return Math.Round(score, 3);
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 5: TEXT → ENTROPY SCORE (STUB)
        // ============================================================
        
        /// <summary>
        /// Calculate Shannon entropy of text.
        /// 
        /// Measures information density and predictability.
        /// Higher entropy = more diverse/random
        /// Lower entropy = more repetitive/predictable
        /// 
        /// Example: =ENTROPY_SCORE("aaaaaa") returns low entropy
        /// Example: =ENTROPY_SCORE("The quick brown fox") returns higher entropy
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <returns>Entropy score (0.0 - 1.0 normalized)</returns>
        [ExcelFunction(
            Name = "ENTROPY_SCORE",
            Description = "Calculate Shannon entropy of text",
            Category = "Feature Engine - Text Analysis"
        )]
        public static object ENTROPY_SCORE(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;
            
            try
            {
                // Calculate character frequency
                var frequency = new Dictionary<char, int>();
                foreach (char c in text.ToLower())
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        if (frequency.ContainsKey(c))
                            frequency[c]++;
                        else
                            frequency[c] = 1;
                    }
                }
                
                if (frequency.Count == 0)
                    return 0.0;
                
                int total = frequency.Values.Sum();
                
                // Calculate Shannon entropy
                double entropy = 0.0;
                foreach (var count in frequency.Values)
                {
                    double probability = (double)count / total;
                    entropy -= probability * Math.Log(probability, 2);
                }
                
                // Normalize to 0-1 (max entropy for English text is roughly 4.7 bits)
                double normalized = Math.Min(entropy / 4.7, 1.0);
                return Math.Round(normalized, 3);
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 6: TEXT → CONTRADICTION SCORE (STUB)
        // ============================================================
        
        /// <summary>
        /// Detect internal contradictions in text.
        /// 
        /// STUB IMPLEMENTATION: Simple pattern matching.
        /// In production: use semantic analysis via LLM.
        /// 
        /// Returns: score from 0.0 (consistent) to 1.0 (contradictory)
        /// 
        /// Example: =CONTRADICTION_SCORE("I love cats. I hate cats.")
        /// Should detect contradiction
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <returns>Contradiction score (0.0 - 1.0)</returns>
        [ExcelFunction(
            Name = "CONTRADICTION_SCORE",
            Description = "Detect contradictions in text (stub)",
            Category = "Feature Engine - Text Analysis"
        )]
        public static object CONTRADICTION_SCORE(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;
            
            try
            {
                // STUB: Look for simple contradiction patterns
                string lower = text.ToLower();
                int contradictions = 0;
                
                // Check for negation patterns
                string[] sentences = lower.Split('.', '!', '?');
                for (int i = 0; i < sentences.Length - 1; i++)
                {
                    string s1 = sentences[i].Trim();
                    for (int j = i + 1; j < sentences.Length; j++)
                    {
                        string s2 = sentences[j].Trim();
                        
                        // Simple heuristic: same words but one has "not"/"never"
                        if (!string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2))
                        {
                            bool s1HasNegation = s1.Contains("not") || s1.Contains("never") || s1.Contains("don't");
                            bool s2HasNegation = s2.Contains("not") || s2.Contains("never") || s2.Contains("don't");
                            
                            if (s1HasNegation != s2HasNegation)
                            {
                                // Check for common words
                                var words1 = s1.Split(' ').Where(w => w.Length > 3).ToHashSet();
                                var words2 = s2.Split(' ').Where(w => w.Length > 3).ToHashSet();
                                int commonWords = words1.Intersect(words2).Count();
                                
                                if (commonWords >= 2)
                                    contradictions++;
                            }
                        }
                    }
                }
                
                // Normalize
                double score = Math.Min(contradictions / 2.0, 1.0);
                return Math.Round(score, 3);
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // ENGINE 7: SIGNAL SHARPNESS (STUB)
        // ============================================================
        
        /// <summary>
        /// Measure signal clarity/sharpness of text.
        /// 
        /// STUB: Combines multiple metrics to assess clarity.
        /// Higher = more clear/precise
        /// Lower = vague/ambiguous
        /// 
        /// Example: =SIGNAL_SHARPNESS("The temperature was 72.5°F at 14:23")
        /// Returns high sharpness (specific numbers)
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <returns>Sharpness score (0.0 - 1.0)</returns>
        [ExcelFunction(
            Name = "SIGNAL_SHARPNESS",
            Description = "Measure signal clarity of text",
            Category = "Feature Engine - Text Analysis"
        )]
        public static object SIGNAL_SHARPNESS(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0.0;
            
            try
            {
                // STUB: Simple heuristics for clarity
                int totalWords = text.Split(new[] { ' ', '\t', '\n', '\r' }, 
                    StringSplitOptions.RemoveEmptyEntries).Length;
                
                if (totalWords == 0)
                    return 0.0;
                
                // Count specific indicators
                int numbers = System.Text.RegularExpressions.Regex.Matches(text, @"\d+").Count;
                int properNouns = System.Text.RegularExpressions.Regex.Matches(text, @"\b[A-Z][a-z]+\b").Count;
                int hedgeWords = System.Text.RegularExpressions.Regex.Matches(
                    text.ToLower(), @"\b(maybe|perhaps|possibly|probably|might|could)\b").Count;
                
                // Calculate sharpness
                double specificityBonus = (numbers + properNouns) / (double)totalWords;
                double hedgePenalty = hedgeWords / (double)totalWords;
                double sharpness = Math.Max(0, Math.Min(1, specificityBonus * 2 - hedgePenalty * 2));
                
                return Math.Round(sharpness, 3);
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
        
        // ============================================================
        // COMPOSITE ENGINE: UTRD (STUB)
        // ============================================================
        
        /// <summary>
        /// Universal Truth Reliability Detector (UTRD)
        /// 
        /// STUB: Aggregates multiple feature scores into single metric.
        /// This is a synthesis function - explicitly formula-driven.
        /// 
        /// Returns JSON with all metrics.
        /// 
        /// Example: =UTRD("Sample text to analyze")
        /// </summary>
        /// <param name="text">Text to analyze</param>
        /// <returns>JSON string with all metrics</returns>
        [ExcelFunction(
            Name = "UTRD",
            Description = "Universal Truth Reliability Detector (composite metric)",
            Category = "Feature Engine - Synthesis"
        )]
        public static object UTRD(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return ExcelError.ExcelErrorValue;
            
            try
            {
                // Calculate all component metrics
                var fallacy = FALLACY_SCORE(text);
                var entropy = ENTROPY_SCORE(text);
                var contradiction = CONTRADICTION_SCORE(text);
                var sharpness = SIGNAL_SHARPNESS(text);
                
                // Aggregate into composite score
                // This is the "explicit formula" - weights can be tuned
                double fallacyScore = Convert.ToDouble(fallacy);
                double entropyScore = Convert.ToDouble(entropy);
                double contradictionScore = Convert.ToDouble(contradiction);
                double sharpnessScore = Convert.ToDouble(sharpness);
                
                double compositeScore = (
                    sharpnessScore * 0.3 +
                    entropyScore * 0.2 +
                    (1 - fallacyScore) * 0.3 +
                    (1 - contradictionScore) * 0.2
                );
                
                // Return as JSON contract
                string inputSummary = text.Length > 50 
                    ? text.Substring(0, 50) + "..." 
                    : text;
                
                var metrics = new
                {
                    input = inputSummary,
                    metrics = new
                    {
                        fallacy_score = fallacyScore,
                        entropy_score = entropyScore,
                        contradiction_score = contradictionScore,
                        signal_sharpness = sharpnessScore,
                        composite_score = Math.Round(compositeScore, 3)
                    },
                    timestamp = DateTime.UtcNow.ToString("o")
                };
                
                return JsonConvert.SerializeObject(metrics, Formatting.Indented);
            }
            catch (Exception ex)
            {
                return $"#ERROR: {ex.Message}";
            }
        }
    }
}
