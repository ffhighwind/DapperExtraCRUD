using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public interface IFilter<T> where T : class
	{
		bool IsFiltered(T obj);
	}
}
