using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Converts a <see cref="Predicate{T}"/> to a WHERE expression in SQL.
	/// </summary>
	/// <typeparam name="T">The input type.</typeparam>
	public class WhereConditionVisitor<T> : ExpressionVisitor
	{
		protected ParameterExpression param;
		protected readonly string tableName;
		protected StringBuilder results = new StringBuilder(150);

		public WhereConditionVisitor() : base()
		{
			tableName = typeof(T).GetCustomAttribute<TableAttribute>(false)?.Name ?? param.Type.Name;
		}

		/// <summary>
		/// Dispatches the <see cref="System.Linq.Expressions.Expression"/> to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public string Create(Expression<Predicate<T>> node)
		{
			param = node.Parameters[0];
			base.Visit(node.Body);
			string str = results.ToString();
			results.Clear();
			return str;
		}

		/// <summary>
		/// Dispatches the <see cref="System.Linq.Expressions.Expression"/> to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public override Expression Visit(Expression node)
		{
			throw new InvalidOperationException("Cannot access directly. Use Expression<Predicate<Ty>>.");
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.BinaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitBinary(BinaryExpression node)
		{
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
					throw new InvalidOperationException("Invalid operator '??'");
				case ExpressionType.ArrayIndex: // a[b]
					throw new InvalidOperationException("Invalid operator 'arr[index]'");
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
			results.Append('(');
			base.Visit(node.Left);
			results.Append(op);
			base.Visit(node.Right);
			results.Append(')');
			return null;
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.ConstantExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitConstant(ConstantExpression node)
		{
			string str = SqlValue(node.Value, node.Type);
			results.Append(str);
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
			string str = SqlDefaultValue(node.Type);
			results.Append(str);
			return null;
		}

		/// <summary>
		///  Visits the children of the <see cref="System.Linq.Expressions.UnaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitUnary(UnaryExpression node)
		{
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
					throw new NotSupportedException("Invalid operator: '(" + node.Method.Name + ")'"); // CAST()... not supported
				case ExpressionType.ArrayLength: // a.Length
					throw new InvalidOperationException("Invalid operator: 'arr.Length'");
				case ExpressionType.TypeAs: // a as b
					throw new InvalidOperationException("Invalid operator: 'as'");
				case ExpressionType.Quote: // Expression<a>
				case ExpressionType.UnaryPlus: // +a
					base.Visit(node.Operand);
					return null;
				default:
					throw new InvalidOperationException("Unknown NodeType " + node.NodeType.ToString());
			}
			results.Append(op);
			base.Visit(node.Operand);
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
			results.Append('[');
			base.Visit(node.Expression);
			//Results.Append(attr.Name ?? node.Member.DeclaringType.Name);
			results.Append("].[");
			results.Append(node.Member.Name);
			results.Append(']');
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
				results.Append('(');
				base.Visit(node.Object);
				results.Append(" = ");
				base.Visit(node.Arguments[0]);
				results.Append(')');
				return null;
			}
			else if (node.Method.Name == "Contains" && node.Arguments.Count > 1) {
				base.Visit(node.Arguments[1]);
				results.Append(" IN ");
				if (node.Arguments[0] is NewArrayExpression newExp) {
					_VisitNewArray(newExp);
				}
				else {
					throw InvalidExpression(node);
				}
				return null;
			}
			throw InvalidExpression(node);
		}

		private void _VisitNewArray(NewArrayExpression node)
		{
			Type type = node.Type.GetElementType();
			// new a[] { b, c, d }
			if (!IsValidSqlType(type)) {
				throw new InvalidOperationException("Invalid type " + node.Type.Name + "\n" + node.ToString());
			}
			bool isQuoted = IsQuotedSqlType(type);
			results.Append('(');
			foreach (Expression exp in node.Expressions) {
				if (exp is ConstantExpression c) {
					if (isQuoted) {
						results.Append('\'');
						results.Append(c.Value.ToString());
						results.Append('\'');
					}
					else {
						results.Append(c.Value.ToString());
					}
					results.Append(',');
				}
				else {
					throw new InvalidOperationException("Arrays can only contain constant values");
				}
			}
			results.Remove(results.Length - 1, 1);
			results.Append(')');
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.NewArrayExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitNewArray(NewArrayExpression node)
		{
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
			if (node.Name != param.Name) {
				throw new InvalidOperationException("Variable '" + node.Name + "' is out of scope. Only allowed to access variable '" + param.Name + "'.");
			}
			results.Append(tableName);
			return null;
		}

		private bool IsValidSqlType(Type type)
		{
			type = Nullable.GetUnderlyingType(type) ?? type;
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					if (type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid)) {
						return true;
					}
					return false;
				case TypeCode.Boolean:
				case TypeCode.Char:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				//case TypeCode.DBNull:
				default:
					return false;
			}
		}

		private bool IsQuotedSqlType(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				case TypeCode.Object:
					return type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid);
				//case TypeCode.Boolean:
				//case TypeCode.SByte:
				//case TypeCode.Byte:
				//case TypeCode.Int16:
				//case TypeCode.UInt16:
				//case TypeCode.Int32:
				//case TypeCode.UInt32:
				//case TypeCode.Int64:
				//case TypeCode.UInt64:
				//case TypeCode.Single:
				//case TypeCode.Double:
				//case TypeCode.Decimal:
				//case TypeCode.DBNull:
				default:
					return false;
			}
		}

		private string SqlValue(object value, Type type)
		{
			if (value == null)
				return "NULL";
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					string tmp;
					if (type == typeof(TimeSpan))
						tmp = ((TimeSpan) value).ToString();
					else if (type == typeof(DateTimeOffset))
						tmp = ((DateTimeOffset) value).ToString();
					else if (value is Guid obj)
						tmp = obj.ToString();
					else if (value is T ty)
						throw new InvalidOperationException("Invalid type: " + typeof(T).ToString());
					else
						throw new InvalidOperationException("Invalid type: " + type.ToString());
					return "'" + tmp + "'";
				case TypeCode.Boolean:
					return ((bool) value) ? "TRUE" : "FALSE";
				case TypeCode.Char:
					return "'" + ((char) value) + "'";
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
					return ((int) value).ToString();
				case TypeCode.UInt32:
					return ((uint) value).ToString();
				case TypeCode.Int64:
					return ((long) value).ToString();
				case TypeCode.UInt64:
					return ((ulong) value).ToString();
				case TypeCode.Single:
					return ((float) value).ToString();
				case TypeCode.Double:
					return ((double) value).ToString();
				case TypeCode.Decimal:
					return ((decimal) value).ToString();
				case TypeCode.DateTime:
					return ((DateTime) value).ToString();
				case TypeCode.String:
					return "'" + ((string) value) + "'";
				//case TypeCode.DBNull:
				case TypeCode.Empty:
					return "NULL";
				default:
					throw new InvalidOperationException("Invalid type " + type.Name.ToString());
			}
		}

		private string SqlDefaultValue(Type type)
		{
			if (Nullable.GetUnderlyingType(type) == null)
				return "NULL";
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					string tmp;
					if (type == typeof(TimeSpan))
						tmp = default(TimeSpan).ToString();
					else if (type == typeof(DateTimeOffset))
						tmp = default(DateTimeOffset).ToString();
					else if (type == typeof(Guid))
						tmp = default(Guid).ToString();
					else
						throw new InvalidOperationException("Invalid type: " + type.ToString());
					return "'" + tmp + "'";
				case TypeCode.Boolean:
					return "FALSE";
				case TypeCode.Char:
					return "'" + default(char) + "'";
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return "0";
				case TypeCode.DateTime:
					return "'" + (default(DateTime)).ToString() + "'";
				case TypeCode.String:
					//case TypeCode.Empty:
					//case TypeCode.DBNull:
					return "NULL";
				default:
					throw new InvalidOperationException("Invalid type " + type.Name.ToString());
			}
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
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.NewExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitNew(NewExpression node)
		{
			// new a(b, c)
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.InvocationExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitInvocation(InvocationExpression node)
		{
			// (a, b) => { return c; )(1,2)
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.DynamicExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDynamic(DynamicExpression node)
		{
			// (a, b) => { return c; }
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.TypeBinaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			// a is b
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.ConditionalExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			// a ? b : c
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.ListInitExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitListInit(ListInitExpression node)
		{
			// new Dictionary() { Add(5, a), Add(6, c) }
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.IndexExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitIndex(IndexExpression node)
		{
			// a[index]
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.CatchBlock"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			throw InvalidExpression(node.ToString());
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
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.LabelTarget"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override LabelTarget VisitLabelTarget(LabelTarget node)
		{
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberAssignment"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			// a.b = c
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref=" System.Linq.Expressions.MemberBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberBinding VisitMemberBinding(MemberBinding node)
		{
			// new a() { b = x }
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberListBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			// { a, b, c }
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberMemberBinding"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			// new a() { b = x, c = y }
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.SwitchCase"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			throw InvalidExpression(node.ToString());
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.BlockExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitBlock(BlockExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the <see cref="System.Linq.Expressions.DebugInfoExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.GotoExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitGoto(GotoExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.LabelExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLabel(LabelExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.Expression"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLambda<Ty>(Expression<Ty> node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.LoopExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLoop(LoopExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.MemberInitExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			// new a() { b = x, d = y }
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.RuntimeVariablesExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.SwitchExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitSwitch(SwitchExpression node)
		{
			throw InvalidExpression(node);
		}

		/// <summary>
		/// Visits the children of the <see cref="System.Linq.Expressions.TryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitTry(TryExpression node)
		{
			throw InvalidExpression(node);
		}

		private InvalidOperationException InvalidExpression(string msg)
		{
			if (msg.StartsWith("value(")) {
				msg = msg.Substring(msg.IndexOf(").") + 2);
			}
			return new InvalidOperationException("Invalid expression: " + msg);
		}

		private InvalidOperationException InvalidExpression(Expression exp)
		{
			return InvalidExpression(exp.ToString());
		}
		#endregion Invalid Expressions
	}
}
