using ThatDeveloperDad.iFX.ServiceModel.Taxonomy;

namespace ThatDeveloperDad.iFX.ServiceModel
{
	public abstract class ServiceProxy<T> where T : IService
	{
		protected T _service;

		protected ServiceProxy(T implementation)
		{
			_service = implementation;
		}
	}

	public abstract class InProcServiceProxy<T> : ServiceProxy<T> where T : IService
	{
		protected InProcServiceProxy(T implementation) : base(implementation)
		{
		}
	}

	//This Child Class will get implemented Later.(tm)
	//public abstract class HttpServiceProxy<T> : ServiceProxy<T> where T : IService
	//{
	//	protected HttpServiceProxy(T implementation) : base(implementation)
	//	{
	//	}
	//}
}
