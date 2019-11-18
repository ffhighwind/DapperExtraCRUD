// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Converts a <see cref="Predicate{T}"/> to a WHERE expression in SQL.
	/// </summary>
	/// <typeparam name="T">The input type.</typeparam>
	public class WhereConditionGenerator<T> : ExpressionVisitor
		where T : class
	{
		protected readonly string TableName;
		protected StringBuilder Results;
		//protected SqlAdapter Adapter;
		protected ParameterExpression InputParam;
		protected IDictionary<string, object> OutputParam;

		public WhereConditionGenerator() : base()
		{
			SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<T>();
			//Adapter = typeInfo.Adapter;
			TableName = typeInfo.TableName;
		}

		/// <summary>
		/// Dispatches the <see cref="System.Linq.Expressions.Expression"/> to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public string Create(Expression<Predicate<T>> node, out IDictionary<string, object> param)
		{
			Results = new StringBuilder(150);
			OutputParam = new ExpandoObject();
			InputParam = node.Parameters[0];
			base.Visit(node.Body);
			string str = Results.ToString();
			Results.Clear();
			param = OutputParam;
			return str;
		}

		/// <summary>
		/// Dispatches the <see cref="System.Linq.Expressions.Expression"/> to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public override Expression Visit(Expression node)
		{
			throw new InvalidOperationException("Cannot access directly. Use Create().");
			//call base.Visit(expr) instead
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.BinaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.CanReduce) {
				base.Visit(node.Reduce());
				return null;
			}
			string op;
			switch (node.NodeType) {
				case ExpressionType.Add: // a + b
				case ExpressionType.AddChecked: // a + b (overflow checked)
					op = " + ";
					break;
				case ExpressionType.Subtract: // a - b
				case ExpressionType.SubtractChecked: // a - b (overflow checked)
					op = " - ";
					break;
				case ExpressionType.Multiply: // a  * b
				case ExpressionType.MultiplyChecked: // a * b (overflow checked)
					op = " * ";
					break;
				case ExpressionType.Divide: // a / b
					op = " / ";
					break;
				case ExpressionType.Power: // Math.Pow(a, b) (visual basic only)
					throw new InvalidOperationException("Unreachable");
				case ExpressionType.Modulo: // a % b
					op = " % ";
					break;
				case ExpressionType.And: // a & b
					op = " & ";
					break;
				case ExpressionType.Or: // a | b
					op = " | ";
					break;
				case ExpressionType.AndAlso: // a && b
					op = " AND ";
					break;
				case ExpressionType.OrElse: // a || b
					op = " OR ";
					break;
				case ExpressionType.LessThan: // a < b
					op = " < ";
					break;
				case ExpressionType.LessThanOrEqual: // a <= b
					op = " <= ";
					break;
				case ExpressionType.GreaterThan: // a > b
					op = " > ";
					break;
				case ExpressionType.GreaterThanOrEqual: // a >= b
					op = " >= ";
					break;
				case ExpressionType.Equal: // a == b
					op = " = ";
					break;
				case ExpressionType.NotEqual: // a != b
					op = " <> ";
					break;
				case ExpressionType.Coalesce: // a ?? b
					Results.Append("COALESCE(");
					base.Visit(node.Left);
					Results.Append(',');
					Results.Append(node.Right);
					Results.Append(")");
					return null;
				//throw new InvalidOperationException("Invalid operator '??'");
				case ExpressionType.ArrayIndex: // a[b]
					CompileExpression(node);
					return null;
				//throw new InvalidOperationException("Invalid operator 'arr[index]'");
				case ExpressionType.RightShift: // a >> b
					op = " >> ";
					break;
				case ExpressionType.LeftShift: // a << b
					op = " << ";
					break;
				case ExpressionType.ExclusiveOr: // a xor b
					op = " ^ ";
					break;
				default:
					throw new InvalidOperationException("Unknown NodeType " + node.NodeType.ToString());
			}
			Results.Append('(');
			base.Visit(node.Left);
			Results.Append(op);
			base.Visit(node.Right);
			Results.Append(')');
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.ConstantExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitConstant(ConstantExpression node)
		{
			CompileValueExpression(node, node.Type, node.Value);
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.DefaultExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDefault(DefaultExpression node)
		{
			// default(a)
			if (!SqlInternal.SqlDefaultValue(node.Type, out object value)) {
				if (value == null)
					throw new InvalidOperationException("Invalid default type: " + node.Type.FullName);
				AddParam(value);
			}
			else
				Results.Append(value);
			return null;
		}

		/// <summary>
		///  Visits the children of the <see cref="System.Linq.Expressions.UnaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.CanReduce) {
				base.Visit(node.Reduce());
			}
			else {
				string op;
				switch (node.NodeType) {
					case ExpressionType.Negate: // -a
					case ExpressionType.NegateChecked: // -a (overflow checked)
						op = "-";
						break;
					case ExpressionType.Not: // !a (boolean), ~a (integral)
						op = " NOT ";
						break;
					case ExpressionType.Convert: // (b) a
					case ExpressionType.ConvertChecked: // (b) a
					case ExpressionType.ArrayLength: // a.Length
					case ExpressionType.TypeAs: // a as b
						CompileExpression(node);
						return null;
					case ExpressionType.Quote: // Expression<a>
					case ExpressionType.UnaryPlus: // +a
						base.Visit(node.Operand);
						return null;
					default:
						throw new InvalidOperationException("Unknown NodeType " + node.NodeType.ToString());
				}
				Results.Append(op);
				base.Visit(node.Operand);
			}
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitMember(MemberExpression node)
		{
			// a.b
			if (node.Expression.NodeType == ExpressionType.MemberAccess) {
				string msg = node.ToString();
				if (msg.StartsWith("value(")) {
					int index = msg.LastIndexOf(").");
					msg = msg.Substring(index + 2);
				}
				throw new InvalidOperationException("Invalid expression: " + msg.ToString());
			}
			else if (node.Expression.NodeType == ExpressionType.Parameter) {
				base.Visit(node.Expression);
				Results.Append(".");
				Results.Append(node.Member.Name);
				return null;
			}
			CompileExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MethodCallExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			// a.b(c, d)
			if (node.Method.Name == "Equals" && node.Arguments.Count == 1) {
				Results.Append('(');
				base.Visit(node.Object);
				Results.Append(" = ");
				base.Visit(node.Arguments[0]);
				Results.Append(')');
				return null;
			}
			else if (node.Method.Name == "Contains") {
				if (node.Arguments.Count > 1) {
					if (node.Arguments[0] is NewArrayExpression newExp) {
						base.Visit(node.Arguments[1]);
						Results.Append(" IN ");
						// new a[] { b, c, d }
						CompileListExpression(newExp.Type.GetElementType(), newExp, newExp.Expressions);
						return null;
					}
				}
				else if (node.Arguments.Count == 1) {
					base.Visit(node.Arguments[0]);
					Results.Append(" IN ");
					CompileExpression(node.Object);
					return null;
				}
			}
			InvalidExpression(node);
			return null;
		}

		protected void CompileListExpression(Type type, Expression node, IEnumerable<Expression> exprs)
		{
			// new a[] { b, c, d }
			Type underlying = Nullable.GetUnderlyingType(type) ?? type;
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Decimal:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.String:
				case TypeCode.Object:
					if (type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid)) {
						break;
					}
					goto default;
				case TypeCode.DateTime:
				case TypeCode.Double:
				case TypeCode.Single:
					break;
				//case TypeCode.DBNull:
				//case TypeCode.Empty:
				default:
					throw new InvalidOperationException("Invalid list type: " + (type?.Name ?? "null"));
			}
			List<object> list = exprs.Select(c => CompileValueExpression(c)).ToList();
			AddParam(list);
		}

		protected object CompileValueExpression(Expression expr)
		{
			return expr is ConstantExpression c ? c.Value : Expression.Lambda(expr).Compile().DynamicInvoke();
		}

		protected void CompileExpression(Expression expr)
		{
			object obj;
			Type type;
			if (expr is ConstantExpression c) {
				obj = c.Value;
				type = c.Type;
			}
			else {
				obj = Expression.Lambda(expr).Compile().DynamicInvoke();
				type = obj.GetType();
			}
			CompileValueExpression(expr, type, obj);
		}

		protected void CompileValueExpression(Expression expr, Type type, object obj)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			string value;
			switch (typeCode) {
				case TypeCode.Decimal:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Boolean:
				case TypeCode.Byte:
					value = obj.ToString();
					break;
				case TypeCode.Char:
					char ch = (char) obj;
					value = ch == '\'' ? "''''" : "'" + ch + "'";
					break;
				case TypeCode.String:
					value = "'" + obj.ToString().Replace("'", "''") + "'";
					break;
				case TypeCode.Object:
				//if (type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid)) {
				//	AddParam(obj);
				//	return;
				//}
				case TypeCode.DateTime:
				case TypeCode.Double:
				case TypeCode.Single:
					AddParam(obj);
					return;
				case TypeCode.DBNull:
				case TypeCode.Empty:
				default:
					throw new InvalidOperationException("Invalid type: " + type.FullName + "\n" + expr.ToString());
			}
			Results.Append(value);
		}

		private void AddParam(object obj)
		{
			string name = "P" + OutputParam.Count;
			OutputParam.Add(name, obj);
			name = "@" + name;
			Results.Append(name);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.NewArrayExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			CompileListExpression(node.Type.GetElementType(), node, node.Expressions);
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.ParameterExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitParameter(ParameterExpression node)
		{
			// x
			if (node.Name != InputParam.Name) {
				throw new InvalidOperationException("Variable '" + node.Name + "' is out of scope. Only allowed to access variable " + InputParam.Name + ".");
			}
			Results.Append(TableName);
			return null;
		}

		#region Invalid Expressions
		/// <summary>
		/// Visits the children of the extension <see cref="System.Linq.Expressions.Expression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitExtension(Expression node)
		{
			// custom user expressions (not Extension methods)
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.NewExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitNew(NewExpression node)
		{
			// new a(b, c)
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.InvocationExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitInvocation(InvocationExpression node)
		{
			// (a, b) => { return c; )(1,2)
			CompileExpression(node);
			return null;
			//throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.DynamicExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDynamic(DynamicExpression node)
		{
			// (a, b) => { return c; }
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.TypeBinaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			// a is b
			CompileExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.ConditionalExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			// a ? b : c
			if (node.CanReduce) {
				base.Visit(node.Reduce());
				return null;
			}
			else {
				Results.Append(" (SELECT CASE WHEN (");
				base.Visit(node.Test);
				Results.Append(") THEN (");
				base.Visit(node.IfTrue);
				Results.Append(") ELSE (");
				base.Visit(node.IfFalse);
				Results.Append(") END) ");
			}
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.ListInitExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitListInit(ListInitExpression node)
		{
			// new Dictionary() { Add(5, a), Add(6, c) }
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.IndexExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitIndex(IndexExpression node)
		{
			// a[index]
			CompileExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.CatchBlock"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.ElementInit"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override ElementInit VisitElementInit(ElementInit node)
		{
			// always part of IReadOnlyCollection<ElementInit>
			// a.Add(5, b)
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.LabelTarget"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override LabelTarget VisitLabelTarget(LabelTarget node)
		{
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberAssignment"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			// a.b = c
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref=" System.Linq.Expressions.MemberBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberBinding VisitMemberBinding(MemberBinding node)
		{
			// new a() { b = x }
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberListBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			// { a, b, c }
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberMemberBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			// new a() { b = x, c = y }
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.SwitchCase"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			InvalidExpression(node.ToString());
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.BlockExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitBlock(BlockExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.DebugInfoExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.GotoExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitGoto(GotoExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.LabelExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLabel(LabelExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.Expression"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLambda<Ty>(Expression<Ty> node)
		{
			CompileExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.LoopExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLoop(LoopExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberInitExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			// new a() { b = x, d = y }
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			// Required for "eval" in dynamic languages and results in an IList<T>
			// Not used in C#
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.SwitchExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitSwitch(SwitchExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.TryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitTry(TryExpression node)
		{
			InvalidExpression(node);
			return null;
		}

		private void InvalidExpression(string msg)
		{
			if (msg.StartsWith("value("))
				msg = msg.Substring(msg.IndexOf(").") + 2);
			throw new InvalidOperationException("Invalid expression: " + msg);
		}

		private void InvalidExpression(Expression exp)
		{
			if (exp.CanReduce)
				base.Visit(exp.Reduce());
			else
				InvalidExpression(exp.ToString());
		}
		#endregion Invalid Expressions
	}
}
