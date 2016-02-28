using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using Autofac;
using Autofac.Core;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using demo;
using MediatR;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;
using TestData.Interface;
using TestData.Interface.DataSet;
using TestData.Interface.MediatR;
using TestData.Interface.Web;

[assembly: OwinStartup(typeof(Startup))]

namespace demo
{
    public class Startup
    {
        static Startup()
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.StartsWith("demo") || a.GetName().Name.StartsWith("TestData")).ToArray();
        }

        public static Assembly[] Assemblies { get; set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            RegisterMediator(builder);
            RegisterTestData(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var resolver = new DefaultInlineConstraintResolver();
            config.MapHttpAttributeRoutes(resolver);

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            
            app.UseWebApi(config);
            app.UseStageMarker(PipelineStage.PreHandlerExecute);
        }

        void RegisterTestData(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(typeof(TestDataController).Assembly);
            builder.RegisterTypes(Assemblies.SelectMany(a => a.GetTypes()).Where(x => !x.IsAbstract && typeof(IDataSet).IsAssignableFrom(x)).ToArray());

            builder
                .Register<Func<Type, IDataSet>>(x =>
                {
                    var scope = x.Resolve<ILifetimeScope>();
                    return (t) => scope.Resolve(t) as IDataSet;
                })
                .InstancePerRequest();

            builder
                .Register(x =>
                {
                    var mediator = x.Resolve<IMediator>();
                    var dispatcher = new MediatRDispatcher(mediator);
                    return dispatcher;
                })
                .As<IDispatcher>()
                .InstancePerRequest();
        }

        void RegisterMediator(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).As<IMediator>().AsImplementedInterfaces();

            foreach (var handler in Assemblies.SelectMany(a => a.GetTypes())
                .Where(x => x.GetInterfaces().Any(i => i.IsClosedTypeOf(typeof(IAsyncRequestHandler<,>)))))
            {
                builder.RegisterType(handler)
                    .AsImplementedInterfaces()
                    .InstancePerRequest();
            }
        }
    }
}