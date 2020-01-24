#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Converts a <see cref="Predicate{T}"/> to a WHERE expression in SQL.
	/// </summary>
	/// <typeparam name="T">The input type.</typeparam>
	public sealed class WhereConditionGenerator : ExpressionVisitor
	{
		private readonly IDictionary<string, object> OutputParam = new ExpandoObject();

		private readonly StringBuilder Results = new StringBuilder(150);

		private readonly Dictionary<string, SqlTypeInfo> TypeInfos = new Dictionary<string, SqlTypeInfo>();

		private readonly Dictionary<Type, SqlTypeInfo> TypeInfosMap = new Dictionary<Type, SqlTypeInfo>();

		/// <summary>
		/// Prevents a default instance of the <see cref="WhereConditionGenerator{T}"/> class from being created.
		/// </summary>
		private WhereConditionGenerator() : base()
		{
		}

		/// <summary>
		/// Converts a <see cref="Predicate{T}"/> to a WHERE expression in SQL.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="param">The Dapper parameters for the WHERE condition.</param>
		/// <returns>The WHERE expression in SQL that the predicate represents.</returns>
		public static string Create<T>(Expression<Func<T, bool>> predicate, out IDictionary<string, object> param)
			where T : class
		{
			WhereConditionGenerator obj = new WhereConditionGenerator();
			obj.Visit(predicate, out param);
			return obj.Results.ToString();
		}

		#region Methods

		/// <summary>
		/// Dispatches the <see cref="System.Linq.Expressions.Expression"/> to one of the more specialized visit methods in this class.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		public override Expression Visit(Expression node)
		{
			throw new InvalidOperationException("Cannot access directly. Use Create().");
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
					if (node.Right is ConstantExpression ce && ce.Value == null)
						op = " is ";
					else
						op = " = ";
					break;
				case ExpressionType.NotEqual: // a != b
					if (node.Right is ConstantExpression ce2 && ce2.Value == null)
						op = " is not ";
					else
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
		/// Visits the children of the <see cref="System.Linq.Expressions.ConditionalExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			// a ? b : c
			Results.Append(" (SELECT CASE WHEN (");
			base.Visit(node.Test);
			Results.Append(") THEN (");
			base.Visit(node.IfTrue);
			Results.Append(") ELSE (");
			base.Visit(node.IfFalse);
			Results.Append(") END) ");
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
		/// Visits the <see cref="System.Linq.Expressions.DefaultExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitDefault(DefaultExpression node)
		{
			// default(a)
			object value = DefaultValue(node.Type);
			if (value is string str)
				Results.Append(str);
			else
				AddParam(value);
			return null;
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
		/// Visits the children of the <see cref="System.Linq.Expressions.InvocationExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitInvocation(InvocationExpression node)
		{
			// (a, b) => { return c; )(1,2)
			CompileExpression(node);
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
		/// Visits the children of the <see cref="System.Linq.Expressions.Expression"/>.
		/// </summary>
		/// <typeparam name="Ty">The type of the lambda expression.</typeparam>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitLambda<Ty>(Expression<Ty> node)
		{
			CompileExpression(node);
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
				var typeInfo = TypeInfosMap[node.Member.DeclaringType];
				var column = typeInfo.Columns.First(c => c.Property.Name == node.Member.Name);
				Results.Append(column.ColumnName);
				return null;
			}
			CompileExpression(node);
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
				if (node.Arguments[0] is ConstantExpression ce && ce.Value == null)
					Results.Append(" is NULL");
				else {
					Results.Append(" = ");
					base.Visit(node.Arguments[0]);
				}
				Results.Append(')');
				return null;
			}
			else if (node.Method.Name == "Contains") {
				if (node.Arguments.Count > 1) {
					if (node.Arguments[0] is NewArrayExpression newExp) {
						base.Visit(node.Arguments[1]);
						Results.Append(" in ");
						// new a[] { b, c, d }
						CompileListExpression(newExp.Type.GetElementType(), newExp.Expressions);
						return null;
					}
				}
				else if (node.Arguments.Count == 1) {
					base.Visit(node.Arguments[0]);
					Results.Append(" in ");
					CompileExpression(node.Object);
					return null;
				}
			}
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
		/// Visits the children of the <see cref="System.Linq.Expressions.NewArrayExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			CompileListExpression(node.Type.GetElementType(), node.Expressions);
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
			if (!TypeInfos.TryGetValue(node.Name, out SqlTypeInfo typeInfo)) {
				throw new InvalidOperationException("Variable '" + node.Name + "' is out of scope.");
			}
			Results.Append(typeInfo.TableName);
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
		/// Visits the children of the <see cref="System.Linq.Expressions.TryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitTry(TryExpression node)
		{
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
		///  Visits the children of the <see cref="System.Linq.Expressions.UnaryExpression"/>.
		/// </summary>
		/// <param name="node">The expression to visit.</param>
		/// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.CanReduce) {
				base.Visit(node.Reduce());
				return null;
			}
			string op;
			switch (node.NodeType) {
				case ExpressionType.Negate: // -a
				case ExpressionType.NegateChecked: // -a (overflow checked)
					op = "-";
					break;
				case ExpressionType.Not: // !a (boolean), ~a (integral)
					op = " NOT ";
					break;
				case ExpressionType.ArrayLength: // a.Length
					CompileExpression(node.Operand);
					return null;
				case ExpressionType.Convert: // (b) a
				case ExpressionType.ConvertChecked: // (b) a
				case ExpressionType.TypeAs: // a as b
					try {
						CompileExpression(node.Operand);
					}
					catch {
						base.Visit(node.Operand);
					}
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
			return null;
		}

		/// <summary>
		/// Creates the default value for a type and converts it to the SQL string representation if possible.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The default value for a given type converted to an SQL string if possible.</returns>
		private static object DefaultValue(Type type)
		{
			if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
				return "NULL";
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					if (type == typeof(TimeSpan))
						return default(TimeSpan);
					else if (type == typeof(DateTimeOffset))
						return default(DateTimeOffset);
					else if (type == typeof(Guid))
						return default(Guid);
					return Activator.CreateInstance(type);
				case TypeCode.Boolean:
					return "FALSE";
				case TypeCode.Char:
					return "'\0'";
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
					return default(DateTime);
				default:
					break;
			}
			throw new InvalidOperationException("Invalid default type: " + type.FullName);
		}

		private void AddParam(object obj)
		{
			string name = "P" + OutputParam.Count;
			OutputParam.Add(name, obj);
			name = "@" + name;
			Results.Append(name);
		}

		private void CompileExpression(Expression expr)
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

		private void CompileListExpression(Type type, IEnumerable<Expression> exprs)
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
					break;
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

		private object CompileValueExpression(Expression expr)
		{
			return expr is ConstantExpression c ? c.Value : Expression.Lambda(expr).Compile().DynamicInvoke();
		}

		private void CompileValueExpression(Expression expr, Type type, object obj)
		{
			if(obj == null) {
				Results.Append("NULL");
				return;
			}
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
					char ch = (char)obj;
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

		private void InvalidExpression(Expression exp)
		{
			if (exp.CanReduce)
				base.Visit(exp.Reduce());
			else
				InvalidExpression(exp.ToString());
		}

		private void InvalidExpression(string msg)
		{
			if (msg.StartsWith("value("))
				msg = msg.Substring(msg.IndexOf(").") + 2);
			throw new InvalidOperationException("Invalid expression: " + msg);
		}

		private void Visit<T>(Expression<Func<T, bool>> predicate, out IDictionary<string, object> param)
			where T : class
		{
			AddParamToMaps<T>(predicate.Parameters[0].Name);
			base.Visit(predicate.Body);
			param = OutputParam;
		}

		private void Visit<T1, T2>(Expression<Func<T1, T2, bool>> predicate, out IDictionary<string, object> param)
			where T1 : class
			where T2 : class
		{
			AddParamToMaps<T1>(predicate.Parameters[0].Name);
			AddParamToMaps<T2>(predicate.Parameters[1].Name);
			base.Visit(predicate.Body);
			param = OutputParam;
		}

		private void Visit<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> predicate, out IDictionary<string, object> param)
			where T1 : class
			where T2 : class
			where T3 : class
		{
			AddParamToMaps<T1>(predicate.Parameters[0].Name);
			AddParamToMaps<T2>(predicate.Parameters[1].Name);
			AddParamToMaps<T3>(predicate.Parameters[2].Name);
			base.Visit(predicate.Body);
			param = OutputParam;
		}

		private void Visit<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> predicate, out IDictionary<string, object> param)
			where T1 : class
			where T2 : class
			where T3 : class
			where T4 : class
		{
			AddParamToMaps<T1>(predicate.Parameters[0].Name);
			AddParamToMaps<T2>(predicate.Parameters[1].Name);
			AddParamToMaps<T3>(predicate.Parameters[2].Name);
			AddParamToMaps<T4>(predicate.Parameters[3].Name);
			base.Visit(predicate.Body);
			param = OutputParam;
		}

		private void AddParamToMaps<T>(string name) where T : class
		{
			SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<T>();
			TypeInfos[name] = typeInfo;
			TypeInfosMap[typeof(T)] = typeInfo;
		}

		#endregion
	}
}
