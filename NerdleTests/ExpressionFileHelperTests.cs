namespace Nerdle.UnitTests
{
    class ExpressionFileHelperTests
    {
        #region NextExpression
        [TestCase("1+5*3=40", 4, "1+5*4000")]
        [TestCase("========", 7, "00000000")]
        public void NextExpression_WhenCalled_ShouldReturnNextExpression(string expression, int index, string expectedExpression)
        {
            Assert.AreEqual(expectedExpression.ToCharArray(), ExpressionFileHelper.NextExpression(expression.ToCharArray(), index));
        }
        #endregion
    }
}