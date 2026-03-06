namespace Tweaker.License
{
    public interface ISecurityChecks
    {
        bool IsDebuggerAttached();
        bool IsAnalysisToolDetected();
    }
}
