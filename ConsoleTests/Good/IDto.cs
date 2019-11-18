using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public interface IDto<T> : IComparable<T>, IEquatable<T>, IEqualityComparer<T>
	{
		T UpdateRandomize(Random random);
		bool IsIdentical(T other);
		bool IsInserted(T other);
		bool IsUpdated(T other);
		string CreateTable();
	}
}
