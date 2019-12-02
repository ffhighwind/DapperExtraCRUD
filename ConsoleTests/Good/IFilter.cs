namespace UnitTests
{
	public interface IFilter<T> where T : class
	{
		bool IsFiltered(T obj);
	}
}
