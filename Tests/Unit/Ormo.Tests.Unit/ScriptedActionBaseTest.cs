// (c) 2024 George Volsky. All rights reserved
// Licensed under the Attribution-NonCommercial 4.0 International (CC BY-NC 4.0)
// License text: https://creativecommons.org/licenses/by-nc/4.0/legalcode
// Human-readable summary: https://creativecommons.org/licenses/by-nc/4.0/
// This code has a less restrictive (dual) license provided on a paid basis
// Contacts: email – rextextau@gmail.com; telegram – @rextextau

namespace Ormo.Tests.Unit
{
    using Ormo.ScriptProviders;
    using Ormo.Tests.Unit.Commands;

    /// <summary>
    /// Test for <see cref="ScriptedActionBase"/> class.
    /// </summary>
    public sealed class ScriptedActionBaseTest
    {
        /// <summary>
        /// Test method to check LoadScript method throws KeyNotFoundException if corresponding script cannot be found by script provider.
        /// </summary>
        [Fact]
        public void LoadScript_NoCorrespondingScript_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                new ScriptedActionBase<Nothing>().LoadScript("Not.Existent", new EmbededResourcesScriptProvider(typeof(ScriptedActionBaseTest).Assembly));
            });
        }

        /// <summary>
        /// Test method to check Setup throws ArgumentException if IEnumerable passed into it.
        /// </summary>
        [Fact]
        public void Setup_IEnumerable_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new ScriptedActionBase<IEnumerable<string>>().Setup(Enumerable.Empty<string>());
            });
        }

        /// <summary>
        /// Test method to check NullToDbNull returns DbNull if null passed into it.
        /// </summary>
        [Fact]
        public void NullToDbNull_Nullvalue_ReturnsDbNull()
        {
            var result = ScriptedActionBase<Nothing>.NullToDbNull(null);

            Assert.Equal(DBNull.Value, result);
        }

        /// <summary>
        /// Test method to check NullToDbNull returns value if non-null value passed into it.
        /// </summary>
        [Fact]
        public void NullToDbNull_NotNullvalue_ReturnsIt()
        {
            var result = ScriptedActionBase<Nothing>.NullToDbNull(42);

            Assert.Equal(42, result);
        }

        /// <summary>
        /// Test method to check Parameters is null if Setup method was not called.
        /// </summary>
        [Fact]
        public void Parameters_SetupNotCalled_Null()
        {
            var sut = new ScriptedActionBase<Nothing>();
            Assert.Null(sut.Parameters);
        }

        /// <summary>
        /// Test method to check Parameters is empty if Setup method was called with Nothing object.
        /// </summary>
        [Fact]
        public void Parameters_SetupNothing_Null()
        {
            var sut = new ScriptedActionBase<Nothing>();
            sut.Setup(new Nothing());

            Assert.NotNull(sut.Parameters);
            Assert.Empty(sut.Parameters);
        }

        /// <summary>
        /// Test method to check Parameters is filled properly if Setup method was called with primitive type.
        /// </summary>
        [Fact]
        public void Parameters_SetupPrimitiveType_ConatainPrimitiveTypeNameAndValue()
        {
            var sut = new ScriptedActionBase<int>();
            sut.Setup(42);

            Assert.NotNull(sut.Parameters);
            Assert.Equal("int32", sut.Parameters.First().Key);
            Assert.Equal(42, sut.Parameters.First().Value);
        }

        /// <summary>
        /// Test method to check Parameters is filled properly if Setup method was called with nullable primitive type.
        /// </summary>
        [Fact]
        public void Parameters_SetupPrimitiveNullableType_ConatainPrimitiveTypeNameAndValue()
        {
            var sut = new ScriptedActionBase<int?>();
            sut.Setup(42);

            Assert.NotNull(sut.Parameters);
            Assert.Equal("int32", sut.Parameters.First().Key);
            Assert.Equal(42, sut.Parameters.First().Value);
        }

        /// <summary>
        /// Test method to check Parameters is filled properly if Setup method was called with class having properties with public getters.
        /// </summary>
        [Fact]
        public void Parameters_SetupClassType_ConatainClassProperties()
        {
            var sut = new ScriptedActionBase<ResourceCommandParameters>();
            sut.Setup(new ResourceCommandParameters { Id = 42, Value = "Test" });

            Assert.NotNull(sut.Parameters);
            Assert.Equal(2, sut.Parameters.Count());
            Assert.Equal("id", sut.Parameters.First().Key);
            Assert.Equal(42, sut.Parameters.First().Value);
            Assert.Equal("value", sut.Parameters.Last().Key);
            Assert.Equal("Test", sut.Parameters.Last().Value);
        }
    }
}
