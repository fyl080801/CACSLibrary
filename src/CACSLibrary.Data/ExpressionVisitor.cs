using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace CACSLibrary.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ExpressionVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected virtual Expression Visit(Expression exp)
        {
            Expression result;
            if (exp == null)
            {
                result = exp;
            }
            else
            {
                switch (exp.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.ArrayIndex:
                    case ExpressionType.Coalesce:
                    case ExpressionType.Divide:
                    case ExpressionType.Equal:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LeftShift:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Modulo:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                    case ExpressionType.Power:
                    case ExpressionType.RightShift:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        result = this.VisitBinary((BinaryExpression)exp);
                        break;
                    case ExpressionType.ArrayLength:
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                    case ExpressionType.Negate:
                    case ExpressionType.UnaryPlus:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                        result = this.VisitUnary((UnaryExpression)exp);
                        break;
                    case ExpressionType.Call:
                        result = this.VisitMethodCall((MethodCallExpression)exp);
                        break;
                    case ExpressionType.Conditional:
                        result = this.VisitConditional((ConditionalExpression)exp);
                        break;
                    case ExpressionType.Constant:
                        result = this.VisitConstant((ConstantExpression)exp);
                        break;
                    case ExpressionType.Invoke:
                        result = this.VisitInvocation((InvocationExpression)exp);
                        break;
                    case ExpressionType.Lambda:
                        result = this.VisitLambda((LambdaExpression)exp);
                        break;
                    case ExpressionType.ListInit:
                        result = this.VisitListInit((ListInitExpression)exp);
                        break;
                    case ExpressionType.MemberAccess:
                        result = this.VisitMemberAccess((MemberExpression)exp);
                        break;
                    case ExpressionType.MemberInit:
                        result = this.VisitMemberInit((MemberInitExpression)exp);
                        break;
                    case ExpressionType.New:
                        result = this.VisitNew((NewExpression)exp);
                        break;
                    case ExpressionType.NewArrayInit:
                    case ExpressionType.NewArrayBounds:
                        result = this.VisitNewArray((NewArrayExpression)exp);
                        break;
                    case ExpressionType.Parameter:
                        result = this.VisitParameter((ParameterExpression)exp);
                        break;
                    case ExpressionType.TypeIs:
                        result = this.VisitTypeIs((TypeBinaryExpression)exp);
                        break;
                    default:
                        result = this.VisitUnknown(exp);
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual Expression VisitUnknown(Expression expression)
        {
            throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            MemberBinding result;
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    result = this.VisitMemberAssignment((MemberAssignment)binding);
                    break;
                case MemberBindingType.MemberBinding:
                    result = this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                    break;
                case MemberBindingType.ListBinding:
                    result = this.VisitMemberListBinding((MemberListBinding)binding);
                    break;
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            ElementInit result;
            if (arguments != initializer.Arguments)
            {
                result = Expression.ElementInit(initializer.AddMethod, arguments);
            }
            else
            {
                result = initializer;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        protected virtual Expression VisitUnary(UnaryExpression u)
        {
            Expression operand = this.Visit(u.Operand);
            return this.UpdateUnary(u, operand, u.Type, u.Method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="operand"></param>
        /// <param name="resultType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected UnaryExpression UpdateUnary(UnaryExpression u, Expression operand, Type resultType, MethodInfo method)
        {
            UnaryExpression result;
            if (u.Operand != operand || u.Type != resultType || u.Method != method)
            {
                result = Expression.MakeUnary(u.NodeType, operand, resultType, method);
            }
            else
            {
                result = u;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            Expression conversion = this.Visit(b.Conversion);
            return this.UpdateBinary(b, left, right, conversion, b.IsLiftedToNull, b.Method);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="conversion"></param>
        /// <param name="isLiftedToNull"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected BinaryExpression UpdateBinary(BinaryExpression b, Expression left, Expression right, Expression conversion, bool isLiftedToNull, MethodInfo method)
        {
            BinaryExpression result;
            if (left != b.Left || right != b.Right || conversion != b.Conversion || method != b.Method || isLiftedToNull != b.IsLiftedToNull)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                {
                    result = Expression.Coalesce(left, right, conversion as LambdaExpression);
                }
                else
                {
                    result = Expression.MakeBinary(b.NodeType, left, right, isLiftedToNull, method);
                }
            }
            else
            {
                result = b;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
        {
            Expression expr = this.Visit(b.Expression);
            return this.UpdateTypeIs(b, expr, b.TypeOperand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="expression"></param>
        /// <param name="typeOperand"></param>
        /// <returns></returns>
        protected TypeBinaryExpression UpdateTypeIs(TypeBinaryExpression b, Expression expression, Type typeOperand)
        {
            TypeBinaryExpression result;
            if (expression != b.Expression || typeOperand != b.TypeOperand)
            {
                result = Expression.TypeIs(expression, typeOperand);
            }
            else
            {
                result = b;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected virtual Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected virtual Expression VisitConditional(ConditionalExpression c)
        {
            Expression test = this.Visit(c.Test);
            Expression ifTrue = this.Visit(c.IfTrue);
            Expression ifFalse = this.Visit(c.IfFalse);
            return this.UpdateConditional(c, test, ifTrue, ifFalse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="test"></param>
        /// <param name="ifTrue"></param>
        /// <param name="ifFalse"></param>
        /// <returns></returns>
        protected ConditionalExpression UpdateConditional(ConditionalExpression c, Expression test, Expression ifTrue, Expression ifFalse)
        {
            ConditionalExpression result;
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                result = Expression.Condition(test, ifTrue, ifFalse);
            }
            else
            {
                result = c;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Expression VisitMemberAccess(MemberExpression m)
        {
            Expression exp = this.Visit(m.Expression);
            return this.UpdateMemberAccess(m, exp, m.Member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="expression"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        protected MemberExpression UpdateMemberAccess(MemberExpression m, Expression expression, MemberInfo member)
        {
            MemberExpression result;
            if (expression != m.Expression || member != m.Member)
            {
                result = Expression.MakeMemberAccess(expression, member);
            }
            else
            {
                result = m;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression obj = this.Visit(m.Object);
            IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
            return this.UpdateMethodCall(m, obj, m.Method, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected MethodCallExpression UpdateMethodCall(MethodCallExpression m, Expression obj, MethodInfo method, IEnumerable<Expression> args)
        {
            MethodCallExpression result;
            if (obj != m.Object || method != m.Method || args != m.Arguments)
            {
                result = Expression.Call(obj, method, args);
            }
            else
            {
                result = m;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            ReadOnlyCollection<Expression> result;
            if (original != null)
            {
                List<Expression> list = null;
                int i = 0;
                int j = original.Count;
                while (i < j)
                {
                    Expression p = this.Visit(original[i]);
                    if (list != null)
                    {
                        list.Add(p);
                    }
                    else
                    {
                        if (p != original[i])
                        {
                            list = new List<Expression>(j);
                            for (int k = 0; k < i; k++)
                            {
                                list.Add(original[k]);
                            }
                            list.Add(p);
                        }
                    }
                    i++;
                }
                if (list != null)
                {
                    result = list.AsReadOnly();
                    return result;
                }
            }
            result = original;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignment"></param>
        /// <returns></returns>
        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            Expression e = this.Visit(assignment.Expression);
            return this.UpdateMemberAssignment(assignment, assignment.Member, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="member"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected MemberAssignment UpdateMemberAssignment(MemberAssignment assignment, MemberInfo member, Expression expression)
        {
            MemberAssignment result;
            if (expression != assignment.Expression || member != assignment.Member)
            {
                result = Expression.Bind(member, expression);
            }
            else
            {
                result = assignment;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            return this.UpdateMemberMemberBinding(binding, binding.Member, bindings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="member"></param>
        /// <param name="bindings"></param>
        /// <returns></returns>
        protected MemberMemberBinding UpdateMemberMemberBinding(MemberMemberBinding binding, MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            MemberMemberBinding result;
            if (bindings != binding.Bindings || member != binding.Member)
            {
                result = Expression.MemberBind(member, bindings);
            }
            else
            {
                result = binding;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            return this.UpdateMemberListBinding(binding, binding.Member, initializers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="member"></param>
        /// <param name="initializers"></param>
        /// <returns></returns>
        protected MemberListBinding UpdateMemberListBinding(MemberListBinding binding, MemberInfo member, IEnumerable<ElementInit> initializers)
        {
            MemberListBinding result;
            if (initializers != binding.Initializers || member != binding.Member)
            {
                result = Expression.ListBind(member, initializers);
            }
            else
            {
                result = binding;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            int i = 0;
            int j = original.Count;
            while (i < j)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else
                {
                    if (b != original[i])
                    {
                        list = new List<MemberBinding>(j);
                        for (int k = 0; k < i; k++)
                        {
                            list.Add(original[k]);
                        }
                        list.Add(b);
                    }
                }
                i++;
            }
            IEnumerable<MemberBinding> result;
            if (list != null)
            {
                result = list;
            }
            else
            {
                result = original;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            int i = 0;
            int j = original.Count;
            while (i < j)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else
                {
                    if (init != original[i])
                    {
                        list = new List<ElementInit>(j);
                        for (int k = 0; k < i; k++)
                        {
                            list.Add(original[k]);
                        }
                        list.Add(init);
                    }
                }
                i++;
            }
            IEnumerable<ElementInit> result;
            if (list != null)
            {
                result = list;
            }
            else
            {
                result = original;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);
            return this.UpdateLambda(lambda, lambda.Type, body, lambda.Parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lambda"></param>
        /// <param name="delegateType"></param>
        /// <param name="body"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected LambdaExpression UpdateLambda(LambdaExpression lambda, Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            LambdaExpression result;
            if (body != lambda.Body || parameters != lambda.Parameters || delegateType != lambda.Type)
            {
                result = Expression.Lambda(delegateType, body, parameters);
            }
            else
            {
                result = lambda;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nex"></param>
        /// <returns></returns>
        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
            return this.UpdateNew(nex, nex.Constructor, args, nex.Members);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nex"></param>
        /// <param name="constructor"></param>
        /// <param name="args"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        protected NewExpression UpdateNew(NewExpression nex, ConstructorInfo constructor, IEnumerable<Expression> args, IEnumerable<MemberInfo> members)
        {
            NewExpression result;
            if (args != nex.Arguments || constructor != nex.Constructor || members != nex.Members)
            {
                if (nex.Members != null)
                {
                    result = Expression.New(constructor, args, members);
                }
                else
                {
                    result = Expression.New(constructor, args);
                }
            }
            else
            {
                result = nex;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected virtual Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression i = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            return this.UpdateMemberInit(init, i, bindings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <param name="nex"></param>
        /// <param name="bindings"></param>
        /// <returns></returns>
        protected MemberInitExpression UpdateMemberInit(MemberInitExpression init, NewExpression nex, IEnumerable<MemberBinding> bindings)
        {
            MemberInitExpression result;
            if (nex != init.NewExpression || bindings != init.Bindings)
            {
                result = Expression.MemberInit(nex, bindings);
            }
            else
            {
                result = init;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <returns></returns>
        protected virtual Expression VisitListInit(ListInitExpression init)
        {
            NewExpression i = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            return this.UpdateListInit(init, i, initializers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="init"></param>
        /// <param name="nex"></param>
        /// <param name="initializers"></param>
        /// <returns></returns>
        protected ListInitExpression UpdateListInit(ListInitExpression init, NewExpression nex, IEnumerable<ElementInit> initializers)
        {
            ListInitExpression result;
            if (nex != init.NewExpression || initializers != init.Initializers)
            {
                result = Expression.ListInit(nex, initializers);
            }
            else
            {
                result = init;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="na"></param>
        /// <returns></returns>
        protected virtual Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
            return this.UpdateNewArray(na, na.Type, exprs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="na"></param>
        /// <param name="arrayType"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        protected NewArrayExpression UpdateNewArray(NewArrayExpression na, Type arrayType, IEnumerable<Expression> expressions)
        {
            NewArrayExpression result;
            if (expressions != na.Expressions || na.Type != arrayType)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    result = Expression.NewArrayInit(arrayType.GetElementType(), expressions);
                }
                else
                {
                    result = Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
                }
            }
            else
            {
                result = na;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        protected virtual Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
            Expression expr = this.Visit(iv.Expression);
            return this.UpdateInvocation(iv, expr, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iv"></param>
        /// <param name="expression"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected InvocationExpression UpdateInvocation(InvocationExpression iv, Expression expression, IEnumerable<Expression> args)
        {
            InvocationExpression result;
            if (args != iv.Arguments || expression != iv.Expression)
            {
                result = Expression.Invoke(expression, args);
            }
            else
            {
                result = iv;
            }
            return result;
        }
    }
}
