namespace Nerdle.UnitTests
{
    class ExpressionFileTests
    {
        #region IsCorrectMathExpression
        [TestCase("1+2+310=0", "Wrong expression length!", -1, false)] // To long
        [TestCase("1+2+3=0", "Wrong expression length!", -1, false)] // To short
        [TestCase("-1+2+1=0", "First character is an operator sign!", 0, false)] // First char is math operator 
        [TestCase("1+-2+1=0", "Two operator signs next to each other!", 2, false)] // Two math operators next to each other
        [TestCase("1/0+12=0", "Division by 0!", 2, false)] // Division by 0
        [TestCase("1/00+1=0", "Division by 0!", 3, false)] // Division by 0
        [TestCase("1+2-1*=0", "Operator right before equal sign!", 5, false)] // Math operator before equal sign
        [TestCase("12345678", "Equal sign not in expression!", 6, false)] // No equal sign
        [TestCase("1234567=", "Equal sign can't be on last position!", 6, false)] // To late equal sign
        [TestCase("1+4=60-1", "Operator after equal sign", 6, false)] // Operator sign after equal sign 
        [TestCase("00/001=0", "Everything is good!", 6, true)] // Fake division by 0
        [TestCase("12+13=20", "Everything is good!", 5, true)] // Correct expression

        public void IsCorrectMathExpression_WhenCalled_ShouldReturnExpectedValue(string expression, string expectedMessage, int expectedIndex, bool expectedBool)
        {
            var result = ExpressionValidator.IsCorrectMathExpression(expression.ToCharArray());
            Assert.AreEqual((expectedMessage, expectedIndex, expectedBool), (result.message, result.index, result.isCorrect));
        }
        #endregion

        #region ChangeExpresisonToReversePolishNotation
        [TestCase("1!+2+3+44", "WrongCharacterInExpression")] // Not accepted sign
        [TestCase("1+2+3+4=", "WrongCharacterInExpression")] // '=' is not accepted
        [TestCase("1++2+3+4", "ExpressionIsNotMathematicallyCorrect")] // Mathematically not correct
        public void ChangeExpresisonToReversePolishNotation_WhenExpressionIsNotCorrect_ShouldThrowException(string expression, string message)
        {
            var ex = Assert.Throws<Exception>(() => ExpressionValidator.ChangeExpresisonToReversePolishNotation(expression));
            if(ex != null)
                Assert.AreEqual(message, ex.Message);
        }
        [Test]
        public void ChangeExpresisonToReversePolishNotation_WhenCalled_ShouldReturnCorrectExpression()
        {
            string expressionToChange = "21+1*38";
            string correctExpression = NerdleWebApi.Constants.NumberStart + "21" + NerdleWebApi.Constants.NumberEnd + NerdleWebApi.Constants.NumberStart + "1" + NerdleWebApi.Constants.NumberEnd + 
                NerdleWebApi.Constants.NumberStart + "38" + NerdleWebApi.Constants.NumberEnd + "*+";
            string returnedExpression = ExpressionValidator.ChangeExpresisonToReversePolishNotation(expressionToChange);
            Assert.AreEqual(returnedExpression, correctExpression);
        }
        #endregion

        #region CountReversePolishNotation
        [TestCase("<21><1><38>*!+", "WrongCharacterInExpression")]
        [TestCase("<21><1><38>*+-", "ExpressionIsNotInReversePolishNotation")]
        public void CountReversePolishNotation_WhenCalledWithWrongInput_ShouldThrowException(string expression, string message)
        {
            var ex = Assert.Throws<Exception>(() => ExpressionValidator.CountReversePolishNotation(expression));
            if(ex != null)
                Assert.AreEqual(message, ex.Message);
        }
        [Test]
        public void CountReversePolishNotation_WhenCalledWithCorrectImput_ShouldReturnValue()
        {
            double value = ExpressionValidator.CountReversePolishNotation("<21><1><38>*+");
            double expectedValue = 59;
            Assert.AreEqual(value, expectedValue);
        }
        #endregion
    }
}