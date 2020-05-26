namespace TilesWalk.BaseInterfaces
{
	public interface IFactory<out T>
	{
		T NewInstance();
	}
}