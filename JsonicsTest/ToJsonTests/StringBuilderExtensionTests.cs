using System.Text;
using Jsonics;
using NUnit.Framework;
using Jsonics.ToJson;

namespace JsonicsTest.ToJsonTests
{
    [TestFixture]
    public class StringBuilderExtensionTests
    {
        [Test]
        public void AppendEscaped_NotEscaped_AppendsUnaltered()
        {
            //arrange
            var input = "Doesn't need escaping";
            var builder = new StringBuilder();

            //act
            builder.AppendEscaped(input);

            //assert
            Assert.That(builder.ToString(), Is.EqualTo($"\"{input}\""));
        }

        [TestCase('\"', "\\\"")]
        [TestCase('\\', "\\\\")]
        [TestCase('/', "\\/")]
        [TestCase('\b', "\\b")]
        [TestCase('\f', "\\f")]
        [TestCase('\n', "\\n")]
        [TestCase('\r', "\\r")]
        [TestCase('\t', "\\t")]
        [TestCase((char)1, "\\u0001")]
        [TestCase((char)31, "\\u001F")]
        public void AppendEscaped_SingleEscapeNeeded_EscapesCorrectly(char character, string expectedEscape)
        {
            //arrange
            var input = $"Doesn't need{character} escaping";
            var builder = new StringBuilder();

            //act
            builder.AppendEscaped(input);

            //assert
            Assert.That(builder.ToString(), Is.EqualTo($"\"Doesn't need{expectedEscape} escaping\""));
        }

        [Test]
        public void AppendEscaped_TwoEscapesSeperated_EscapesCorrectly()
        {
            //arrange
            var input = "Doesn't\nneed\t escaping";
            var builder = new StringBuilder();

            //act
            builder.AppendEscaped(input);

            //assert
            Assert.That(builder.ToString(), Is.EqualTo("\"Doesn't\\nneed\\t escaping\""));
        }

        [TestCase(0, "0")]
        [TestCase(1, "1")]
        [TestCase(-1, "-1")]
        [TestCase(12345, "12345")]
        [TestCase(-54321, "-54321")]
        [TestCase(int.MaxValue, "2147483647")]
        [TestCase(int.MinValue, "-2147483648")]
        public void AppendInt_CorrectValue(int input, string expectedString)
        {
            //arrange
            var builder = new StringBuilder();

            //act
            builder.AppendInt(input);

            //assert
            Assert.That(builder.ToString(), Is.EqualTo(expectedString));
        }
    }
}