using Autofac;

namespace NextDoor.Services.Identity.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(InfrastructureModule).Assembly;

            // Register all the interfaces along with its default implementations
            // within my assembly being Identity project
            builder.RegisterAssemblyTypes(assembly)
                .AsImplementedInterfaces();
        }
    }
}
