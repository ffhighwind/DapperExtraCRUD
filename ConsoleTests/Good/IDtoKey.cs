using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public interface IDtoKey<T, KeyType> : IDto<T>
		where T : IDtoKey<T, KeyType> 
	{
		KeyType GetKey();
	}
}
