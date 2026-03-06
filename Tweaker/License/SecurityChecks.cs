namespace Tweaker.License
{
    public class SecurityChecks : ISecurityChecks
    {
        public bool IsDebuggerAttached()
        {
#if DEBUG
            return false;
#else
            if (AntiDebugger.IsDevelopmentEnvironment()) return false;
            return AntiDebugger.IsDebuggerAttached();
#endif
        }

        public bool IsAnalysisToolDetected()
        {
#if DEBUG
            return false;
#else
            if (AnalysisToolDetector.IsDevelopmentEnvironment()) return false;
            return AnalysisToolDetector.PerformFullCheck();
#endif
        }
    }
}
