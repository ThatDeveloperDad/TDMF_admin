using ThatDeveloperDad.iFX.ServiceModel;
using DevDad.SaaSAdmin.AccountManager.Contracts;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DevDad.SaaSAdmin.AccountManager
{
	public class CustomerAccountManagerFactory : IServiceFactory<IAccountManager>
	{
		public IAccountManager CreateService(IConfiguration config)
		{
			IAccountManager instance = ConfigureService(config);

			var proxy = new AccountManagerInProc(instance);

			return proxy;
		}

		private IAccountManager ConfigureService(IConfiguration config)
		{
			string componentConfigName = CustomerAccountManagerOptions.ConfigSectionName;

			// Here's where we build the instnace of the Accountmanager.
			CustomerAccountManager service = new();

			var componentConfig = config.GetSection(componentConfigName);

			// Configure the Service Here.
			var serviceOptions = CustomerAccountManagerOptions.FromConfiguration(componentConfig);
			IEnumerable<Type> dependencies = CustomerAccountManager.GetRequiredDependencies();
			
			foreach(Type depType in dependencies)
			{
				string typeName = depType.Name;
				var depConfig = componentConfig.GetSection(typeName);

				if (depConfig == null)
				{
					throw new Exception($"CustomerAccountManager requires an instance of {typeName} that has no configuration specification.");
				}

				string depAssemblyName = depConfig["Assembly"];
				Assembly depAssembly = Assembly.Load(depAssemblyName);
				Type builderInterface = typeof(IServiceFactory<>).MakeGenericType(depType);
				var builderType = depAssembly.ExportedTypes
					.Where(t=> t.IsAssignableTo(builderInterface))
					.First();
				var builder = Activator.CreateInstance(builderType);
				var createMethod = builderType.GetMethod("CreateService");
				var dep = createMethod.Invoke(builder, new[] { depConfig });

				// Need to get the generic SetDependency method on the service, typed to depType.
				var setDepMethod = service.GetType()
					.GetMethod("SetDependency", BindingFlags.Instance|BindingFlags.NonPublic)
					.MakeGenericMethod(depType);

				setDepMethod.Invoke(service, new[] { dep });
			}

			service.SetConfiguration(serviceOptions);

			return service;
		}
	}
}
