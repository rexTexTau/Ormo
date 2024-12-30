// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit.ScriptProviders
{
    using Ormo.ScriptProviders;

    /// <summary>
    /// Test for <see cref="FolderScriptProvider"/> class (using scripts in Ormo.Tests.Unit assembly that are copied to output dir).
    /// </summary>
    public class FolderScriptProviderTest
    {
        private readonly FolderScriptProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderScriptProviderTest"/> class.
        /// </summary>
        /// <remarks>
        /// Creates <see cref="FolderScriptProvider"/> using base app directory.
        /// </remarks>
        public FolderScriptProviderTest()
        {
            _provider = new FolderScriptProvider(AppContext.BaseDirectory);
        }

        /// <summary>
        /// Test method to check script whether provider actually fills in its internal storage.
        /// </summary>
        [Fact]
        public void Ctor_FillsIn_Storage()
        {
            Assert.NotEmpty(_provider._storage);
        }

        /// <summary>
        /// Test method to check whether script provider returns not-null script value while being queried using existent script name.
        /// </summary>
        /// <param name="name">
        /// Script name to get script by.
        /// </param>
        [Theory]
        [InlineData("Commands.FileCommand")]
        [InlineData("Queries.FileQuery")]
        public void GetExistingName_Returns_NotNull(string name)
        {
            var value = _provider.Get(name);
            Assert.NotNull(value);
        }

        /// <summary>
        /// Test method to check whether script provider returns null script value while being queried using notexistent script name.
        /// </summary>
        /// <param name="name">
        /// Script name to get script by.
        /// </param>
        [Theory]
        [InlineData("Not.Existent")]
        public void GetNotexistentName_Returns_Null(string name)
        {
            var value = _provider.Get(name);
            Assert.Null(value);
        }
    }
}
