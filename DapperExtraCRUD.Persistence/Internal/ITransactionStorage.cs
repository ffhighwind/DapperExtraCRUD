using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Persistence.Internal
{
	internal interface ITransactionStorage : IDisposable
	{
		void Commit();
		void Rollback();
		void Rollback(string savePointName);
		void Save(string savePointName);
	}
}
