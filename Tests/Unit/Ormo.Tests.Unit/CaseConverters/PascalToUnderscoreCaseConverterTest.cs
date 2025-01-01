// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com

namespace Ormo.Tests.Unit.CaseConverters
{
    using Ormo.CaseConverters;

    /// <summary>
    /// Test for <see cref="PascalToUnderscoreCaseConverter"/> class.
    /// </summary>
    public sealed class PascalToUnderscoreCaseConverterTest
    {
        /// <summary>
        /// Test method to check converter returns expected results.
        /// </summary>
        /// <param name="toConvert">
        /// Class field name to convert.
        /// </param>
        /// <param name="expected">
        /// Expected database field name.
        /// </param>
        [Theory]
        [InlineData("Id", "id")]
        [InlineData("Data1", "data1")]
        [InlineData("SomeValue", "some_value")]
        [InlineData("SomeOtherValue", "some_other_value")]
        public void Convert_GivenData_ReturnsExpectedResult(string toConvert, string expected)
        {
            var sut = new PascalToUnderscoreCaseConverter();
            var result = sut.Convert(toConvert);
            Assert.Equal(expected, result);
        }
    }
}
