using System;

using Moq;

using Tweaker.License;

using Xunit;

namespace Tweaker.Tests
{
    public class LicenseValidatorTests
    {
        private readonly Mock<IKeyVault> _keyVaultMock;
        private readonly Mock<ISecurityChecks> _securityChecksMock;
        private readonly LicenseValidator _validator;

        public LicenseValidatorTests()
        {
            _keyVaultMock = new Mock<IKeyVault>();
            _securityChecksMock = new Mock<ISecurityChecks>();
            _validator = new LicenseValidator(_keyVaultMock.Object, _securityChecksMock.Object);
        }

        [Theory]
        [InlineData("XXXXX-XXXXX-XXXXX-XXXXX", true)]
        [InlineData("12345-67890-ABCDE-FGHIJ", true)]
        [InlineData("XXXX-XXXXX-XXXXX-XXXXX", false)]
        [InlineData("XXXXX-XXXXX-XXXXX-XXXX", false)]
        [InlineData("XXXXX-XXXXX-XXXXX-XXXXX-", false)]
        [InlineData("XXXXX-XXXXX-XXXXX-XXXXX-XXXXX", false)]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("  ", false)]
        public void IsValidFormat_ShouldReturnExpectedResult(string? licenseKey, bool expected)
        {
            var result = _validator.IsValidFormat(licenseKey);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidateLicenseKey_ShouldReturnNull_WhenDebuggerIsAttached()
        {
            _securityChecksMock.Setup(s => s.IsDebuggerAttached()).Returns(true);
            var result = _validator.ValidateLicenseKey("XXXXX-XXXXX-XXXXX-XXXXX", "fingerprint", DateTime.Now);
            Assert.Null(result);
        }

        [Fact]
        public void ValidateLicenseKey_ShouldReturnNull_WhenAnalysisToolIsDetected()
        {
            _securityChecksMock.Setup(s => s.IsAnalysisToolDetected()).Returns(true);
            var result = _validator.ValidateLicenseKey("XXXXX-XXXXX-XXXXX-XXXXX", "fingerprint", DateTime.Now);
            Assert.Null(result);
        }

        [Fact]
        public void ValidateLicenseKey_ShouldReturnNull_ForInvalidFormat()
        {
            var result = _validator.ValidateLicenseKey("invalid-format", "fingerprint", DateTime.Now);
            Assert.Null(result);
        }
    }
}
