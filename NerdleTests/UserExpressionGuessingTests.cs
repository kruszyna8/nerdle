namespace Nerdle.UnitTests
{
    class UserExpressionGuessingTests
    {
        #region MakeResponse
        [TestCase("3+2+9=14", "3*71=213", "gbvbbvgb")]
        [TestCase("3*6-15=3", "3*71=213", "ggbbvbvg")]
        [TestCase("3*71=213", "3*71=213", "gggggggg")]
        [TestCase("9+8-7=10", "62*8=496", "vbvbbvbb")]
        [TestCase("6*8/2=24", "62*8=496", "gvvbvvbv")]
        [TestCase("62*8=496", "62*8=496", "gggggggg")]
        public void MakeResponse_When_Called_ShouldReturnGoodResponse(string guess, string correctExpression, string expectedResponse)
        {
            var userExpressionGuessing = new UserExpressionGuessing();
                userExpressionGuessing.StartGame();
                userExpressionGuessing.CorrectExpression = correctExpression;
                string response = userExpressionGuessing.MakeResponse(guess);
            Assert.AreEqual(expectedResponse, response);
        }
        #endregion
    }
}