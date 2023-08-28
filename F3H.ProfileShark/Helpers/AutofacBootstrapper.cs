using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Caliburn.Micro;
using IContainer = Autofac.IContainer;
#nullable disable
namespace F3H.ProfileShark.Helpers;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class AutofacBootstrapper : BootstrapperBase
{
    public AutofacBootstrapper()
    {
        Initialize(); // Initalizes the Caliburn.Micro framework
    }

    protected IContainer Container { get; private set; }

    // can be overridden by AppBootstrapper 
    protected override void Configure()
    {
        ConfigureBootstrapper();
        //  validate settings
        if (CreateWindowManager == null)
            throw new ArgumentNullException($"CreateWindowManager");
        if (CreateEventAggregator == null)
            throw new ArgumentNullException($"CreateEventAggregator");
        var builder = new ContainerBuilder();

        //  register view models
        builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            //  must be a type with a name that ends with ViewModel
            .Where(type => type.Name.EndsWith("ViewModel"))
            //  must be in a namespace ending with ViewModels
            .Where(type =>
                !EnforceNamespaceConvention ||
                (!string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("ViewModels")))
            //  must implement INotifyPropertyChanged (deriving from PropertyChangedBase will satisfy this)
            .Where(type => type.GetInterface(ViewModelBaseType.Name, false) != null)
            //  registered as self
            .AsSelf()
            //  always create a new one
            .InstancePerDependency();
        // views
        builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
            //  must be a type with a name that ends with View
            .Where(type => type.Name.EndsWith("View"))
            //  must be in a namespace that ends in Views
            .Where(type =>
                !EnforceNamespaceConvention ||
                (!string.IsNullOrWhiteSpace(type.Namespace) && type.Namespace.EndsWith("Views")))
            //  registered as self
            .AsSelf()
            //  always create a new one
            .InstancePerDependency();
        builder.Register(_ => CreateWindowManager()).InstancePerLifetimeScope();
        //  register the single event aggregator for this container
        builder.Register(_ => CreateEventAggregator()).InstancePerLifetimeScope();
        // TODO: register all viewmodels that are interested in latest LogData to receive event 
        // builder.RegisterModule<EventAggregationAutoSubscriptionModule>();

        //  allow derived classes to add to the container
        ConfigureContainer(builder);

        Container = builder.Build();
    }

    protected virtual void ConfigureBootstrapper()
    {
        //  by default, do not enforce the namespace convention, this frees us up to organize the solution 
        // better by function area instead of lumping it all under ViewModels and Views
        EnforceNamespaceConvention = false;
        //  the default view model base type
        ViewModelBaseType = typeof(INotifyPropertyChanged);
        //  default window manager
        CreateWindowManager = () => new WindowManager();
        //  default event aggregator
        CreateEventAggregator = () => new EventAggregator();
    }

    /// <summary>
    ///     Override to include your own Autofac configuration after the framework has finished its configuration, but
    ///     before the container is created.
    /// </summary>
    /// <param name="builder">The Autofac configuration builder.</param>
    protected virtual void ConfigureContainer(ContainerBuilder builder)
    {
        // overridden in each AppBootstrapper to register app-specific services
    }

    // override to select the assemblies making up our application
    protected override IEnumerable<Assembly> SelectAssemblies()
    {
        // since we're using this from a separate assembly and not the one one that holds our 
        // ViewModels and Views, we need to add the Assembly for the main exe (the current AppDomain)
        return new[]
        {
            Assembly.GetEntryAssembly(),
            Assembly.GetExecutingAssembly()
        };
    }

    protected override object GetInstance(Type service, string key)
    {
        object instance;
        if (string.IsNullOrWhiteSpace(key))
        {
            if (Container.TryResolve(service, out instance))
                return instance;
        }
        else
        {
            if (Container.TryResolveNamed(key, service, out instance))
                return instance;
        }

        throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
    }

    #region Public Properties

    public bool EnforceNamespaceConvention { get; set; }
    public Type ViewModelBaseType { get; set; }
    public Func<IWindowManager> CreateWindowManager { get; set; }
    public Func<IEventAggregator> CreateEventAggregator { get; set; }

    #endregion
}
#nullable restore