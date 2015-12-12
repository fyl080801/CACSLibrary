using System;
using System.Linq.Expressions;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ParameterExpressionVisitor : ExpressionVisitor
    {
        private ParameterExpression newParameterExpression;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public ParameterExpressionVisitor(ParameterExpression p)
        {
            this.newParameterExpression = p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public Expression ChangeParameter(Expression exp)
        {
            return this.Visit(exp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            return this.newParameterExpression;
        }
    }
}
