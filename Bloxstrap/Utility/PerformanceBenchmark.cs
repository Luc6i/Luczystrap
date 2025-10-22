using System.Diagnostics;
using System.Management;
using Bloxstrap.Enums.FlagPresets;

namespace Bloxstrap.Utility
{
    public static class PerformanceBenchmark
    {
        private const string LOG_IDENT = "PerformanceBenchmark";

        public static LuciPreset DetectOptimalPreset()
        {
            try
            {
                int score = CalculatePerformanceScore();
                
                App.Logger.WriteLine(LOG_IDENT, $"Performance score calculated: {score}");

                // Score ranges:
                // 0-30: Potato (low-end)
                // 31-60: Low (mid-end)
                // 61+: Ultra (high-end)
                
                if (score <= 30)
                    return LuciPreset.Potato;
                else if (score <= 60)
                    return LuciPreset.Low;
                else
                    return LuciPreset.Ultra;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error during benchmark: {ex.Message}");
                // Default to Low preset if benchmark fails
                return LuciPreset.Low;
            }
        }

        private static int CalculatePerformanceScore()
        {
            int score = 0;

            // CPU Score (0-30 points)
            score += GetCPUScore();

            // RAM Score (0-25 points)
            score += GetRAMScore();

            // GPU Score (0-45 points)
            score += GetGPUScore();

            return score;
        }

        private static int GetCPUScore()
        {
            try
            {
                int coreCount = Environment.ProcessorCount;
                int cpuScore = 0;

                // Core count scoring
                if (coreCount >= 8)
                    cpuScore += 15;
                else if (coreCount >= 4)
                    cpuScore += 10;
                else if (coreCount >= 2)
                    cpuScore += 5;

                // CPU speed scoring
                using (var searcher = new ManagementObjectSearcher("SELECT MaxClockSpeed FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        uint maxClockSpeed = (uint)obj["MaxClockSpeed"]; // MHz
                        
                        if (maxClockSpeed >= 3500)
                            cpuScore += 15;
                        else if (maxClockSpeed >= 2500)
                            cpuScore += 10;
                        else if (maxClockSpeed >= 1500)
                            cpuScore += 5;
                        
                        break; // Only check first processor
                    }
                }

                App.Logger.WriteLine(LOG_IDENT, $"CPU Score: {cpuScore} (Cores: {coreCount})");
                return cpuScore;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error getting CPU info: {ex.Message}");
                return 10; // Default mid-range score
            }
        }

        private static int GetRAMScore()
        {
            try
            {
                ulong totalMemoryBytes = 0;
                
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        totalMemoryBytes = (ulong)obj["TotalPhysicalMemory"];
                        break;
                    }
                }

                double totalMemoryGB = totalMemoryBytes / (1024.0 * 1024.0 * 1024.0);
                int ramScore = 0;

                if (totalMemoryGB >= 16)
                    ramScore = 25;
                else if (totalMemoryGB >= 8)
                    ramScore = 15;
                else if (totalMemoryGB >= 4)
                    ramScore = 8;
                else
                    ramScore = 3;

                App.Logger.WriteLine(LOG_IDENT, $"RAM Score: {ramScore} (Total: {totalMemoryGB:F2} GB)");
                return ramScore;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error getting RAM info: {ex.Message}");
                return 10; // Default mid-range score
            }
        }

        private static int GetGPUScore()
        {
            try
            {
                int gpuScore = 0;
                string gpuName = "";
                uint adapterRAM = 0;

                using (var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        gpuName = obj["Name"]?.ToString() ?? "";
                        
                        if (obj["AdapterRAM"] != null)
                        {
                            adapterRAM = Convert.ToUInt32(obj["AdapterRAM"]);
                        }
                        
                        break; // Only check primary GPU
                    }
                }

                // GPU RAM scoring (0-20 points)
                double gpuMemoryGB = adapterRAM / (1024.0 * 1024.0 * 1024.0);
                
                if (gpuMemoryGB >= 6)
                    gpuScore += 20;
                else if (gpuMemoryGB >= 4)
                    gpuScore += 15;
                else if (gpuMemoryGB >= 2)
                    gpuScore += 10;
                else if (gpuMemoryGB >= 1)
                    gpuScore += 5;

                // GPU brand/model scoring (0-25 points)
                gpuName = gpuName.ToLower();
                
                // High-end GPUs
                if (gpuName.Contains("rtx 40") || gpuName.Contains("rtx 30") || 
                    gpuName.Contains("rx 7") || gpuName.Contains("rx 6"))
                {
                    gpuScore += 25;
                }
                // Mid-high GPUs
                else if (gpuName.Contains("rtx 20") || gpuName.Contains("gtx 16") || 
                         gpuName.Contains("rx 5") || gpuName.Contains("vega"))
                {
                    gpuScore += 20;
                }
                // Mid-range GPUs
                else if (gpuName.Contains("gtx 10") || gpuName.Contains("rx 4") || 
                         gpuName.Contains("gtx 9"))
                {
                    gpuScore += 15;
                }
                // Low-mid GPUs
                else if (gpuName.Contains("gtx") || gpuName.Contains("rx"))
                {
                    gpuScore += 10;
                }
                // Integrated graphics
                else if (gpuName.Contains("intel") && (gpuName.Contains("uhd") || 
                         gpuName.Contains("iris") || gpuName.Contains("hd graphics")))
                {
                    gpuScore += 5;
                }
                // AMD integrated
                else if (gpuName.Contains("radeon") && gpuName.Contains("graphics"))
                {
                    gpuScore += 8;
                }
                else
                {
                    gpuScore += 7; // Unknown GPU
                }

                App.Logger.WriteLine(LOG_IDENT, $"GPU Score: {gpuScore} (Name: {gpuName}, VRAM: {gpuMemoryGB:F2} GB)");
                return gpuScore;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error getting GPU info: {ex.Message}");
                return 15; // Default mid-range score
            }
        }

        public static string GetPerformanceTier(int score)
        {
            if (score <= 30)
                return "Low-End";
            else if (score <= 60)
                return "Mid-End";
            else
                return "High-End";
        }

        public static (int score, string tier, LuciPreset preset) GetDetailedBenchmark()
        {
            int score = CalculatePerformanceScore();
            string tier = GetPerformanceTier(score);
            LuciPreset preset = DetectOptimalPreset();
            
            return (score, tier, preset);
        }
    }
}